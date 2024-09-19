using Kunigi.Entities;
using Kunigi.ViewModels.Account;
using Kunigi.ViewModels.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

[Route("account")]
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet("manage-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UserManagement(int pageNumber = 1, int pageSize = 10)
    {
        var query = _userManager.Users.AsQueryable();
        var totalItems = await query.CountAsync();
        
        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var paginatedUsers = new List<AppUserDetailsViewModel>();
        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            paginatedUsers.Add(new AppUserDetailsViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserRoles = userRoles.ToList()
            });
        }

        var viewModel = new PaginatedViewModel<AppUserDetailsViewModel>
        {
            Items = paginatedUsers,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> EditRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            TempData["error"] = "User not found";
            return RedirectToAction("UserManagement");
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var roleList =
            await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        var viewModel = new AppUserDetailsViewModel
        {
            Id = userId,
            Email = user.Email,
            UserRoles = userRoles.ToList(),
            RoleList = roleList
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditRoles(
        AppUserDetailsViewModel appUserDetailsViewModel,
        List<string> selectedRoles)
    {
        var user = await _userManager.FindByIdAsync(appUserDetailsViewModel.Id);
        if (user == null)
        {
            TempData["error"] = "User not found";
            return RedirectToAction("UserManagement");
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in userRoles)
        {
            if (!selectedRoles.Contains(role))
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }
        }

        foreach (var role in selectedRoles.Where(role =>
                     !userRoles.Contains(role)))
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        TempData["success"] = "Roles updated successfully";
        return RedirectToAction("UserManagement");
    }
}