using Kunigi.Entities;
using Microsoft.AspNetCore.Identity;

namespace Kunigi.Data;

public abstract class SeedData
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = ["Admin", "Moderator", "Manager"];

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (roleExist) continue;
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleResult.Succeeded)
            {
                throw new Exception($"Role creation failed: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }
    }

    public static async Task SeedMainAdmin(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        
        var user = new AppUser { UserName = "iperpirakis@gmail.com", Email = "iperpirakis@gmail.com" };
        var result = await userManager.CreateAsync(user, "0123456789");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}