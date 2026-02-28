using CameraTuanA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;

namespace CameraTuanA.Controllers
{
    public class CartController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<CartController> _logger;

        public CartController(DBContext db,ILogger<CartController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]

        public async Task<ActionResult> ListData()
        {
            List<Models.ShoppingCart> listData = null;
            try
            {
                var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var list = _db.ShoppingCart.Include(p => p.Product).Where(p => p.AccountId.ToString() == UserId).AsQueryable().AsNoTracking();

                listData = await list.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Product list data.");
            }
            return PartialView(listData);
        }

        [HttpPost]
        public async Task<JsonResult> PriceCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json("0 đ");
            }
            var price = _db.ShoppingCart
            .Include(x => x.Product)
            .AsEnumerable() // chuyển sang xử lý trong RAM
            .Where(x => x.AccountId.ToString() == userId)
            .Sum(x => x.Product.SellingPrice * x.Quantity);


            var formattedPrice = String.Format(new CultureInfo("vi-VN"), "{0:C0}", price);
            return Json(formattedPrice);
        }


        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddToCart(int Id = 0, int Quantity = 0)
        {
            if (Id == 0)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc vừa bị xóa!" });
            }
            if (Quantity == 0)
            {
                Quantity = 1;
            }
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này!" });
                }
                var product = _db.Product.FirstOrDefault(g => g.Id == Id);
                if(product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tài hoặc vừa bị xóa!" });
                }
                var Cart = _db.ShoppingCart.FirstOrDefault(c => c.ProductId == product.Id && c.AccountId == int.Parse(userId));
                if(Cart != null)
                {
                    Cart.Quantity += Quantity;
                    if(Cart.Quantity > product.StockQuantity)
                    {
                        return Json(new { success = false, message = "Sản phẩm đã hết hàng!" });
                    }
                    Cart.UpdatedAt = DateTime.Now;
                    _db.ShoppingCart.Update(Cart);
                }
                else
                {
                    var obj = new ShoppingCart
                    {
                        ProductId = product.Id,
                        AccountId = int.Parse(userId),
                        Quantity = Quantity,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    if (obj.Quantity > product.StockQuantity)
                    {
                        return Json(new { success = false, message = "Sản phẩm đã hết hàng!" });
                    }
                    _db.ShoppingCart.Add(obj);
                }
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Thêm giỏ hàng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gửi liên hệ thất bại!" });
            }
        }

        public JsonResult Delete(int? id)
        {
            ShoppingCart obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.ShoppingCart.Find(id);
                if (obj != null)
                {
                    _db.ShoppingCart.Remove(obj);
                    _db.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Json(new { success = false, message = "Không xóa được sản phẩm này khỏi giỏ hàng" });
            }

            return Json(new { success = true, message = "Đã xóa sản phẩm" });
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
