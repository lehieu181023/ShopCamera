using CameraTuanA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace CameraTuanA.Controllers
{
    public class PaymentController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(DBContext db,ILogger<PaymentController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = _db.ShoppingCart.Include(c => c.Product).Where(p => p.AccountId.ToString() == UserId).ToList();
            var user = _db.Account.Find(int.Parse(UserId ?? "0"));
            ViewData["User"] = user;
            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(Models.Order obj)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này!" });
            }

            obj.AccountId = int.Parse(userId);

            var cart = await _db.ShoppingCart
                .Include(c => c.Product)
                .Where(p => p.AccountId.ToString() == userId)
                .ToListAsync();

            if (cart == null || !cart.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống!" });
            }

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                ModelState.Remove("Account");
                ModelState.Remove("OrderCode");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }

                // Tạo đơn hàng
                obj.OrderCode = "DH" + DateTime.Now.ToString("yyyyMMddHHmmss");
                obj.OrderDate = DateTime.Now;
                obj.Status = 0; // Mới tạo

                _db.Order.Add(obj);
                await _db.SaveChangesAsync(); // để lấy Id

                // Tạo chi tiết đơn hàng
                List<OrderDetail> orderDetails = new();
                decimal totalAmount = 0;

                foreach (var item in cart)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = obj.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = item.Product.ProductName,
                        ProductImage = item.Product.MainImage,
                        SellingPrice = item.Product.SellingPrice,
                        Subtotal = item.Product.SellingPrice * item.Quantity
                    };
                    totalAmount += orderDetail.Subtotal;
                    orderDetails.Add(orderDetail);

                    // Trừ tồn kho
                    var product = await _db.Product.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                        _db.Product.Update(product);
                    }
                }

                _db.OrderDetail.AddRange(orderDetails);
                _db.ShoppingCart.RemoveRange(cart);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo đơn hàng: {obj.OrderCode}");

                // Nếu là COD → hoàn tất luôn
                if (obj.PaymentMethod == "cod")
                {
                    return Json(new { success = true, message = "Tạo đơn hàng thành công", cod = true });
                }

                // Nếu là VNPay → trả URL để redirect
                string paymentUrl = Url.Action("PaymentDemo", "Payment", new { orderCode = obj.OrderCode, amount = totalAmount }, protocol: Request.Scheme);
                return Json(new { success = true, message = "Tạo đơn hàng thành công", url = paymentUrl });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi tạo đơn hàng");
                return Json(new { success = false, message = "Lỗi tạo đơn hàng, vui lòng thử lại!" });
            }
        }



        public IActionResult PaymentDemo(string orderCode, decimal amount)
        {
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_Returnurl = Url.Action("VnpayReturn", "Payment", null, protocol: Request.Scheme);
            string vnp_TmnCode = "5T449IW5";   // demo
            string vnp_HashSecret = "IFDNHBTORS0USOPSHDRV5UN9TTEZUZ7T"; // demo

            var vnpayData = new SortedList<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", ((int)(amount * 100)).ToString() },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", $"Thanh toán đơn hàng {orderCode}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", vnp_Returnurl },
                { "vnp_TxnRef", orderCode }
            };

            // Tạo chuỗi dữ liệu ký
            string signData = string.Join("&", vnpayData.Select(kv => $"{kv.Key}={kv.Value}"));
            string vnp_SecureHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(vnp_HashSecret)))
            {
                vnp_SecureHash = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(signData))).Replace("-", "").ToLower();
            }

            string paymentUrl = $"{vnp_Url}?{signData}&vnp_SecureHash={vnp_SecureHash}";
            return Redirect(paymentUrl);
        }


        public IActionResult VnpayReturn()
        {
            var query = Request.Query;
            string responseCode = query["vnp_ResponseCode"];
            string orderCode = query["vnp_TxnRef"];

            if (string.IsNullOrEmpty(orderCode))
            {
                ViewBag.Message = "❌ Không tìm thấy thông tin đơn hàng.";
                return View();
            }

            var order = _db.Order.FirstOrDefault(o => o.OrderCode == orderCode);
            if (order == null)
            {
                ViewBag.Message = "❌ Đơn hàng không tồn tại.";
                return View();
            }

            if (responseCode == "00")
            {
                order.Status = 1; // Đã thanh toán
                _db.SaveChanges();
                ViewBag.Message = $"✅ Thanh toán thành công cho đơn hàng {orderCode}";
            }
            else
            {
                ViewBag.Message = "❌ Thanh toán thất bại hoặc bị hủy";
            }

            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
