
using DrMarko.Models;
using Microsoft.AspNetCore.Identity;

namespace DrMarko.Data;

public class AuthSeed
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IConfiguration _configuration;

	public AuthSeed(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
	{
		_userManager = userManager;
		_roleManager = roleManager;
		_configuration = configuration;
	}
	public async Task SeedAdminUserAsync()
    {
        var adminUsername = _configuration["AdminUser:Email"];
        var adminPassword = _configuration["AdminUser:Password"];
        var adminRoleName = _configuration["AdminUser:RoleName"];

        if (string.IsNullOrWhiteSpace(adminUsername) || string.IsNullOrWhiteSpace(adminPassword) || string.IsNullOrWhiteSpace(adminRoleName))
        {
            throw new InvalidOperationException("AdminUser configuration is incomplete. Please check appsettings.json.");
        }

        // Create Admin role if it doesn't exist
        if (!await _roleManager.RoleExistsAsync(adminRoleName))
        {
            await _roleManager.CreateAsync(new IdentityRole(adminRoleName));
        }

        // Check if there is any admin user
        var existingAdmin = await _userManager.GetUsersInRoleAsync(adminRoleName);
        if (existingAdmin.Count != 0)
        {
            return;
        }
        // Create admin user if there's none
        var adminUser = new ApplicationUser { UserName = adminUsername, Email = adminUsername, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            // Assign admin role to admin user
            await _userManager.AddToRoleAsync(adminUser, adminRoleName);
        }
        else
        {
            throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}

