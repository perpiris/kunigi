﻿using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

[Route("games")]
public class GameController : Controller
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public GameController(
        DataContext context, 
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GameList(int pageIndex = 1)
    {
        var resultCount = _context.GameYears.Count();
        var pageInfo = new PageInfo(resultCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var gameYearList =
            await _context.GameYears
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .Skip(skip)
                .Take(pageInfo.PageSize)
                .ToListAsync();

        var viewModel =
            gameYearList
                .Select(GetMappedDetailsViewModel)
                .OrderBy(x => x.Year)
                .ToList();

        return View(viewModel);
    }

    [HttpGet("{gameYear}")]
    public async Task<IActionResult> GameDetails(string gameYear)
    {
        if (string.IsNullOrEmpty(gameYear))
        {
            return RedirectToAction("GameList");
        }

        var gameYearDetails =
            await _context.GameYears
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .FirstOrDefaultAsync(x => x.Slug == gameYear.Trim());

        if (gameYearDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("GameList");
        }

        var viewModel = GetMappedDetailsViewModel(gameYearDetails);
        return View(viewModel);
    }

    [HttpGet("create-parent-game")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateParentGame()
    {
        var viewModel = new ParentGameCreateOrUpdateViewModel
        {
            Year = (short)DateTime.Now.Year,
            Order = 1
        };

        await PrepareViewModel(viewModel);

        return View(viewModel);
    }

    [HttpPost("create-parent-game")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateParentGame(ParentGameCreateOrUpdateViewModel viewModel, IFormFile profileImage)
    {
        ModelState.Remove("Title");
        ModelState.Remove("Description");

        var extraErrors = false;

        var gameYearList = await _context.GameYears.ToListAsync();
        var exists = gameYearList.Any(x => x.Year == viewModel.Year);
        if (exists)
        {
            extraErrors = true;
            ModelState.AddModelError("Year", "Έχει ήδη καταχωρηθεί παιχνίδι για αυτή τη χρονιά.");
        }
        else
        {
            ModelState.Remove("Year");
        }

        exists = gameYearList.Any(x => x.Order == viewModel.Order);
        if (exists)
        {
            extraErrors = true;
            ModelState.AddModelError("Order", "Έχει ήδη καταχωρηθεί παιχνίδι για αυτή τη σειρά.");
        }
        else
        {
            ModelState.Remove("Order");
        }

        if (viewModel.HostId <= 0)
        {
            extraErrors = true;
            ModelState.AddModelError("HostId", "Παρακαλώ επιλέξτε ομάδα.");
        }
        else
        {
            ModelState.Remove("HostId");
        }

        if (viewModel.WinnerId <= 0)
        {
            extraErrors = true;
            ModelState.AddModelError("WinnerId", "Παρακαλώ επιλέξτε ομάδα.");
        }
        else
        {
            ModelState.Remove("WinnerId");
        }

        if (!ModelState.IsValid || extraErrors)
        {
            await PrepareViewModel(viewModel);
            return View(viewModel);
        }

        var slug = viewModel.Year.ToString();
        var newGameYear = new ParentGame
        {
            Year = viewModel.Year,
            Order = viewModel.Order,
            Slug = slug,
            Title = "Τίτλος",
            Description = "Περιγραφή",
            WinnerId = viewModel.WinnerId,
            HostId = viewModel.HostId,
            Games = new List<Game>()
        };

        foreach (var gameTypeId in viewModel.SelectedGameTypeIds)
        {
            newGameYear.Games.Add(new Game
            {
                GameTypeId = gameTypeId
            });
        }

        _context.GameYears.Add(newGameYear);
        await _context.SaveChangesAsync();

        var basePath = _configuration["ImageStoragePath"];
        var teamFolderPath = Path.Combine(basePath!, "years", slug);
        Directory.CreateDirectory(teamFolderPath);
        newGameYear.TeamFolderUrl = teamFolderPath;
        if (profileImage != null)
        {
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var relativePath = Path.Combine("years", slug, fileName);
            var absolutePath = Path.Combine(basePath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
            await using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            newGameYear.ProfileImageUrl = $"/media/{relativePath.Replace("\\", "/")}";
        }

        TempData["success"] = "Το παιχνίδι δημιουργήθηκε.";
        return RedirectToAction("GameList");
    }

    [HttpGet("{gameYear}/edit-parent-game")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditParentGame(string gameYear)
    {
        if (string.IsNullOrEmpty(gameYear))
        {
            return RedirectToAction("GameList");
        }

        var gameYearDetails =
            await _context.GameYears
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .FirstOrDefaultAsync(x => x.Slug == gameYear.Trim());

        if (gameYearDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("GameList");
        }

        if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var team =
                await _context.Teams
                    .Include(t => t.Managers)
                    .FirstOrDefaultAsync(t => t.Managers.Any(m => m.Id == userId));

            if (team == null || team.Id != gameYearDetails.Host.Id)
            {
                TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού.";
                return RedirectToAction("GameList");
            }
        }

        var viewModel = GetMappedCreateOrEditViewModel(gameYearDetails);
        return View(viewModel);
    }

    [HttpPost("{gameYear}/edit-parent-game")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditParentGame(string gameYear, ParentGameCreateOrUpdateViewModel viewModel, IFormFile profileImage)
    {
        if (string.IsNullOrEmpty(gameYear))
        {
            return RedirectToAction("GameList");
        }

        var gameYearDetails =
            await _context.GameYears
                .Include(x => x.Host)
                .SingleOrDefaultAsync(x => x.Id == viewModel.Id);

        if (gameYearDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("GameList");
        }

        if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var team = await _context.Teams
                .Include(t => t.Managers)
                .FirstOrDefaultAsync(t => t.Managers.Any(m => m.Id == userId));

            if (team == null || team.Id != gameYearDetails.Host.Id)
            {
                TempData["error"] = "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού.";
                return RedirectToAction("GameList");
            }
        }

        gameYearDetails.Title = viewModel.Title;
        gameYearDetails.Description = viewModel.Description;
        gameYearDetails.Slug = SlugGenerator.GenerateSlug(viewModel.Title);

        if (profileImage != null)
        {
            var basePath = _configuration["ImageStoragePath"];
            var teamFolderPath = Path.Combine(basePath!, "years", gameYearDetails.Slug);
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

            var relativePath = Path.Combine("years", gameYearDetails.Slug, fileName).Replace("\\", "/");
            gameYearDetails.ProfileImageUrl = $"/media/{relativePath}";
        }

        _context.GameYears.Update(gameYearDetails);
        await _context.SaveChangesAsync();

        TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
        return RedirectToAction("GameList");
    }

    [HttpGet("manage")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GameYearManagement(int pageIndex = 1)
    {
        var resultCount = _context.GameYears.Count();
        var pageInfo = new PageInfo(resultCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var gameYearList =
            await _context.GameYears
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .Skip(skip)
                .Take(pageInfo.PageSize)
                .ToListAsync();

        var viewModel =
            gameYearList
                .Select(GetMappedDetailsViewModel)
                .OrderBy(x => x.Year)
                .ToList();

        return View(viewModel);
    }

    private async Task PrepareViewModel(ParentGameCreateOrUpdateViewModel viewModel)
    {
        var teamList = await _context.Teams.ToListAsync();
        var gameTypes = await _context.GameTypes.ToListAsync();
        teamList.Insert(0, new Team { Id = 0, Name = "Επιλέξτε..." });

        viewModel.HostSelectList = new SelectList(teamList, "Id", "Name", 0);
        viewModel.WinnerSelectList = new SelectList(teamList, "Id", "Name", 0);

        viewModel.GameTypes = gameTypes;
    }

    private static ParentGameDetailsViewModel GetMappedDetailsViewModel(ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameDetailsViewModel
        {
            Id = parentGameDetails.Id,
            Title = parentGameDetails.Title,
            Year = parentGameDetails.Year,
            Order = parentGameDetails.Order,
            ProfileImageUrl = parentGameDetails.ProfileImageUrl,
            Winner = parentGameDetails.Winner.Name,
            WinnerSlug = parentGameDetails.Winner.Slug,
            Host = parentGameDetails.Host.Name,
            HostSlug = parentGameDetails.Host.Slug
        };

        return viewModel;
    }

    private static ParentGameCreateOrUpdateViewModel GetMappedCreateOrEditViewModel(ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameCreateOrUpdateViewModel
        {
            Id = parentGameDetails.Id,
            Title = parentGameDetails.Title,
            Description = parentGameDetails.Description
        };

        return viewModel;
    }
}