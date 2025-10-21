using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CameraTuanA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UploadController : Controller
    {

        private readonly string _uploadPath;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // Giới hạn 5MB


        public UploadController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _uploadPath = Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "Vui lòng chọn một file để tải lên!" });
                }

                // Kiểm tra định dạng file hợp lệ
                var allowedExtensions = new[] { ".jpg", ".png", ".pdf" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (Array.IndexOf(allowedExtensions, fileExtension) == -1)
                {
                    return Json(new { success = false, message = "Định dạng file không hợp lệ! Chỉ cho phép .jpg, .png, .pdf." });
                }

                // Lưu file vào thư mục
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine("uploads", fileName);  // Đường dẫn tương đối
                var savePath = Path.Combine(_uploadPath, fileName); // Đường dẫn đầy đủ trên server

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Json(new
                {
                    success = true,
                    message = "Tải file thành công!",
                    fileName = fileName,
                    filePath = "/" + filePath.Replace("\\", "/") // Trả về đường dẫn tương đối
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi tải file: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteFile(string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return Json(new { success = false, message = "Tên file không hợp lệ!" });
                }

                // Đảm bảo chỉ lấy tên file, tránh truyền toàn bộ đường dẫn
                fileName = Path.GetFileName(fileName);
                var filePath = Path.Combine(_uploadPath, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Json(new { success = true, message = "Xóa file thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "File không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa file: " + ex.Message });
            }
        }

    }
}
