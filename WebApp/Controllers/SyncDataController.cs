using Common;
using Common.Common;
using Common.ICommon;
using Data.Admin;
using Data.API;
using Data.InterfaceDA.Admin;
using Data.InterfaceDA.API;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.API;
using Model.ModelExtend.Base;
using Quartz;
using Quartz.Impl.Matchers;
using WebApp.Common;
using WebApp.Services;
using WebApp.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class SyncDataController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly ISyncDataDA _syncDataDA;
        readonly IInsertDataDA _insertDataDA;
        readonly ISyncDataFromApi_SaveToDB _syncDataFromApi_SaveToDB = new SyncDataFromApi_SaveToDB();
        readonly ISysLogDA _sysLogDA;
        readonly ProcessService _processService = new ProcessService();
        BaseController _helperController = new BaseController();

        public SyncDataController(ISyncDataDA syncDataDA = null, IInsertDataDA insertDataDA = null, ISysLogDA sysLogDA = null)
        {
            _syncDataDA = syncDataDA ?? new SyncDataDA();
            _insertDataDA = insertDataDA ?? new InsertDataDA();
            _sysLogDA = sysLogDA ?? new SysLogDA();
        }

        // GET: SyncData
        [HasCredential(ControllerName = "SyncData")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAllByPage(ModelSearch modelSearch, string status = "1")
        {
            ObjectMessage obj = new ObjectMessage { Error = false };
            try
            {
                int totalItems = 0;
                var data = _syncDataDA.GetAllByPage(modelSearch, ref totalItems, status);
                
                var formattedData = data.Select(x => new
                {
                    Api_Id = x.Api_Id,
                    Api_Code = x.Api_Code,
                    NameSyncdata = x.NameSyncdata,
                    ReportId = x.ReportId,
                    HrefApi = x.HrefApi,
                    IsActive = x.IsActive,
                    TimeReCall = x.TimeReCall,
                    Start_Time_Sync = x.Start_Time_Sync.HasValue ? x.Start_Time_Sync.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                    End_Time_Sync = x.End_Time_Sync.HasValue ? x.End_Time_Sync.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                    Message = x.Message,
                    maduan = x.maduan
                }).ToList();

                AddLog("Lấy danh sách tiến trình đồng bộ (keyword: " + modelSearch.KeyWord + ", status: " + status + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = formattedData, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                AddLog("Lấy danh sách tiến trình đồng bộ lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public ActionResult GetBottomAction()
        {
            ObjectMessage obj = new ObjectMessage { Error = false };
            try
            {
                var menu = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var bottoms = _helperController.GetBottomRoleByController(controllerName, menu);
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return Json(obj);
            }
        }

        #region Đồng bộ Thủ công (Manual Sync)
        [HttpPost]
        public async Task<object> SyncDataFromApi(int Id)
        {
            ObjectMessage obj = new ObjectMessage { Error = false };
            try
            {
                var session = (UserLogin)Session["USER_SESSION"];
                var tableNames = new List<string>();
                var infoApi = _insertDataDA.GetApiInfo(Id, ref tableNames);

                var apiResult = await _syncDataFromApi_SaveToDB.GetDataFromApi_SaveToDB(
                    infoApi.HrefApi, 
                    infoApi.TokenApi, 
                    infoApi.ReportId, 
                    infoApi.maduan, 
                    tableNames, 
                    infoApi.Api_Code, 
                    infoApi.RawOrLabel
                );

                obj.Error = !apiResult.Success;
                obj.Title = apiResult.Success ? "Đồng bộ thành công." : apiResult.Message;

                await SendTelegramMessage("589101034", "🎉 API " + infoApi.ReportId + " đồng bộ thủ công thành công! [SyncDataFromApi][Manual Sync]");
                await SendTelegramMessage("-1002496745464", "🎉 API " + infoApi.ReportId + " đồng bộ thủ công thành công! [SyncDataFromApi][Manual Sync]");

                if (obj.Error)
                    AddLog("Lấy dữ liệu từ api (" + infoApi.HrefApi + ") lỗi: " + obj.Title);
                else
                    AddLog("Lấy dữ liệu từ api (" + infoApi.HrefApi + ") thành công.");

                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                AddLog("Lấy dữ liệu từ api (ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }
        #endregion

        #region Quản trị Tự động Đồng bộ (Quartz.NET Auto-Sync Scheduler)

        /// <summary>
        /// Lấy trạng thái tổng thể của Scheduler và lịch chạy tiếp theo của các Job
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> GetSchedulerStatus()
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                bool isAdmin = user != null && user.IsAdmin;

                var scheduler = JobScheduler.Scheduler;
                bool isRunning = scheduler != null && scheduler.IsStarted && !scheduler.IsShutdown && !scheduler.InStandbyMode;
                bool isStandby = scheduler != null && scheduler.InStandbyMode;
                int jobCount = 0;
                var jobList = new List<object>();

                if (scheduler != null && !scheduler.IsShutdown)
                {
                    var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
                    jobCount = jobKeys.Count;

                    foreach (var jobKey in jobKeys)
                    {
                        var triggers = await scheduler.GetTriggersOfJob(jobKey);
                        var trigger = triggers.FirstOrDefault();

                        DateTimeOffset? nextFire = trigger?.GetNextFireTimeUtc();
                        DateTimeOffset? prevFire = trigger?.GetPreviousFireTimeUtc();

                        jobList.Add(new
                        {
                            JobName = jobKey.Name,
                            NextFireTime = nextFire.HasValue ? nextFire.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") : "—",
                            PrevFireTime = prevFire.HasValue ? prevFire.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") : "—"
                        });
                    }
                }

                return Json(new
                {
                    success = true,
                    isRunning = isRunning,
                    isStandby = isStandby,
                    isAutoSyncEnabled = JobScheduler.IsAutoSyncEnabled,
                    lastStartedTime = JobScheduler.LastStartedTime?.ToString("dd/MM/yyyy HH:mm:ss") ?? "—",
                    jobCount = jobCount,
                    jobs = jobList,
                    isAdmin = isAdmin
                });
            }
            catch (Exception ex)
            {
                log.Error("GetSchedulerStatus Error: " + ex.Message, ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Bật hoặc Tắt Tự động đồng bộ toàn hệ thống (Chỉ Admin)
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> ToggleAutoSync(bool enable)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                if (user == null || !user.IsAdmin)
                {
                    return Json(new { success = false, message = "Bạn không có quyền quản trị để thay đổi cài đặt này." });
                }

                if (enable)
                {
                    await JobScheduler.ResumeAll();
                    AddLog("Quản trị viên đã BẬT tính năng Tự động Đồng bộ hệ thống.");
                }
                else
                {
                    await JobScheduler.PauseAll();
                    AddLog("Quản trị viên đã TẠM DỪNG tính năng Tự động Đồng bộ hệ thống.");
                }

                return Json(new { success = true, isAutoSyncEnabled = JobScheduler.IsAutoSyncEnabled, message = enable ? "Đã bật tự động đồng bộ." : "Đã tạm dừng tự động đồng bộ." });
            }
            catch (Exception ex)
            {
                log.Error("ToggleAutoSync Error: " + ex.Message, ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Khởi động lại toàn bộ Quartz Scheduler (Chỉ Admin)
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> RestartScheduler()
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                if (user == null || !user.IsAdmin)
                {
                    return Json(new { success = false, message = "Bạn không có quyền quản trị để khởi động lại tiến trình." });
                }

                await JobScheduler.StartAll();
                AddLog("Quản trị viên đã Khởi động lại toàn bộ Quartz.NET Scheduler.");

                return Json(new { success = true, message = "Đã khởi động lại Scheduler thành công." });
            }
            catch (Exception ex)
            {
                log.Error("RestartScheduler Error: " + ex.Message, ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật Chu kỳ đồng bộ (TimeLoop) và Trạng thái (Active) của 1 API (Chỉ Admin)
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> UpdateSchedule(int apiId, int timeLoop, bool active)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                if (user == null || !user.IsAdmin)
                {
                    return Json(new { success = false, message = "Bạn không có quyền quản trị để thực hiện thao tác này." });
                }

                // Cập nhật CSDL
                using (var dbContext = new BVTL_REPORTINGEntities())
                {
                    var api = dbContext.BVTL_API.FirstOrDefault(x => x.Api_Id == apiId);
                    if (api == null)
                    {
                        return Json(new { success = false, message = "Không tìm thấy thông tin API." });
                    }

                    api.TimeReCall = timeLoop;
                    api.IsActive = active;
                    dbContext.SaveChanges();

                    // Cập nhật Trigger của Quartz
                    var processList = _processService.GetListProcess();
                    var processItem = processList.FirstOrDefault(x => x.ReportId == api.ReportId || x.Code == api.Api_Code);
                    if (processItem != null)
                    {
                        processItem.TimeLoop = timeLoop;
                        processItem.Active = active;
                        await JobScheduleChangeTimeloop.ChangeTimeloop(processItem);
                    }
                }

                AddLog($"Cập nhật lịch chạy API (ID: {apiId}): TimeLoop = {timeLoop}, Active = {active}");
                return Json(new { success = true, message = "Cập nhật cấu hình lịch đồng bộ thành công." });
            }
            catch (Exception ex)
            {
                log.Error("UpdateSchedule Error: " + ex.Message, ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Kích hoạt chạy ngay trong luồng ngầm cho 1 API
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> TriggerJobNow(string reportId)
        {
            try
            {
                if (string.IsNullOrEmpty(reportId))
                {
                    return Json(new { success = false, message = "Mã ReportId không hợp lệ." });
                }

                await JobScheduler.TriggerJobNow(reportId);
                AddLog($"Kích hoạt chạy ngay tiến trình ngầm cho API [{reportId}].");

                return Json(new { success = true, message = $"Tiến trình {reportId} đã được đưa vào hàng đợi thực thi ngay!" });
            }
            catch (Exception ex)
            {
                log.Error("TriggerJobNow Error: " + ex.Message, ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Kích hoạt chạy ngay toàn bộ API trong luồng ngầm (Chỉ Admin)
        /// </summary>
        [HttpPost]
        public async Task<JsonResult> TriggerAllNow()
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                if (user == null || !user.IsAdmin)
                {
                    return Json(new { success = false, message = "Bạn không có quyền quản trị để thực hiện thao tác này." });
                }

                var list = _processService.GetListProcess();
                int triggered = 0;
                foreach (var item in list)
                {
                    if (item.Active)
                    {
                        await JobScheduler.TriggerJobNow(item.ReportId);
                        triggered++;
                    }
                }

                AddLog($"Kích hoạt chạy ngay {triggered} tiến trình đồng bộ ngầm.");
                return Json(new { success = true, message = $"Đã kích hoạt {triggered} tiến trình đồng bộ ngầm thành công!" });
            }
            catch (Exception ex)
            {
                log.Error("TriggerAllNow Error: " + ex.Message, ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Helpers
        public static async Task SendTelegramMessage(string chatId, string message)
        {
            string botToken = "7553997923:AAFBabzEfRLdluri42vy3VixZwrLfv2BHPs";
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage";

            try
            {
                using (var client = new HttpClient())
                {
                    var parameters = new Dictionary<string, string>
                    {
                        { "chat_id", chatId },
                        { "text", message }
                    };

                    var content = new FormUrlEncodedContent(parameters);
                    HttpResponseMessage response = await client.PostAsync(url, content);
                }
            }
            catch (Exception ex)
            {
                log.Error("SendTelegramMessage Error: " + ex.Message);
            }
        }

        [HttpPost]
        public JsonResult GetEndTimeSync(string apiCode)
        {
            try
            {
                using (var dbContext = new BVTL_REPORTINGEntities())
                {
                    var endTimeSync = dbContext.BVTL_API
                        .Where(x => x.Api_Code == apiCode)
                        .Select(x => x.End_Time_Sync)
                        .FirstOrDefault();

                    if (endTimeSync.HasValue)
                    {
                        return Json(new { success = true, endTimeSync = endTimeSync.Value.ToString("dd/MM/yyyy HH:mm") }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("GetEndTimeSync EF Error: " + ex.Message, ex);
            }

            return Json(new { success = false, message = "Không tìm thấy thông tin đồng bộ" }, JsonRequestBehavior.AllowGet);
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                new BVTL_QT_LOG
                {
                    ControllerName = "SyncData",
                    UserName = user != null ? user.UserName : "SYSTEM",
                    DateLog = DateTime.Now,
                    Content = content
                }
            );
        }
        #endregion
    }
}