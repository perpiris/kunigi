using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Enums;
using Kunigi.Utilities;
using Kunigi.ViewModels.Puzzle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

[Route("puzzles")]
[Authorize(Roles = "Admin,Manager")]
public class PuzzleController : Controller
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public PuzzleController(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("{gameYear}/{gameSlug}")]
    public async Task<IActionResult> PuzzleList(string gameYear, string gameSlug)
    {
        var game = await _context.Games
            .Include(g => g.GameType)
            .Include(g => g.Puzzles)
            .ThenInclude(p => p.MediaFiles)
            .ThenInclude(pm => pm.MediaFile)
            .FirstOrDefaultAsync(g =>
                g.ParentGame.Year.ToString() == gameYear && g.GameType.Slug == gameSlug.Trim());

        if (game == null)
        {
            TempData["error"] = "Το παιχνίδι δεν βρέθηκε.";
            return RedirectToAction("GameDetails", "Game");
        }

        var viewModel = new GamePuzzlesViewModel
        {
            GameId = game.Id,
            GameType = game.GameType.Description,
            Puzzles = game.Puzzles.Select(p => new PuzzleDetailsViewModel
            {
                Id = p.Id,
                Question = p.Question,
                Answer = p.Answer,
                Type = p.Type.ToString(),
                Order = p.Order,
                QuestionMedia = p.MediaFiles
                    .Where(m => m.MediaType == PuzzleMediaType.Question)
                    .Select(m => m.MediaFile.Path)
                    .ToList(),
                AnswerMedia = p.MediaFiles
                    .Where(m => m.MediaType == PuzzleMediaType.Answer)
                    .Select(m => m.MediaFile.Path)
                    .ToList()
            }).OrderBy(p => p.Order).ToList()
        };

        return View(viewModel);
    }

    [HttpGet("manage/{gameId:int}")]
    public async Task<IActionResult> ManagePuzzles(int gameId)
    {
        var game = await _context.Games
            .Include(g => g.ParentGame)
            .Include(g => g.Puzzles)
            .ThenInclude(p => p.MediaFiles)
            .ThenInclude(pm => pm.MediaFile).Include(game => game.GameType)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        if (game == null)
        {
            TempData["error"] = "Το παιχνίδι δεν βρέθηκε.";
            return RedirectToAction("ParentGameList", "Game");
        }

        var viewModel = new PuzzleListViewModel
        {
            GameId = gameId,
            ParentGameYear = game.ParentGame.Year,
            GameType = game.GameType.Description,
            Puzzles = game.Puzzles.Select(p => new PuzzleDetailsViewModel
            {
                Id = p.Id,
                Question = p.Question,
                Answer = p.Answer,
                Type = p.Type.ToString(),
                Order = p.Order
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet("create/{gameId:int}")]
    public IActionResult CreatePuzzle(int gameId)
    {
        var viewModel = new PuzzleCreateViewModel
        {
            GameId = gameId,
            PuzzleTypes = Enum.GetValues(typeof(PuzzleType))
                .Cast<PuzzleType>()
                .Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost("create/{gameId:int}")]
    public async Task<IActionResult> CreatePuzzle(int gameId, PuzzleCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.PuzzleTypes = Enum.GetValues(typeof(PuzzleType))
                .Cast<PuzzleType>()
                .Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                }).ToList();
            return View(model);
        }

        var game = await _context.Games.FindAsync(model.GameId);
        if (game == null)
        {
            return NotFound("Game not found");
        }

        var puzzleType = Enum.Parse<PuzzleType>(model.Type);

        var maxOrder = await _context.Puzzles
            .Where(p => p.GameId == model.GameId && p.Type == puzzleType)
            .MaxAsync(p => (int?)p.Order) ?? 0;

        var puzzle = new Puzzle
        {
            GameId = model.GameId,
            Question = model.Question,
            Answer = model.Answer,
            Type = puzzleType,
            Order = maxOrder + 1,
            MediaFiles = new List<PuzzleMedia>()
        };

        if (model.QuestionMediaFiles is { Count: > 0 })
        {
            foreach (var mediaFile in model.QuestionMediaFiles)
            {
                puzzle.MediaFiles.Add(await CreatePuzzleMedia(game, mediaFile,
                    PuzzleMediaType.Question));
            }
        }

        if (model.AnswerMediaFiles is { Count: > 0 })
        {
            foreach (var mediaFile in model.AnswerMediaFiles)
            {
                puzzle.MediaFiles.Add(await CreatePuzzleMedia(game, mediaFile,
                    PuzzleMediaType.Answer));
            }
        }

        _context.Puzzles.Add(puzzle);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ManagePuzzles), new { gameId = model.GameId });
    }

    private async Task<PuzzleMedia> CreatePuzzleMedia(Game game, IFormFile mediaFile,
        PuzzleMediaType mediaType)
    {
        var basePath = _configuration["ImageStoragePath"];
        var gameFolderPath = CrossPlatformPathUtility.CombineAndNormalize(basePath, "games",
            game.ParentGame.Slug, game.GameType.Slug);
        Directory.CreateDirectory(gameFolderPath);

        var fileName = FileNameGenerator.GetUniqueFileName(mediaFile.FileName);
        var fullFilePath = CrossPlatformPathUtility.CombineAndNormalize(gameFolderPath, fileName);

        await using (var stream = new FileStream(fullFilePath, FileMode.Create))
        {
            await mediaFile.CopyToAsync(stream);
        }

        return new PuzzleMedia
        {
            MediaFile = new MediaFile
            {
                Path = CrossPlatformPathUtility.GetRelativePath(basePath, fullFilePath)
            },
            MediaType = mediaType
        };
    }
}