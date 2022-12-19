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
        ISysLogDA _sysLogDA = new SysLogDA();
        IUserDA _userDA = new UserDA();
        ISysParameterDA _sysParameterDA = new SysParameterDA();
        IPageMenuDA _pageMenuDA = new PageMenuDA();
        IEncryptor _encryptor = new Encryptor();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _userDA.Login(model.UserName, _encryptor.MD5Hash(model.Password));
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
                    return RedirectToAction("Index", "Home");

                }
                else if (result == 0)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Tài khoản không tồn tại.");
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                else if (result == -1)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Tài khoản đang bị khóa.");
                    ModelState.AddModelError("", "Tài khoản đang bị khóa.");
                }
                else if (result == -2)
                {
                    AddLog("Đăng nhập( UserName: " + model.UserName + ") lỗi: Mật khẩu không đúng.");
                    ModelState.AddModelError("", "Mật khẩu không đúng.");
                }
                else
                {
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

                            // Convert byte[] to Base64 String
                            base64String = "data:image/jpeg;base64," + Convert.ToBase64String(imageBytes);
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

        public ActionResult Logout()
        {
            var user = Session["USER_SESSION"] as UserLogin;
            AddLog("Đăng xuất( UserName: " + user.UserName + ") thành công.");
            Session["USER_SESSION"] = null;
            Session["Menus"] = null;
            Session["Notifications"] = null;
            return Redirect("/Login/Index");
        }
    }
}