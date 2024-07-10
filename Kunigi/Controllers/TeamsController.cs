using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        var viewModel = await context.Teams.ProjectTo<TeamDetailsViewModel>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(x => x.Id == id);
        if (viewModel != null) return View(viewModel);

        return RedirectToAction("List");
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
            Slug = slug
        });
        await context.SaveChangesAsync();

        TempData["success"] = "Η ομάδα δημιουργήθηκε.";
        return RedirectToAction("List");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("List");
        }

        var viewModel = await context.Teams.ProjectTo<TeamEditViewModel>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(x => x.Id == id);
        if (viewModel != null) return View(viewModel);

        return RedirectToAction("List");
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Edit(TeamEditViewModel viewModel, IFormFile profileImage)
    {
        if (viewModel.Id <= 0)
        {
            return RedirectToAction("List");
        }

        var team = await context.Teams.SingleOrDefaultAsync(x => x.Id == viewModel.Id);
        if (team == null)
        {
            return RedirectToAction("List");
        }

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

            team.ImageUrl = Path.Combine(productPath, fileName);
        }

        context.Teams.Update(team);
        await context.SaveChangesAsync();

        TempData["success"] = $"Η ομάδα {team.Name} επεξεργάστηκε επιτυχώς.";
        return RedirectToAction("List");
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
}