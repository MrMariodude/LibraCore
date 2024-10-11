using Microsoft.AspNetCore.Mvc;

namespace LiberaryManagmentSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
