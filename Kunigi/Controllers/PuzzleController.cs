using Kunigi.Entities;
using Kunigi.Enums;
using Kunigi.Services;
using Kunigi.ViewModels.Puzzle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kunigi.Controllers;

[Route("puzzles")]
public class PuzzleController : Controller
{
    private readonly IPuzzleService _puzzleService;

    public PuzzleController(IPuzzleService puzzleService)
    {
        _puzzleService = puzzleService;
    }

    [HttpGet("create/{gameId:int}")]
    [Authorize(Roles = "Admin,Manager")]
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
    [Authorize(Roles = "Admin,Manager")]
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

        // var game = await _context.Games.FindAsync(model.GameId);
        // if (game == null)
        // {
        //     return NotFound("Game not found");
        // }
        //
        // var puzzleType = Enum.Parse<PuzzleType>(model.Type);
        //
        // var maxOrder = await _context.Puzzles
        //     .Where(p => p.GameId == model.GameId && p.Type == puzzleType)
        //     .MaxAsync(p => (int?)p.Order) ?? 0;

        // var puzzle = new Puzzle
        // {
        //     GameId = model.GameId,
        //     Question = model.Question,
        //     Answer = model.Answer,
        //     Type = puzzleType,
        //     Order = maxOrder + 1,
        //     MediaFiles = new List<PuzzleMedia>()
        // };
        //
        // if (model.QuestionMediaFiles is { Count: > 0 })
        // {
        //     foreach (var mediaFile in model.QuestionMediaFiles)
        //     {
        //         puzzle.MediaFiles.Add(await CreatePuzzleMedia(game, mediaFile,
        //             PuzzleMediaType.Question));
        //     }
        // }
        //
        // if (model.AnswerMediaFiles is { Count: > 0 })
        // {
        //     foreach (var mediaFile in model.AnswerMediaFiles)
        //     {
        //         puzzle.MediaFiles.Add(await CreatePuzzleMedia(game, mediaFile,
        //             PuzzleMediaType.Answer));
        //     }
        // }

        // _context.Puzzles.Add(puzzle);
        // await _context.SaveChangesAsync();

        return View();
    }

    private async Task<PuzzleMedia> CreatePuzzleMedia(Game game, IFormFile mediaFile,
        PuzzleMediaType mediaType)
    {
        // var basePath = _configuration["ImageStoragePath"];
        // var gameFolderPath = CrossPlatformPathUtility.CombineAndNormalize(basePath, "games",
        //     game.ParentGame.Slug, game.GameType.Slug);
        // Directory.CreateDirectory(gameFolderPath);
        //
        // var fileName = FileNameGenerator.GetUniqueFileName(mediaFile.FileName);
        // var fullFilePath = CrossPlatformPathUtility.CombineAndNormalize(gameFolderPath, fileName);
        //
        // await using (var stream = new FileStream(fullFilePath, FileMode.Create))
        // {
        //     await mediaFile.CopyToAsync(stream);
        // }
        //
        // return new PuzzleMedia
        // {
        //     MediaFile = new MediaFile
        //     {
        //         Path = CrossPlatformPathUtility.GetRelativePath(basePath, fullFilePath)
        //     },
        //     MediaType = mediaType
        // };

        return new PuzzleMedia();
    }
}