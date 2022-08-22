using Common;
using SyncBVTL.Push.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SyncBVTL.Push.Controllers
{
    public class BaseController : Controller
    {
        protected static string ConnectionStr = ConfigurationManager.AppSettings.Get("ConnectionString");

        protected ApiBase apiBase = new ApiBase();

    }
}