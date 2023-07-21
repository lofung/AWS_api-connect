using Microsoft.AspNetCore.Mvc;

namespace MvcMovie
{
    public class controll : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
