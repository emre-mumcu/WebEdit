using System;
using Microsoft.EntityFrameworkCore;

namespace WebEdit.AppData;

public static class DatabaseMigrationExtension
{
	public static async Task ApplyMigrationsAsync(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		
		var services = scope.ServiceProvider;

		try
		{
			var context = services.GetRequiredService<AppDbContext>();
			await context.Database.MigrateAsync();
		}
		catch (Exception ex)
		{
			var logger = services.GetRequiredService<ILogger<Program>>();
			logger.LogError(ex, "Application Error while migrating database!");
		}
	}
}
