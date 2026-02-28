using System.Diagnostics;
using CameraTuanA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CameraTuanA.Controllers
{
    public class BestSellerController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<BestSellerController> _logger;

        public BestSellerController(DBContext db,ILogger<BestSellerController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var bestSellers = _db.Product
                .Include(p => p.OrderDetails)
                .OrderByDescending(p => p.OrderDetails.Count)
                .Take(6)
                .AsNoTracking()
                .ToList();
            var newProducts = _db.Product
                .OrderByDescending(p => p.CreatedAt)
                .Take(4)
                .AsNoTracking()
                .ToList();
            var allProducts = _db.Product.OrderBy(p => p.UpdatedAt).Take(8).AsNoTracking().ToList();
            ViewData["BestSellers"] = bestSellers;
            ViewData["NewProducts"] = newProducts;
            ViewData["AllProducts"] = allProducts;
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
