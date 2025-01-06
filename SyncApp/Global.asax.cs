using log4net.Config;
using Quartz;
using Quartz.Impl;
using SyncBVTL.Push.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SyncBVTL.Push
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();
            string logDirectory = @"C:\Logs";
            string logFilePath = Path.Combine(logDirectory, "AppStartLog.txt");

            try
            {
                // Kiểm tra và tạo thư mục nếu nó không tồn tại
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Ghi log vào file
                File.AppendAllText(logFilePath, $"Application_Start called at {DateTime.Now}\n");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và ghi ra console hoặc log thêm vào file khác
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }

            JobScheduler.StartAll();
            //XmlConfigurator.ConfigureAndWatch(new FileInfo(Server.MapPath("~/") + "App_Start/logging.config"));
        }
    }
}
