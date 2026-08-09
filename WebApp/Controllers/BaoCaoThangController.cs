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
    public class BaoCaoThangController : BaseController
    {
        readonly ICityDA _CityDA;
        readonly IDuAnDA _DuAnDA;
        readonly IBaoCaoTongHopDA _BaoCaoTongHopDA;
        readonly IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA;
        readonly ISysLogDA _sysLogDA;
        BaseController _helperController = new BaseController();
        readonly IExcelReportService _excelReportService;

        public BaoCaoThangController(
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

        // GET: BaoCaoThang
        [HasCredential(ControllerName = "BaoCaoThang")]
        public ActionResult Index()
        {
            try
            {
                // Kiểm tra quyền 
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
        public ActionResult SearchData(ReportSearchModel modelSearch)
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
                modelSearch.TypeReport = 1;
                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);
                AddLog("Lấy dữ liệu báo cáo tháng( tháng: " + modelSearch.Months + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") thành công.");
                return Json(new { data = data, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu báo cáo tháng(tháng: " + modelSearch.Months + ", năm: " + modelSearch.Year + ", tỉnh: " + modelSearch.CityCodes + ") lỗi: " + ex.Message);

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
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Citys = citys, DuAns = duAns, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "BaoCaoThang",
                        UserName = user != null ? user.UserName : "System",
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        [HttpPost]
        public ActionResult GetNhomTBHByCityCodes(string CityCodes)
        {
            // Lấy danh sách nhóm TBH theo tỉnh
            var nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
            return Json(new { NhomTBHs = nhomTBHs, Error = false, Title = "Lấy dữ liệu thành công." }); ;
        }

        [HttpGet]
        public ActionResult ExportData(int Year, string Months, string CityCodes, string maNhomTBHs, string maDuAn)
        {
            try
            {
                var modelSearch = new ReportSearchModel() { Year = Year, Months = Months, CityCodes = CityCodes, TypeReport = 1, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };
                var file_name = maDuAn;
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);
                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";

                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);

                // Lấy danh sách nhóm TBH theo tỉnh
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
                else {
                    file_name += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                }

                var lsM = Months.Split(',');
                string sMFilenam = lsM[0];
                if (lsM.Count() > 1)
                {
                    sMFilenam = lsM.First() + "-" + lsM.Last();
                }

                var titleReport = "Kỳ báo cáo: Báo cáo Tháng " + Months + " - " + Year;
                var sheetName = "Báo cáo tháng " + Months + " năm " + Year;

                string finalFileName;
                byte[] fileBytes = _excelReportService.ExportReport(
                    data, 
                    titleReport, 
                    sheetName, 
                    user, 
                    tenNhomTBHs, 
                    file_name + "_BC_THANG_" + sMFilenam + "-" + Year,
                    out finalFileName
                );

                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", finalFileName);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}