using log4net;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Common
{
    public class QLTBAuthorizeAttribute : ActionFilterAttribute
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string AppCode = ConfigurationManager.AppSettings["AppCode"];
        private static readonly string ApiUri = ConfigurationManager.AppSettings["ApiUri"];
        private static readonly string IsDev = ConfigurationManager.AppSettings["IsDev"].ToString();

        public string function { get; set; }
        public string role { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var pageMenu = (List<MenuModel>)filterContext.HttpContext.Session.Contents["Menus"];
            if (!string.IsNullOrEmpty(role))
            {
                Controller controller = filterContext.Controller as Controller;
                string controllerName = controller.RouteData.Values["controller"].ToString();
                string actionName = controller.RouteData.Values["action"].ToString();
                var listAction = "";

                var data = pageMenu.FirstOrDefault(x => !string.IsNullOrEmpty(x.HREF_URL) && x.HREF_URL.ToUpper().Contains(controllerName.ToUpper()));
                if (data != null && !string.IsNullOrEmpty(data.Actions))
                    listAction = data.Actions;

                if (!string.IsNullOrEmpty(listAction) && listAction.Contains(role))
                    return;
                else
                {
                    filterContext.HttpContext.Session["USER_SESSION"] = null;
                    filterContext.HttpContext.Session["CEDENTIALS_SESSION"] = null;
                    filterContext.HttpContext.Session["Menus"] = null;
                    filterContext.HttpContext.Session["PVDuLieuQS"] = null;
                    if (IsDev == "0")
                        filterContext.Result = new RedirectResult(ApiUri + "Login/Index?appcode=" + AppCode);
                    else
                        filterContext.Result = new RedirectResult("/Login/Index");
                    return;
                }
            }
            else
            {
                Controller controller = filterContext.Controller as Controller;
                string controllerName = controller.RouteData.Values["controller"].ToString();
                string actionName = controller.RouteData.Values["action"].ToString();

                if (controller != null && filterContext.HttpContext.Session["USER_SESSION"] != null)
                    return;
                else
                {
                    filterContext.HttpContext.Session["USER_SESSION"] = null;
                    filterContext.HttpContext.Session["CEDENTIALS_SESSION"] = null;
                    filterContext.HttpContext.Session["Menus"] = null;
                    filterContext.HttpContext.Session["PVDuLieuQS"] = null;
                    if (IsDev == "0")
                        filterContext.Result = new RedirectResult(ApiUri + "Login/Index?appcode=" + AppCode);
                    else
                        filterContext.Result = new RedirectResult("/Login/Index");
                    return;
                }

            }
            base.OnActionExecuting(filterContext);
        }

    }
}