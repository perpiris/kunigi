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
        var resultsCount = context.Teams.Count();

        var pageInfo = new PageInfo(resultsCount, pageIndex);
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

        var itemList = await context.Teams.ToListAsync();
        var exists = itemList.Any(x => x.Name.Equals(viewModel.Name, StringComparison.OrdinalIgnoreCase));
        if (exists)
        {
            ModelState.AddModelError("Name", "Υπάρχει ήδη ομάδα με αυτό το όνομα.");
            return View();
        }

        context.Teams.Add(new Team
        {
            Name = viewModel.Name,
            Slug = SlugGenerator.GenerateSlug(viewModel.Name)
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

        var selected = await context.Teams.SingleOrDefaultAsync(x => x.Id == viewModel.Id);
        if (selected == null)
        {
            return RedirectToAction("List");
        }

        selected.Facebook = viewModel.Facebook;
        selected.Youtube = viewModel.Youtube;
        selected.Instagram = viewModel.Instagram;
        selected.Website = viewModel.Website;

        if (profileImage != null)
        {
            var wwwRootPath = webHostEnvironment.WebRootPath;
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var productPath = $"images/{selected.Slug}/";
            var finalPath = Path.Combine(wwwRootPath, productPath);

            if (!Directory.Exists(finalPath))
                Directory.CreateDirectory(finalPath);

            var filePath = Path.Combine(finalPath, fileName);

            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(fileStream);
            }

            selected.ImageUrl = Path.Combine(productPath, fileName);
        }

        context.Teams.Update(selected);
        await context.SaveChangesAsync();

        TempData["success"] = $"Η ομάδα {selected.Name} επεξεργάστηκε επιτυχώς.";
        return RedirectToAction("List");
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Management(int pageIndex = 1)
    {
        var resultsCount = context.Teams.Count();

        var pageInfo = new PageInfo(resultsCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var viewModel = await context.Teams.Skip(skip).Take(pageInfo.PageSize)
            .ProjectTo<TeamDetailsViewModel>(mapper.ConfigurationProvider).ToListAsync();
        return View(viewModel);
    }
}