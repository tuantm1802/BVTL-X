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
        ISyncDataDA _syncDataDA = new SyncDataDA();
        IInsertDataDA _insertDataDA = new InsertDataDA();
        ISyncDataFromApi_SaveToDB syncDataFromApi_SaveToDB = new SyncDataFromApi_SaveToDB();
        IApiBase _apiBase = new ApiBase();

        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

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

    }

}