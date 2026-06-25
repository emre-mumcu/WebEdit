using Microsoft.AspNetCore.Mvc;
using WebEdit.AppLib;

namespace WebEdit.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
 			// HttpContext.Session.SetKey<string>("war", "warning");
			// HttpContext.Session.SetKey<string>("err", "error");
			// HttpContext.Session.SetKey<string>("scc", "success");
			// HttpContext.Session.SetKey<string>("inf", "info");
			// HttpContext.Session.SetKey<string>("que", "question"); 
			HttpContext.Session.SetKey<string>("toast", "question");

            return View();
        }

    }
}
