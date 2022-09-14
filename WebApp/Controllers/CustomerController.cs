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
    public class CustomerController : BaseController
    {
        ICustomerDA _CustomerDA = new CustomerDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: Customer
        [HasCredential(ControllerName = "Customer")]
        public ActionResult Index()
        {
            var modelSearch = new ModelSearch
            {
                KeyWord = string.Empty,
                currentPage = 1,
                pageSize = int.MaxValue,
                SortColumn = "hoten"
            };
            var data = _CustomerDA.GetAllByPage(modelSearch);
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
                modelSearch.pageSize = int.MaxValue;
                int totalItems = 0;
                var data = _CustomerDA.GetAllByPage(modelSearch);
                if (data != null && data.Count > 0)
                    totalItems = data.FirstOrDefault().TotalRow;

                //var returnData = new DatatablesModel();
                //returnData.data = (List<object>)data;

                AddLog("Lấy dữ liệu theo trang bảng khách hàng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu theo trang bảng khách hàng( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

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
                AddLog("Lấy danh sách các botom được thực hiện trên from khách hàng thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên from khách hàng lỗi: " + ex.Message);
                return Json(obj);
            }
        }

        private void AddLog(string content)
        {
            var user = Session["USER_SESSION"] as UserLogin;
            _sysLogDA.Add(
                    new BVTL_QT_LOG
                    {
                        ControllerName = "Customer",
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
                var data = _CustomerDA.GetItemByCode(code);
                AddLog("Lấy dữ liệu theo code bảng khách hàng( code: " + code + ") thành công.");
                return Json(new { Error = false, Title = "Lấy dữ liệu thành công.", data = data });
            }
            catch (Exception ex)
            {
                AddLog("Lấy dữ liệu theo code bảng khách hàng( code: " + code + ") lỗi: " + ex.Message);
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
                    KeyWord = keyword == "undefined"? string.Empty : keyword,
                    currentPage = 1,
                    pageSize = int.MaxValue,
                    SortColumn = "hoten"
                };
                var data = _CustomerDA.GetAllByPage(modelSearch);

                string file_name = "KhachHang_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString() + ".xlsx";

                DataTable dt = new DataTable();
                dt.Columns.AddRange(new DataColumn[10] { new DataColumn("STT"),
                        new DataColumn("Mã KH"),
                        new DataColumn("Họ tên KH"),
                        new DataColumn("Giới tính"),
                         new DataColumn("Năm sinh"),
                         new DataColumn("Đối tượng"),
                         new DataColumn("Ngày tiếp cận"),
                         new DataColumn("Số điện thoại"),
                         new DataColumn("Tỉnh"),
                         new DataColumn("Địa chỉ")
                });
                var stt = 1;
                foreach (var item in data)
                {
                    dt.Rows.Add(stt,
                        item.makh,
                        item.hoten,
                        item.GioiTinhText,
                        item.namsinh,
                        item.LoaiDoiTuong,
                        item.ngaytiepcantext,
                        item.sodienthoai,
                        item.CityName,
                        item.diachi
                        );
                    stt++;
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add("Khách hàng");
                    ws.Cell("A2").Value = "KHÁCH HÀNG";
                    ws.Range("A2:J2").Row(1).Merge();
                    ws.Cell("A2").Style.Font.Bold = true;
                    ws.Cell("A2").Style.Font.FontSize = 20;
                    ws.Column("A").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
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
                    ws.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("G").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("H").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("I").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Column("J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Column("J").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    ws.Row(3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Row(3).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    ws.Cell(3, 1).InsertTable(dt);
                    AddLog("Khách hàng");
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
                AddLog("Export Khách hàng lỗi: " + ex.Message);
                return Json(new { message = "Lỗi xử lý dữ liệu" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion 
    }
}