using System.Security.Claims;
using Kunigi.Data;
using Kunigi.ViewModels.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

public class HomeController : Controller
{
    private readonly DataContext _context;

    public HomeController(DataContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }

    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Dashboard()
    {
        if (User.IsInRole("Manager"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var teamList =
                await _context.Teams
                    .Include(x => x.Managers)
                    .Where(x => x.Managers.Any(y => y.Id.Equals(userId)))
                    .Select(x => new TeamDetailsViewModel { Name = x.Name, Slug = x.Slug})
                    .ToListAsync();
            
            ViewBag.TeamList = teamList;
        }
        
        return View();
    }
}