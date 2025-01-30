using Microsoft.AspNetCore.Identity;
using MvcProject.Models;

namespace MvcProject.Data;

public static class ApplicationDbInitializer
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        if (!await roleManager.RoleExistsAsync("Player"))
        {
            await roleManager.CreateAsync(new IdentityRole("Player"));
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        const string adminEmail = "admin@gmail.com";
        const string adminPassword = "Admin23$";
        const string username = "admin@gmail.com";

        if (await userManager.FindByEmailAsync(adminEmail) != null)
        {
            return;
        }

        var newAdmin = new User
        {
            Email = adminEmail,
            UserName = username,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(newAdmin, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}
