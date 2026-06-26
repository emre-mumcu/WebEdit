using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebEdit.AppData;

namespace WebEdit.AppLib;

// Attribute içine direkt DbContext inject etmek klasik kullanımda sorun çıkarır.
/* public class ProfileRequiredAttribute : Attribute, IAsyncAuthorizationFilter
{
	private readonly AppDbContext _db;

	public ProfileRequiredAttribute(AppDbContext db)
	{
		_db = db;
	}

	public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
	{
		var user = context.HttpContext.User;

		// 1) Rol kontrolü
		if (!user.IsInRole("Administrator"))
		{
			context.Result = new ForbidResult();
			return;
		}

		// 2) DB kontrolü (örnek: UserProfile tablosu)
		string username = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

		var exists = _db.Users.Any(x => x.UserName == username);

		if (!exists)
		{
			// login sayfasına veya kayıt sayfasına yönlendir
			context.Result = new RedirectToActionResult(
				"CreateProfile",
				"Account",
				null
			);
		}
	}
} */

public class ProfileRequiredFilter : IAsyncAuthorizationFilter
{
	private readonly AppDbContext _db;

	public ProfileRequiredFilter(AppDbContext db)
	{
		_db = db;
	}

	public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
	{
		var user = context.HttpContext.User;

		if (!user.IsInRole("Administrator"))
		{
			context.Result = new ForbidResult();
			return;
		}

		string username = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;

		var exists = _db.Users.Any(x => x.UserName == username);

		if (!exists)
		{
			context.Result = new RedirectToActionResult(
				"CreateProfile",
				"Account",
				null
			);
		}
	}
}

public class ProfileRequiredAttribute : TypeFilterAttribute
{
	public ProfileRequiredAttribute() : base(typeof(ProfileRequiredFilter))
	{
	}
}