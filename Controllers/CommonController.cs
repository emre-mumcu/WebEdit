using Microsoft.AspNetCore.Mvc;

namespace WebEdit.Controllers
{
    public class CommonController : Controller
    {        
        public ActionResult Settings()
        {
            return View();
        }

    }
}
