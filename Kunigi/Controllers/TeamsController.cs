using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels.GameYear;
using Kunigi.ViewModels.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

public class TeamsController(DataContext context, IConfiguration configuration)
    : Controller
{
    [HttpGet]
    public async Task<IActionResult> List(int pageIndex = 1)
    {
        var resultCount = await context.Teams.CountAsync();

        var pageInfo = new PageInfo(resultCount, pageIndex);
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

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("List");
        }

        var teamDetails = await context
            .Teams
            .Include(x => x.HostedYears.OrderBy(y => y.Year))
            .Include(x => x.WonYears.OrderBy(y => y.Year))
            .FirstOrDefaultAsync(x => x.Id == id);

        if (teamDetails is null)
        {
            TempData["error"] = "Η ομάδα δεν υπάρχει.";
            return RedirectToAction("List");
        }

        var viewModel = GetMappedDetailsViewModel(teamDetails);
        return View(viewModel);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Create(TeamCreateOrEditViewModel viewModel, IFormFile profileImage)
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
        return RedirectToAction("Manage");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("Manage");
        }

        var teamDetails = await context.Teams.SingleOrDefaultAsync(x => x.Id == id);
        var viewModel = GetMappedCreateOrEditViewModel(teamDetails);
        if (viewModel != null) return View(viewModel);

        return RedirectToAction("Manage");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Edit(TeamCreateOrEditViewModel updatedTeamDetails, IFormFile profileImage)
    {
        if (updatedTeamDetails.Id <= 0)
        {
            return RedirectToAction("Manage");
        }

        var teamDetails = await context.Teams.SingleOrDefaultAsync(x => x.Id == updatedTeamDetails.Id);
        if (teamDetails == null)
        {
            return RedirectToAction("Manage");
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
        return RedirectToAction("Manage");
    }

    

    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Manage(int pageIndex = 1)
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

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditManagers(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("List");
        }

        var team = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Id == id);

        if (team == null)
        {
            return RedirectToAction("Manage");
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
            TeamId = team.Id,
            TeamName = team.Name,
            ManagerSelectList = new SelectList(managerSelectList, "Value", "Text"),
            ManagerList = team.Managers.Select(m => new TeamManagerDetailsViewModel
            {
                Id = m.Id,
                Email = m.Email
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditManagers(TeamManagerUpdateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await PopulateUpdateManagerViewModel(viewModel);
            return View(viewModel);
        }

        var teamToUpdate = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Id == viewModel.TeamId);

        if (teamToUpdate == null)
        {
            return RedirectToAction("Manage");
        }

        var selectedManager = await context.AppUsers
            .SingleOrDefaultAsync(u => u.Id == viewModel.SelectedManagerId);

        if (selectedManager != null)
        {
            if (teamToUpdate.Managers.All(m => m.Id != selectedManager.Id))
            {
                teamToUpdate.Managers.Add(selectedManager);
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

        return RedirectToAction("EditManagers", new { id = viewModel.TeamId });
    }

    private async Task PopulateUpdateManagerViewModel(TeamManagerUpdateViewModel viewModel)
    {
        var team = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Id == viewModel.TeamId);

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

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveManager(int teamId, string managerId)
    {
        var team = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Id == teamId);

        if (team == null)
        {
            return RedirectToAction("Manage");
        }

        var managerToRemove = team.Managers.SingleOrDefault(m => m.Id == managerId);
        if (managerToRemove != null)
        {
            team.Managers.Remove(managerToRemove);
            await context.SaveChangesAsync();
            
            TempData["success"] = "Ο διαχειριστής αφαιρέθηκε επιτυχώς.";
        }
        else
        {
            TempData["error"] = "Δεν βρέθηκε ο επιλεγμένος διαχειριστής.";
        }

        return RedirectToAction("EditManagers", new { id = teamId });
    }

    private static TeamDetailsViewModel GetMappedDetailsViewModel(Team teamDetails)
    {
        var viewModel = new TeamDetailsViewModel
        {
            Id = teamDetails.Id,
            Name = teamDetails.Name,
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
}