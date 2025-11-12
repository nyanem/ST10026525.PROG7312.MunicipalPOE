using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ST10026525.PROG7312.MunicipalPOE.Models;

namespace ST10026525.PROG7312.MunicipalPOE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult MainMenu()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
