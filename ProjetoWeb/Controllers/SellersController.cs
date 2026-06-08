using Microsoft.AspNetCore.Mvc;

namespace ProjetoWeb.Controllers
{
    public class SellersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
