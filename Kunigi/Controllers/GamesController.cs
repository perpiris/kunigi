using System.Security.Claims;
using Kunigi.Data;
using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

public class GamesController(DataContext context, IConfiguration configuration) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int pageIndex = 1)
    {
        var resultCount = context.GameYears.Count();

        var pageInfo = new PageInfo(resultCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var gameYearList =
            await context.GameYears
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

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("Index");
        }

        var gameYearDetails =
            await context.GameYears
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .FirstOrDefaultAsync(x => x.Id == id);

        if (gameYearDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("Index");
        }

        var viewModel = GetMappedDetailsViewModel(gameYearDetails);
        return View(viewModel);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Create()
    {
        var viewModel = new GameCreateOrUpdateViewModel
        {
            Year = (short)DateTime.Now.Year,
            Order = 1
        };

        await PrepareViewModel(viewModel);

        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Create(GameCreateOrUpdateViewModel viewModel, IFormFile profileImage)
    {
        ModelState.Remove("Title");
        ModelState.Remove("Description");

        if (!ModelState.IsValid)
        {
            await PrepareViewModel(viewModel);
            return View(viewModel);
        }

        var extraErrors = false;

        var gameYearList = await context.GameYears.ToListAsync();
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

        if (extraErrors)
        {
            await PrepareViewModel(viewModel);
            return View(viewModel);
        }

        var slug = viewModel.Year.ToString();
        var newGameYear = new GameYear
        {
            Year = viewModel.Year,
            Order = viewModel.Order,
            Slug = slug,
            Title = "Τίτλος Παιχνιδιού",
            Description = "περιγραφή παιχνιδιού",
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

        context.GameYears.Add(newGameYear);
        await context.SaveChangesAsync();

        var basePath = configuration["ImageStoragePath"];
        var teamFolderPath = Path.Combine(basePath!, "games", slug);
        Directory.CreateDirectory(teamFolderPath);
        newGameYear.TeamFolderUrl = teamFolderPath;
        if (profileImage != null)
        {
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var relativePath = Path.Combine("games", slug, fileName);
            var absolutePath = Path.Combine(basePath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
            await using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            newGameYear.ProfileImageUrl = $"/media/{relativePath.Replace("\\", "/")}";
        }

        TempData["success"] = "Το παιχνίδι δημιουργήθηκε.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return RedirectToAction("Index");
        }

        var gameYearDetails =
            await context.GameYears
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .FirstOrDefaultAsync(x => x.Id == id);

        if (gameYearDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("Index");
        }

        var viewModel = GetMappedCreateOrEditViewModel(gameYearDetails);
        return View(viewModel);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Moderator,Manager")]
    public async Task<IActionResult> Edit(GameCreateOrUpdateViewModel viewModel, IFormFile profileImage)
    {
        if (viewModel.Id <= 0)
        {
            return RedirectToAction("Index");
        }

        var gameYearDetails = await context.GameYears.SingleOrDefaultAsync(x => x.Id == viewModel.Id);
        if (gameYearDetails == null)
        {
            return RedirectToAction("Index");
        }

        gameYearDetails.Title = viewModel.Title;
        gameYearDetails.Description = viewModel.Description;

        if (profileImage != null)
        {
            var basePath = configuration["ImageStoragePath"];
            var teamFolderPath = Path.Combine(basePath!, "games", gameYearDetails.Slug);
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

            var relativePath = Path.Combine("teams", gameYearDetails.Slug, fileName).Replace("\\", "/");
            gameYearDetails.ProfileImageUrl = $"/media/{relativePath}";
        }

        context.GameYears.Update(gameYearDetails);
        await context.SaveChangesAsync();

        TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderator")]
    public async Task<IActionResult> Manage(int pageIndex = 1)
    {
        var resultCount = context.GameYears.Count();

        var pageInfo = new PageInfo(resultCount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var gameYearList =
            await context.GameYears
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

    [HttpGet]
    public async Task<IActionResult> ManagerDashboard()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var managedGames = await context.Teams
            .Where(t => t.Managers.Any(m => m.Id == userId))
            .SelectMany(t => t.HostedYears.SelectMany(gy => gy.Games))
            .Select(g => new ManagerViewModel
            {
                Id = g.Id,
                Description = g.Description,
                GameTypeName = g.GameType.Description,
                GameYearTitle = g.GameYear.Title,
                HostTeamName = g.GameYear.Host.Name
            })
            .ToListAsync();

        return View(managedGames);
    }

    private async Task PrepareViewModel(GameCreateOrUpdateViewModel viewModel)
    {
        var teamList = await context.Teams.ToListAsync();
        var gameTypes = await context.GameTypes.ToListAsync();
        teamList.Insert(0, new Team { Id = 0, Name = "Επιλέξτε..." });

        viewModel.HostSelectList = new SelectList(teamList, "Id", "Name", 0);
        viewModel.WinnerSelectList = new SelectList(teamList, "Id", "Name", 0);

        viewModel.GameTypes = gameTypes;
    }

    private static GameDetailsViewModel GetMappedDetailsViewModel(GameYear gameDetails)
    {
        var viewModel = new GameDetailsViewModel
        {
            Id = gameDetails.Id,
            Title = gameDetails.Title,
            Order = gameDetails.Order,
            ProfileImageUrl = gameDetails.ProfileImageUrl,
            Winner = gameDetails.Winner.Name,
            WinnerId = gameDetails.WinnerId,
            Host = gameDetails.Host.Name,
            HostId = gameDetails.HostId
        };

        return viewModel;
    }

    private static GameCreateOrUpdateViewModel GetMappedCreateOrEditViewModel(GameYear gameDetails)
    {
        var viewModel = new GameCreateOrUpdateViewModel
        {
            Id = gameDetails.Id,
            Title = gameDetails.Title,
            Description = gameDetails.Description
        };

        return viewModel;
    }
}