using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebEdit.Controllers
{
    public class AccountController : Controller
    {
		[Route("login")]
		public async Task<IActionResult> TestLogin()
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, "Bob Bobbity"),
				new Claim(ClaimTypes.NameIdentifier, "bob"),
				new Claim(ClaimTypes.Role, "Administrator"),
				new Claim(ClaimTypes.Role, "User"),
				new Claim(ClaimTypes.Role, "Guest")
			};

			var principal = new ClaimsPrincipal(
				new ClaimsIdentity(
					claims,
					CookieAuthenticationDefaults.AuthenticationScheme));

			await HttpContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				principal);

			// HttpContext.Response.Redirect("/");

			return LocalRedirect("/");
		}

		[Route("logout")]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			
			return LocalRedirect("/");
		}

		[Route("access-denied")]
		public async Task<IActionResult> AccessDenied() => View();
		
			
		

	}
}
