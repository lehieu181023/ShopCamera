
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CameraTuanA.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace CameraTuanA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ContactController> _logger;
        private const string KeyCache = "Contact";

        public ContactController(DBContext db, ILogger<ContactController> logger)
        {
            _db = db;
            _logger = logger;
        }

        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<PartialViewResult> ListData(string keySearch = "", int status = 0, int page = 1, int pageSize = 10)
        {
            List<Models.Contact> listData = null;
            try
            {
                var list = _db.Contact.AsQueryable().AsNoTracking();
                if (!string.IsNullOrEmpty(keySearch))
                {
                    list = list.Where(g => g.Subject.Contains(keySearch));
                }
                list = list.OrderByDescending(g => g.CreatedAt);
                var count = await list.CountAsync();
                var totalPages = (int)Math.Ceiling(count / (double)pageSize);
                listData = await list.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                ViewBag.PageNumber = totalPages;
                ViewBag.PageCurrent = page;
                ViewBag.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Contact list data.");
            }
            return PartialView(listData);
        }

        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Contact objData = await _db.Contact.FindAsync(id);
            objData.ProcessingStatus = "read";
            _db.Contact.Update(objData);
            await  _db.SaveChangesAsync();
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(objData);
        }


        [Authorize(Roles = "admin")]
        public JsonResult Delete(int? id)
        {
            Contact obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Contact.Find(id);
                if (obj != null)
                {
                    _db.Contact.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa Contact: Id = {obj.Id}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa Contact: Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
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