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
        catch (NotFoundException)
        {
            return RedirectToAction("ParentGameList");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("ParentGameList");
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
        catch (NotFoundException)
        {
            return RedirectToAction("ParentGameList");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("ParentGameList");
        }
        catch (Exception)
        {
            return RedirectToAction("ParentGameList");
        }
    }
    
    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("manage-puzzle-list/{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> ManageGamePuzzleList(short gameYear, string gameTypeSlug)
    {
        try
        {
            var viewModel = await _gameService.GetGamePuzzleList(gameYear, gameTypeSlug);
            return View(viewModel);
        }
        catch (NotFoundException)
        {
            return RedirectToAction("ParentGameList");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("ParentGameList");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("ParentGameList", "Game");
        }
    }
    
    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("create-game-puzzle/{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> CreateGamePuzzle(short gameYear, string gameTypeSlug)
    {
        try
        {
            var viewModel = await _gameService.PrepareCreateGamePuzzleViewModel(gameYear, gameTypeSlug);
            return View(viewModel);
        }
        catch (NotFoundException)
        {
            return RedirectToAction("ParentGameList");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("ParentGameList", "Game");
        }
    }
    
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("create-game-puzzle/{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> CreateGamePuzzle(short gameYear, string gameTypeSlug, GamePuzzleCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        try
        {
            await _gameService.CreateGamePuzzle(viewModel);
            TempData["success"] = "Το γρίφος δημιουργήθηκε επιτυχώς.";
            return RedirectToAction("ManageGamePuzzleList", new { gameYear, gameTypeSlug });            
        }
        catch (NotFoundException)
        {
            return RedirectToAction("ParentGameList");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("ParentGameList");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return View(viewModel);
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
            TempData["success"] = "Το παιχνίδι δημιουργήθηκε επιτυχώς.";
            return RedirectToAction("ParentGameManagement");
        }
        catch (NotFoundException)
        {
            return RedirectToAction("ParentGameManagement");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("ParentGameManagement");
        }
        catch (Exception)
        {
            viewModel = await _gameService.PrepareCreateParentGameViewModel(viewModel);
            return View(viewModel);
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("edit-parent-game/{gameYear}")]
    public async Task<IActionResult> EditParentGame(short gameYear)
    {
        try
        {
            var viewModel = await _gameService.PrepareParentGameEditViewModel(gameYear, User);
            return View(viewModel);
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("edit-parent-game/{gameYear}")]
    public async Task<IActionResult> EditParentGame(short gameYear,
        ParentGameEditViewModel viewModel, IFormFile profileImage)
    {
        if (!ModelState.IsValid) return View(viewModel);
        
        try
        {
            await _gameService.EditParentGame(gameYear, viewModel, profileImage, User);
            TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
            return RedirectToAction("ParentGameActions", new { gameYear });
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return View(viewModel);
        }
    }
    
    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("edit-game/{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> EditGame(short gameYear, string gameTypeSlug)
    {
        try
        {
            var viewModel = await _gameService.PrepareGameEditViewModel(gameYear, gameTypeSlug, User);
            return View(viewModel);
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }
    
    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("edit-game/{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> EditGame(short gameYear, string gameTypeSlug, GameEditViewModel viewModel)
    {
        try
        {
            await _gameService.EditGame(gameYear, gameTypeSlug, viewModel, User);
            TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
            return RedirectToAction("GameActions", new { gameYear, gameTypeSlug });
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return View(viewModel);
        }
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("manage-parent-gaems")]
    public async Task<IActionResult> ParentGameManagement(int pageNumber = 1, int pageSize = 15)
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
            var viewModel = await _gameService.GetParentGameDetails(gameYear, User);
            return View(viewModel);
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
    }
    
    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("manage-parent-game-media/{gameYear}")]
    public async Task<IActionResult> ParentGameMediaManagement(short gameYear)
    {
        try
        {
            var viewModel = await _gameService.GetParentGameMEdia(gameYear, User);
            return View(viewModel);
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("upload-team-media/{gameYear}")]
    public async Task<IActionResult> UploadParentGameMedia(short gameYear, ParentGameMediaViewModel viewModel)
    {
        try
        {
            await _gameService.AddParentGameMedia(gameYear, viewModel.NewMediaFiles, User);
            TempData["success"] = "Τα αρχεία ανέβηκαν επιτυχώς.";
            return RedirectToAction("ParentGameMediaManagement", new { gameYear });
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("delete-parent-game-media/{gameYear}")]
    public async Task<IActionResult> DeleteParentGameMedia(short gameYear, int mediaId)
    {
        try
        {
            await _gameService.DeleteParentGameMedia(gameYear, mediaId, User);
            TempData["success"] = "Τα αρχείο διαγράφηκε επιτυχώς.";
            return RedirectToAction("ParentGameMediaManagement", new { gameYear });
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }
    
    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("manage-game/{gameYear}/{gameTypeSlug}")]
    public async Task<IActionResult> GameActions(short gameYear, string gameTypeSlug)
    {
        try
        {
            var viewModel = await _gameService.GetGameDetails(gameYear, gameTypeSlug, User);
            return View(viewModel);
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
            return RedirectToAction("Dashboard", "Home");
        }
    }
}