using AutoMapper;
using AutoMapper.QueryableExtensions;
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

public class TeamsController(DataContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment) : Controller
{
    [HttpGet]
    public async Task<IActionResult> List(int pageIndex = 1)
    {
        var resultCount = context.Teams.Count();

        var pageInfo = new PageInfo(resultCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var viewModel = await context.Teams.Skip(skip).Take(pageInfo.PageSize)
            .ProjectTo<TeamDetailsViewModel>(mapper.ConfigurationProvider).ToListAsync();
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
    public async Task<IActionResult> Create(TeamCreateViewModel viewModel)
    {
        if (!ModelState.IsValid) return View();

        var teamList = await context.Teams.ToListAsync();
        var exists = teamList.Any(x => x.Name.Equals(viewModel.Name, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            ModelState.AddModelError("Name", "Υπάρχει ήδη ομάδα με αυτό το όνομα.");
            return View();
        }
        
        var slug = SlugGenerator.GenerateSlug(viewModel.Name);
        var wwwRootPath = webHostEnvironment.WebRootPath;
        var imagePath = Path.Combine(wwwRootPath, "media", "teams", slug);
        if (!Directory.Exists(imagePath))
        {
            Directory.CreateDirectory(imagePath);
        }

        context.Teams.Add(new Team
        {
            Name = viewModel.Name,
            Description = viewModel.Description,
            ProfileImageUrl = viewModel.ProfileImageUrl,
            Facebook = viewModel.Facebook,
            Youtube = viewModel.Youtube,
            Instagram = viewModel.Instagram,
            Website = viewModel.Website,
            Slug = slug
        });
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

        var viewModel = await context.Teams.ProjectTo<TeamEditViewModel>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(x => x.Id == id);
        if (viewModel != null) return View(viewModel);

        return RedirectToAction("Manage");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Edit(TeamEditViewModel viewModel, IFormFile profileImage)
    {
        if (viewModel.Id <= 0)
        {
            return RedirectToAction("Manage");
        }

        var team = await context.Teams.SingleOrDefaultAsync(x => x.Id == viewModel.Id);
        if (team == null)
        {
            return RedirectToAction("Manage");
        }

        team.Description = viewModel.Description;
        team.Facebook = viewModel.Facebook;
        team.Youtube = viewModel.Youtube;
        team.Instagram = viewModel.Instagram;
        team.Website = viewModel.Website;

        if (profileImage != null)
        {
            var wwwRootPath = webHostEnvironment.WebRootPath;
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var productPath = $"images/{team.Slug}/";
            var finalPath = Path.Combine(wwwRootPath, productPath);

            if (!Directory.Exists(finalPath))
                Directory.CreateDirectory(finalPath);

            var filePath = Path.Combine(finalPath, fileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(fileStream);
            }

            team.ProfileImageUrl = Path.Combine(productPath, fileName);
        }

        context.Teams.Update(team);
        await context.SaveChangesAsync();

        TempData["success"] = $"Η ομάδα {team.Name} επεξεργάστηκε επιτυχώς.";
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
        var viewModel = await context.Teams.Skip(skip).Take(pageInfo.PageSize)
            .ProjectTo<TeamDetailsViewModel>(mapper.ConfigurationProvider).ToListAsync();
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
        var viewModel = mapper.Map<TeamDetailsViewModel>(team);
        var managerSelectList = new List<SelectListItem>
        {
            new() { Value = "", Text = "Επιλέξτε" }
        };

        managerSelectList.AddRange(users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.Email
        }));
        viewModel.ManagerSelectList = new SelectList(managerSelectList, "Value", "Text");
        
        viewModel.ManagerList = team.Managers.Select(m => new TeamManagerViewModel
        {
            Id = m.Id,
            Email = m.Email
        }).ToList();

        return View(viewModel);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditManagers(TeamDetailsViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var team = await context.Teams
                .Include(t => t.Managers)
                
                .SingleOrDefaultAsync(t => t.Id == viewModel.Id);

            if (team == null)
            {
                return RedirectToAction("Manage");
            }

            var users = await context.AppUsers.ToListAsync();
            viewModel.ManagerSelectList = new SelectList(users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.Email
            }).ToList(), "Value", "Text");

            viewModel.ManagerList = team.Managers.Select(m => new TeamManagerViewModel
            {
                Id = m.Id,
                Email = m.Email
            }).ToList();

            return View(viewModel);
        }

        var teamToUpdate = await context.Teams
            .Include(t => t.Managers)
            .SingleOrDefaultAsync(t => t.Id == viewModel.Id);

        if (teamToUpdate == null)
        {
            return RedirectToAction("Manage");
        }

        var selectedManager = await context.AppUsers
            .SingleOrDefaultAsync(u => u.Id == viewModel.ManagerId);

        if (selectedManager != null)
        {
            if (teamToUpdate.Managers.All(m => m.Id != selectedManager.Id))
            {
                teamToUpdate.Managers.Add(selectedManager);
            }
            else
            {
                TempData["error"] = "This user is already a manager.";
            }
        }
    
        await context.SaveChangesAsync();

        TempData["success"] = "The manager list has been updated.";
        return RedirectToAction("EditManagers", new { id = viewModel.Id });
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

            TempData["success"] = "The manager has been removed.";
        }
        else
        {
            TempData["error"] = "The manager was not found.";
        }

        return RedirectToAction("EditManagers", new { id = teamId });
    }

    private static TeamDetailsViewModel GetMappedDetailsViewModel(Team teamDetails)
    {
        var viewModel = new TeamDetailsViewModel
        {
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
}