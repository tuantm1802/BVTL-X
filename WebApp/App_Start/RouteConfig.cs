using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApp
{
    public class RouteConfig
    {
        private static readonly string IsDev = ConfigurationManager.AppSettings["IsDev"].ToString();
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            if (IsDev == "1")
            {
                routes.MapRoute(
                    name: "Default",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
                );
            }
            else
            {
                routes.MapRoute(
                    name: "Default",
                    url: "{controller}/{action}/{id}",
                    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                    //,  namespaces: new[] { string.Format("{0}.Controllers", BuildManager.GetGlobalAsaxType().BaseType.Assembly.GetName().Name) }
                );
            }
        }
    }
}
