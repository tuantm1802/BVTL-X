using Model.ModelExtend;
using Model.ModelExtend.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Common.Common
{
    public class HasCredentialAttribute : AuthorizeAttribute
    {
        public string ControllerName { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            List<Model.ModelExtend.MenuModel> sesion = (List<Model.ModelExtend.MenuModel>)HttpContext.Current.Session["Menus"];
            if (sesion == null)
                return false;

            // kiểm tra xem có quyền vào menu không
            List<string> privilegeLevels = sesion.Where(x=> !string.IsNullOrEmpty(x.HREF_URL)).Select(x=>x.HREF_URL).ToList();
            if (privilegeLevels.Contains(ControllerName))
                return true;
            else
                return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            UserLogin sesion = (UserLogin)HttpContext.Current.Session["USER_SESSION"];
            if (sesion != null)
            {
                //filterContext.Result = new ViewResult
                //{
                //    ViewName = "~/Views/Shared/401.cshtml"
                //};
            }
            else
            {
                UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.Result = new RedirectResult("/Login/Index");
            }
        }
        
    }
}
