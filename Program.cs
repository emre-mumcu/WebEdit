using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebEdit.AppData;
using WebEdit.AppLib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=webedit.db"));

builder.Services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();

builder.Services.AddDataProtection();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services
	.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/login"; // 401
		options.LogoutPath = "/logout";
		options.AccessDeniedPath = "/access-denied"; // 403
	});

builder.Services.AddAuthorization();

builder.Services.AddEfExceptionLogging();

var app = builder.Build();

App.Init(app);

// UseExceptionHandler, ASP.NET Core’da uygulama içinde yakalanmayan (unhandled) hataları merkezi olarak yakalayıp tek bir yere yönlendiren middleware’dir.
app.UseExceptionHandler("/error");

app.UseHttpsRedirection();

// Static file (css, js, img) istekleri pipeline’a girmeden servis edilsin,
// Authentication / Authorization çalışmasın,
// Gereksiz CPU harcanmasın
app.UseStaticFiles();

// UseRouting      → Nereye gidiyorum
// UseAuth         → Gidebilir miyim
// MapControllers  → Orası neresi
app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.UseEfExceptionLogging();

app.UseStatusCodePages(async context =>
{
	var response = context.HttpContext.Response;

	var logger = context.HttpContext.RequestServices.GetRequiredService<IExceptionLogger>();

	if (response.StatusCode >= 400)
	{
		await logger.LogAsync(
			new Exception($"HTTP {response.StatusCode}"),
			context.HttpContext,
			response.StatusCode);
	}
});

app.MapControllers(); // API + attribute routing

app.MapDefaultControllerRoute(); // MVC Views

app.Use(async (context, next) =>
{
	if (app.Environment.IsDevelopment() && context.User?.Identity?.IsAuthenticated != true)
	{

	}

	await next();
});

if(app.Environment.IsDevelopment())
{
	await app.ApplyMigrationsAsync();
}

app.Run();