using Microsoft.EntityFrameworkCore;
using WebEdit.AppData.Entities;

namespace WebEdit.AppData;

// dotnet ef migrations add InitialCreate -o AppData/Migrations
// dotnet ef database update

public class AppDbContext : DbContext
{
	public static IConfiguration? _Configuration { get; set; }
	
	// public DbSet<WebDocEntity> WebDocs => Set<WebDocEntity>();
	public DbSet<WebDocEntity> WebDocs { get; set; }
	public DbSet<UserSetting> UserSettings { get; set; }
	public DbSet<ExceptionLogEntity> Exceptions { get; set; }

	public AppDbContext() { }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// TODO To enable debugging in Migrations, uncomment following:
		// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();  

		base.OnConfiguring(optionsBuilder);

#if DEBUG
		optionsBuilder.EnableSensitiveDataLogging();
#endif

		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlite("Data Source=webedit.db");
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// TODO To enable debugging during Migrations, uncomment following:
		// if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch(); 

		modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
	}
}
