using Mapster;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebEdit.AppData;
using WebEdit.AppData.Entities;
using WebEdit.ViewModels;

namespace WebEdit.Controllers
{
    public class Tiny5DefaultController(AppDbContext db, IDataProtectionProvider provider, ILogger<Tiny5Controller> logger) : Controller
    {
		private readonly IDataProtector _protector = provider.CreateProtector(nameof(Tiny5Controller));

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
		public async Task<ActionResult> Editor(string? protectedId)
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

			return View(viewName: "Editor", model: model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UploadImage([FromServices] IWebHostEnvironment env, IFormFile file)
		{
			if (file == null || file.Length == 0) return BadRequest();

			var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");

			if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

			var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

			var filePath = Path.Combine(uploadsFolder, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			return Json(new { location = "/uploads/" + fileName });
		}

	}
}
