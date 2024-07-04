﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels.Game;
using Kunigi.ViewModels.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

public class GamesController(DataContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment) : Controller
{
    [HttpGet]
    public async Task<IActionResult> List(int pageIndex = 1)
    {
        var resultsCount = context.Games.Count();
        
        var pageInfo = new PageInfo(resultsCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;
        
        var viewModel = await context.Games.Skip(skip).Take(pageInfo.PageSize).ProjectTo<GameDetailsViewModel>(mapper.ConfigurationProvider).ToListAsync();
        return View(viewModel);
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("List");
        }

        var viewModel = await context.Games.ProjectTo<GameDetailsViewModel>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(x => x.Id == id);
        if (viewModel != null) return View(viewModel);
        
        return RedirectToAction("List");
    }
    
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new GameCreateViewModel();
        await PrepareViewModel(viewModel);

        viewModel.Year = (short)DateTime.Now.Year;
        
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(GameCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await PrepareViewModel(viewModel);
            return View(viewModel);
        }

        var extraErrors = false;

        if (viewModel.HostId <=0)
        {
            extraErrors = true;
            ModelState.AddModelError("HostId", "Παρακαλώ επιλέξτε ομάδα.");
        }
        else
        {
            ModelState.Remove("HostId");
        }
        
        if (viewModel.WinnerId <=0)
        {
            extraErrors = true;
            ModelState.AddModelError("WinnerId", "Παρακαλώ επιλέξτε ομάδα.");
        }
        else
        {
            ModelState.Remove("WinnerId");
        }

        if (extraErrors)
        {
            await PrepareViewModel(viewModel);
            return View(viewModel);
        }
        
        context.Games.Add(new Game
        {
        });
        await context.SaveChangesAsync();

        TempData["success"] = "Το παιχνίδι δημιουργήθηκε.";
        return RedirectToAction("List");
    }
    
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("List");
        }

        var viewModel = await context.Games.ProjectTo<GameEditViewModel>(mapper.ConfigurationProvider).SingleOrDefaultAsync(x => x.Id == id);
        if (viewModel != null) return View(viewModel);
        
        return RedirectToAction("List");
    }

    [HttpPost]
    public async Task<IActionResult> Edit(GameDetailsViewModel viewModel, IFormFile profileImage)
    {
        if (viewModel.Id <= 0)
        {
            return RedirectToAction("List");
        }
        
        var selected = await context.Games.SingleOrDefaultAsync(x => x.Id == viewModel.Id);
        if (selected == null)
        {
            return RedirectToAction("List");
        }

        selected.Title = viewModel.Title;
        selected.Description = viewModel.Description;

        // if (profileImage != null)
        // {
        //     var wwwRootPath = _webHostEnvironment.WebRootPath;
        //     var fileName = "profile" + Path.GetExtension(profileImage.FileName);
        //     var productPath = $@"images\teams\{selectedTeam.Name}\";
        //     var finalPath = Path.Combine(wwwRootPath, productPath);
        //
        //     if (!Directory.Exists(finalPath))
        //         Directory.CreateDirectory(finalPath);
        //
        //     await using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create)) {
        //         await profileImage.CopyToAsync(fileStream);
        //     }
        //     
        //     selectedTeam.ImageUrl = productPath + fileName;
        // }

        context.Games.Update(selected);
        await context.SaveChangesAsync();
        
        TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
        return RedirectToAction("List");
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Manage(int pageIndex = 1)
    {
        var resultsCount = context.Games.Count();

        var pageInfo = new PageInfo(resultsCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var viewModel = await context.Games.Skip(skip).Take(pageInfo.PageSize)
            .ProjectTo<GameDetailsViewModel>(mapper.ConfigurationProvider).ToListAsync();
        return View(viewModel);
    }

    private async Task PrepareViewModel(GameCreateViewModel viewModel)
    {
        var teamList = await context.Teams.ToListAsync();
        teamList.Insert(0, new Team { Id = 0, Name = "Επιλέξτε..." });
        
        viewModel.HostSelectList = new SelectList(teamList, "Id", "Name", 0);
        viewModel.WinnerSelectList = new SelectList(teamList, "Id", "Name", 0);
    }
}