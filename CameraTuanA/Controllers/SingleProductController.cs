using System.Diagnostics;
using CameraTuanA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Controllers
{
    public class SingleProductController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<SingleProductController> _logger;

        public SingleProductController(DBContext db, ILogger<SingleProductController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index(int id = 0)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            var product = _db.Product.Include(x => x.Category).FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var productReviews = _db.ProductReview.Include(r => r.Account).Where(r => r.ProductId == id).ToList();
            ViewData["reviews"] = productReviews;
            return View(product);
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
