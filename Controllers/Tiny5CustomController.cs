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
				return View(new WebDocViewModel());
			}
			else
			{
				string id = _protector.Unprotect(protectedId);

				WebDocEntity? wd = await db.WebDocs.FindAsync(Guid.Parse(id));

				if(wd != null)
                {
					if (wd.IsEncrypted) wd.Content = encryption.Decrypt(wd.Content);

					WebDocViewModel? model = wd.Adapt<WebDocViewModel>();

					model?.ProtectedId = protectedId;

					return View(model);                    
                }
				else
                {
					return View(new WebDocViewModel());
				}
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Save(WebDocViewModel model)
		{
			if (model.IsEncrypted) model.Content = encryption.Encrypt(model.Content);

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

			if (model.IsEncrypted) model.Content = encryption.Decrypt(model.Content);

			return View(viewName: "Edit", model: model);
		}

	}
}
