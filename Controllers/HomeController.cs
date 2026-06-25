using Microsoft.AspNetCore.Mvc;

namespace WebEdit.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
			throw new Exception("hehe");
            return View();
        }

    }
}
