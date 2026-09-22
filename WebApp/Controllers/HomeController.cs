using Data.Admin;
using Data.InterfaceDA.Admin;
using DocumentFormat.OpenXml.Drawing.Charts;
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
        readonly ISysLogDA _sysLogDA;
        readonly IBaoCaoTongHopDA _BaoCaoTongHopDA;
        readonly IDashboardCD45DA _dashboardCD45DA;
        readonly ICityDA _cityDA;
        readonly IBVTL_NHOM_TBHDA _nhomDA;

        public HomeController(
            ISysLogDA sysLogDA, 
            IBaoCaoTongHopDA BaoCaoTongHopDA,
            IDashboardCD45DA dashboardCD45DA,
            ICityDA cityDA,
            IBVTL_NHOM_TBHDA nhomDA)
        {
            _sysLogDA = sysLogDA;
            _BaoCaoTongHopDA = BaoCaoTongHopDA;
            _dashboardCD45DA = dashboardCD45DA;
            _cityDA = cityDA;
            _nhomDA = nhomDA;
        }

        public ActionResult Index()
        {
            AddLog("Truy cập trang chủ Dashboard CD45.");
            return View();
        }

        [HttpPost]
        public JsonResult GetFilterData(string cityMode = "NEW34")
        {
            try
            {
                object cities;
                if (string.Equals(cityMode, "OLD63", StringComparison.OrdinalIgnoreCase))
                {
                    var allOld = _cityDA.GetAll();
                    cities = allOld.OrderByDescending(x => !string.IsNullOrEmpty(x.Code_Map))
                                   .ThenBy(x => x.Name)
                                   .Select(x => new { 
                                       CityCode = x.Code, 
                                       CityName = (!string.IsNullOrEmpty(x.Code_Map) ? "⭐ " : "") + x.Name + (!string.IsNullOrEmpty(x.Code_Map) ? " (" + x.Code_Map + ")" : ""),
                                       IsKey = !string.IsNullOrEmpty(x.Code_Map),
                                       OldCodes = new[] { x.Code }
                                   })
                                   .ToList();
                }
                else
                {
                    var newCities = _cityDA.GetAllNewCities();
                    var mappings = _cityDA.GetCityMappings();
                    var mapGroup = mappings.GroupBy(m => m.NewCityCode).ToDictionary(g => g.Key, g => g.Select(m => m.OldCityCode).ToArray());

                    cities = newCities.OrderByDescending(x => x.IsKeyProvince)
                                      .ThenBy(x => x.DisplayOrder)
                                      .ThenBy(x => x.Name)
                                      .Select(x => new { 
                                          CityCode = x.Code, 
                                          CityName = (x.IsKeyProvince ? "⭐ " : "") + x.Name + (x.OldCount > 1 ? string.Format(" ({0} tỉnh gộp)", x.OldCount) : ""),
                                          IsKey = x.IsKeyProvince,
                                          OldCodes = mapGroup.ContainsKey(x.Code) ? mapGroup[x.Code] : new[] { x.Code }
                                      })
                                      .ToList();
                }

                var nhoms = _nhomDA.GetAll()
                                   .Where(x => x.maduan == "CD45")
                                   .OrderBy(x => x.city_code)
                                   .ThenBy(x => x.tennhom_tbh)
                                   .Select(x => new { 
                                       MaNhom = x.manhom_tbh, 
                                       MaNhomMap = x.manhom_tbh_map, 
                                       TenNhom = x.tennhom_tbh, 
                                       CityCode = x.city_code 
                                   })
                                   .ToList();

                return Json(new { Success = true, Cities = cities, Nhoms = nhoms });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetDashboardCD45Data(string cityCode, string maNhom, string fromDate, string toDate, string nhomTuoiTable1 = null, string cityMode = "NEW34")
        {
            try
            {
                var data = _dashboardCD45DA.GetDashboardData(cityCode, maNhom, fromDate, toDate, nhomTuoiTable1, cityMode);
                var jsonResult = Json(new { Success = true, Data = data, Error = false, Title = "Lấy dữ liệu thành công." });
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message, Error = true });
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpPost]
        public JsonResult GetTanSuatChemsex3ThangTheoDoTuoi(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetTanSuatChemsex3ThangTheoDoTuoi(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
        
        [HttpPost]
        public JsonResult GetTanSuatChemsex3ThangTheoDiemAssist(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetTanSuatChemsex3ThangTheoDiemAssist(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
        
        [HttpPost]
        public JsonResult GetTanSuatChemsex3ThangTheoDiemACE(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetTanSuatChemsex3ThangTheoDiemACE(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
        
        [HttpPost]
        public JsonResult GetSuDungDaChatTrongChemsexTheoDoTuoi(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetSuDungDaChatTrongChemsexTheoDoTuoi(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
         
        [HttpPost]
        public JsonResult GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetSuDungDaChatTrongChemsexTheoDoiTuongQHTD(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
         
        [HttpPost]
        public JsonResult GetSuDungDaChatTrongChemsexTheoQHTDTT(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetSuDungDaChatTrongChemsexTheoQHTDTT(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
         
        [HttpPost]
        public JsonResult GetSuDungDaChatTrongChemsexTheoBanDam(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetSuDungDaChatTrongChemsexTheoBanDam(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
        
        [HttpPost]
        public JsonResult GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetSuDungDaChatTrongChemsexTheoDiemAssistMaTuyDa(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
        
        [HttpPost]
        public JsonResult GetSuDungDaChatTrongChemsexTheoDiemACE(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetSuDungDaChatTrongChemsexTheoDiemACE(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
        
        [HttpPost]
        public JsonResult GetSuDungDaChatTrongChemsexTheoDiemQST(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetSuDungDaChatTrongChemsexTheoDiemQST(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
        }
        
        [HttpPost]
        public JsonResult GetTanSuatChemsexTrong3ThangTheoDiemQST(string maNhom, string maTinh)
        {
            var lsDashboard = _BaoCaoTongHopDA.GetTanSuatChemsexTrong3ThangTheoDiemQST(maNhom, maTinh);
            return Json(new { data = lsDashboard, Error = false, Title = "Lấy dữ liệu thành công." });
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

        [HttpGet]
        public JsonResult GetSystemVersion()
        {
            var info = global::Common.Common.AppVersionHelper.GetVersionInfo();
            return Json(new { Success = true, Data = info }, JsonRequestBehavior.AllowGet);
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