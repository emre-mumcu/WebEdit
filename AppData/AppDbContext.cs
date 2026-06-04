using Microsoft.EntityFrameworkCore;
using WebEdit.AppData.Entities;

namespace WebEdit.AppData;

public class AppDbContext : DbContext
{
	public static IConfiguration? _Configuration { get; set; }
	
	public DbSet<WebDocEntity> WebDocs => Set<WebDocEntity>();


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
