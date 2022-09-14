using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KetQuaHIVController : BaseController
    {
        IKetQuaHIVDA _KetQuaHIVDA = new KetQuaHIVDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: KetQuaHIV
        [HasCredential(ControllerName = "KetQuaHIV")]
        public ActionResult Index()
        {
            var modelSearch = new ModelSearch
            {
                KeyWord = string.Empty,
                currentPage = 1,
                pageSize = int.MaxValue,
                SortColumn = "kqxnhiv_id"
            };
            var data = _KetQuaHIVDA.GetAllByPage(modelSearch);
            return View(data);
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
                int totalItems = 0;
                var data = _KetQuaHIVDA.GetAllByPage(modelSearch);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu theo trang bảng kết quả HIV( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công."}); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng kết quả HIV( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

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
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả HIV thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from kết quả HIV lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "KetQuaHIV",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }


        [HttpPost]
        public object GetItemByID(int Id)
        {
            try
            {
                var data = _KetQuaHIVDA.GetItemById(Id);
                AddLog("Lấy dữ liệu theo ID bảng kết quả HIV( ID: " + Id + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo ID bảng kết quả HIV( ID: " + Id + ") lỗi: " + ex.Message);
                return Json(new { Error = true, Title = ex.Message });
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
                    SortColumn = "kqxnhiv_id"
                };
                var data = _KetQuaHIVDA.GetAllByPage(modelSearch);

                string file_name = "KetQuaHIV_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".xlsx";

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[8] { new DataColumn("Tỉnh"),
                        new DataColumn("Mã nhóm TBH"),
                        new DataColumn("Tên nhóm TBH"),
                        new DataColumn("Mã KH"),
                         new DataColumn("Họ tên KH"),
                         new DataColumn("Ngày xét nghiệm"),
                         new DataColumn("Kết quả"),
                         new DataColumn("Đang điều trị HIV")

                });
                foreach (var item in data)
                {
                    dt.Rows.Add(
                        item.CityName,
                        item.manhom_tbh,
                        item.tennhom_tbh,
                        item.makh,
                        item.hoten,
                        item.ngayxntext,
                        item.ketqua,
                        item.dangdieutri_hiv
                        );
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Kết quả HIV");
                    ws.Cell("A2").Value = "KẾT QUẢ HIV";
                    ws.Range("A2:H2").Row(1).Merge();
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
                    ws.Column("F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("F").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Column("H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell(3, 1).InsertTable(dt);
                    AddLog("KẾT QUẢ HIV");
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
                AddLog("Export KẾT QUẢ HIV lỗi: " + ex.Message);
                return Json(new { message = "Lỗi xử lý dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion 
    }
}