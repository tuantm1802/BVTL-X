using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApp.App_Start.AutofacConfig.RegisterDependencies();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();

            string addMin = ".min";
            if (System.Diagnostics.Debugger.IsAttached) { addMin = ""; }  // don't use minified files when executing locally
            Application["JSVer"] = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace('.', '0') + ".js";
            Application["CSSVer"] = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace('.', '0') + ".css";

            // Khởi động Quartz.NET Background Scheduler
            try
            {
                System.Threading.Tasks.Task.Run(async () =>
                {
                    await WebApp.Services.ScheduleTasks.JobScheduler.StartAll();
                });
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger(typeof(MvcApplication)).Error("Lỗi khởi động JobScheduler: " + ex.Message, ex);
            }
        }

        protected void Application_End()
        {
            try
            {
                WebApp.Services.ScheduleTasks.JobScheduler.Shutdown().Wait(3000);
            }
            catch { }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
        }
    }
}
