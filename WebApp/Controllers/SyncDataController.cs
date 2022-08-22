using Common;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Data.Admin;
using log4net;
using WebApp.Common;
using Model.ModelExtend.API;
using System.Net.Http;
using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.ComponentModel;
using Data.API;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class SyncDataController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        SyncDataDA _syncDataDA = new SyncDataDA();
        InsertDataDA _insertDataDA = new InsertDataDA();
        GetDataFromAPI _getDataFromAPI = new GetDataFromAPI();
        SyncDataFromApi_SaveToDB syncDataFromApi_SaveToDB = new SyncDataFromApi_SaveToDB();

        SysLogDA _sysLogDA = new SysLogDA();
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

                var apiResult = await syncDataFromApi_SaveToDB.GetDataFromApi_SaveToDB(infoApi.HrefApi, infoApi.TokenApi, infoApi.ReportId, tableNames);
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

        /// <summary>
        /// Chuyển đổi list model to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        /// <summary>
        /// Lấy dữ liệu từ api
        /// </summary>
        /// <param name="input"></param>
        /// <param name="url"></param>
        /// <param name="apitype"></param>
        /// <returns></returns>
        public static ApiResult GetDataFromApi(string input, string url, string apitype)
        {
            try
            {
                var resultSync = new ApiResult();
                log.Info("****************************BEGIN GET DATA FROM API "+ url + " ********************************");
                log.Info("JSON: " + input);

                if (apitype == "POST")
                {
                    HttpResponseMessage response = (ApiBase.PostJsonAsync( url, input)).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        log.Info("****************************END GET DATA FROM API " + url + " ********************************");
                        return new ApiResult
                        {
                            code = "401",
                            message = "Bạn không có quyền sử dụng API này"
                        };
                    }
                    else
                    {
                        string responseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                        log.Info(responseString);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            resultSync = JsonConvert.DeserializeObject<ApiResult>(responseString);
                        }
                        else
                        {
                            
                            resultSync.code = response.StatusCode.ToString();
                            resultSync.message = responseString;
                        }
                        log.Info("****************************END GET DATA FROM API " + url + " *******************************");
                        return resultSync;
                    }
                }
                else if (apitype == "PUSH")
                {
                    HttpResponseMessage response = (ApiBase.PutJsonAsyncResponse( url, input)).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        log.Info("****************************END GET DATA FROM API " + url + " ********************************");
                        return new ApiResult
                        {
                            code = "401",
                            message = "Bạn không có quyền sử dụng API này"
                        };
                    }
                    else
                    {
                        string responseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                        log.Info(responseString);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            resultSync = JsonConvert.DeserializeObject<ApiResult>(responseString);
                        }
                        else
                        {
                           
                            resultSync.code = response.StatusCode.ToString();
                            resultSync.message = responseString;
                        }
                        log.Info("****************************END GET DATA FROM API " + url + " *******************************");
                        return resultSync;
                    }
                }
                else if (apitype == "GET")
                {
                    HttpResponseMessage response = (ApiBase.GetJsonAsyncResponse(url + input)).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        log.Info("****************************END GET DATA FROM API " + url + " ********************************");
                        return new ApiResult
                        {
                            code = "401",
                            message = "Bạn không có quyền sử dụng API này"
                        };
                    }
                    else
                    {
                        string responseString = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result).ToString();
                        log.Info(responseString);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            resultSync = JsonConvert.DeserializeObject<ApiResult>(responseString);
                        }
                        else
                        {

                            resultSync.code = response.StatusCode.ToString();
                            resultSync.message = responseString;
                        }
                        log.Info("****************************END GET DATA FROM API " + url + " *******************************");
                        return resultSync;
                    }
                }
                return resultSync;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return new ApiResult { code = "404", message = "Lỗi xử lý dữ liệu." };
            }
        }
    }

}