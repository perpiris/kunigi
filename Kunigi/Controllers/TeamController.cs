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

    [HttpGet("{teamSlug}")]
    public async Task<IActionResult> TeamDetails(string teamSlug)
    {
        try
        {
            var viewModel = await _teamService.GetTeamDetails(teamSlug);
            return View(viewModel);
        }
        catch (NotFoundException)
        {
            return RedirectToAction("TeamList");
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("TeamList");
        }
        catch (Exception)
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
        catch (Exception)
        {
            return RedirectToAction("TeamManagement");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("edit-team/{teamSlug}")]
    public async Task<IActionResult> EditTeam(string teamSlug)
    {
        try
        {
            var viewModel = await _teamService.PrepareEditTeamViewModel(teamSlug, User);
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("edit-team/{teamSlug}")]
    public async Task<IActionResult> EditTeam(string teamSlug, TeamEditViewModel viewModel, IFormFile profileImage)
    {
        if (!ModelState.IsValid) return View(viewModel);
        
        try
        {
            await _teamService.EditTeam(teamSlug, viewModel, profileImage, User);
            TempData["success"] = "Η ομάδα επεξεργάστηκε επιτυχώς.";
            return RedirectToAction("TeamActions", new { teamSlug });
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
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
    [HttpGet("edit-team-managers/{teamSlug}")]
    public async Task<IActionResult> EditTeamManagers(string teamSlug)
    {
        try
        {
            var viewModel = await _teamService.PrepareTeamManagerEditViewModel(teamSlug, User);
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("edit-team-managers/{teamSlug}")]
    public async Task<IActionResult> EditTeamManagers(string teamSlug,
        TeamManagerEditViewModel viewModel)
    {
        try
        {
            await _teamService.AddTeamManager(viewModel, User);
            TempData["success"] = "Ο χρήστης προστέθηκε με επιτυχία.";
            return RedirectToAction("EditTeamManagers", new { teamSlug });
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("remove-team-manager/{teamSlug}")]
    public async Task<IActionResult> RemoveManager(string teamSlug, string managerId)
    {
        try
        {
            await _teamService.RemoveTeamManager(teamSlug, managerId, User);
            TempData["success"] = "Ο χρήστης αφαιρέθηκε με επιτυχία.";
            return RedirectToAction("EditTeamManagers", new { teamSlug });
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("manage-team-media/{teamSlug}")]
    public async Task<IActionResult> TeamMediaManagement(string teamSlug)
    {
        try
        {
            var viewModel = await _teamService.GetTeamMedia(teamSlug, User);
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("upload-team-media/{teamSlug}")]
    public async Task<IActionResult> UploadTeamMedia(string teamSlug, TeamMediaViewModel viewModel)
    {
        try
        {
            await _teamService.AddTeamMedia(teamSlug, viewModel.NewMediaFiles, User);
            TempData["success"] = "Τα αρχεία ανέβηκαν επιτυχώς.";
            return RedirectToAction("TeamMediaManagement", new { teamSlug });
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("delete-team-media/{teamSlug}")]
    public async Task<IActionResult> DeleteTeamMedia(string teamSlug, int mediaId)
    {
        try
        {
            await _teamService.DeleteTeamMedia(teamSlug, mediaId, User);
            TempData["success"] = "Τα αρχείο διαγράφηκε επιτυχώς.";
            return RedirectToAction("TeamMediaManagement", new { teamSlug });
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("team-actions/{teamSlug}")]
    public async Task<IActionResult> TeamActions(string teamSlug)
    {
        try
        {
            var viewModel = await _teamService.GetTeamDetails(teamSlug, User);
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
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
            return RedirectToAction("Dashboard", "Home");
        }
        catch (Exception)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }
}