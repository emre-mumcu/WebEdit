using Microsoft.AspNetCore.Mvc;

namespace WebEdit.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ServicesController(IWebHostEnvironment _env) : ControllerBase
    {
		[HttpPost("uploadimage")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UploadImage(IFormFile file)
		{
			if (file == null || file.Length == 0) return BadRequest("File is empty");

			var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

			var path = Path.Combine(_env.WebRootPath, "uploads", fileName);

			using (var stream = System.IO.File.Create(path))
			{
				await file.CopyToAsync(stream);
			}

			return Ok(new
			{
				location = $"/uploads/{fileName}"
			});
		}

	}
}
