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

builder.Services.AddAuthentication();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSession();

app.UseStaticFiles();

app.MapControllers(); // API + attribute routing

app.MapDefaultControllerRoute(); // MVC Views

app.UseAuthentication();

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
