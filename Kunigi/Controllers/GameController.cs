using System.Security.Claims;
using Kunigi.CustomAttributes;
using Kunigi.Entities;
using Kunigi.Exceptions;
using Kunigi.Services;
using Kunigi.ViewModels.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kunigi.Controllers;

[Route("games")]
public class GameController : Controller
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ParentGameList()
    {
        var viewModel = await _gameService.GetAllGames();
        return View(viewModel);
    }

    [HttpGet("{gameYear}")]
    public async Task<IActionResult> ParentGameDetails(short gameYear)
    {
        try
        {
            var viewModel = await _gameService.GetParentGameDetails(gameYear);
            return View(viewModel);
        }
        catch (Exception)
        {
            return RedirectToAction("ParentGameList");
        }
    }

    [HttpGet("{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> GameDetails(string gameYear, string gameTypeSlug)
    {
        // var game = await _context.Games
        //     .Include(g => g.ParentGame)
        //     .Include(g => g.GameType)
        //     .Include(g => g.Puzzles)
        //     .ThenInclude(p => p.MediaFiles)
        //     .ThenInclude(pm => pm.MediaFile)
        //     .FirstOrDefaultAsync(g =>
        //         g.ParentGame.Year.ToString() == gameYear && g.GameType.Slug == gameTypeSlug.Trim());
        //
        // if (game == null)
        // {
        //     TempData["error"] = "Το παιχνίδι δεν βρέθηκε.";
        //     return RedirectToAction("ParentGameDetails", "Game");
        // }
        //
        // var viewModel = new GamePuzzlesViewModel
        // {
        //     Id = game.Id,
        //     Type = game.GameType.Description,
        //     Title = game.ParentGame.Title,
        //     Year = game.ParentGame.Year,
        //     Description = game.Description,
        //     Puzzles = game.Puzzles.Select(p => new PuzzleDetailsViewModel
        //     {
        //         Id = p.Id,
        //         Question = p.Question,
        //         Answer = p.Answer,
        //         Type = p.Type.ToString(),
        //         Order = p.Order,
        //         QuestionMedia = p.MediaFiles
        //             .Where(m => m.MediaType == PuzzleMediaType.Question)
        //             .Select(m => m.MediaFile.Path)
        //             .ToList(),
        //         AnswerMedia = p.MediaFiles
        //             .Where(m => m.MediaType == PuzzleMediaType.Answer)
        //             .Select(m => m.MediaFile.Path)
        //             .ToList()
        //     }).OrderBy(p => p.Order).ToList()
        // };

        return View();
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("create-parent-game")]
    public async Task<IActionResult> CreateParentGame()
    {
        var viewModel = await _gameService.PrepareCreateParentGameViewModel(null);
        return View(viewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create-parent-game")]
    public async Task<IActionResult> CreateParentGame(
        ParentGameCreateViewModel viewModel)
    {
        var parentGameList = await _gameService.GetAllGames();
        var exists = parentGameList.Any(x => x.Year == viewModel.Year);
        if (exists)
        {
            ModelState.AddModelError("Year",
                "Έχει ήδη καταχωρηθεί παιχνίδι για αυτή τη χρονιά.");
        }
        else
        {
            ModelState.Remove("Year");
        }

        exists = parentGameList.Any(x => x.Order == viewModel.Order);
        if (exists)
        {
            ModelState.AddModelError("Order",
                "Έχει ήδη καταχωρηθεί παιχνίδι για αυτή τη σειρά.");
        }
        else
        {
            ModelState.Remove("Order");
        }
        
        if (viewModel.HostId <= 0)
        {
            ModelState.AddModelError("HostId", "Το πεδίο απαιτείται.");
        }
        else
        {
            ModelState.Remove("HostId");
        }

        if (viewModel.WinnerId <= 0)
        {
            ModelState.AddModelError("WinnerId", "Το πεδίο απαιτείται.");
        }
        else
        {
            ModelState.Remove("WinnerId");
        }
        
        if (!ModelState.IsValid)
        {
            viewModel = await _gameService.PrepareCreateParentGameViewModel(viewModel);
            return View(viewModel);
        }

        try
        {
            await _gameService.CreateParentGame(viewModel);
            TempData["success"] = "Το παιχνίδι δημιουργήθηκε.";
        }
        catch (Exception)
        {
            viewModel = await _gameService.PrepareCreateParentGameViewModel(viewModel);
            return View(viewModel);
        }
        
        return RedirectToAction("ParentGameList");
    }


    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("edit-parent-game/{gameYear}")]
    public async Task<IActionResult> EditParentGame(short gameYear)
    {
        try
        {
            var viewModel = await _gameService.PrepareEditParentGameViewModel(gameYear, User);
            return View(viewModel);
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού.";
        }
        catch (Exception)
        {
            return RedirectToAction("ParentGameList");
        }

        return RedirectToAction("ParentGameList");
    }


    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("edit-parent-game/{gameYear}")]
    public async Task<IActionResult> EditParentGame(short gameYear,
        ParentGameEditViewModel viewModel, IFormFile profileImage)
    {
        try
        {
            await _gameService.EditParentGame(gameYear, viewModel, profileImage, User);
            TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
            return RedirectToAction("ParentGameActions", new { gameYear });
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }

        return RedirectToAction("Dashboard", "Home");
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("manage-parent-gaems")]
    public async Task<IActionResult> ParentGameManagement(int pageIndex = 1)
    {
        var viewModel = await _gameService.GetAllGames();
        return View(viewModel);
    }


    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("manage-parent-game/{gameYear}")]
    public async Task<IActionResult> ParentGameActions(short gameYear)
    {
        try
        {
            var viewModel = await _gameService.GetParentGameDetails(gameYear);
            return View(viewModel);
        }
        catch (Exception)
        {
            return RedirectToAction("ParentGameList");
        }
    }


    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("manage-game/{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> GameActions(short gameYear, string gameTypeSlug)
    {
        try
        {
            var viewModel = await _gameService.GetGameDetails(gameYear, gameTypeSlug);
            return View(viewModel);
        }
        catch (Exception)
        {
            return RedirectToAction("ParentGameList");
        }
    }

    private static GameDetailsViewModel GetFullMappedGameDetailsViewModel(
        Game parentGameDetails)
    {
        var viewModel = new GameDetailsViewModel
        {
            Id = parentGameDetails.GameId,
            Title = parentGameDetails.ParentGame.Title,
            Description = parentGameDetails.Description,
            Year = parentGameDetails.ParentGame.Year,
            Type = parentGameDetails.GameType.Description
        };

        return viewModel;
    }

    private static ParentGameEditViewModel GetMappedCreateOrEditViewModel(
        ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameEditViewModel()
        {
            Title = parentGameDetails.Title,
            Description = parentGameDetails.Description
        };

        return viewModel;
    }
}