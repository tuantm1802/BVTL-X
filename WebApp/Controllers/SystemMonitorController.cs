using Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.ModelExtend.Base;
using Model.ModelExtend.SystemMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class SystemMonitorController : BaseController
    {
        private readonly ISystemMonitorDA _systemMonitorDA;
        private readonly IUserDA _userDA;

        public SystemMonitorController(ISystemMonitorDA systemMonitorDA = null, IUserDA userDA = null)
        {
            _systemMonitorDA = systemMonitorDA ?? new SystemMonitorDA();
            _userDA = userDA ?? new UserDA();
        }

        // GET: SystemMonitor/UserAccess
        public ActionResult UserAccess()
        {
            return View();
        }

        // GET: SystemMonitor/FeatureUsage
        public ActionResult FeatureUsage()
        {
            return View();
        }

        #region Heartbeat Endpoint
        [HttpPost]
        public ActionResult Heartbeat(string currentUrl)
        {
            try
            {
                var session = Session["USER_SESSION"] as UserLogin;
                if (session != null && !string.IsNullOrEmpty(Session.SessionID))
                {
                    _systemMonitorDA.UpdateHeartbeat(Session.SessionID, currentUrl, null, null);
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region User Access & Traffic APIs
        [HttpPost]
        public ActionResult GetOnlineUsers(string keyword = "", int page = 1, int pageSize = 10)
        {
            try
            {
                int totalItems = 0;
                var data = _systemMonitorDA.GetOnlineUsers(keyword, page, pageSize, ref totalItems);
                return Json(new { data = data, totalItems = totalItems, Error = false });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<UserOnlineModel>(), totalItems = 0, Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetLoginHistory(SystemMonitorSearchModel search)
        {
            try
            {
                if (search == null) search = new SystemMonitorSearchModel();
                int totalItems = 0;
                var data = _systemMonitorDA.GetLoginHistory(search, ref totalItems);
                return Json(new { data = data, totalItems = totalItems, Error = false });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<LoginHistoryModel>(), totalItems = 0, Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTrafficStats(int days = 7)
        {
            try
            {
                var stats = _systemMonitorDA.GetTrafficStats(days);
                return Json(new { stats = stats, Error = false });
            }
            catch (Exception ex)
            {
                return Json(new { stats = new TrafficStatsModel(), Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ForceLogout(string sessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionId))
                    return Json(new { success = false, message = "Session ID không hợp lệ." });

                _systemMonitorDA.RecordLogout(sessionId);
                return Json(new { success = true, message = "Đã ngắt phiên làm việc của người dùng thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetUserProfile(int userId)
        {
            try
            {
                var user = _userDA.GetItemById(userId);
                if (user == null)
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng." });

                return Json(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region Feature Usage & Analytics APIs
        [HttpPost]
        public ActionResult GetFeatureUsageStats(SystemMonitorSearchModel search)
        {
            try
            {
                if (search == null) search = new SystemMonitorSearchModel();
                int totalItems = 0;
                var data = _systemMonitorDA.GetFeatureUsageStats(search, ref totalItems);
                return Json(new { data = data, totalItems = totalItems, Error = false });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<FeatureUsageModel>(), totalItems = 0, Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetTopFeatures(int top = 10, int days = 30)
        {
            try
            {
                var data = _systemMonitorDA.GetTopFeatures(top, days);
                return Json(new { data = data, Error = false });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<FeatureRankModel>(), Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetUnderutilizedFeatures(int days = 30)
        {
            try
            {
                var data = _systemMonitorDA.GetUnderutilizedFeatures(days);
                return Json(new { data = data, Error = false });
            }
            catch (Exception ex)
            {
                return Json(new { data = new List<FeatureRankModel>(), Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetRoleUsageDistribution(int days = 30)
        {
            try
            {
                var data = _systemMonitorDA.GetRoleUsageDistribution(days);
                return Json(new { data = data, Error = false });
            }
            catch (Exception ex)
            {
                return Json(new { data = new Dictionary<string, int>(), Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetDashboardKPIs()
        {
            try
            {
                var kpi = _systemMonitorDA.GetDashboardKPIs();
                return Json(new { kpi = kpi, Error = false });
            }
            catch (Exception ex)
            {
                return Json(new { kpi = new SystemMonitorDashboardKPIs(), Error = true, Title = ex.Message });
            }
        }
        #endregion
    }
}
