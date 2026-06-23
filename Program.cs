using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebEdit.AppData;
using WebEdit.AppLib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=webedit.db"));

// builder.Services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();

builder.Services.AddSingleton<IEncryptionService>(sp => new AesGcmEncryptionService("BenimGizliParolam"));


builder.Services.AddDataProtection();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddDataProtection();

builder.Services
	.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/dev-login";
		options.AccessDeniedPath = "/dev-denied";
	});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSession();

app.UseStaticFiles();

app.MapControllers(); // API + attribute routing

app.MapDefaultControllerRoute(); // MVC Views

app.UseAuthentication();

app.Use(async (context, next) =>
{
	if (app.Environment.IsDevelopment() && context.User?.Identity?.IsAuthenticated != true)
	{

	}

	await next();
});

app.UseAuthorization();

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
	logger.Log(logLevel: LogLevel.Error, exception: ex, message: "Application Error!");
}

if (app.Environment.IsDevelopment())
{
	app.MapGet("/dev-login", async context =>
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.Name, "Bob Bobbity"),
			new Claim(ClaimTypes.NameIdentifier, "bbobbity"),
			new Claim(ClaimTypes.Role, "Administrator"),
			new Claim(ClaimTypes.Role, "User"),
			new Claim(ClaimTypes.Role, "Guest")
		};

		var principal = new ClaimsPrincipal(
			new ClaimsIdentity(
				claims,
				CookieAuthenticationDefaults.AuthenticationScheme));

		await context.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal);

		context.Response.Redirect("/");
	});

	app.MapGet("/dev-logout", async context =>
	{
		await context.SignOutAsync(
			CookieAuthenticationDefaults.AuthenticationScheme);

		context.Response.Redirect("/");
	});
}

app.Run();




/*
builder.Services.AddSingleton<IEncryptionService>(sp =>
{
	return new AesGcmEncryptionService("BenimGizliParolam");
});

builder.Services.AddSingleton<IEncryptionService>(sp =>
{
	var config = sp.GetRequiredService<IConfiguration>();

	var password = config["Encryption:Password"]
		?? throw new InvalidOperationException();

	return new AesGcmEncryptionService(password);
});


Options Pattern

public class EncryptionOptions
{
    public string Password { get; set; } = string.Empty;
}

builder.Services.Configure<EncryptionOptions>(builder.Configuration.GetSection("Encryption"));

public AesGcmEncryptionService(IOptions<EncryptionOptions> options)
{
    var password = options.Value.Password;
    ...
}

builder.Services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();

*/
