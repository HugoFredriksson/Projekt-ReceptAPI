using Microsoft.AspNetCore.Mvc;

namespace Projekt_Recept.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
