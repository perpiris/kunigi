using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

[Route("account")]
[Authorize(Roles = "Admin")]
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

    [HttpGet("manage")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UserManagement(int pageIndex = 1)
    {
        var users =
            await _userManager.Users
                .ToListAsync();
        var userList = new List<UserDetailsUpdateViewModel>();

        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            userList.Add(new UserDetailsUpdateViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserRoles = userRoles.ToList()
            });
        }

        var viewModel = new ManageUserViewModel
        {
            UserList = userList
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
        var viewModel = new UserDetailsUpdateViewModel
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
        UserDetailsUpdateViewModel userDetailsViewModel,
        List<string> selectedRoles)
    {
        var user = await _userManager.FindByIdAsync(userDetailsViewModel.Id);
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