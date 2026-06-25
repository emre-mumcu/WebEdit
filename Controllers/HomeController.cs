using Microsoft.AspNetCore.Mvc;

namespace WebEdit.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
