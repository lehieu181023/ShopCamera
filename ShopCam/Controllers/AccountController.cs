using CameraTuanA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace CameraTuanA.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<CartController> _logger;

        public AccountController(DBContext db, ILogger<CartController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
            }
            var passwordHelper = new PasswordHelper();
            // Tìm user trong DB
            var account = await _db.Account
                .FirstOrDefaultAsync(x => x.Username == model.Username);

            if (account == null)
            {
                return Json(new { success = false, message = "Tài khoản không tồn tại" });
            }
            if (!passwordHelper.VerifyPassword(account.Password, model.Password))
            {
                return Json(new { success = false, message = "Mật khẩu không chính xác" });
            }
            if(account.Status == false)
            {
                return Json(new { success = false, message = "Tài khoản đã bị khóa" });
            }

            // Tạo danh sách Claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Role ?? "")
            };

            // Tạo Identity & Principal
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Đăng nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });
            if(account.Role == "admin")
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Json(new { success = true, message = "Đăng nhập thành công", redirectUrl = returnUrl });
                }
                return Json(new { success = true, message = "Đăng nhập thành công", redirectUrl = "/admin/dashboard" });
            }

            return Json(new { success = true, message = "Đăng nhập thành công", redirectUrl = Url.Action("Index", "Home") });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Register(Models.Account obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    obj.CreatedAt = DateTime.Now;
                    obj.UpdatedAt = DateTime.Now;
                    var passwordHelper = new PasswordHelper();
                    obj.Password = passwordHelper.HashPassword(obj.Password);
                    obj.Status = true;
                    obj.Role = "customer";
                    _db.Account.Add(obj);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đăng ký thất bại, vui lòng thử lại!" });
            }
            return Json(new { success = true, message = "Đăng ký thành công, giờ có thể đăng nhập băng tài khoản: "+obj.Username });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Denied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
