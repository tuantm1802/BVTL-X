using Common;
using Common.Common;
using Common.ICommon;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        readonly ISysLogDA _sysLogDA;
        readonly IUserDA _userDA;
        readonly ISysParameterDA _sysParameterDA;
        readonly IPageMenuDA _pageMenuDA;
        readonly ISystemMonitorDA _systemMonitorDA;
        IEncryptor _encryptor = new Encryptor();

        public LoginController(ISysLogDA sysLogDA = null, IUserDA userDA = null, ISysParameterDA sysParameterDA = null, IPageMenuDA pageMenuDA = null, ISystemMonitorDA systemMonitorDA = null)
        {
            _sysLogDA = sysLogDA ?? new SysLogDA();
            _userDA = userDA ?? new UserDA();
            _sysParameterDA = sysParameterDA ?? new SysParameterDA();
            _pageMenuDA = pageMenuDA ?? new PageMenuDA();
            _systemMonitorDA = systemMonitorDA ?? new SystemMonitorDA();
        }
        // GET: Login
        public ActionResult Index()
        {
            AddLog("Redireact vào Login/Index.");

            return View();
        }
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string ip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.ServerVariables["REMOTE_ADDR"] ?? "").Split(',')[0].Trim();
                string userAgent = Request.UserAgent;

                var result = _userDA.Login(model.UserName, model.Password);
                if (result == 1)
                {
                    var userSession = new UserLogin();
                    var user = _userDA.GetItemByUserName(model.UserName);
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.ID;
                    userSession.Name = user.Name;
                    userSession.IsAdmin = user.IsAdmin;
                    userSession.Avartar = user.Avartar;
                    userSession.GroupId = user.GroupID;
                    userSession.CityCodes = user.CityCodes;
                    userSession.MaDuAn = user.MaDuAn;

                    var listPermission = _userDA.GetListCredentials(model.UserName);
                    var menus = _pageMenuDA.GetMenuByUser((int)user.ID);

                    // Lấy thông báo
                    var notificationSearch = new ReportSearchModel
                    {
                        MaDuAn = user.MaDuAn,
                        CityCodes = user.CityCodes
                    };
                    var notifications = _userDA.GetNotification(notificationSearch);

                    Session.Add("CEDENTIALS_SESSION", listPermission);
                    Session.Add("Menus", menus);
                    Session.Add("USER_SESSION", userSession);
                    Session.Add("Notifications", notifications);
                    AddLog("Đăng nhập( UserName: " + user.UserName + ") thành công.");

                    // Ghi nhận phiên đăng nhập & trực tuyến vào Giám sát Hệ thống
                    _systemMonitorDA.RecordLogin(userSession, ip, userAgent, "Success", Session.SessionID);

                    return RedirectToAction("Index", "Home");
                }
                else if (result == 0)
                {
                    _systemMonitorDA.RecordLogin(new UserLogin { UserName = model.UserName }, ip, userAgent, "UserNotFound", Session.SessionID);
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Tài khoản không tồn tại.");
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    _systemMonitorDA.RecordLogin(new UserLogin { UserName = model.UserName }, ip, userAgent, "AccountLocked", Session.SessionID);
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Tài khoản đang bị khóa.");
                    ModelState.AddModelError("", "Tài khoản đang bị khóa.");
                }
                else if (result == -2)
                {
                    _systemMonitorDA.RecordLogin(new UserLogin { UserName = model.UserName }, ip, userAgent, "WrongPassword", Session.SessionID);
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Mật khẩu không đúng.");
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else
                {
                    _systemMonitorDA.RecordLogin(new UserLogin { UserName = model.UserName }, ip, userAgent, "Failed", Session.SessionID);
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Đăng nhập không thành công.");
                    ModelState.AddModelError("", "Đăng nhập không thành công.");
                }
            }
            return View("Index");
        }

        public string ConvertPathImageToBase64(string path)
        {
            string base64String = "";
            var pathRoot = Server.MapPath("~/FileUpload");

            var arrayPath = path.IndexOf("FileUpload");
            string pathGet = path.Substring(0, arrayPath) + "FileUpload";
            string patttt = path.Replace(pathGet, pathRoot);

            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    using (Image image = Image.FromFile(path))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();
                            base64String = Convert.ToBase64String(imageBytes);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return base64String;
        }

        private void AddLog(string content)
        {
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "Login",
                        UserName = "",
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        [HttpGet]
        public JsonResult GetSystemVersion()
        {
            var info = global::Common.Common.AppVersionHelper.GetVersionInfo();
            return Json(new { Success = true, Data = info }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            if (user != null)
            {
                AddLog("Đăng xuất( UserName: " + user.UserName + ") thành công.");
                _systemMonitorDA.RecordLogout(Session.SessionID);
            }
            Session["USER_SESSION"] = null;
            Session["Menus"] = null;
            Session["Notifications"] = null;
            return Redirect("/Login/Index");
        }
    }
}