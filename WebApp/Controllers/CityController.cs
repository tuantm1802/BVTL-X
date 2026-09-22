using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class CityController : BaseController
    {
        readonly ICityDA _CityDA;
        readonly ISysLogDA _sysLogDA;
        readonly IDuAnDA _DuAnDA;
        BaseController _helperController = new BaseController();

        public CityController(ICityDA CityDA, ISysLogDA sysLogDA, IDuAnDA DuAnDA)
        {
            _CityDA = CityDA;
            _sysLogDA = sysLogDA;
            _DuAnDA = DuAnDA;
        }

        // GET: City
        [HasCredential(ControllerName = "City")]
        public ActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult GetAll(ModelSearch modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                if (modelSearch == null)
                {
                    modelSearch = new ModelSearch();
                }
                if (string.IsNullOrEmpty(modelSearch.SortColumn))
                {
                    modelSearch.SortColumn = "KeyFirst";
                }
                if (modelSearch.currentPage <= 0)
                {
                    modelSearch.currentPage = 1;
                }
                if (modelSearch.pageSize <= 0)
                {
                    modelSearch.pageSize = 20;
                }

                int totalItems = 0;
                var data = _CityDA.GetAllByPage(modelSearch);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng tỉnh( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, cityMode = modelSearch.CityMode ?? "NEW34", Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng tỉnh( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

                return Json(obj);
            }
        }

        [HttpPost]
        public ActionResult GetCitySummary()
        {
            try
            {
                var newCities = _CityDA.GetAllNewCities();
                var oldCities = _CityDA.GetAll();
                int totalNew = newCities.Count;
                int totalOld = oldCities.Count;
                int keyNew = newCities.Count(x => x.IsKeyProvince);
                int keyOld = oldCities.Count(x => !string.IsNullOrEmpty(x.Code_Map));
                return Json(new { Error = false, TotalNew = totalNew, TotalOld = totalOld, KeyNew = keyNew, KeyOld = keyOld });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Message = ex.Message });
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
                // Lấy danh sách du an
                var duAns = _DuAnDA.GetAll().Select(x => new { Code = x.maduan, Name = x.tenduan }).ToList();

                AddLog("Lấy danh sách các botom được thực hiện trên from tỉnh thành công.");
                return Json(new { Buttoms = bottoms, Error = false, DuAns = duAns, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from tỉnh lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "City",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }


        [HttpPost]
        public object GetItemByCode(string code)
        {
            try
            {
                var data = _CityDA.GetItemByCode(code);
                AddLog("Lấy dữ liệu theo code bảng tỉnh( code: " + code + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo code bảng tỉnh( code: " + code + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateKeyProvince(string code, string codeMap, bool isKey)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                if (user == null || !user.IsAdmin)
                {
                    return Json(new { Error = true, Title = "Bạn không có quyền quản trị để thực hiện thao tác này." });
                }

                var result = _CityDA.UpdateKeyProvince(code, codeMap, isKey);
                AddLog(string.Format("Cập nhật tỉnh trọng điểm {0}: isKey={1}, codeMap={2}", code, isKey, codeMap));
                return Json(result);
            }
            catch (Exception ex)
            {
                AddLog(string.Format("Lỗi cập nhật tỉnh trọng điểm {0}: {1}", code, ex.Message));
                return Json(new { Error = true, Title = ex.Message });
            }
        }
    }
}