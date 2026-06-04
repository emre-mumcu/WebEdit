using Microsoft.EntityFrameworkCore;
using WebEdit.AppData;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=webedit.db"));

builder.Services.AddDataProtection();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();

app.MapControllers(); // API + attribute routing

app.MapDefaultControllerRoute(); // MVC Views

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
}

app.Run();
