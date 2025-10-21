using CameraTuanA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CameraTuanA.Controllers
{
    public class ProductController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ProductController> _logger;

        public ProductController(DBContext db, ILogger<ProductController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var categories = _db.Category.Where(g => g.Status).Include(g => g.Products).OrderBy(g => g.CategoryName).AsNoTracking().ToList();
            ViewData["Categories"] = categories;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult>  ListData(int categoryId = 0,string keySearch = "",string SortBy  = "", int page = 1)
        {
            List<Models.Product> listData = null;
            try
            {
                var list = _db.Product.AsQueryable().AsNoTracking().Where(g => g.Status);
                if (!string.IsNullOrEmpty(keySearch))
                {
                    list = list.Where(g => g.ProductName.Contains(keySearch));
                }
                if (categoryId > 0)
                {
                    list = list.Where(g => g.CategoryId == categoryId);
                }
                switch(SortBy)
                {
                    case "price_asc":
                        list = list.OrderBy(g => g.SellingPrice);
                        break;
                    case "price_dsc":
                        list = list.OrderByDescending(g => g.SellingPrice);
                        break;
                    case "name":
                        list = list.OrderBy(g => g.ProductName);
                        break;
                    case "rating":
                        list = list.OrderByDescending(g => g.RatingScore);
                        break;
                    case "Popularity":
                        list = list.Include(g => g.OrderDetails).OrderByDescending(g => g.OrderDetails.Count);
                        break;
                    default:
                        list = list.OrderByDescending(g => g.UpdatedAt);
                        break;
                }
                int pageSize = 9;
                var count = await list.CountAsync();
                var totalPages = (int)Math.Ceiling(count / (double)pageSize);
                listData = await list.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                ViewBag.PageNumber = totalPages;
                ViewBag.PageCurrent = page;
                ViewBag.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Product list data.");
            }
            return PartialView(listData);
        }

        [HttpGet]
        public async Task<ActionResult> Products()
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
            return PartialView();
        }

        [HttpGet]
        public async Task<ActionResult> BestSeller()
        {
            var bestSellers = _db.Product
                .Include(p => p.OrderDetails)
                .OrderByDescending(p => p.OrderDetails.Count)
                .Take(6)
                .AsNoTracking()
                .ToList();
            return PartialView(bestSellers);
        }
        
        [HttpGet]
        public async Task<ActionResult> featuredProduct()
        {
            var bestSellers = _db.Product
                .OrderByDescending(p => p.RatingCount)  // nhiều lượt đánh giá
                .ThenByDescending(p => p.RatingScore)   // điểm cao hơn
                .Take(6)
                .AsNoTracking()
                .ToList();
            return PartialView(bestSellers);
        }
        
        [HttpGet]
        public async Task<ActionResult> relatedProduct(int id = 0)
        {
            List<Product> products = null;
            if(id > 0)
            {
                var product = _db.Product.Find(id);
                if(product != null)
                {
                    products = _db.Product.Where(x => x.CategoryId == product.CategoryId).Take(5).ToList();
                    if(products.Count <= 0)
                    {
                        products = _db.Product.OrderByDescending(p => p.UpdatedAt).Take(5).ToList();
                    }
                }
            }
            return PartialView(products);
        }

        [HttpPost]
        public async Task<JsonResult> Review(ProductReview obj)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(UserId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này!" });
            }
            var reviewed = await _db.ProductReview.Where(r => r.ProductId == obj.ProductId && r.AccountId.ToString() == UserId).CountAsync();
            if (reviewed > 0)
            {
                return Json(new { success = false, message = "Bạn đã đánh giá sản phẩm này rồi!" });
            }
            obj.AccountId = int.Parse(UserId);
            try
            {
                obj.ReviewedAt = DateTime.Now;
                _db.ProductReview.Add(obj);
                await _db.SaveChangesAsync();

                var product = await _db.Product.FindAsync(obj.ProductId);
                product.RatingCount = (product.RatingCount ?? 0) + 1;
                product.RatingScore = Math.Round(((product.RatingScore ?? 0) * ((product.RatingCount ?? 1) - 1) + obj.RatingScore) / (product.RatingCount ?? 1), 1);

                await _db.SaveChangesAsync();

                return Json(new { success = true, message = "Thêm mới thành công", data = obj.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thêm Product");
                return Json(new { success = false, message = "Thêm review thất bại, vui lòng thử lại!" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
