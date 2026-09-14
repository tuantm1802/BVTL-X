using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using log4net;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebApp.Services;
using WebApp.Services.ScheduleTasks;

namespace WebApp.Controllers
{
    public class ScheduledReportController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ScheduledReportController));
        private readonly IScheduledReportDA _scheduledReportDA;
        private readonly IReportExportService _reportExportService;

        public ScheduledReportController(
            IScheduledReportDA scheduledReportDA = null,
            IReportExportService reportExportService = null)
        {
            _scheduledReportDA = scheduledReportDA ?? new ScheduledReportDA();
            _reportExportService = reportExportService ?? new ReportExportService(_scheduledReportDA);
        }

        // GET: ScheduledReport
        public ActionResult Index()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            if (user == null) return Redirect("/Login/Index");
            return View();
        }

        [HttpGet]
        public JsonResult GetSettings()
        {
            try
            {
                var settings = _scheduledReportDA.GetSettings();
                return Json(new { Success = true, Settings = settings }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetSettings: " + ex.Message, ex);
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveSettings(int runDay, int runHour, bool isActive)
        {
            try
            {
                if (runDay < 1 || runDay > 28)
                {
                    return Json(new { Success = false, Message = "Ngày chạy trong tháng phải từ 1 đến 28." });
                }
                if (runHour < 0 || runHour > 23)
                {
                    return Json(new { Success = false, Message = "Giờ chạy trong ngày phải từ 0 đến 23." });
                }

                bool updated = _scheduledReportDA.UpdateSettings(runDay, runHour, isActive);
                if (updated)
                {
                    await JobScheduler.ReschedulePeriodicReportJob(runDay, runHour);
                    return Json(new { Success = true, Message = "Cập nhật cấu hình lịch chạy báo cáo thành công!" });
                }
                return Json(new { Success = false, Message = "Không thể cập nhật cấu hình CSDL." });
            }
            catch (Exception ex)
            {
                log.Error("Lỗi SaveSettings: " + ex.Message, ex);
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetLogs(string reportType = null, int? year = null, int? month = null, int pageIndex = 1, int pageSize = 15)
        {
            try
            {
                int totalRows = 0;
                var logs = _scheduledReportDA.GetExportLogs(reportType, year, month, pageIndex, pageSize, out totalRows);
                return Json(new
                {
                    Success = true,
                    Data = logs,
                    TotalRows = totalRows,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalRows / pageSize)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetLogs: " + ex.Message, ex);
                return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> TriggerExportNow(int? year = null, int? month = null, string reportType = "ALL")
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                string userName = user != null ? user.UserName : "Admin";

                var now = DateTime.Now;
                int targetYear = year.HasValue && year.Value > 2000 ? year.Value : (now.Month == 1 ? now.Year - 1 : now.Year);
                int targetMonth = month.HasValue && month.Value >= 1 && month.Value <= 12 ? month.Value : (now.Month == 1 ? 12 : now.Month - 1);

                if (reportType == "TCV_CD45")
                {
                    var res = await _reportExportService.ExportTCVCD45ZipAsync(targetYear, targetMonth, null, null, "Manual", userName);
                    return Json(new { Success = res.Success, Message = res.Message, Result = res });
                }
                else if (reportType == "HOATDONG_CD45")
                {
                    var res = await _reportExportService.ExportHoatDongCD45ExcelAsync(targetYear, targetMonth, null, null, "Manual", userName);
                    return Json(new { Success = res.Success, Message = res.Message, Result = res });
                }
                else if (reportType == "TONGHOP_BVTL")
                {
                    var res = await _reportExportService.ExportTongHopBVTLExcelAsync(targetYear, targetMonth, "Manual", userName);
                    return Json(new { Success = res.Success, Message = res.Message, Result = res });
                }
                else // ALL
                {
                    var results = await _reportExportService.ExecuteAllMonthlyReportsAsync(targetYear, targetMonth, "Manual", userName);
                    int success = results.FindAll(r => r.Success).Count;
                    return Json(new
                    {
                        Success = success > 0,
                        Message = $"Đã hoàn thành xử lý {results.Count} báo cáo ({success} thành công).",
                        Results = results
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi TriggerExportNow: " + ex.Message, ex);
                return Json(new { Success = false, Message = "Lỗi kích hoạt xuất báo cáo: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult DownloadFile(long id)
        {
            try
            {
                var logItem = _scheduledReportDA.GetById(id);
                if (logItem == null || string.IsNullOrEmpty(logItem.FileName))
                {
                    return Content("<script>alert('Không tìm thấy bản ghi báo cáo!'); window.history.back();</script>");
                }

                string resolvedPath = logItem.FilePath;

                // 1. Kiểm tra nếu đường dẫn lưu trong DB tồn tại trên máy chủ hiện tại
                if (string.IsNullOrEmpty(resolvedPath) || !System.IO.File.Exists(resolvedPath))
                {
                    // 2. Thử tìm theo cấu trúc thư mục chuẩn trong App_Data của WebApp
                    if (logItem.Year > 0 && logItem.Month.HasValue && !string.IsNullOrEmpty(logItem.FileName))
                    {
                        string candidate = Server.MapPath($"~/App_Data/ExportedReports/{logItem.Year}/{logItem.Month.Value:D2}/{logItem.FileName}");
                        if (System.IO.File.Exists(candidate))
                        {
                            resolvedPath = candidate;
                        }
                    }

                    // 3. Thử tìm trực tiếp theo tên file trong App_Data/ExportedReports
                    if ((string.IsNullOrEmpty(resolvedPath) || !System.IO.File.Exists(resolvedPath)) && !string.IsNullOrEmpty(logItem.FileName))
                    {
                        string candidateFlat = Server.MapPath($"~/App_Data/ExportedReports/{logItem.FileName}");
                        if (System.IO.File.Exists(candidateFlat))
                        {
                            resolvedPath = candidateFlat;
                        }
                    }
                }

                if (string.IsNullOrEmpty(resolvedPath) || !System.IO.File.Exists(resolvedPath))
                {
                    return Content("<script>alert('File vật lý không còn tồn tại trên máy chủ!'); window.history.back();</script>");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(resolvedPath);
                string contentType = "application/octet-stream";
                if (logItem.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/zip";
                }
                else if (logItem.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }

                return File(fileBytes, contentType, logItem.FileName);
            }
            catch (Exception ex)
            {
                log.Error("Lỗi DownloadFile: " + ex.Message, ex);
                return Content($"<script>alert('Lỗi tải file: {ex.Message}'); window.history.back();</script>");
            }
        }
    }
}
