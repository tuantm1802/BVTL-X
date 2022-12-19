using Common;
using Common.Common;
using Common.ICommon;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using Model.ModelExtend.User;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class NotificationController : Controller
    {
        ISysLogDA _sysLogDA = new SysLogDA();
        // GET: Notification
        public ActionResult Index()
        {
            var notifications = Session["Notifications"] as List<NotificationModel>;
            return View(notifications);
        }

        public ActionResult GetAllNotification()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var notifications = Session["Notifications"] as List<NotificationModel>;

                AddLog("Lấy dữ liệu thông báo thành công.");

                return Json(new { Notifications = notifications, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu thông báo lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "Notification",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

    }
}