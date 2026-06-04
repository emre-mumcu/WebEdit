using Microsoft.AspNetCore.Mvc;

namespace WebEdit.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomeController
        public ActionResult Index()
        {
            return View();
        }

    }
}
