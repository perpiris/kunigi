using Kunigi.Exceptions;
using Kunigi.Services;
using Kunigi.ViewModels.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kunigi.Controllers;

[Route("teams")]
public class TeamController : Controller
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> TeamList(int pageNumber = 1, int pageSize = 10)
    {
        var viewModel = await _teamService.GetPaginatedTeams(pageNumber, pageSize);
        return View(viewModel);
    }

    [HttpGet("details")]
    public async Task<IActionResult> TeamDetails(Guid teamId)
    {
        try
        {
            var viewModel = await _teamService.GetTeamDetails(teamId);
            return View(viewModel);
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("TeamList");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("TeamList");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("TeamList");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("create-team")]
    public IActionResult CreateTeam()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create-team")]
    public async Task<IActionResult> CreateTeam(TeamCreateViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        try
        {
            await _teamService.CreateTeam(viewModel);
            TempData["success"] = "Η ομάδα δημιουργήθηκε.";
            return RedirectToAction("TeamManagement");
        }
        catch (Exception exception)
        {
            return RedirectToAction("TeamManagement");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("edit-team")]
    public async Task<IActionResult> EditTeam(Guid teamId)
    {
        try
        {
            var viewModel = await _teamService.PrepareEditTeamViewModel(teamId, User);
            return View(viewModel);
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("edit-team")]
    public async Task<IActionResult> EditTeam(TeamEditViewModel viewModel, IFormFile profileImage)
    {
        if (!ModelState.IsValid) return View(viewModel);

        try
        {
            await _teamService.EditTeam(viewModel, profileImage, User);
            TempData["success"] = "Η ομάδα επεξεργάστηκε επιτυχώς.";
            return RedirectToAction("TeamActions", new { viewModel.TeamId });
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return View(viewModel);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("manage-teams")]
    public async Task<IActionResult> TeamManagement(int pageNumber = 1, int pageSize = 15)
    {
        var viewModel = await _teamService.GetPaginatedTeams(pageNumber, pageSize);
        return View(viewModel);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("edit-team-managers")]
    public async Task<IActionResult> EditTeamManagers(Guid teamId)
    {
        try
        {
            var viewModel = await _teamService.PrepareTeamManagerEditViewModel(teamId, User);
            return View(viewModel);
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("edit-team-managers")]
    public async Task<IActionResult> EditTeamManagers(TeamManagerEditViewModel viewModel)
    {
        try
        {
            await _teamService.AddTeamManager(viewModel, User);
            TempData["success"] = "Ο χρήστης προστέθηκε με επιτυχία.";
            return RedirectToAction("EditTeamManagers", new { viewModel.TeamId });
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("remove-team-manager")]
    public async Task<IActionResult> RemoveManager(Guid teamId, string managerId)
    {
        try
        {
            await _teamService.RemoveTeamManager(teamId, managerId, User);
            TempData["success"] = "Ο χρήστης αφαιρέθηκε με επιτυχία.";
            return RedirectToAction("EditTeamManagers", new { teamId });
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("manage-team-media")]
    public async Task<IActionResult> TeamMediaManagement(Guid teamId)
    {
        try
        {
            var viewModel = await _teamService.GetTeamMedia(teamId, User);
            return View(viewModel);
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("upload-team-media")]
    public async Task<IActionResult> UploadTeamMedia(Guid teamId, TeamMediaViewModel viewModel)
    {
        try
        {
            await _teamService.AddTeamMedia(teamId, viewModel.NewMediaFiles, User);
            TempData["success"] = "Τα αρχεία ανέβηκαν επιτυχώς.";
            return RedirectToAction("TeamMediaManagement", new { teamId });
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("delete-team-media")]
    public async Task<IActionResult> DeleteTeamMedia(Guid teamId, Guid mediaId)
    {
        try
        {
            await _teamService.DeleteTeamMedia(teamId, mediaId, User);
            TempData["success"] = "Τα αρχείο διαγράφηκε επιτυχώς.";
            return RedirectToAction("TeamMediaManagement", new { teamId });
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("team-actions")]
    public async Task<IActionResult> TeamActions(Guid teamId)
    {
        try
        {
            var viewModel = await _teamService.GetTeamDetails(teamId, User);
            return View(viewModel);
        }
        catch (NotFoundException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (ArgumentNullException exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (UnauthorizedOperationException exception)
        {
            TempData["error"] = exception.Message;
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }
}