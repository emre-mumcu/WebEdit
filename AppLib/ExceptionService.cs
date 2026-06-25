using System.Security.Claims;
using WebEdit.AppData;
using WebEdit.AppData.Entities;

namespace WebEdit.AppLib;

public interface IExceptionLogger
{
	Task LogAsync(Exception ex, HttpContext context, int? statusCode = null);
}

public class EfExceptionLogger(AppDbContext _db) : IExceptionLogger
{
	public async Task LogAsync(Exception ex, HttpContext context, int? statusCode = null)
	{
		try
		{
			var log = new DbLogEntity
			{
				UserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "undefined",
				Path = context.Request.Path,
				Method = context.Request.Method,
				StatusCode = context.Response.StatusCode,
				Message = ex.Message,
				StackTrace = ex.ToString(),
				InnerException = ex?.InnerException?.ToString(),
				IsHandled = true,
				ExceptionType = "",

				CreatedAt = DateTime.UtcNow,
				CreatedBy = context.User?.Identity?.Name,				
				ClientIP = context.GetClientIpAddress(),
			};

			_db.DbLogs.Add(log);

			await _db.SaveChangesAsync();
		}
		catch
		{
			// logging failure asla sistemi kırmamalı
		}
	}
}

public class ExceptionLoggingMiddleware
{
	private readonly RequestDelegate _next;

	public ExceptionLoggingMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context, IExceptionLogger logger)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await logger.LogAsync(ex, context, 500);

			throw; // pipeline devam etsin (UseExceptionHandler vs isterse)
		}
	}
}

// Kullanımı:app.UseEfExceptionLogging(); builder.Services.AddEfExceptionLogging();

public static class ExceptionLoggingExtensions
{
	public static IServiceCollection AddEfExceptionLogging(this IServiceCollection services)
	{
		services.AddScoped<IExceptionLogger, EfExceptionLogger>();
		return services;
	}

	public static IApplicationBuilder UseEfExceptionLogging(this IApplicationBuilder app)
	{
		return app.UseMiddleware<ExceptionLoggingMiddleware>();
	}

	public static Task LogToDbAsync(this Exception ex)
	{
		try
		{
			var logger = App.Instance.HttpContextAccessor.HttpContext!.RequestServices.GetRequiredService<IExceptionLogger>();
			var context = App.Instance.HttpContextAccessor.HttpContext;
			return logger.LogAsync(ex, context);			
		}
		catch (System.Exception)
		{
			throw;
		}
	}
}