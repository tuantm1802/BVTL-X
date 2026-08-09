using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend.Base;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.Data;
using System.IO;

namespace WebApp.Controllers
{
    public class TTKHMaDaVIIVController : Controller
    {
        // GET: TTKHMaDaVIIV
        readonly ITTKHMaDaVIIVDA _TTKHMaDaVIIVDA;
        readonly ISysLogDA _sysLogDA;
        readonly IDuAnDA _DuAnDA;
        BaseController _helperController = new BaseController();
        readonly ICityDA _CityDA;

        public TTKHMaDaVIIVController(ITTKHMaDaVIIVDA TTKHMaDaVIIVDA, ISysLogDA sysLogDA, IDuAnDA DuAnDA, ICityDA CityDA)
        {
            _TTKHMaDaVIIVDA = TTKHMaDaVIIVDA;
            _sysLogDA = sysLogDA;
            _DuAnDA = DuAnDA;
            _CityDA = CityDA;
        }

        public ActionResult Index()
        {
            try
            {
                // Kiểm tra quyền 
                var modelSearch = new ModelSearch
                {
                    KeyWord = string.Empty,
                    currentPage = 1,
                    pageSize = int.MaxValue,
                    SortColumn = "record_id"
                };
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);
                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);

                if (user.IsAdmin || (duAn != null && user.MaDuAn.Contains(duAn.maduan)))
                {
                    modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";

                    var citys = _CityDA.GetCityReport((int)user.UserID);
                    if (citys != null && citys.Count > 0)
                        modelSearch.CityCodes = string.Join(",", citys.Select(x => x.Code));
                    var data = _TTKHMaDaVIIVDA.GetAllByPage(modelSearch);
                    return View(data);
                }
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
        public object GetItemByID(int Id)
        {
            try
            {
                var data = _TTKHMaDaVIIVDA.GetItemById(Id);
                AddLog("Lấy dữ liệu theo ID bảng kết quả TTKHMaDaVIIV ( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng kết quả TTKHMaDaVIIV( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
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

                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả TTKHMaDaVIIV thành công.");
                return Json(new { Buttoms = bottoms, Error = false, DuAns = duAns, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả TTKHMaDaVIIV lỗi: " + ex.Message);
                return Json(obj);
            }
        }


        #region Xuất dữ liệu ra excel
        [HttpGet]
        public ActionResult ExportData(string keyword)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;
                var modelSearch = new ModelSearch
                {
                    KeyWord = keyword == "undefined" ? string.Empty : keyword,
                    currentPage = 1,
                    pageSize = int.MaxValue,
                    SortColumn = "record_id"
                };
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");
                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);
               
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
                modelSearch.MaDuAn = duAn != null ? duAn.maduan : "BVTL";

                var citys = _CityDA.GetCityReport((int)user.UserID);
                if (citys != null && citys.Count > 0)
                    modelSearch.CityCodes = string.Join(",", citys.Select(x => x.Code));
                var data = _TTKHMaDaVIIVDA.GetAllByPage(modelSearch);

                string file_name = "TTKHMaDaVIIV_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".xlsx";

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[7] {
                        new DataColumn("Dự án"),
                        new DataColumn("Tỉnh"),
                        new DataColumn("Mã nhóm TBH"),
                        new DataColumn("Tên nhóm TBH"),
                        new DataColumn("Mã KH"),
                        new DataColumn("Họ tên KH"),
                        new DataColumn("Ngày mất dấu"),

                });
                foreach (var item in data)
                {
                    dt.Rows.Add(
                        item.maduan,
                        item.CityName,
                        item.manhom_tbh,
                        item.tennhom_tbh,
                        item.makh,
                        item.hoten,
                        item.ngay_md
                        );
                }

                //Tên nhóm:,Mã KH,Ngày xét nghiệm:,Kết quả xét nghiệm ma túy đá,Kết quả xét nghiệm heroin

                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Viiv - Thông tin Khách hàng mất dấu");
                    ws.Cell("A2").Value = "Viiv - THÔNG TIN KHÁCH HÀNG MẤT DẤU";
                    ws.Range("A2:L2").Row(1).Merge();
                    ws.Cell("A2").Style.Font.Bold = true;
                    ws.Cell("A2").Style.Font.FontSize = 20;
                    ws.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("A").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("B").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("B").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("C").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("D").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("E").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell(3, 1).InsertTable(dt);
                    AddLog("Viiv - THÔNG TIN KHÁCH HÀNG MẤT DẤU");
                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 10).AdjustToContents();
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                    }
                }
            }
            catch (Exception ex)
            {
                AddLog("Export Viiv - THÔNG TIN KHÁCH HÀNG MẤT DẤU lỗi: " + ex.Message);
                return Json(new { message = "Lỗi xử lý dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "TTKHMaDaVIIV",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }
    }
}