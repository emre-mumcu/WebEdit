namespace WebEdit.AppLib;

// https://csharpindepth.com/articles/singleton (Sixth version)
public sealed class App
{
	private static readonly Lazy<App> appInstance = new Lazy<App>(() => new App());

	public static App Instance { get { return appInstance.Value; } }

	private App()
    {
		// Lazy<T> sayesinde ilk erişimde çalışır:
		// Program.cs içinden App.Init(app.Services); çağrıldığında new App() tetiklenir
		// Sonraki her App.Instance çağrısında constructor çalışmaz, mevcut instance döner.
	}

	public IHttpContextAccessor HttpContextAccessor { get; private set; } = null!;
	public IConfiguration Configuration { get; private set; } = null!;
	public IWebHostEnvironment WebHostEnvironment { get; private set; } = null!;

	public static void Init(WebApplication app)
	{
		Instance.HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
		Instance.Configuration = app.Configuration;
		Instance.WebHostEnvironment = app.Environment;
	}
}
