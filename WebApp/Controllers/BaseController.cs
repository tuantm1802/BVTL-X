using Data;
using Data.Admin;
using Model.ModelExtend;
using Newtonsoft.Json;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using WebApp.Service;
using Common;

namespace WebApp.Controllers
{
    public class BaseController : Controller
    {
        PageMenuDA _pageMenuDA = new PageMenuDA();
        UserDA _userDA = new UserDA();
        ITokenService _ITokenService = new TokenService();
        private static readonly string IsDev = ConfigurationManager.AppSettings["IsDev"].ToString();
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["USER_SESSION"] == null)
            {
                var token = Request.Cookies["UserToken"].Value.ToString();
                // kiểm tra xem token còn hạn không
                var userName = _ITokenService.ValidateToken(token);
                if (!string.IsNullOrEmpty(userName))
                {

                    var user = _userDA.GetItemByUserName(userName);
                    // Kiểm xem người dùng có bị khóa không
                    var check = _userDA.CheckLock((int)user.ID);
                    if (check == false)
                    {
                        var session = (UserLogin)Session["USER_SESSION"];
                        if (session == null)
                        {
                            var userSession = new UserLogin
                            {
                                UserName = user.UserName,
                                UserID = user.ID,
                                Name = user.Name,
                                UserGroup = user.GroupID
                            };
                            var listPermission = _userDA.GetListCredentials(userName);
                            var menus = _pageMenuDA.GetMenuByUser((int)user.ID);
                            Session.Add("CEDENTIALS_SESSION", listPermission);
                            Session.Add("Menus", menus);
                            Session.Add("USER_SESSION", userSession);
                        }
                        writeLogUserLogin(session, filterContext);
                    }
                    else
                    {
                        resetSession();
                        filterContext.Result = new RedirectResult("/Login/Index");
                        return;
                    }
                }
                else
                {
                    resetSession();
                    filterContext.Result = new RedirectResult("/Login/Index");
                    return;
                }
            }
            else
            {
                var session = (UserLogin)Session["USER_SESSION"];
                writeLogUserLogin(session, filterContext);
                // Kiểm xem người dùng có bị khóa không
                var check = _userDA.CheckLock((int)session.UserID);
                if (check)
                {
                    resetSession();
                    filterContext.Result = new RedirectResult("/Login/Index");
                    return;
                }
                if (Request.Cookies["UserToken"] == null)
                {
                    var token = _ITokenService.GenerateAccessToken(session.UserName);
                    var listIdValue = new HttpCookie("UserToken", token);
                    Response.Cookies.Add(listIdValue);
                }
            }

            // Kiểm tra xem người dùng có quyền truy cập menu không
            var pageMenu = (List<MenuModel>)filterContext.HttpContext.Session.Contents["Menus"];
            Controller controller = filterContext.Controller as Controller;
            string controllerName = controller.RouteData.Values["controller"].ToString();
            string actionName = controller.RouteData.Values["action"].ToString();
            //Bỏ qua và cho phép truy cập cac Actions cập thông tin của tài khoản
            if (controllerName == "User" && !Constants.ActionsAllowAcess.Any(s => s == actionName))
            {
                var data = pageMenu.FirstOrDefault(x => !string.IsNullOrEmpty(x.HREF_URL) && x.HREF_URL.ToUpper().Contains(controllerName.ToUpper()));
                if (data == null)
                {
                    resetSession();
                    filterContext.Result = new RedirectResult("/ErrorPage/Error404");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }

        public List<string> GetBottomRoleByController(string controllerName, List<MenuModel> menu)
        {
            var result = new List<string>();
            try
            {
                var cs = menu.FirstOrDefault(x => !string.IsNullOrEmpty(x.HREF_URL) && x.HREF_URL.ToUpper().Contains("/" + controllerName.ToUpper() + "/"));
                if (cs != null && !string.IsNullOrEmpty(cs.Actions))
                    result = cs.Actions.Split('|').ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
            }
            return result;
        }
        //public static bool GuiThongBaoBase(string noiDung, int chucNang, int ngGuiId, string cbTenNgGui, int ngNhanId, string urlChucNang, int chucNangId)
        //{
        //    try
        //    {
        //        ThongBaoDA thongBaoDA = new ThongBaoDA();
        //        if (ngGuiId > 0 && ngNhanId > 0)
        //        {
        //            var data = thongBaoDA.ThemMoiThongBao(noiDung, chucNang, ngGuiId, cbTenNgGui, ngNhanId, urlChucNang, chucNangId);
        //            return data.Error ? false : true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        private void resetSession()
        {
            Session["USER_SESSION"] = null;
            Session["CEDENTIALS_SESSION"] = null;
            Session["Menus"] = null;
            Session["PVDuLieuQS"] = null;
            Request.Cookies.Remove("UserToken");
            if (Request.Cookies["UserToken"] != null)
            {
                Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
            }
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddDays(-1);
            }
        }
        private void writeLogUserLogin(UserLogin userLogin, ActionExecutingContext filterContext)
        {
            log4net.GlobalContext.Properties["UserName"] = userLogin is null ? null : userLogin.UserName;
            log4net.GlobalContext.Properties["IpAddress"] = (filterContext.HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? filterContext.HttpContext.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
            var logger = new Common.ActionLogger();
            logger.InsertRequestAudit(filterContext);
        }
    }
}