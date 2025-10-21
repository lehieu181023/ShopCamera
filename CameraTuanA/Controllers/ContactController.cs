using CameraTuanA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CameraTuanA.Controllers
{
    public class ContactController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ContactController> _logger;

        public ContactController(DBContext db,ILogger<ContactController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(Contact obj)
        {
            try
            {
                obj.CreatedAt = DateTime.Now;
                _db.Contact.Add(obj);
                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Gửi liên hệ thành công!", data = obj.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gửi liên hệ thất bại!" });
            }
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
