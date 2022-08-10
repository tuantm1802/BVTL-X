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

namespace WebApp.Controllers
{
    public class BaseNoRoleController : Controller
    {
        PageMenuDA _pageMenuDA = new PageMenuDA();
        UserDA _userDA = new UserDA();
        ITokenService _ITokenService = new TokenService();
        private static readonly string AppCode = ConfigurationManager.AppSettings["AppCode"];
        private static readonly string ApiUri = ConfigurationManager.AppSettings["ApiUri"];
        private static readonly string IsDev = ConfigurationManager.AppSettings["IsDev"].ToString();
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["USER_SESSION"] == null)
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
                if (IsDev == "0")
                    filterContext.Result = new RedirectResult(ApiUri + "Login/Index?appcode=" + AppCode);
                else
                    filterContext.Result = new RedirectResult("Login/Index");
                return;
            }
            else
            {
                var session = (UserLogin)Session["USER_SESSION"];
                // Kiểm xem người dùng có bị khóa không
                var check = _userDA.CheckLock((int)session.UserID);
                if (check)
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
                    if (IsDev == "0")
                        filterContext.Result = new RedirectResult(ApiUri + "Login/Index?appcode=" + AppCode);
                    else
                        filterContext.Result = new RedirectResult("Login/Index");
                    return;
                }
            }

            //// Kiểm tra xem người dùng có quyền truy cập menu không
            //var pageMenu = (List<MenuModel>)filterContext.HttpContext.Session.Contents["Menus"];
            //Controller controller = filterContext.Controller as Controller;
            //string controllerName = controller.RouteData.Values["controller"].ToString();
            //var data = pageMenu.FirstOrDefault(x => !string.IsNullOrEmpty(x.HREF_URL) && x.HREF_URL.ToUpper().Contains(controllerName.ToUpper()));
            //if (data == null)
            //{
            //    Session["USER_SESSION"] = null;
            //    Session["CEDENTIALS_SESSION"] = null;
            //    Session["Menus"] = null;
            //    Session["PVDuLieuQS"] = null;
            //    Request.Cookies.Remove("UserToken");
            //    if (Request.Cookies["UserToken"] != null)
            //    {
            //        Response.Cookies["UserToken"].Expires = DateTime.Now.AddDays(-1);
            //    }
            //    filterContext.Result = new RedirectResult(ApiUri + "Login/Index?appcode=" + AppCode);
            //    return;
            //}  

            base.OnActionExecuting(filterContext);
        }


        public List<string> GetBottomRoleByController(string controllerName, List<MenuModel> menu)
        {
            var result = new List<string>();
            try
            {
                var cs = menu.FirstOrDefault(x => !string.IsNullOrEmpty(x.HREF_URL) && x.HREF_URL.ToUpper().Contains(controllerName.ToUpper()));
                if (cs != null && !string.IsNullOrEmpty(cs.Actions))
                    result = cs.Actions.Split('|').ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
            }
            return result;
        }
    }
}