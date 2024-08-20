using Kunigi.Entities;
using Kunigi.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kunigi.Data;

public abstract class SeedData
{
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = ["Admin", "Manager"];

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

    public static async Task SeedGameTypes(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<DataContext>();
        if (await context.GameTypes.AnyAsync()) return;

        var gameTypes = new List<GameType>
        {
            new() { Description = "Χωρός", Slug = SlugGenerator.GenerateSlug("Χωρός") },
            new() { Description = "Σάββατο", Slug = SlugGenerator.GenerateSlug("Σάββατο") },
            new() { Description = "Κυριακή", Slug = SlugGenerator.GenerateSlug("Κυριακή") },
            new() { Description = "Διαδικτυακό", Slug = SlugGenerator.GenerateSlug("Διαδικτυακό") },
            new() { Description = "Παιδικό", Slug = SlugGenerator.GenerateSlug("Παιδικό") },
            new() { Description = "Εφηβικό", Slug = SlugGenerator.GenerateSlug("Εφηβικό") }
        };

        foreach (var gameType in gameTypes)
        {
            context.GameTypes.Add(gameType);
        }

        await context.SaveChangesAsync();
    }
}