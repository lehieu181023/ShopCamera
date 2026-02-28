
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CameraTuanA.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace CameraTuanA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<OrderController> _logger;
        private const string KeyCache = "Order";

        public OrderController(DBContext db, ILogger<OrderController> logger)
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
        public async Task<PartialViewResult> ListData(string keySearch = "", int status = -1, int page = 1, int pageSize = 10)
        {
            List<Models.Order> listData = null;
            try
            {
                var list = _db.Order.AsQueryable().AsNoTracking();
                if (!string.IsNullOrEmpty(keySearch))
                {
                    list = list.Where(g => g.OrderCode.Contains(keySearch) || g.RecipientName.Contains(keySearch));
                }
                if(status > -1)
                {
                    list = list.Where(g => g.Status == status);
                }
                list = list.OrderByDescending(g => g.OrderDate);
                var count = await list.CountAsync();
                var totalPages = (int)Math.Ceiling(count / (double)pageSize);
                listData = await list.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
                ViewBag.PageNumber = totalPages;
                ViewBag.PageCurrent = page;
                ViewBag.PageSize = pageSize;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Order list data.");
            }
            return PartialView(listData);
        }

        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Order objData = await _db.Order
                .Include(x => x.OrderDetails)
                    .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return View(objData);
        }

        [Authorize(Roles = "admin")]
        public JsonResult Delete(int? id)
        {
            Order obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Order.Find(id);
                if (obj != null)
                {
                    _db.Order.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa Order: Id = {obj.Id}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa Order: Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "admin")]
        public async Task<JsonResult> changeStatus(int? id,int Status = 0)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Order.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            if(Status == 0) {
                return Json(new { success = false, message = "Đơn không thể đưa lại trạng thái chờ sử lý" });
            }
            else if(Status == 1)
            {
                if(objData.PaymentMethod == "cod")
                {
                    return Json(new { success = false, message = "Đây là đơn thanh toán khi nhận hàng" });
                }
            }
            try
            {
                objData.Status = Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã thay đổi trạng thái Order: Id = {objData.Id}, Status = {objData.Status}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thay đổi trạng thái Order: Id = {id}");
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