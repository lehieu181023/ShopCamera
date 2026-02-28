using System.Diagnostics;
using CameraTuanA.Models;
using Microsoft.AspNetCore.Mvc;

namespace CameraTuanA.Controllers
{
    public class HomeController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(DBContext db,ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
