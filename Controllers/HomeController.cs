using Microsoft.AspNetCore.Mvc;

namespace tema2mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
