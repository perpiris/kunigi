﻿using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels;
using Kunigi.ViewModels.Game;
using Kunigi.ViewModels.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

[Route("teams")]
public class TeamController : Controller
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;

    public TeamController(
        DataContext context,
        IConfiguration configuration,
        UserManager<AppUser> userManager)
    {
        _context = context;
        _configuration = configuration;
        _userManager = userManager;
    }

    [HttpGet("list")]
    public async Task<IActionResult> TeamList()
    {
        var teamList =
            await _context
                .Teams
                .ToListAsync();

        var viewModel =
            teamList
                .Select(GetFullMappedDetailsViewModel)
                .OrderBy(x => x.Name)
                .ToList();

        return View(viewModel);
    }

    [HttpGet("{teamSlug}")]
    public async Task<IActionResult> TeamDetails(string teamSlug)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamList");
        }

        var teamDetails =
            await _context.Teams
                .Include(x => x.HostedYears.OrderBy(y => y.Year))
                .Include(x => x.WonYears.OrderBy(y => y.Year))
                .Include(x => x.MediaFiles)
                .ThenInclude(x => x.MediaFile)
                .FirstOrDefaultAsync(x => x.Slug == teamSlug.Trim());

        if (teamDetails is null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει";
            return RedirectToAction("TeamList");
        }

        var viewModel = GetFullMappedDetailsViewModel(teamDetails);
        return View(viewModel);
    }

    [HttpGet("create")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateTeam()
    {
        return View();
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateTeam(TeamCreateOrEditViewModel viewModel)
    {
        if (!ModelState.IsValid) return View();

        var teamList = await _context.Teams.ToListAsync();
        var exists = teamList.Any(x =>
            x.Name.Equals(viewModel.Name, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            ModelState.AddModelError("Name", "Υπάρχει ήδη ομάδα με αυτό το όνομα.");
            return View(viewModel);
        }

        var slug = SlugGenerator.GenerateSlug(viewModel.Name);
        var newTeam = new Team
        {
            Name = viewModel.Name,
            Slug = slug
        };

        var basePathFromConfig = _configuration["ImageStoragePath"];
        var teamFolderPath = Path.Combine(basePathFromConfig!, "teams", slug);
        Directory.CreateDirectory(teamFolderPath);
        newTeam.TeamFolderUrl = teamFolderPath;

        _context.Teams.Add(newTeam);
        await _context.SaveChangesAsync();

        TempData["success"] = "Η ομάδα δημιουργήθηκε.";
        return RedirectToAction("TeamManagement");
    }

    [HttpGet("edit/{teamSlug}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditTeam(string teamSlug)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamManagement");
        }

        var teamDetails =
            await _context.Teams
                .Include(team => team.Managers)
                .SingleOrDefaultAsync(x => x.Slug == teamSlug.Trim());

        if (teamDetails == null)
        {
            return RedirectToAction("TeamManagement");
        }

        if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (teamDetails.Managers.All(x => x.Id != userId))
            {
                TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
                return RedirectToAction("TeamList");
            }
        }

        var viewModel = GetMappedCreateOrEditViewModel(teamDetails);
        if (viewModel != null) return View(viewModel);

        return RedirectToAction("TeamManagement");
    }

    [HttpPost("edit/{teamSlug}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditTeam(string teamSlug,
        TeamCreateOrEditViewModel updatedTeamDetails,
        IFormFile profileImage)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamManagement");
        }

        var teamDetails =
            await _context.Teams
                .Include(team => team.Managers)
                .SingleOrDefaultAsync(x => x.Slug == teamSlug.Trim());

        if (teamDetails == null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει";
            return RedirectToAction("TeamManagement");
        }

        if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (teamDetails.Managers.All(x => x.Id != userId))
            {
                TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
                return RedirectToAction("TeamList");
            }
        }

        teamDetails.Description = updatedTeamDetails.Description;
        teamDetails.Facebook = updatedTeamDetails.Facebook;
        teamDetails.Youtube = updatedTeamDetails.Youtube;
        teamDetails.Instagram = updatedTeamDetails.Instagram;
        teamDetails.Website = updatedTeamDetails.Website;

        if (profileImage != null)
        {
            var basePathFromConfig = _configuration["ImageStoragePath"];
            var teamFolderPath = Path.Combine(basePathFromConfig!, "teams", teamDetails.Slug);
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var filePath = Path.Combine(teamFolderPath, fileName);

            if (!Directory.Exists(teamFolderPath))
            {
                Directory.CreateDirectory(teamFolderPath);
            }

            await using (var fileStream = new FileStream(filePath, FileMode.Create,
                             FileAccess.Write, FileShare.None))
            {
                await profileImage.CopyToAsync(fileStream);
            }

            var relativePath = Path.Combine("teams", teamDetails.Slug, fileName).Replace("\\", "/");
            teamDetails.ProfileImageUrl = $"/media/{relativePath}";
        }

        _context.Teams.Update(teamDetails);
        await _context.SaveChangesAsync();

        TempData["success"] = "Οι αλλαγές αποθηκεύτηκαν επιτυχώς.";
        return RedirectToAction("TeamManagement");
    }


    [HttpGet("manage")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> TeamManagement()
    {
        var teamList =
            await _context
                .Teams
                .ToListAsync();

        var viewModel =
            teamList
                .Select(GetFullMappedDetailsViewModel)
                .OrderBy(x => x.Name)
                .ToList();

        return View(viewModel);
    }

    [HttpGet("edit-managers/{teamSlug}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditTeamManagers(string teamSlug)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamList");
        }

        var teamToUpdate = await _context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());

        if (teamToUpdate == null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει";
            return RedirectToAction("TeamManagement");
        }

        var users = await _context.AppUsers.ToListAsync();
        var managerSelectList = new List<SelectListItem>
        {
            new() { Value = "", Text = "Επιλέξτε" }
        };

        managerSelectList.AddRange(users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Email
        }));

        var viewModel = new TeamManagerEditViewModel
        {
            Slug = teamToUpdate.Slug,
            TeamName = teamToUpdate.Name,
            ManagerSelectList = new SelectList(managerSelectList, "Value", "Text"),
            ManagerList = teamToUpdate.Managers.Select(m => new TeamManagerDetailsViewModel
            {
                Id = m.Id,
                Email = m.Email
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost("edit-managers/{teamSlug}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditTeamManagers(string teamSlug,
        TeamManagerEditViewModel viewModel)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamList");
        }

        if (!ModelState.IsValid)
        {
            await PopulateUpdateManagerViewModel(viewModel);
            return View(viewModel);
        }

        var teamToUpdate = await _context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());

        if (teamToUpdate == null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει";
            return RedirectToAction("TeamManagement");
        }

        var selectedManager = await _context.AppUsers
            .SingleOrDefaultAsync(u => u.Id == viewModel.SelectedManagerId);

        if (selectedManager != null)
        {
            var isInManagerRole = await _userManager.IsInRoleAsync(selectedManager, "Manager");

            if (!isInManagerRole)
            {
                var result = await _userManager.AddToRoleAsync(selectedManager, "Manager");
                if (!result.Succeeded)
                {
                    TempData["error"] = "Αποτυχία προσθήκης του χρήστη στο ρόλο του Διαχειριστή.";
                    return RedirectToAction("EditTeamManagers", new { id = viewModel.Slug });
                }
            }

            if (teamToUpdate.Managers.All(m => m.Id != selectedManager.Id))
            {
                teamToUpdate.Managers.Add(selectedManager);

                var teamManager = new TeamManager
                {
                    TeamId = teamToUpdate.Id,
                    AppUserId = selectedManager.Id
                };

                _context.TeamManagers.Add(teamManager);
                await _context.SaveChangesAsync();

                TempData["success"] = "Ο διαχειριστής προστέθηκε επιτυχώς.";
            }
            else
            {
                TempData["error"] = "Αυτός ο χρήστης είναι ήδη διαχειριστής.";
            }
        }
        else
        {
            TempData["error"] = "Δεν βρέθηκε ο επιλεγμένος διαχειριστής.";
        }

        return RedirectToAction("EditTeamManagers", new { id = viewModel.Slug });
    }

    [HttpPost("remove-manager/{teamSlug}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveManager(string teamSlug, string managerId)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamList");
        }

        var teamToUpdate = await _context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());

        if (teamToUpdate == null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει";
            return RedirectToAction("TeamManagement");
        }

        var managerToRemove = teamToUpdate.Managers.SingleOrDefault(m => m.Id == managerId);
        if (managerToRemove != null)
        {
            teamToUpdate.Managers.Remove(managerToRemove);
            var teamManager = await _context.TeamManagers
                .SingleOrDefaultAsync(tm =>
                    tm.TeamId == teamToUpdate.Id && tm.AppUserId == managerId);
            if (teamManager != null)
            {
                _context.TeamManagers.Remove(teamManager);
            }

            await _context.SaveChangesAsync();

            TempData["success"] = "Ο διαχειριστής αφαιρέθηκε επιτυχώς.";
        }
        else
        {
            TempData["error"] = "Δεν βρέθηκε ο επιλεγμένος διαχειριστής.";
        }

        return RedirectToAction("EditTeamManagers", new { teamSlug });
    }

    [HttpGet("manage-media/{teamSlug}/")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> TeamMediaManagement(string teamSlug)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamManagement");
        }

        var teamDetails = await _context.Teams
            .Include(team => team.Managers)
            .Include(t => t.MediaFiles)
            .ThenInclude(tm => tm.MediaFile)
            .SingleOrDefaultAsync(x => x.Slug == teamSlug.Trim());

        if (teamDetails == null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει";
            return RedirectToAction("TeamManagement");
        }

        if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (teamDetails.Managers.All(x => x.Id != userId))
            {
                TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτής της ομάδας.";
                return RedirectToAction("TeamList");
            }
        }

        var basePath = _configuration["ImageStoragePath"];
        var viewModel = new TeamMediaViewModel
        {
            TeamSlug = teamDetails.Slug,
            MediaFiles = teamDetails.MediaFiles.Select(tm => new MediaFileViewModel
            {
                Id = tm.MediaFile.Id,
                FileName = Path.GetFileName(tm.MediaFile.Path),
                Path = CrossPlatformPathUtility.CombineAndNormalize(basePath, tm.MediaFile.Path)
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost("upload-media/{teamSlug}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UploadMedia(string teamSlug, TeamMediaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(TeamMediaManagement), new { teamSlug });
        }

        var teamDetails = await _context.Teams
            .FirstOrDefaultAsync(t => t.Slug == teamSlug);

        if (teamDetails == null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει";
            return RedirectToAction(nameof(TeamMediaManagement), new { teamSlug });
        }

        var uploadPath =
            Path.Combine(_configuration["ImageStoragePath"]!, "teams", teamDetails.Slug);
        Directory.CreateDirectory(uploadPath);

        foreach (var file in model.NewMediaFiles)
        {
            var fileName = FileNameGenerator.GetUniqueFileName(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var mediaFile = new MediaFile
            {
                Path = $"/media/teams/{teamDetails.Slug}/{fileName}"
            };

            _context.MediaFiles.Add(mediaFile);
            await _context.SaveChangesAsync();

            var teamMedia = new TeamMedia
            {
                TeamId = teamDetails.Id,
                MediaFileId = mediaFile.Id
            };

            _context.TeamMediaFiles.Add(teamMedia);
        }

        await _context.SaveChangesAsync();

        TempData["success"] = "Media files uploaded successfully.";
        return RedirectToAction("TeamMediaManagement", new { teamSlug });
    }

    [HttpPost("delete-media{teamSlug}/")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteMedia(string teamSlug, int mediaId)
    {
        var media = await _context.MediaFiles.FindAsync(mediaId);
        if (media == null)
        {
            return RedirectToAction(nameof(TeamMediaManagement), new { teamSlug });
        }

        var filePath = Path.Combine(_configuration["ImageStoragePath"]!, media.Path.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _context.MediaFiles.Remove(media);
        await _context.SaveChangesAsync();

        TempData["success"] = "Media file deleted successfully.";
        return RedirectToAction("TeamMediaManagement", new { teamSlug });
    }

    [HttpGet("manage/{teamSlug}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> TeamActions(string teamSlug)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("Dashboard", "Home");
        }

        var viewModel = await _context.Teams
            .Include(x => x.HostedYears.OrderBy(y => y.Year))
            .Include(x => x.WonYears.OrderBy(y => y.Year))
            .Where(x => x.Slug == teamSlug.Trim())
            .Select(teamDetails => GetFullMappedDetailsViewModel(teamDetails))
            .SingleOrDefaultAsync();

        return View(viewModel);
    }

    private static TeamDetailsViewModel GetFullMappedDetailsViewModel(Team teamDetails)
    {
        var viewModel = new TeamDetailsViewModel
        {
            Name = teamDetails.Name,
            Slug = teamDetails.Slug,
            Description = teamDetails.Description,
            ProfileImageUrl = teamDetails.ProfileImageUrl,
            Facebook = teamDetails.Facebook,
            Youtube = teamDetails.Youtube,
            Instagram = teamDetails.Instagram,
            Website = teamDetails.Website,
            GamesWon = [],
            GamesHosted = [],
            MediaFiles = []
        };

        if (teamDetails.WonYears != null)
        {
            foreach (var year in teamDetails.WonYears)
            {
                viewModel.GamesWon.Add(new ParentGameDetailsViewModel
                {
                    Id = year.Id,
                    Title = year.Title,
                    Year = year.Year
                });
            }
        }

        if (teamDetails.HostedYears != null)
        {
            foreach (var year in teamDetails.HostedYears)
            {
                viewModel.GamesHosted.Add(new ParentGameDetailsViewModel
                {
                    Id = year.Id,
                    Title = year.Title,
                    Year = year.Year
                });
            }
        }

        if (teamDetails.MediaFiles != null)
        {
            foreach (var teamMedia in teamDetails.MediaFiles)
            {
                viewModel.MediaFiles.Add(new MediaFileViewModel
                {
                    Id = teamMedia.MediaFile.Id,
                    FileName = Path.GetFileName(teamMedia.MediaFile.Path),
                    Path = teamMedia.MediaFile.Path
                });
            }
        }

        return viewModel;
    }

    private static TeamCreateOrEditViewModel GetMappedCreateOrEditViewModel(Team teamDetails)
    {
        var viewModel = new TeamCreateOrEditViewModel
        {
            Name = teamDetails.Name,
            Description = teamDetails.Description,
            ProfileImageUrl = teamDetails.ProfileImageUrl,
            Facebook = teamDetails.Facebook,
            Youtube = teamDetails.Youtube,
            Instagram = teamDetails.Instagram,
            Website = teamDetails.Website
        };

        return viewModel;
    }

    private async Task PopulateUpdateManagerViewModel(TeamManagerEditViewModel viewModel)
    {
        var team = await _context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Slug == viewModel.Slug);

        if (team != null)
        {
            var users = await _context.AppUsers.ToListAsync();
            viewModel.TeamName = team.Name;
            viewModel.ManagerSelectList = new SelectList(users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email
            }), "Value", "Text");

            viewModel.ManagerList = team.Managers.Select(m => new TeamManagerDetailsViewModel
            {
                Id = m.Id,
                Email = m.Email
            }).ToList();
        }
    }
}