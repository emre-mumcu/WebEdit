```cs
// Background Writer (Channel tabanlı, sıfır blocking)
public sealed class ExceptionLogChannel
{
	private readonly Channel<ExceptionLogEntity> _channel =
		Channel.CreateBounded<ExceptionLogEntity>(new BoundedChannelOptions(512)
		{
			FullMode = BoundedChannelFullMode.DropOldest,
			SingleReader = true
		});

	public ChannelReader<ExceptionLogEntity> Reader => _channel.Reader;

	public void TryWrite(ExceptionLogEntity log) => _channel.Writer.TryWrite(log);
}

public sealed class ExceptionLogWriterService(
	ExceptionLogChannel channel,
	IServiceScopeFactory scopeFactory,
	ILogger<ExceptionLogWriterService> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken ct)
	{
		await foreach (var log in channel.Reader.ReadAllAsync(ct))
		{
			try
			{
				await using var scope = scopeFactory.CreateAsyncScope();
				var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
				db.Exceptions.Add(log);
				await db.SaveChangesAsync(ct);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "ExceptionLog yazılamadı.");
			}
		}
	}
}

public sealed class GlobalExceptionHandler(
	ExceptionLogChannel channel,
	IHttpContextAccessor accessor) : IExceptionHandler
{
	public ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken ct)
	{
		channel.TryWrite(BuildLog(httpContext, exception, isHandled: false));
		return ValueTask.FromResult(false); // pipeline'ı durdurmaz
	}

	internal static ExceptionLogEntity BuildLog(
		HttpContext ctx,
		Exception ex,
		bool isHandled,
		int? statusCode = null)
	{
		var inner = ex.InnerException;

		return new ExceptionLogEntity
		{
			ExceptionType = ex.GetType().FullName ?? ex.GetType().Name,
			Message = ex.Message,
			StackTrace = ex.StackTrace,
			InnerException = inner is null ? null : $"{inner.GetType().Name}: {inner.Message}",
			Path = ctx.Request.Path,
			Method = ctx.Request.Method,
			QueryString = ctx.Request.QueryString.HasValue
								? ctx.Request.QueryString.Value : null,
			StatusCode = statusCode ?? ctx.Response.StatusCode,
			UserId = ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
			IsHandled = isHandled,
			CreatedBy = nameof(GlobalExceptionHandler),
			CreatedAt = DateTime.UtcNow
		};
	}
}

// Handle edilmiş exceptionlar için:
public static class ExceptionLogExtensions
{
	//Kullanımı: catch (MyBusinessException ex) { ex.LogToDb(HttpContext, _channel); } 
	public static void LogToDb(
		this Exception ex,
		HttpContext ctx,
		ExceptionLogChannel channel,
		int? statusCode = null)
	{
		channel.TryWrite(
			GlobalExceptionHandler.BuildLog(ctx, ex, isHandled: true, statusCode));
	}
}

// Kullanımı: builder.Services.AddExceptionLogging();
// app.UseExceptionHandler(_ => { }); // IExceptionHandler'ları aktif eder
// UseExceptionHandler(_ => { }) boş pipeline trick'i — middleware'i aktive eder ama hiçbir şeyi override etmez, kendi error sayfana/handler'ına müdahale etmez.
public static class ExceptionLoggingExtensions
{
	public static IServiceCollection AddExceptionLogging(
		this IServiceCollection services)
	{
		services.AddSingleton<ExceptionLogChannel>();
		services.AddHostedService<ExceptionLogWriterService>();
		services.AddExceptionHandler<GlobalExceptionHandler>();
		services.AddHttpContextAccessor();
		return services;
	}
}


```