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
    public async Task<IActionResult> ParentGameList(int pageNumber = 1, int pageSize = 10)
    {
        var viewModel = await _gameService.GetPaginatedGame(pageNumber, pageSize);
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

    [HttpGet("puzzle-list/{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> GamePuzzleList(short gameYear, string gameTypeSlug)
    {
        try
        {
            var viewModel = await _gameService.GetGamePuzzleList(gameYear, gameTypeSlug);
            return View(viewModel);
        }
        catch (Exception)
        {
            return RedirectToAction("ParentGameList", "Game");
        }
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
    public async Task<IActionResult> ParentGameManagement(int pageNumber = 1, int pageSize = 10)
    {
        var viewModel = await _gameService.GetPaginatedGame(pageNumber, pageSize);
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
}