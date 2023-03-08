using Data.Admin;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Service;

namespace WebApp.Controllers
{
    public class HomeController : BaseController
    {
        ISysLogDA _sysLogDA = new SysLogDA();

        public ActionResult Index()
        {
            AddLog("Redireact vào home.");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult HomeOther()
        {
            return View();
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
    }
}