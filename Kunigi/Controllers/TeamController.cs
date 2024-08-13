using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels.GameYear;
using Kunigi.ViewModels.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

[Route("teams")]
public class TeamController(DataContext context, IConfiguration configuration, UserManager<AppUser> userManager)
    : Controller
{
    [HttpGet("list")]
    public async Task<IActionResult> TeamList(int pageIndex = 1)
    {
        var resultCount = await context.Teams.CountAsync();
        var pageInfo = new PageInfo(resultCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var teamList =
            await context.Teams
                .Include(t => t.WonYears)
                .Include(t => t.HostedYears)
                .Skip(skip)
                .Take(pageInfo.PageSize)
                .ToListAsync();

        var viewModel =
            teamList
                .Select(GetMappedDetailsViewModel)
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

        var teamDetails = await context
            .Teams
            .Include(x => x.HostedYears.OrderBy(y => y.Year))
            .Include(x => x.WonYears.OrderBy(y => y.Year))
            .FirstOrDefaultAsync(x => x.Slug == teamSlug.Trim());

        if (teamDetails is null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει.";
            return RedirectToAction("TeamList");
        }

        var viewModel = GetMappedDetailsViewModel(teamDetails);
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
    public async Task<IActionResult> CreateTeam(TeamCreateOrEditViewModel viewModel, IFormFile profileImage)
    {
        if (!ModelState.IsValid) return View();

        var teamList = await context.Teams.ToListAsync();
        var exists = teamList.Any(x => x.Name.Equals(viewModel.Name, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            ModelState.AddModelError("Name", "Υπάρχει ήδη ομάδα με αυτό το όνομα.");
            return View(viewModel);
        }

        var slug = SlugGenerator.GenerateSlug(viewModel.Name);
        var newTeam = new Team
        {
            Name = viewModel.Name,
            Description = viewModel.Description,
            ProfileImageUrl = viewModel.ProfileImageUrl,
            Facebook = viewModel.Facebook,
            Youtube = viewModel.Youtube,
            Instagram = viewModel.Instagram,
            Website = viewModel.Website,
            Slug = slug
        };

        var basePath = configuration["ImageStoragePath"];
        var teamFolderPath = Path.Combine(basePath!, "teams", slug);
        Directory.CreateDirectory(teamFolderPath);
        newTeam.TeamFolderUrl = teamFolderPath;
        if (profileImage != null)
        {
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var relativePath = Path.Combine("teams", slug, fileName);
            var absolutePath = Path.Combine(basePath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
            await using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            newTeam.ProfileImageUrl = $"/media/{relativePath.Replace("\\", "/")}";
        }

        context.Teams.Add(newTeam);
        await context.SaveChangesAsync();

        TempData["success"] = "Η ομάδα δημιουργήθηκε.";
        return RedirectToAction("TeamManagement");
    }

    [HttpGet("{teamSlug}/edit")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditTeam(string teamSlug)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamManagement");
        }

        var teamDetails =
            await context.Teams
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
                TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού.";
                return RedirectToAction("TeamList");
            }
        }

        var viewModel = GetMappedCreateOrEditViewModel(teamDetails);
        if (viewModel != null) return View(viewModel);

        return RedirectToAction("TeamManagement");
    }

    [HttpPost("{teamSlug}/edit")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditTeam(string teamSlug, TeamCreateOrEditViewModel updatedTeamDetails, IFormFile profileImage)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamManagement");
        }

        var teamDetails =
            await context.Teams
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
                TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού.";
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
            var basePath = configuration["ImageStoragePath"];
            var teamFolderPath = Path.Combine(basePath!, "teams", teamDetails.Slug);
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var filePath = Path.Combine(teamFolderPath, fileName);

            if (!Directory.Exists(teamFolderPath))
            {
                Directory.CreateDirectory(teamFolderPath);
            }

            await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await profileImage.CopyToAsync(fileStream);
            }

            var relativePath = Path.Combine("teams", teamDetails.Slug, fileName).Replace("\\", "/");
            teamDetails.ProfileImageUrl = $"/media/{relativePath}";
        }

        context.Teams.Update(teamDetails);
        await context.SaveChangesAsync();

        TempData["success"] = $"Η ομάδα {teamDetails.Name} επεξεργάστηκε επιτυχώς.";
        return RedirectToAction("TeamManagement");
    }


    [HttpGet("manage")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> TeamManagement(int pageIndex = 1)
    {
        var resultcount = context.Teams.Count();
        var pageInfo = new PageInfo(resultcount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var teamList = await context.Teams
            .Include(t => t.WonYears)
            .Include(t => t.HostedYears)
            .Skip(skip)
            .Take(pageInfo.PageSize)
            .ToListAsync();

        var viewModel = teamList.Select(GetMappedDetailsViewModel).ToList();
        return View(viewModel);
    }

    [HttpGet("{teamSlug}/edit-managers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditTeamManagers(string teamSlug)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamList");
        }

        var teamToUpdate = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());

        if (teamToUpdate == null)
        {
            return RedirectToAction("TeamManagement");
        }

        var users = await context.AppUsers.ToListAsync();
        var managerSelectList = new List<SelectListItem>
        {
            new() { Value = "", Text = "Επιλέξτε" }
        };

        managerSelectList.AddRange(users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Email
        }));

        var viewModel = new TeamManagerUpdateViewModel
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

    [HttpPost("{teamSlug}/edit-managers")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditTeamManagers(string teamSlug, TeamManagerUpdateViewModel viewModel)
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

        var teamToUpdate = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());

        if (teamToUpdate == null)
        {
            return RedirectToAction("TeamManagement");
        }

        var selectedManager = await context.AppUsers
            .SingleOrDefaultAsync(u => u.Id == viewModel.SelectedManagerId);

        if (selectedManager != null)
        {
            var isInManagerRole = await userManager.IsInRoleAsync(selectedManager, "Manager");

            if (!isInManagerRole)
            {
                var result = await userManager.AddToRoleAsync(selectedManager, "Manager");
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
                
                context.TeamManagers.Add(teamManager);
                await context.SaveChangesAsync();
                
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

    [HttpPost("{teamSlug}/remove-manager")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveManager(string teamSlug, string managerId)
    {
        if (string.IsNullOrEmpty(teamSlug))
        {
            return RedirectToAction("TeamList");
        }

        var teamToUpdate = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Slug == teamSlug.Trim());

        if (teamToUpdate == null)
        {
            return RedirectToAction("TeamManagement");
        }

        var managerToRemove = teamToUpdate.Managers.SingleOrDefault(m => m.Id == managerId);
        if (managerToRemove != null)
        {
            teamToUpdate.Managers.Remove(managerToRemove);
            var teamManager = await context.TeamManagers
                .SingleOrDefaultAsync(tm => tm.TeamId == teamToUpdate.Id && tm.AppUserId == managerId);
            if (teamManager != null)
            {
                context.TeamManagers.Remove(teamManager);
            }

            await context.SaveChangesAsync();

            TempData["success"] = "Ο διαχειριστής αφαιρέθηκε επιτυχώς.";
        }
        else
        {
            TempData["error"] = "Δεν βρέθηκε ο επιλεγμένος διαχειριστής.";
        }

        return RedirectToAction("EditTeamManagers", new { teamSlug });
    }

    private static TeamDetailsViewModel GetMappedDetailsViewModel(Team teamDetails)
    {
        var viewModel = new TeamDetailsViewModel
        {
            Id = teamDetails.Id,
            Name = teamDetails.Name,
            Slug = teamDetails.Slug,
            Description = teamDetails.Description,
            ProfileImageUrl = teamDetails.ProfileImageUrl,
            Facebook = teamDetails.Facebook,
            Youtube = teamDetails.Youtube,
            Instagram = teamDetails.Instagram,
            Website = teamDetails.Website,
            GamesWon = [],
            GamesHosted = []
        };

        foreach (var year in teamDetails.WonYears)
        {
            viewModel.GamesWon.Add(new GameYearDetailsViewModel
            {
                Id = year.Id,
                Title = year.Title,
                Year = year.Year
            });
        }

        foreach (var year in teamDetails.HostedYears)
        {
            viewModel.GamesHosted.Add(new GameYearDetailsViewModel
            {
                Id = year.Id,
                Title = year.Title,
                Year = year.Year
            });
        }

        return viewModel;
    }

    private static TeamCreateOrEditViewModel GetMappedCreateOrEditViewModel(Team teamDetails)
    {
        var viewModel = new TeamCreateOrEditViewModel
        {
            Id = teamDetails.Id,
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
    
    private async Task PopulateUpdateManagerViewModel(TeamManagerUpdateViewModel viewModel)
    {
        var team = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Slug == viewModel.Slug);

        if (team != null)
        {
            var users = await context.AppUsers.ToListAsync();
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