// using Kunigi.Exceptions;
// using Kunigi.Services;
// using Kunigi.ViewModels.Game;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Kunigi.Controllers;
//
// [Route("games")]
// public class GameController : Controller
// {
//     private readonly IGameService _gameService;
//
//     public GameController(IGameService gameService)
//     {
//         _gameService = gameService;
//     }
//
//     [HttpGet("list")]
//     public async Task<IActionResult> ParentGameList(int pageNumber = 1, int pageSize = 10)
//     {
//         var viewModel = await _gameService.GetPaginatedGame(pageNumber, pageSize);
//         return View(viewModel);
//     }
//
//     [HttpGet("details")]
//     public async Task<IActionResult> ParentGameDetails(Guid parentGameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.GetParentGameDetails(parentGameId);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//     }
//
//     [HttpGet("puzzle-list")]
//     public async Task<IActionResult> GamePuzzleList(Guid gameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.GetGamePuzzleList(gameId);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("manage-puzzle-list")]
//     public async Task<IActionResult> ManageGamePuzzleList(Guid gameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.GetGamePuzzleList(gameId);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("ParentGameList", "Game");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("create-game-puzzle")]
//     public async Task<IActionResult> CreateGamePuzzle(Guid gameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.PrepareCreateGamePuzzleViewModel(gameId);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("ParentGameList", "Game");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpPost("create-game-puzzle")]
//     public async Task<IActionResult> CreateGamePuzzle(PuzzleCreateViewModel viewModel)
//     {
//         if (!ModelState.IsValid)
//         {
//             return View(viewModel);
//         }
//
//         try
//         {
//             await _gameService.CreateGamePuzzle(viewModel);
//             TempData["success"] = "Το γρίφος δημιουργήθηκε επιτυχώς.";
//             return RedirectToAction("ManageGamePuzzleList", new { viewModel.GameId });            
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return View(viewModel);
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("edit-game-puzzle")]
//     public async Task<IActionResult> EditGamePuzzle(Guid puzzleId)
//     {
//         try
//         {
//             var viewModel = await _gameService.PrepareEditGamePuzzleViewModel(puzzleId);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("ParentGameList", "Game");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpPost("edit-game-puzzle")]
//     public async Task<IActionResult> EditGamePuzzle(PuzzleEditViewModel viewModel)
//     {
//         if (!ModelState.IsValid)
//         {
//             return View(viewModel);
//         }
//
//         try
//         {
//             await _gameService.EditGamePuzzle(viewModel);
//             TempData["success"] = "Το γρίφος επεξεργάστηκε επιτυχώς.";
//             return RedirectToAction("ManageGamePuzzleList", new { viewModel.PuzzleId });            
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("ParentGameList");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return View(viewModel);
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpPost("delete-game-puzzle")]
//     public async Task<IActionResult> DeleteGamePuzzle(Guid puzzleId)
//     {
//         try
//         {
//             await _gameService.DeleteGamePuzzle(puzzleId);
//             TempData["success"] = "Ο γρίφος διαγράφηκε επιτυχώς.";
//             return RedirectToAction("ManageGamePuzzleList", new { parentGameId, gameId });
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("delete-game-puzzle")]
//     public async Task<IActionResult> DeleteGamePuzzleMedia(Guid puzzleId)
//     {
//         try
//         {
//             return View();
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//
//     [Authorize(Roles = "Admin")]
//     [HttpGet("create-parent-game")]
//     public async Task<IActionResult> CreateParentGame()
//     {
//         var viewModel = await _gameService.PrepareCreateParentGameViewModel(null);
//         return View(viewModel);
//     }
//
//     [Authorize(Roles = "Admin")]
//     [HttpPost("create-parent-game")]
//     public async Task<IActionResult> CreateParentGame(
//         ParentGameCreateViewModel viewModel)
//     {
//         // if (viewModel.HostId <= 0)
//         // {
//         //     ModelState.AddModelError("HostId", "Το πεδίο απαιτείται.");
//         // }
//         // else
//         // {
//         //     ModelState.Remove("HostId");
//         // }
//         //
//         // if (viewModel.WinnerId <= 0)
//         // {
//         //     ModelState.AddModelError("WinnerId", "Το πεδίο απαιτείται.");
//         // }
//         // else
//         // {
//         //     ModelState.Remove("WinnerId");
//         // }
//         
//         if (!ModelState.IsValid)
//         {
//             viewModel = await _gameService.PrepareCreateParentGameViewModel(viewModel);
//             return View(viewModel);
//         }
//
//         try
//         {
//             await _gameService.CreateParentGame(viewModel);
//             TempData["success"] = "Το παιχνίδι δημιουργήθηκε επιτυχώς.";
//             return RedirectToAction("ParentGameManagement");
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("ParentGameManagement");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("ParentGameManagement");
//         }
//         catch (Exception)
//         {
//             viewModel = await _gameService.PrepareCreateParentGameViewModel(viewModel);
//             return View(viewModel);
//         }
//     }
//
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("edit-parent-game")]
//     public async Task<IActionResult> EditParentGame(Guid parentGameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.PrepareParentGameEditViewModel(parentGameId, User);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpPost("edit-parent-game")]
//     public async Task<IActionResult> EditParentGame(ParentGameEditViewModel viewModel, IFormFile profileImage)
//     {
//         if (!ModelState.IsValid) return View(viewModel);
//         
//         try
//         {
//             await _gameService.EditParentGame(viewModel, profileImage, User);
//             TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
//             return RedirectToAction("ParentGameActions", new { viewModel.ParentGameId });
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return View(viewModel);
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("edit-game")]
//     public async Task<IActionResult> EditGame(Guid gameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.PrepareGameEditViewModel(gameId, User);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpPost("edit-game")]
//     public async Task<IActionResult> EditGame(GameEditViewModel viewModel)
//     {
//         try
//         {
//             await _gameService.EditGame(viewModel, User);
//             TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
//             return RedirectToAction("GameActions", new { viewModel.GameId });
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return View(viewModel);
//         }
//     }
//     
//     [Authorize(Roles = "Admin")]
//     [HttpGet("manage-parent-gaems")]
//     public async Task<IActionResult> ParentGameManagement(int pageNumber = 1, int pageSize = 15)
//     {
//         var viewModel = await _gameService.GetPaginatedGame(pageNumber, pageSize);
//         return View(viewModel);
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("manage-parent-game")]
//     public async Task<IActionResult> ParentGameActions(Guid parentGameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.GetParentGameDetails(parentGameId, User);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("manage-parent-game-media")]
//     public async Task<IActionResult> ParentGameMediaManagement(Guid gameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.GetParentGameMedia(gameId, User);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("manage-game-puzzle-media")]
//     public async Task<IActionResult> GamePuzzleMediaManagement(Guid puzzleId)
//     {
//         try
//         {
//             var viewModel = await _gameService.GetPuzzleMedia(puzzleId);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException exception)
//         {
//             TempData["error"] = exception.Message;
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpPost("upload-team-media")]
//     public async Task<IActionResult> UploadParentGameMedia(Guid parentGameId, ParentGameMediaViewModel viewModel)
//     {
//         try
//         {
//             await _gameService.AddParentGameMedia(parentGameId, viewModel.NewMediaFiles, User);
//             TempData["success"] = "Τα αρχεία ανέβηκαν επιτυχώς.";
//             return RedirectToAction("ParentGameMediaManagement", new { parentGameId });
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException)
//         {
//             TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpPost("delete-parent-game-media")]
//     public async Task<IActionResult> DeleteParentGameMedia(Guid parentGameId, Guid mediaId)
//     {
//         try
//         {
//             await _gameService.DeleteParentGameMedia(parentGameId, mediaId, User);
//             TempData["success"] = "Τα αρχείο διαγράφηκε επιτυχώς.";
//             return RedirectToAction("ParentGameMediaManagement", new { parentGameId });
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException)
//         {
//             TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (Exception)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
//     
//     [Authorize(Roles = "Admin,Manager")]
//     [HttpGet("manage-game")]
//     public async Task<IActionResult> GameActions(Guid gameId)
//     {
//         try
//         {
//             var viewModel = await _gameService.GetGameDetails(gameId, User);
//             return View(viewModel);
//         }
//         catch (NotFoundException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (ArgumentNullException)
//         {
//             return RedirectToAction("Dashboard", "Home");
//         }
//         catch (UnauthorizedOperationException)
//         {
//             TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού";
//             return RedirectToAction("Dashboard", "Home");
//         }
//     }
// }