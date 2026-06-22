using Mapster;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEdit.AppData;
using WebEdit.AppData.Entities;
using WebEdit.AppLib;
using WebEdit.ViewModels;

namespace WebEdit.Controllers
{
    public class Tiny5CustomController(AppDbContext db, IDataProtectionProvider provider, ILogger<Tiny5Controller> logger, IEncryptionService encryption) : Controller
    {
		private readonly IDataProtector _protector = provider.CreateProtector(nameof(Tiny5CustomController));

		public async Task<ActionResult> Index()
		{
			TypeAdapterConfig<WebDocEntity, WebDocViewModel>
				.NewConfig()
				.Map(dest => dest.ProtectedId, src => _protector.Protect(src.Id.ToString()));

			List<WebDocViewModel> model = await db.WebDocs
				.AsNoTracking()
				.ProjectToType<WebDocViewModel>()
				.ToListAsync();

			return View(model);
		}

		[HttpGet("[controller]/[action]/{protectedId?}")]
		public async Task<ActionResult> ViewContent(string? protectedId)
		{
			if (string.IsNullOrEmpty(protectedId)) return RedirectToAction("Index");
			else
			{
				string id = _protector.Unprotect(protectedId);

				WebDocEntity? wd = await db.WebDocs.FindAsync(Guid.Parse(id));

				WebDocViewModel? model = wd.Adapt<WebDocViewModel>();

				model?.ProtectedId = protectedId;

				return View(model);
			}
		}

		[HttpGet("[controller]/[action]/{protectedId?}")]
		public async Task<ActionResult> Delete(string? protectedId)
		{
			if (string.IsNullOrEmpty(protectedId)) return RedirectToAction("Index");
			else
			{
				string id = _protector.Unprotect(protectedId);

				WebDocEntity? wd = await db.WebDocs.FindAsync(Guid.Parse(id));

				if (wd != null)
				{
					db.WebDocs.Remove(wd);
					await db.SaveChangesAsync();
				}

				return RedirectToAction("Index");
			}
		}

		[HttpGet("[controller]/[action]/{protectedId?}")]
		public async Task<ActionResult> Edit(string? protectedId)
		{
			if (protectedId is null)
			{
				WebDocViewModel model = new WebDocViewModel();
				
				return View(model);
			}
			else
			{
				string id = _protector.Unprotect(protectedId);

				WebDocEntity? wd = await db.WebDocs.FindAsync(Guid.Parse(id));

				WebDocViewModel? model = wd.Adapt<WebDocViewModel>();

				model?.ProtectedId = protectedId;

				return View(model);
			}
		}

		public async Task<ActionResult> Save(WebDocViewModel model)
		{
			if (model.ProtectedId is null)
			{
				WebDocEntity wd = model.Adapt<WebDocEntity>();
				await db.WebDocs.AddAsync(wd);

				model.ProtectedId = _protector.Protect(wd.Id.ToString());
			}
			else
			{
				string id = _protector.Unprotect(model.ProtectedId);
				var entity = await db.WebDocs.FirstOrDefaultAsync(x => x.Id == model.Id);
				model.Adapt(entity);
			}

			await db.SaveChangesAsync();

			return View(viewName: "Edit", model: model);
		}

	}
}
