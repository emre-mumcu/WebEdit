using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebEdit.AppData;
using WebEdit.AppLib;
using WebEdit.Models;
using WebEdit.ViewModels;

namespace WebEdit.Controllers
{
	public class CommonController(AppDbContext db) : Controller
	{
		private IQueryable<AppData.Entities.DbLogEntity> ApplyFilter(IQueryable<AppData.Entities.DbLogEntity> query, ModelFilter modelFilter)
		{
			if (!string.IsNullOrEmpty(modelFilter.SearchText))
			{
				var s = modelFilter.SearchText.Trim().ToLower();

				query = query.Where(x =>
					(x.Message != null && x.Message.ToLower().Contains(s)) ||
					(x.Path != null && x.Path.ToLower().Contains(s)) ||
					(x.ExceptionType != null && x.ExceptionType.ToLower().Contains(s)) ||
					(x.UserId != null && x.UserId.ToLower().Contains(s))
				);
			}

			if (!string.IsNullOrEmpty(modelFilter.Path))
			{
				var s = modelFilter.Path.Trim().ToLower();

				query = query.Where(x => x.Path != null && x.Path.ToLower().Contains(s));
			}

			if (!string.IsNullOrEmpty(modelFilter.SelectedMethod))
				query = query.Where(x => x.Method != null && x.Method.Contains(modelFilter.SelectedMethod));

			if (modelFilter.StatusCode.HasValue)
				query = query.Where(x => x.StatusCode == modelFilter.StatusCode);

			if (modelFilter.IsHandled.HasValue)
				query = query.Where(x => x.IsHandled == modelFilter.IsHandled);

			if (!string.IsNullOrWhiteSpace(modelFilter.Method))
				query = query.Where(x => x.Method == modelFilter.Method);

			if (modelFilter.StartDate.HasValue)
				query = query.Where(x => x.CreatedAt >= modelFilter.StartDate.Value);

			if (modelFilter.EndDate.HasValue)
				query = query.Where(x => x.CreatedAt < modelFilter.EndDate.Value.AddDays(1));

			return query;
		}


		private IQueryable<AppData.Entities.DbLogEntity> ApplySorting(IQueryable<AppData.Entities.DbLogEntity> query, ModelFilter modelFilter)
		{
			query = modelFilter.SortBy switch
			{
				"path" => modelFilter.SortDesc ? query.OrderByDescending(x => x.Path) : query.OrderBy(x => x.Path),
				"method" => modelFilter.SortDesc ? query.OrderByDescending(x => x.Method) : query.OrderBy(x => x.Method),
				"statusCode" => modelFilter.SortDesc ? query.OrderByDescending(x => x.StatusCode) : query.OrderBy(x => x.StatusCode),
				"isHandled" => modelFilter.SortDesc ? query.OrderByDescending(x => x.IsHandled) : query.OrderBy(x => x.IsHandled),
				_ => modelFilter.SortDesc ? query.OrderByDescending(x => x.Id) : query.OrderByDescending(x => x.Id)
			};

			return query;
		}

		private async Task RefreshModelFilter(ModelFilter modelFilter)
		{
			modelFilter.MethodList = await db.DbLogs
				.Select(x => x.Method).Distinct().OrderBy(x => x)
				.Select(y => new SelectListItem { Text = y, Value = y })
				.ToListAsync();

			modelFilter.StatusCodeList = await db.DbLogs
				.Select(x => x.StatusCode).Distinct().OrderBy(x => x)
				.Select(y => new SelectListItem { Text = y.ToString(), Value = y.ToString() })
				.ToListAsync();
		}

		[HttpGet]
		public async Task<IActionResult> DbLogs(ModelFilter modelFilter)
		{
			var query = db.DbLogs.AsNoTracking().AsQueryable();

			query = ApplyFilter(query, modelFilter);

			query = ApplySorting(query, modelFilter);

			int totalCount = await query.CountAsync();
			int totalPages = (int)Math.Ceiling(totalCount / (double)modelFilter.PageSize);
			int page = Math.Max(1, Math.Min(modelFilter.CurrentPage, totalPages == 0 ? 1 : totalPages));

			var sql = query.ToQueryString();

			var expression = query.Expression;

			var items = await query
				.Skip((page - 1) * modelFilter.PageSize)
				.Take(modelFilter.PageSize)
				.ToListAsync();

			await RefreshModelFilter(modelFilter);

			modelFilter.TotalCount = totalCount;
			modelFilter.TotalPages = totalPages;
			modelFilter.CurrentPage = page;

			var vm = new DbLogViewModel
			{
				Items = items,
				ModelFilter = modelFilter,
			};

			return View(model: vm);
		}


		public async Task<IActionResult> DbLogDetail(Guid id)
		{
			var log = await db.DbLogs.FindAsync(id);
			if (log is null) return NotFound();
			return View(log);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> MarkHandled(Guid id)
		{
			var log = await db.DbLogs.FindAsync(id);
			if (log is null) return NotFound();

			log.IsHandled = true;
			log.UpdatedAt = DateTime.Now;
			await db.SaveChangesAsync();

			return Ok();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(Guid id, ModelFilter filter)
		{
			var log = await db.DbLogs.FindAsync(id);
			if (log is not null)
			{
				db.DbLogs.Remove(log);
				await db.SaveChangesAsync();
			}

			return RedirectToAction(nameof(DbLogs), filter);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Truncate()
		{
			// await db.Database.ExecuteSqlRawAsync(@"DELETE FROM Users; DELETE FROM sqlite_sequence WHERE name = 'Users';");

			await db.DbLogs.ExecuteDeleteAsync();

			return RedirectToAction(nameof(DbLogs));
		}


		[Route("/error")]
		public async Task<IActionResult> Error()
		{
			var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			var exception = exceptionFeature?.Error ?? new Exception("Unknown Exception");

			await exception.LogToDbAsync();

			var model = new ErrorViewModel
			{
				Message = exception.Message,
				StackTrace = exception.StackTrace,
				Path = exceptionFeature?.Path ?? string.Empty
			};

			return View(model);
		}

		[Route("/policy")] public async Task<IActionResult> Policy() => View();
		[Route("/terms")] public async Task<IActionResult> Terms() => View();



	}
}
