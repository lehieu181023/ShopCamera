
using CameraTuanA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;


namespace CameraTuanA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ProductController> _logger;
        private const string KeyCache = "Product";

        public ProductController(DBContext db, ILogger<ProductController> logger)
        {
            _db = db;
            _logger = logger;
        }


        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> ListData(string keySearch = "", int status = 0, int page = 1, int pageSize = 10)
        {
            List<Models.Product> listData = null;
            try
            {
                var list = _db.Product.AsQueryable().AsNoTracking();
                if (!string.IsNullOrEmpty(keySearch))
                {
                    list = list.Where(g => g.ProductName.Contains(keySearch));
                }
                if (status == 1)
                {
                    list = list.Where(g => g.Status == true);
                }
                else if (status == 2)
                {
                    list = list.Where(g => g.Status == false);
                }
                list = list.OrderByDescending(g => g.UpdatedAt);
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

        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Product objData = await _db.Product
                .Include(g => g.Brand)
                .Include(g => g.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(objData);
        }

        public ActionResult Create()
        {
            ViewData["Categories"] = _db.Category.Where(c => c.Status == true).OrderBy(c => c.CategoryName).ToList();
            ViewData["Brands"] = _db.Brand.Where(b => b.Status == true).OrderBy(b => b.BrandName).ToList();
            return PartialView();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(Product obj)
        {
            try
            {
                // Xóa lỗi cho navigation properties trước khi kiểm tra IsValid
                ModelState.Remove("Brand");
                ModelState.Remove("Category");
                // Validate dữ liệu nhập
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập. Vui lòng kiểm tra lại!" });
                }

                obj.CreatedAt = DateTime.Now;
                obj.UpdatedAt = DateTime.Now;
                obj.ViewCount ??= 0;
                obj.RatingScore ??= 0;
                obj.RatingCount ??= 0;

                // Nếu có yêu cầu check unique ProductCode:
                bool isCodeExist = await _db.Product.AnyAsync(p => p.ProductCode == obj.ProductCode);
                if (isCodeExist)
                {
                    return Json(new { success = false, message = "Mã sản phẩm đã tồn tại!" });
                }

                // Thêm vào database
                _db.Product.Add(obj);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo Product mới: Name = {obj.ProductName}, Code = {obj.ProductCode}");

                return Json(new { success = true, message = "Thêm mới thành công", data = obj.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thêm Product");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }


        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Product obj = await _db.Product.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            ViewData["Categories"] = _db.Category.Where(c => c.Status == true).OrderBy(c => c.CategoryName).ToList();
            ViewData["Brands"] = _db.Brand.Where(b => b.Status == true).OrderBy(b => b.BrandName).ToList();

            return PartialView(obj);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditPost(Product obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            // Lấy bản ghi từ DB
            var objData = await _db.Product.FindAsync(Id);
            if (objData == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa"
                });
            }
            // Xóa lỗi cho navigation properties trước khi kiểm tra IsValid
            ModelState.Remove("Brand");
            ModelState.Remove("Category");
            // Kiểm tra model hợp lệ
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dữ liệu nhập không hợp lệ" });
            }

            try
            {
                // Gán lại các trường cần cập nhật (chỉ lấy từ obj)
                objData.ProductName = obj.ProductName;
                objData.ProductCode = obj.ProductCode;
                objData.ShortDescription = obj.ShortDescription;
                objData.DetailedDescription = obj.DetailedDescription;
                objData.SellingPrice = obj.SellingPrice;
                objData.OriginalPrice = obj.OriginalPrice;
                objData.StockQuantity = obj.StockQuantity;
                objData.MainImage = obj.MainImage;
                objData.ImageGallery = obj.ImageGallery;
                objData.TechnicalSpecs = obj.TechnicalSpecs;
                objData.CategoryId = obj.CategoryId;
                objData.BrandId = obj.BrandId;
                objData.Status = obj.Status;

                // không update CreatedAt để giữ thời gian tạo cũ
                objData.UpdatedAt = DateTime.Now;
                bool isCodeExist = await _db.Product
                .AnyAsync(p => p.ProductCode == obj.ProductCode && p.Id != Id);
                if (isCodeExist)
                {
                    return Json(new { success = false, message = "Mã sản phẩm đã tồn tại!" });
                }

                // Lưu thay đổi
                await _db.SaveChangesAsync();

                _logger.LogInformation(
                    $"[{User.Identity?.Name}] Đã cập nhật Product: Id = {objData.Id}, Name = {objData.ProductName}");

                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    return Json(new { success = false, message = "Bản ghi này đã bị xóa bởi người dùng khác" });
                }
                else
                {
                    return Json(new { success = false, message = "Bản ghi này đã bị sửa bởi người dùng khác" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật Product");
                return Json(new { success = false, message = "Không thể lưu được" });
            }
        }

        [Authorize(Roles = "admin")]
        public JsonResult Delete(int? id)
        {
            Product obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Product.Find(id);
                if (obj != null)
                {
                    _db.Product.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa Product: Id = {obj.Id}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa Product: Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "admin")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Product.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã thay đổi trạng thái Product: Id = {objData.Id}, Status = {objData.Status}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thay đổi trạng thái Product: Id = {id}");
                return Json(new { success = false, message = "Không thay đổi được trạng thái bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được cập nhật trạng thái thành công" });
        }

        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}