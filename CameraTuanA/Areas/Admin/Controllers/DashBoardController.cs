
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CameraTuanA.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace CameraTuanA.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<DashBoardController> _logger;
        private const string KeyCache = "DashBoard";

        public DashBoardController(DBContext db, ILogger<DashBoardController> logger)
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
        public async Task<PartialViewResult> salecard(string filter)
        {
            reportCard reportCard = null;
            var report = _db.OrderDetail.Include(x => x.Order).AsNoTracking().AsQueryable().Where(x => x.Order.Status == 3);
            var reportAgo = _db.OrderDetail.Include(x => x.Order).AsNoTracking().AsQueryable().Where(x => x.Order.Status == 3);
            switch (filter)
            {
                case "today":
                    report = report.Where(x => x.Order.OrderDate == DateTime.Now.Date);
                    reportAgo = reportAgo.Where(x => x.Order.OrderDate == DateTime.Now.AddDays(-1).Date);
                    break;
                case "this_month":
                    report = report.Where(x => x.Order.OrderDate.Month == DateTime.Now.Month && x.Order.OrderDate.Year == DateTime.Now.Year);
                    reportAgo = reportAgo.Where(x => x.Order.OrderDate.Month == DateTime.Now.AddMonths(-1).Month && x.Order.OrderDate.Year == DateTime.Now.AddMonths(-1).Year);
                    break;
                case "this_year":
                    report = report.Where(x => x.Order.OrderDate.Year == DateTime.Now.Year);
                    reportAgo = reportAgo.Where(x => x.Order.OrderDate.Year == DateTime.Now.AddYears(-1).Year);
                    break;
                default:
                    report = report.Where(x => x.Order.OrderDate == DateTime.Now.Date);
                    reportAgo = reportAgo.Where(x => x.Order.OrderDate == DateTime.Now.AddDays(-1).Date);
                    break;
            }
            var reportCount = await report.SumAsync(x => x.Quantity);
            var reportAgoCount = await reportAgo.SumAsync(x => x.Quantity);
            reportCard = new reportCard
            {
                filter = filter,
                quantity = reportCount,
                isAsc = reportCount - reportAgoCount > 0 ? true : false,
                percent = reportAgoCount == 0 ? 100 : (float)Math.Round(((double)(reportCount - reportAgoCount) / reportAgoCount) * 100, 2)
            };


            return PartialView(reportCard);
        }
        [HttpPost]
        public async Task<PartialViewResult> customercard(string filter)
        {
            reportCard reportCard = null;
            var report = _db.Account.AsNoTracking().AsQueryable().Where(x => x.Role == "customer");
            var reportAgo = _db.Account.AsNoTracking().AsQueryable().Where(x => x.Role == "customer");
            switch (filter)
            {
                case "today":
                    report = report.Where(x => x.CreatedAt == DateTime.Now.Date);
                    reportAgo = reportAgo.Where(x => x.CreatedAt == DateTime.Now.AddDays(-1).Date);
                    break;
                case "this_month":
                    report = report.Where(x => x.CreatedAt.Month == DateTime.Now.Month && x.CreatedAt.Year == DateTime.Now.Year);
                    reportAgo = reportAgo.Where(x => x.CreatedAt.Month == DateTime.Now.AddMonths(-1).Month && x.CreatedAt.Year == DateTime.Now.AddMonths(-1).Year);
                    break;
                case "this_year":
                    report = report.Where(x => x.CreatedAt.Year == DateTime.Now.Year);
                    reportAgo = reportAgo.Where(x => x.CreatedAt.Year == DateTime.Now.AddYears(-1).Year);
                    break;
                default:
                    report = report.Where(x => x.CreatedAt == DateTime.Now.Date);
                    reportAgo = reportAgo.Where(x => x.CreatedAt == DateTime.Now.AddDays(-1).Date);
                    break;
            }
            var reportCount = await report.CountAsync();
            var reportAgoCount = await reportAgo.CountAsync();
            reportCard = new reportCard
            {
                filter = filter,
                quantity = reportCount,
                isAsc = reportCount - reportAgoCount > 0 ? true : false,
                percent = reportAgoCount == 0 ? 100 : (float)Math.Round(((double)(reportCount - reportAgoCount) / reportAgoCount) * 100, 2)
            };


            return PartialView(reportCard);
        }
        [HttpPost]
        public async Task<PartialViewResult> revenuecard(string filter)
        {
            reportCard reportCard = null;
            var report = _db.OrderDetail.Include(x => x.Order).AsNoTracking().AsQueryable().Where(x => x.Order.Status == 3);
            var reportAgo = _db.OrderDetail.Include(x => x.Order).AsNoTracking().AsQueryable().Where(x => x.Order.Status == 3);
            switch (filter)
            {
                case "today":
                    report = report.Where(x => x.Order.OrderDate == DateTime.Now.Date);
                    reportAgo = reportAgo.Where(x => x.Order.OrderDate == DateTime.Now.AddDays(-1).Date);
                    break;
                case "this_month":
                    report = report.Where(x => x.Order.OrderDate.Month == DateTime.Now.Month && x.Order.OrderDate.Year == DateTime.Now.Year);
                    reportAgo = reportAgo.Where(x => x.Order.OrderDate.Month == DateTime.Now.AddMonths(-1).Month && x.Order.OrderDate.Year == DateTime.Now.AddMonths(-1).Year);
                    break;
                case "this_year":
                    report = report.Where(x => x.Order.OrderDate.Year == DateTime.Now.Year);
                    reportAgo = reportAgo.Where(x => x.Order.OrderDate.Year == DateTime.Now.AddYears(-1).Year);
                    break;
                default:
                    report = report.Where(x => x.Order.OrderDate == DateTime.Now.Date);
                    reportAgo = reportAgo.Where(x => x.Order.OrderDate == DateTime.Now.AddDays(-1).Date);
                    break;
            }
            var reportCount = await report.SumAsync(x => (decimal?)x.Subtotal) ?? 0;
            var reportAgoCount = await reportAgo.SumAsync(x => (decimal?)x.Subtotal) ?? 0;

            reportCard = new reportCard
            {
                filter = filter,
                quantity = reportCount,
                isAsc = reportCount > reportAgoCount,
                percent = reportAgoCount == 0
                    ? 100f
                    : (float)Math.Round(
                        ((reportCount - reportAgoCount) / reportAgoCount) * 100m,
                        2
                    )
            };
            return PartialView(reportCard);
        }
        [HttpPost]
        public async Task<PartialViewResult> recentsales()
        {
            List<Order> recentsales = null;
            var report = _db.Order.Include(x => x.Account).AsNoTracking().OrderByDescending(x => x.OrderDate).Take(5).ToList();
            recentsales = report;
            return PartialView(recentsales);
        }
        [HttpPost]
        public async Task<PartialViewResult> TopSelling(string filter)
        {
            reportTopSell reportCard = null;
            var report = _db.OrderDetail.Include(x => x.Order).Include(x => x.Product).AsNoTracking().AsQueryable().Where(x => x.Order.Status != 5);
            switch (filter)
            {
                case "today":
                    report = report.Where(x => x.Order.OrderDate == DateTime.Now.Date);
                    break;
                case "this_month":
                    report = report.Where(x => x.Order.OrderDate.Month == DateTime.Now.Month && x.Order.OrderDate.Year == DateTime.Now.Year);
                    break;
                case "this_year":
                    report = report.Where(x => x.Order.OrderDate.Year == DateTime.Now.Year);
                    break;
                default:
                    report = report.Where(x => x.Order.OrderDate == DateTime.Now.Date);
                    break;
            }
            var listTopSell = await report
                .GroupBy(x => new { x.ProductId, x.Product.ProductName, x.Product.MainImage, x.Product.SellingPrice })
                .Select(g => new ProductTopSell
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    ProductImage = g.Key.MainImage,
                    PriceNow = g.Key.SellingPrice,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    revenue = g.Sum(x => x.Subtotal)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(5)
                .ToListAsync();
            reportCard = new reportTopSell
            {
                filter = filter,
                listTopSell = listTopSell
            };

            return PartialView(reportCard);
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