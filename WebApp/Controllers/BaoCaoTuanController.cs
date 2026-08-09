using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebApp.Service;

namespace WebApp.Controllers
{
    public class ReportSearchModelTuan : ReportSearchModel
    {
        public int? Week { get; set; }
    }

    public class BaoCaoTuanController : BaseController
    {
        readonly ICityDA _CityDA;
        readonly IDuAnDA _DuAnDA;
        readonly IBaoCaoTongHopDA _BaoCaoTongHopDA;
        readonly IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA;
        readonly ISysLogDA _sysLogDA;
        BaseController _helperController = new BaseController();
        readonly IExcelReportService _excelReportService;

        public BaoCaoTuanController(
            IExcelReportService excelReportService,
            ICityDA cityDA,
            IDuAnDA duAnDA,
            IBaoCaoTongHopDA baoCaoTongHopDA,
            IBVTL_NHOM_TBHDA bvtlNhomTbhDA,
            ISysLogDA sysLogDA)
        {
            _excelReportService = excelReportService;
            _CityDA = cityDA;
            _DuAnDA = duAnDA;
            _BaoCaoTongHopDA = baoCaoTongHopDA;
            _BVTL_NHOM_TBHDA = bvtlNhomTbhDA;
            _sysLogDA = sysLogDA;
        }

        // GET: BaoCaoTuan
        [HasCredential(ControllerName = "BaoCaoTuan")]
        public ActionResult Index()
        {
            try
            {
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);
                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                if (user.IsAdmin || (duAn != null && user.MaDuAn.Contains(duAn.maduan)))
                    return View();
                else
                    return Redirect("/ErrorPage/Error404");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
                return Redirect("/ErrorPage/Error404");
            }
        }

        [HttpPost]
        public ActionResult SearchData(ReportSearchModelTuan modelSearch)
        {
            ObjectMessage obj = new ObjectMessage
            {
                Error = false
            };
            try
            {
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);
                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";
                modelSearch.TypeReport = 1; // Map to Monthly Report data

                // Week to Month conversion mapping
                int weekNum = modelSearch.Week ?? 1;
                int monthNum = (weekNum - 1) / 4 + 1;
                if (monthNum > 12) monthNum = 12;
                modelSearch.Months = monthNum.ToString();

                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);
                AddLog("Lấy dữ liệu báo cáo tuần (tuần: " + modelSearch.Week + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") thành công.");
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                AddLog("Lấy dữ liệu báo cáo tuần (tuần: " + modelSearch.Week + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") lỗi: " + ex.Message);
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
                var user = Session["USER_SESSION"] as UserLogin;
                var citys = _CityDA.GetCityReport((int)user.UserID);
                var duAns = _DuAnDA.GetDuAnReport((int)user.UserID);
                AddLog("Lấy danh sách các bottom được thực hiện trên form Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Citys = citys, DuAns = duAns, Error = false, Title = "Lấy dữ liệu thành công." });
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                AddLog("Lấy danh sách các bottom được thực hiện trên form Người dùng lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        [HttpPost]
        public ActionResult GetNhomTBHByCityCodes(string CityCodes)
        {
            var nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
            return Json(new { NhomTBHs = nhomTBHs, Error = false, Title = "Lấy dữ liệu thành công." });
        }

        [HttpGet]
        public ActionResult ExportData(int Year, int Week, string CityCodes, string maNhomTBHs, string maDuAn)
        {
            try
            {
                int monthNum = (Week - 1) / 4 + 1;
                if (monthNum > 12) monthNum = 12;

                var modelSearch = new ReportSearchModel() 
                { 
                    Year = Year, 
                    Months = monthNum.ToString(), 
                    CityCodes = CityCodes, 
                    TypeReport = 1, 
                    MaNhomTBH = maNhomTBHs, 
                    MaDuAn = maDuAn 
                };
                var file_name = maDuAn;

                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);
                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";

                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);

                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
                else
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhoms(maNhomTBHs); 

                var tenNhomTBHs = "";
                if (nhomTBHs != null && nhomTBHs.Count > 0)
                {
                    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                }

                string sNhomFilename = "";
                if (nhomTBHs != null && nhomTBHs.Count == 1)
                {
                    sNhomFilename = string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                    file_name += "_" + sNhomFilename;
                }
                else
                {
                    file_name += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                }
                
                var titleReport = "Kỳ báo cáo: Báo cáo Tuần " + Week + " - " + Year;
                var sheetName = "Báo cáo tuần " + Week + " năm " + Year;
                
                string finalFileName;
                byte[] fileBytes = _excelReportService.ExportReport(
                    data, 
                    titleReport, 
                    sheetName, 
                    user, 
                    tenNhomTBHs, 
                    file_name + "_BC_TUAN_" + Week + "-" + Year,
                    out finalFileName
                );

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", finalFileName);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                new BVTL_QT_LOG
                {
                    ControllerName = "BaoCaoTuan",
                    UserName = user != null ? user.UserName : "System",
                    DateLog = DateTime.Now,
                    Content = content
                }
            );
        }
    }
}
