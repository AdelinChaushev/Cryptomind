using Cryptomind.Data.Entities;
using Microsoft.AspNetCore.Identity;

public static class ApplicationBuilderExtensions
{
	public static async Task SeedDatabaseAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var services = scope.ServiceProvider;
		try
		{
			var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

			string[] roleNames = { "Admin", "User" };
			foreach (var roleName in roleNames)
			{
				if (!await roleManager.RoleExistsAsync(roleName))
					await roleManager.CreateAsync(new IdentityRole(roleName));
			}

			await SeedUserAsync(userManager, "admin@cryptomind.com", "Admin123!", "Admin");
			await SeedUserAsync(userManager, "user@cryptomind.com", "User123!", "User");
		}
		catch (Exception ex)
		{
			var logger = services.GetRequiredService<ILogger<Program>>();
			logger.LogError(ex, "An error occurred while seeding the database.");
		}
	}

	private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, string email, string password, string role)
	{
		if (await userManager.FindByEmailAsync(email) != null) return;

		var user = new ApplicationUser
		{
			UserName = email,
			Email = email,
			EmailConfirmed = true
		};

		var result = await userManager.CreateAsync(user, password);
		if (result.Succeeded)
			await userManager.AddToRoleAsync(user, role);
	}
}