using Kunigi.Entities;
using Kunigi.Utilities;
using Kunigi.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Controllers;

public class AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
    : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var result =
            await signInManager.PasswordSignInAsync(model.Email, model.Password, true,
                lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid login attempt.");

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (User.Identity is { IsAuthenticated: true })
        {
            return RedirectToAction("Index", "Home");
        }

        if (!ModelState.IsValid) return View(model);
        var user = new AppUser { UserName = model.Email, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage(int pageIndex = 1)
    {
        var resultcount = userManager.Users.Count();
        var pageInfo = new PageInfo(resultcount, pageIndex);
        var skip = (pageIndex - 1) * pageInfo.PageSize;
        ViewBag.PageInfo = pageInfo;

        var users =
            await userManager.Users
                .Skip(skip)
                .Take(pageInfo.PageSize)
                .ToListAsync();
        var userList = new List<UserDetailsUpdateViewModel>();

        foreach (var user in users)
        {
            var userRoles = await userManager.GetRolesAsync(user);
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditRoles(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            TempData["error"] = "User not found";
            return RedirectToAction("Manage");
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var roleList = await roleManager.Roles.Select(r => r.Name).ToListAsync();
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditRoles(UserDetailsUpdateViewModel userDetailsViewModel, List<string> selectedRoles)
    {
        var user = await userManager.FindByIdAsync(userDetailsViewModel.Id);
        if (user == null)
        {
            TempData["error"] = "User not found";
            return RedirectToAction("Manage");
        }

        var userRoles = await userManager.GetRolesAsync(user);
        
        foreach (var role in userRoles)
        {
            if (!selectedRoles.Contains(role))
            {
                await userManager.RemoveFromRoleAsync(user, role);
            }
        }
        
        foreach (var role in selectedRoles.Where(role => !userRoles.Contains(role)))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        TempData["success"] = "Roles updated successfully";
        return RedirectToAction("Manage");
    }
    
    [HttpGet("dashboard")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Dashboard()
    {
        return View();
    }
}