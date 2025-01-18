using Microsoft.AspNetCore.Identity;
using MvcProject.Models;

namespace MvcProject.Data;

public static class ApplicationDbInitializer
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        // Check whether Player role exists and if not create it.
        if (!await roleManager.RoleExistsAsync("Player"))
        {
            await roleManager.CreateAsync(new IdentityRole("Player"));
        }

        // Check whether Admin role exists and if not create it.
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create Admin
        const string adminEmail = "admin@gmail.com";
        const string adminPassword = "Admin23$";
        const string username = "admin@gmail.com";

        // Check whether such user exist
        if (await userManager.FindByEmailAsync(adminEmail) != null)
        {
            return;
        }

        // Create new user
        var newAdmin = new User
        {
            Email = adminEmail,
            UserName = username,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(newAdmin, adminPassword);

        //Add admin role
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}
