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
        public JsonResult GetFilterData()
        {
            try
            {
                var cities = _cityDA.GetAll()
                                    .Where(x => new[] { "HNO", "HPG", "HYE", "NAN", "NBI", "HCM" }.Contains(x.Code))
                                    .Select(x => new { CityCode = x.Code, CityName = x.Name })
                                    .ToList();

                var nhoms = _nhomDA.GetAll()
                                   .Where(x => x.maduan == "CD45")
                                   .Select(x => new { MaNhom = x.manhom_tbh, TenNhom = x.tennhom_tbh, CityCode = x.city_code })
                                   .ToList();

                return Json(new { Success = true, Cities = cities, Nhoms = nhoms });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetDashboardCD45Data(string cityCode, string maNhom, string fromDate, string toDate)
        {
            try
            {
                var data = _dashboardCD45DA.GetDashboardData(cityCode, maNhom, fromDate, toDate);
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