using Common;
using Model.Model;
using Model.ModelExtend;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Admin;
using log4net;
using Data.InterfaceDA;
using WebApp.Common;
using Model.ModelExtend.API;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.ComponentModel;
using Data.API;
using System.Threading.Tasks;
using Data.InterfaceDA.API;
using Model.ModelExtend.Base;
using Common.Common;
using Common.ICommon;
using Data.InterfaceDA.Admin;

namespace WebApp.Controllers
{
    public class SyncDataController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        readonly ISyncDataDA _syncDataDA;
        readonly IInsertDataDA _insertDataDA;
        ISyncDataFromApi_SaveToDB syncDataFromApi_SaveToDB = new SyncDataFromApi_SaveToDB();
        IApiBase _apiBase = new ApiBase();
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        readonly ISysLogDA _sysLogDA;
        BaseController _helperController = new BaseController();

        public SyncDataController(ISyncDataDA syncDataDA, IInsertDataDA insertDataDA, ISysLogDA sysLogDA)
        {
            _syncDataDA = syncDataDA;
            _insertDataDA = insertDataDA;
            _sysLogDA = sysLogDA;
        }

        // GET: SyncData
        [HasCredential(ControllerName = "SyncData")]
        public ActionResult Index()
        {
            return View();
        }
      
        [HttpPost]
        public ActionResult GetAllByPage(ModelSearch modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                int totalItems = 0;
                var data = _syncDataDA.GetAllByPage(modelSearch, ref totalItems);
                AddLog("Lấy dữ liệu theo trang bảng Danh sách tiến trình đồng bộ( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng Danh sách tiến trình đồng bộ( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public ActionResult GetBottomAction()
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var menu = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var bottoms = _helperController.GetBottomRoleByController(controllerName, menu);
                AddLog("Lấy danh sách các botom được thực hiện trên from Danh sách tiến trình đồng bộ thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from Danh sách tiến trình đồng bộ lỗi: " + ex.Message);
                return Json(obj);
            }
        }


        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "Role",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        [HttpPost]
        public async Task<object> SyncDataFromApi(int Id)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var session = (UserLogin)Session["USER_SESSION"];
                var tableNames = new List<string>();
                var infoApi = _insertDataDA.GetApiInfo(Id, ref tableNames);

                var apiResult = await syncDataFromApi_SaveToDB.GetDataFromApi_SaveToDB(infoApi.HrefApi, infoApi.TokenApi, infoApi.ReportId, infoApi.maduan, tableNames, infoApi.Api_Code, infoApi.RawOrLabel);
                obj.Error = !apiResult.Success;
                obj.Title = apiResult.Success ? "Đồng bộ thành công.": apiResult.Message;
                
                await SendTelegramMessage("589101034", "🎉 API "+ infoApi.ReportId + " đồng bộ thủ công thành công! [SyncDataFromApi][Manual Sync]");
                await SendTelegramMessage("-1002496745464", "🎉 API "+ infoApi.ReportId + " đồng bộ thủ công thành công! [SyncDataFromApi][Manual Sync]");

                if (obj.Error)
                    AddLog("Lấy dữ liệu từ api (" + infoApi.HrefApi + ") lỗi: " + obj.Title);
                else
                    AddLog("Lấy dữ liệu từ api (ID: " + infoApi.HrefApi + ") thành công.");
                return Json(obj);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu từ api (ID: " + Id + ") lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        public static async Task SendTelegramMessage(string chatId, string message)
        {
            string botToken = "7553997923:AAFBabzEfRLdluri42vy3VixZwrLfv2BHPs";
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage";

            using (var client = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
        {
            { "chat_id", chatId },
            { "text", message }
        };

                var content = new FormUrlEncodedContent(parameters);
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error sending Telegram message: {error}");
                }
            }
        }

        [HttpPost]
        public JsonResult GetEndTimeSync(string apiCode)
        {
            var endTimeSync = db.BVTL_API
                .Where(x => x.Api_Code == apiCode)
                .Select(x => x.End_Time_Sync)
                .FirstOrDefault();

            if (endTimeSync.HasValue)
            {
                return Json(new { success = true, endTimeSync = endTimeSync.Value.ToString("dd/MM/yyyy HH:mm") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin" }, JsonRequestBehavior.AllowGet);
            }
        }


    }

}