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
    public async Task<IActionResult> TeamList()
    {
        var viewModel = await _teamService.GetAllTeams();
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
        }
        catch (Exception)
        {
            TempData["error"] = "Η ομάδα δεν δημιουργήθηκε.";
        }

        return RedirectToAction("TeamManagement");
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

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost("edit-team/{teamSlug}")]
    public async Task<IActionResult> EditTeam(string teamSlug, TeamEditViewModel viewModel, IFormFile profileImage)
    {
        try
        {
            await _teamService.EditTeam(teamSlug, viewModel, profileImage, User);
            TempData["success"] = "Η ομάδα επεξεργάστηκε επιτυχώς.";
            return RedirectToAction("TeamActions", new { teamSlug });
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
    [HttpGet("manage-teams")]
    public async Task<IActionResult> TeamManagement()
    {
        var viewModel = await _teamService.GetAllTeams();
        return View(viewModel);
    }

    // [Authorize(Roles = "Admin,Manager")]
    // [HttpGet("edit-team-managers/{teamSlug}")]
    // public async Task<IActionResult> EditTeamManagers(string teamSlug)
    // {
    //     if (string.IsNullOrEmpty(teamSlug))
    //     {
    //         return RedirectToAction("Dashboard", "Home");
    //     }
    //
    //     var teamToUpdate = await _context.Teams
    //         .Include(t => t.Managers)
    //         .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());
    //
    //     if (teamToUpdate == null)
    //     {
    //         TempData["error"] = "Η ομάδα δεν υπάρχει";
    //         return RedirectToAction("Dashboard", "Home");
    //     }
    //
    //     var users = await _context.AppUsers.ToListAsync();
    //     var managerSelectList = new List<SelectListItem>
    //     {
    //         new() { Value = "", Text = "Επιλέξτε" }
    //     };
    //
    //     managerSelectList.AddRange(users.Select(u => new SelectListItem
    //     {
    //         Value = u.Id.ToString(),
    //         Text = u.Email
    //     }));
    //
    //     var viewModel = new TeamManagerEditViewModel
    //     {
    //         Slug = teamToUpdate.Slug,
    //         TeamName = teamToUpdate.Name,
    //         ManagerSelectList = new SelectList(managerSelectList, "Value", "Text"),
    //         ManagerList = teamToUpdate.Managers.Select(m => new TeamManagerDetailsViewModel
    //         {
    //             Id = m.Id,
    //             Email = m.Email
    //         }).ToList()
    //     };
    //
    //     return View(viewModel);
    // }

    // [Authorize(Roles = "Admin,Manager")]
    // [HttpPost("edit-team-managers/{teamSlug}")]
    // public async Task<IActionResult> EditTeamManagers(string teamSlug,
    //     TeamManagerEditViewModel viewModel)
    // {
    //     if (string.IsNullOrEmpty(teamSlug))
    //     {
    //         return RedirectToAction("TeamList");
    //     }
    //
    //     if (!ModelState.IsValid)
    //     {
    //         await PopulateUpdateManagerViewModel(viewModel);
    //         return View(viewModel);
    //     }
    //
    //     var teamToUpdate = await _context.Teams
    //         .Include(t => t.Managers)
    //         .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());
    //
    //     if (teamToUpdate == null)
    //     {
    //         TempData["error"] = "Η ομάδα δεν υπάρχει";
    //         return RedirectToAction("Dashboard", "Home");
    //     }
    //
    //     var selectedManager = await _context.AppUsers
    //         .SingleOrDefaultAsync(u => u.Id == viewModel.SelectedManagerId);
    //
    //     if (selectedManager != null)
    //     {
    //         var isInManagerRole = await _userManager.IsInRoleAsync(selectedManager, "Manager");
    //
    //         if (!isInManagerRole)
    //         {
    //             var result = await _userManager.AddToRoleAsync(selectedManager, "Manager");
    //             if (!result.Succeeded)
    //             {
    //                 TempData["error"] = "Αποτυχία προσθήκης του χρήστη στο ρόλο του Διαχειριστή.";
    //                 return RedirectToAction("EditTeamManagers", new { teamSlug = viewModel.Slug });
    //             }
    //         }
    //
    //         if (teamToUpdate.Managers.All(m => m.Id != selectedManager.Id))
    //         {
    //             teamToUpdate.Managers.Add(selectedManager);
    //
    //             var teamManager = new TeamManager
    //             {
    //                 TeamId = teamToUpdate.TeamId,
    //                 AppUserId = selectedManager.Id
    //             };
    //
    //             _context.TeamManagers.Add(teamManager);
    //             await _context.SaveChangesAsync();
    //
    //             TempData["success"] = "Ο διαχειριστής προστέθηκε επιτυχώς.";
    //         }
    //         else
    //         {
    //             TempData["error"] = "Αυτός ο χρήστης είναι ήδη διαχειριστής.";
    //         }
    //     }
    //     else
    //     {
    //         TempData["error"] = "Δεν βρέθηκε ο επιλεγμένος διαχειριστής.";
    //     }
    //
    //     return RedirectToAction("EditTeamManagers", new { teamSlug = viewModel.Slug });
    // }


    // [Authorize(Roles = "Admin,Manager")]
    // [HttpPost("remove-team-manager/{teamSlug}")]
    // public async Task<IActionResult> RemoveManager(string teamSlug, string managerId)
    // {
    //     if (string.IsNullOrEmpty(teamSlug))
    //     {
    //         return RedirectToAction("Dashboard", "Home");
    //     }
    //
    //     var teamToUpdate = await _context.Teams
    //         .Include(t => t.Managers)
    //         .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());
    //
    //     if (teamToUpdate == null)
    //     {
    //         TempData["error"] = "Η ομάδα δεν υπάρχει";
    //         return RedirectToAction("Dashboard", "Home");
    //     }
    //
    //     var managerToRemove = teamToUpdate.Managers.SingleOrDefault(m => m.Id == managerId);
    //     if (managerToRemove != null)
    //     {
    //         teamToUpdate.Managers.Remove(managerToRemove);
    //         var teamManager = await _context.TeamManagers
    //             .SingleOrDefaultAsync(tm =>
    //                 tm.TeamId == teamToUpdate.TeamId && tm.AppUserId == managerId);
    //         if (teamManager != null)
    //         {
    //             _context.TeamManagers.Remove(teamManager);
    //         }
    //
    //         await _context.SaveChangesAsync();
    //
    //         TempData["success"] = "Ο διαχειριστής αφαιρέθηκε επιτυχώς.";
    //     }
    //     else
    //     {
    //         TempData["error"] = "Δεν βρέθηκε ο επιλεγμένος διαχειριστής.";
    //     }
    //
    //     return RedirectToAction("EditTeamManagers", new { teamSlug });
    // }


    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("{teamSlug}/manage-team-media")]
    public async Task<IActionResult> TeamMediaManagement(string teamSlug)
    {
        try
        {
            var viewModel = await _teamService.GetTeamMedia(teamSlug, User);
            return View(viewModel);
        }
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
        }
        catch (Exception)
        {
            return RedirectToAction("TeamMediaManagement", new { teamSlug });
        }
        
        return RedirectToAction("TeamMediaManagement", new { teamSlug });
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
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
        }
        catch (Exception)
        {
            return RedirectToAction("TeamList");
        }

        return RedirectToAction("TeamList");
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
        catch (UnauthorizedOperationException)
        {
            TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
        }
        catch (Exception)
        {
            return RedirectToAction("TeamList");
        }

        return RedirectToAction("TeamList");
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet("team-actions/{teamSlug}")]
    public async Task<IActionResult> TeamActions(string teamSlug)
    {
        try
        {
            var viewModel = await _teamService.GetTeamDetails(teamSlug);
            return View(viewModel);
        }
        catch (ArgumentNullException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
        catch (NotFoundException)
        {
            return RedirectToAction("Dashboard", "Home");
        }
    }

    // private async Task PopulateUpdateManagerViewModel(TeamManagerEditViewModel viewModel)
    // {
    //     var team = await _context.Teams
    //         .Include(t => t.Managers)
    //         .SingleOrDefaultAsync(t => t.Slug == viewModel.Slug);
    //
    //     if (team != null)
    //     {
    //         var users = await _context.AppUsers.ToListAsync();
    //         viewModel.TeamName = team.Name;
    //         viewModel.ManagerSelectList = new SelectList(users.Select(u => new SelectListItem
    //         {
    //             Value = u.Id.ToString(),
    //             Text = u.Email
    //         }), "Value", "Text");
    //
    //         viewModel.ManagerList = team.Managers.Select(m => new TeamManagerDetailsViewModel
    //         {
    //             Id = m.Id,
    //             Email = m.Email
    //         }).ToList();
    //     }
    // }
}