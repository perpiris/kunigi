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
    public async Task<IActionResult> ParentGameList(int pageIndex = 1)
    {
        var parentGameList =
            await _context.ParentGames
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .ToListAsync();

        var viewModel =
            parentGameList
                .Select(GetMappedParentDetailsViewModel)
                .OrderBy(x => x.Year)
                .ToList();

        return View(viewModel);
    }

    [HttpGet("{gameYear}")]
    public async Task<IActionResult> GameDetails(string gameYear)
    {
        if (string.IsNullOrEmpty(gameYear))
        {
            return RedirectToAction("ParentGameList");
        }

        var parentGameDetails =
            await _context.ParentGames
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .Include(x => x.Games)
                .ThenInclude(x => x.GameType)
                .FirstOrDefaultAsync(x => x.Slug == gameYear.Trim());

        if (parentGameDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("ParentGameList");
        }

        var viewModel = GetFullMappedParentDetailsViewModel(parentGameDetails);
        return View(viewModel);
    }

    [HttpGet("create-parent-game")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateParentGame()
    {
        var viewModel = new ParentGameCreateOrEditViewModel
        {
            Year = (short)DateTime.Now.Year,
            Order = 1
        };

        await PrepareViewModel(viewModel);

        return View(viewModel);
    }

    [HttpPost("create-parent-game")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateParentGame(
        ParentGameCreateOrEditViewModel viewModel, IFormFile profileImage)
    {
        ModelState.Remove("Title");
        ModelState.Remove("Description");

        var extraErrors = false;

        var parentGameList = await _context.ParentGames.ToListAsync();
        var exists = parentGameList.Any(x => x.Year == viewModel.Year);
        if (exists)
        {
            extraErrors = true;
            ModelState.AddModelError("Year",
                "Έχει ήδη καταχωρηθεί παιχνίδι για αυτή τη χρονιά.");
        }
        else
        {
            ModelState.Remove("Year");
        }

        exists = parentGameList.Any(x => x.Order == viewModel.Order);
        if (exists)
        {
            extraErrors = true;
            ModelState.AddModelError("Order",
                "Έχει ήδη καταχωρηθεί παιχνίδι για αυτή τη σειρά.");
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
        var newParentGame = new ParentGame
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
            newParentGame.Games.Add(new Game
            {
                GameTypeId = gameTypeId
            });
        }

        _context.ParentGames.Add(newParentGame);
        await _context.SaveChangesAsync();

        var basePath = _configuration["ImageStoragePath"];
        var teamFolderPath = Path.Combine(basePath!, "games", slug);
        Directory.CreateDirectory(teamFolderPath);
        newParentGame.ParentGameFolderUrl = teamFolderPath;
        if (profileImage != null)
        {
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var relativePath = Path.Combine("games", slug, fileName);
            var absolutePath = Path.Combine(basePath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
            await using (var stream =
                         new FileStream(absolutePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            newParentGame.ProfileImageUrl =
                $"/media/{relativePath.Replace("\\", "/")}";
        }

        TempData["success"] = "Το παιχνίδι δημιουργήθηκε.";
        return RedirectToAction("ParentGameList");
    }

    [HttpGet("edit-parent-game/{gameYear}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditParentGame(string gameYear)
    {
        if (string.IsNullOrEmpty(gameYear))
        {
            return RedirectToAction("ParentGameList");
        }

        var parentGameDetails =
            await _context.ParentGames
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .FirstOrDefaultAsync(x => x.Slug == gameYear.Trim());

        if (parentGameDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("ParentGameList");
        }

        if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var team =
                await _context.Teams
                    .Include(t => t.Managers)
                    .FirstOrDefaultAsync(t =>
                        t.Managers.Any(m => m.Id == userId));

            if (team == null || team.Id != parentGameDetails.Host.Id)
            {
                TempData["error"] =
                    "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού.";
                return RedirectToAction("ParentGameList");
            }
        }

        var viewModel = GetMappedCreateOrEditViewModel(parentGameDetails);
        return View(viewModel);
    }

    [HttpPost("edit-parent-game/{gameYear}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> EditParentGame(string gameYear,
        ParentGameCreateOrEditViewModel viewModel, IFormFile profileImage)
    {
        if (string.IsNullOrEmpty(gameYear))
        {
            return RedirectToAction("ParentGameList");
        }

        var parentGameDetails =
            await _context.ParentGames
                .Include(x => x.Host)
                .SingleOrDefaultAsync(x => x.Slug == gameYear.Trim());

        if (parentGameDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("ParentGameList");
        }

        if (User.IsInRole("Manager") && !User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var team = await _context.Teams
                .Include(t => t.Managers)
                .FirstOrDefaultAsync(t => t.Managers.Any(m => m.Id == userId));

            if (team == null || team.Id != parentGameDetails.Host.Id)
            {
                TempData["error"] =
                    "Δεν έχετε δικαίωμα επεξεργασίας αυτού του παιχνιδιού.";
                return RedirectToAction("ParentGameList");
            }
        }

        parentGameDetails.Title = viewModel.Title;
        parentGameDetails.Description = viewModel.Description;

        if (profileImage != null)
        {
            var basePath = _configuration["ImageStoragePath"];
            var teamFolderPath =
                Path.Combine(basePath!, "games", parentGameDetails.Slug);
            var fileName = "profile" + Path.GetExtension(profileImage.FileName);
            var filePath = Path.Combine(teamFolderPath, fileName);

            if (!Directory.Exists(teamFolderPath))
            {
                Directory.CreateDirectory(teamFolderPath);
            }

            await using (var fileStream = new FileStream(filePath,
                             FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await profileImage.CopyToAsync(fileStream);
            }

            var relativePath = Path
                .Combine("games", parentGameDetails.Slug, fileName)
                .Replace("\\", "/");
            parentGameDetails.ProfileImageUrl = $"/media/{relativePath}";
        }

        _context.ParentGames.Update(parentGameDetails);
        await _context.SaveChangesAsync();

        TempData["success"] = "Το παιχνίδι επεξεργάστηκε επιτυχώς.";
        return RedirectToAction("ParentGameList");
    }

    [HttpGet("manage")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ParentGameManagement(int pageIndex = 1)
    {
        var parentGameList =
            await _context.ParentGames
                .Include(x => x.Host)
                .Include(x => x.Winner)
                .ToListAsync();

        var viewModel =
            parentGameList
                .Select(GetMappedParentDetailsViewModel)
                .OrderBy(x => x.Year)
                .ToList();

        return View(viewModel);
    }

    [HttpGet("list/{gameYear}")]
    public async Task<IActionResult> GameList(string gameYear)
    {
        if (string.IsNullOrEmpty(gameYear))
        {
            return RedirectToAction("ParentGameList");
        }

        var parentGameDetails =
            await _context
                .ParentGames
                .Include(x => x.Games)
                .ThenInclude(x => x.GameType)
                .FirstOrDefaultAsync(x => x.Slug == gameYear.Trim());

        if (parentGameDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("ParentGameList");
        }

        var viewModel =
            parentGameDetails
                .Games
                .Select(GetMappedGameDetailsViewModel)
                .ToList();

        return View(viewModel);
    }
    
    [HttpGet("manage/{gameYear}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ParentGameActions(string gameYear)
    {
        if (string.IsNullOrEmpty(gameYear))
        {
            return RedirectToAction("ParentGameList");
        }

        var parentGameDetails =
            await _context
                .ParentGames
                .Include(x => x.Games)
                .ThenInclude(x => x.GameType)
                .FirstOrDefaultAsync(x => x.Slug == gameYear.Trim());

        if (parentGameDetails is null)
        {
            TempData["error"] = "Το παιχνίδι δεν υπάρχει.";
            return RedirectToAction("ParentGameList");
        }

        var viewModel =
            parentGameDetails
                .Games
                .Select(GetMappedGameDetailsViewModel)
                .ToList();

        return View(viewModel);
    }

    private async Task PrepareViewModel(
        ParentGameCreateOrEditViewModel viewModel)
    {
        var teamList = await _context.Teams.ToListAsync();
        var gameTypes = await _context.GameTypes.ToListAsync();
        teamList.Insert(0, new Team { Id = 0, Name = "Επιλέξτε..." });

        viewModel.HostSelectList = new SelectList(teamList, "Id", "Name", 0);
        viewModel.WinnerSelectList = new SelectList(teamList, "Id", "Name", 0);

        viewModel.GameTypes = gameTypes;
    }

    private static ParentGameDetailsViewModel GetMappedParentDetailsViewModel(
        ParentGame parentGameDetails)
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
    
    private static ParentGameDetailsViewModel GetFullMappedParentDetailsViewModel(
        ParentGame parentGameDetails)
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
            HostSlug = parentGameDetails.Host.Slug,
            GameList = []
        };
        
        foreach (var gameDetails in parentGameDetails.Games)
        {
            viewModel.GameList.Add(new GameDetailsViewModel
            {
                Id = gameDetails.Id,
                GameType = gameDetails.GameType.Description,
                Year = parentGameDetails.Year.ToString(),
                Slug = gameDetails.GameType.Slug
            });
        }

        return viewModel;
    }

    private static GameDetailsViewModel GetMappedGameDetailsViewModel(
        Game gameDetails)
    {
        var viewModel = new GameDetailsViewModel
        {
            Id = gameDetails.Id,
            Description = gameDetails.Description,
            GameType = gameDetails.GameType.Description
        };

        return viewModel;
    }

    private static ParentGameCreateOrEditViewModel
        GetMappedCreateOrEditViewModel(ParentGame parentGameDetails)
    {
        var viewModel = new ParentGameCreateOrEditViewModel
        {
            Id = parentGameDetails.Id,
            Title = parentGameDetails.Title,
            Description = parentGameDetails.Description
        };

        return viewModel;
    }
}