using System.Diagnostics;
using MBA.DevXpert.Marketplace.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace MBA.DevXpert.Marketplace.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(Guid? categoriaId, string? descricao)
        {
            return Redirect("/Identity/Account/Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
