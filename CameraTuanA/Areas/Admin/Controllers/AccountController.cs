
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CameraTuanA.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace CameraTuanA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<AccountController> _logger;
        private const string KeyCache = "Account";

        public AccountController(DBContext db, ILogger<AccountController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles="admin")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> ListData(string keySearch = "", int status = 0, int page = 1, int pageSize = 10)
        {
            List<Models.Account> listData = null;
            try
            {
                var list = _db.Account.AsQueryable().AsNoTracking();
                if (!string.IsNullOrEmpty(keySearch))
                {
                    list = list.Where(g => g.FullName.Contains(keySearch));
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
                _logger.LogError(ex, "Error fetching Account list data.");
            }
            return PartialView(listData);
        }

        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Account objData = await _db.Account.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(objData);
        }

        public PartialViewResult Create()
        {
            return PartialView();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create( Models.Account obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    obj.CreatedAt = DateTime.Now;
                    obj.UpdatedAt = DateTime.Now;
                    var passwordHelper = new PasswordHelper();
                    obj.Password = passwordHelper.HashPassword(obj.Password);
                    _db.Account.Add(obj);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo Account mới: Name = {obj.FullName}");
                }
                else
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }
            }
            catch (DbUpdateException ex)
            {
                // Check inner exception for SQL error
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    // Kiểm tra constraint nào bị vi phạm
                    if (sqlEx.Message.Contains("UQ__accounts__AB6E6164107CBFEC"))
                    {
                        return Json(new { success = false, message = "Email đã tồn tại!" });
                    }
                    else if (sqlEx.Message.Contains("UQ__accounts__F3DBC572845D03FF"))
                    {
                        return Json(new { success = false, message = "Tài khoản đã tồn tại!" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Dữ liệu đã tồn tại (vi phạm unique constraint)!" });
                    }
                }

                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thêm Account");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }

            return Json(new { success = true, message = "Thêm mới thành công" });
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Account obj = await _db.Account.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(obj);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> EditPost( Models.Account obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Account.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                if (ModelState.IsValid == false)
                {
                    return Json(new { success = false, message = "Dữ liệu nhập không hợp lệ" });
                }
                objData.FullName = obj.FullName;
                objData.Email = obj.Email;
                objData.PhoneNumber = obj.PhoneNumber;
                objData.Address = obj.Address;
                objData.Status = obj.Status;
                objData.UpdatedAt = DateTime.Now;
                if (!string.IsNullOrEmpty(obj.Password))
                {
                    var passwordHelper = new PasswordHelper();
                    objData.Password = passwordHelper.HashPassword(obj.Password);
                }

                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật Account: Id = {objData.Id}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật Account");
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        [Authorize(Roles = "admin")]
        public JsonResult Delete(int? id)
        {
            Account obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Account.Find(id);
                if (obj != null)
                {
                    _db.Account.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa Account: Id = {obj.Id}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa Account: Id = {id}");
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
            var objData = await _db.Account.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã thay đổi trạng thái Account: Id = {objData.Id}, Status = {objData.Status}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thay đổi trạng thái Account: Id = {id}");
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