using Data;
using Data.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WebApp.Common
{
    public class ActionLogger
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void InsertRequestAudit(ActionExecutingContext filterContext)
        {
            try
            {
                HttpRequestBase request = filterContext.HttpContext.Request;
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var session = (Simple.Base.UserLogin)HttpContext.Current.Session["USER_SESSION"];

                HttpRequest Request = HttpContext.Current.Request;
                var json = new JavaScriptSerializer().Serialize(
                                         request.Form.Keys.Cast<string>()
                                             .ToDictionary(k => k, k => request.Form[k]));
                //Generate an audit
                AuditModel audit = new AuditModel()
                {
                    USER_NAME = session.UserName,
                    IP_ADDRESS = (request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim(),
                    LINK_PAGE = controllerName,
                    LINK_ACCESSED = request.RawUrl,
                    APP_CODE = session.Appcode,
                    TIME_STAMP = DateTime.UtcNow,
                    DATA = json
                };

                BVTL_QT_LOG log = new BVTL_QT_LOG()
                {
                    ControllerName = controllerName,
                    UserName = session.UserName,
                    DateLog = DateTime.UtcNow,
                    Content = new JavaScriptSerializer().Serialize(audit)
                };

                var logger = new SysLogDA();
                logger.Add(log);
            }
            catch (Exception ex)
            {
                log.Error("Lỗi khi lưu thông tin thao tác" + ex, ex);
            }
        }
    }
}