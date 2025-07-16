using ClosedXML.Excel;
using Common;
using Common.Common;
using Data.Admin;
using Data.InterfaceDA.Admin;
using DocumentFormat.OpenXml.Office2010.Excel;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class KhoVatPhamController : BaseController
    {
        IKhoVatPhamDA _KhoVatPhamDA = new KhoVatPhamDA();
        ISysLogDA _sysLogDA = new SysLogDA();
        IDuAnDA _DuAnDA = new DuAnDA();
        BaseController _helperController = new BaseController();
        private BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA = new BVTL_NHOM_TBHDA();

        // GET: Kho Vat Pham        
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BaoCaoTonKho()
        {
            return View();
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetAllChiTietPhieuXuatNhaps([FromBody] DataTableRequest request)
        {
            var khovatpham = _KhoVatPhamDA.GetAllChiTietPhieuXuatNhaps(request);
            return Json(khovatpham, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult CapNhatPhieuXuatNhap(ChiTietPhieuXuatNhapModel model)
        {
            try
            {
                // Gọi lớp dữ liệu (KhoVatPhamDA) để thực hiện cập nhật
                var result = _KhoVatPhamDA.CapNhatPhieuXuatNhap(model);
                if (result)
                {
                    return Json(new { success = true, message = "Cập nhật phiếu thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Cập nhật thất bại. Vui lòng kiểm tra dữ liệu!" });
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return Json(new { success = false, message = "Đã xảy ra lỗi: " + ex.Message });
            }
        }

        // API endpoint để lấy dữ liệu tồn kho theo ngày
        //[System.Web.Mvc.HttpGet]
        //public JsonResult GetTonKhoData(string fromDate, string toDate)
        //{
        //    var result = _KhoVatPhamDA.GetTonKhoData(DateTime.Parse(fromDate), DateTime.Parse(toDate));
        //    return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        //}

        [System.Web.Mvc.HttpGet]
        public JsonResult GetTonKhoData(string fromDate, string toDate, string maNhomTBH)
        {
            var result = _KhoVatPhamDA.GetTonKhoData(DateTime.Parse(fromDate), DateTime.Parse(toDate), maNhomTBH);
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [System.Web.Mvc.HttpPost]
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

                AddLog("Lấy danh sách các botom được thực hiện trên form Kho Vat Pham thành công.");
                return Json(new { Buttoms = bottoms, Error = false, DuAns = duAns, Title = "Lấy dữ liệu thành công." }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy danh sách các botom được thực hiện trên form Kho Vat Pham lỗi: " + ex.Message);
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

        #region Xuất dữ liệu ra excel
        [System.Web.Mvc.HttpGet]
        public ActionResult ExportData(string fromDate, string toDate, string maNhomTBHs)
        {
            try
            {
                var menus = Session["Menus"] as List<MenuModel>;
                var controllerName = Request.RequestContext.RouteData.GetRequiredString("controller");

                var menu = menus.FirstOrDefault(x => x.CONTROLLER_NAME == controllerName);

                var user = Session["USER_SESSION"] as UserLogin;
                var duAn = _DuAnDA.GetAll().FirstOrDefault(x => x.tenduan == menu.TEN_DU_AN);
             
                var data = _KhoVatPhamDA.GetTonKhoData(DateTime.Parse(fromDate), DateTime.Parse(toDate));                

                var tenDuAn = "";
                string maDuAn = "CD43";
                string sThoiGian = fromDate + " - " + toDate;
                if (!string.IsNullOrEmpty(maDuAn))
                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);

                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes("");
                else
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhomMap(maNhomTBHs);

                var tenNhomTBHs = "";
                if (nhomTBHs != null && nhomTBHs.Count > 0)
                {
                    tenNhomTBHs = string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                }

                var file_name = maDuAn + "_" + "_BAO_CAO_TON_KHO_" + fromDate + "_" + toDate + ".xlsx";

                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo tồn kho");
                    
                    CreateHeader(ws, maDuAn, user, "", sThoiGian, tenNhomTBHs);

                    var columnName = "";
                    var columnNumber = 0;
                    var row = 7;
                    int stt = 1;
                    if (data.Any())
                    {

                        foreach (var rowReport in data)
                        {
                            // Thêm dữ liệu cột STT
                            InsertDataCell(ws, "A", row, stt.ToString(), true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                            stt++;

                            if (!string.IsNullOrEmpty(rowReport.TenSanPham))
                            {
                                InsertDataCell(ws, "B", row, rowReport.TenSanPham, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                            }
                            
                            InsertDataCell(ws, "C", row, rowReport.DVT, false, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                            InsertDataCell(ws, "D", row, rowReport.TonDauKy > 0 ? rowReport.TonDauKy.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.TonDauKy > 0 ? true : false);

                            InsertDataCell(ws, "E", row, rowReport.NhapKho > 0 ? rowReport.NhapKho.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.NhapKho > 0 ? true : false);

                            InsertDataCell(ws, "F", row, rowReport.XuatKho > 0 ? rowReport.XuatKho.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.XuatKho > 0 ? true : false);

                            InsertDataCell(ws, "G", row, rowReport.TonCuoiKy > 0 ? rowReport.TonCuoiKy.ToString() : "", false, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.TonCuoiKy > 0 ? true : false);

                            row++;
                        }
                    }

                    CreateFooter(ws, "", user, row);

                    ws.Range("A6:G" + row).Style.Font.FontName = "Times New Roman";
                    ws.Range("A6:G" + row).Style.Font.FontSize = 13;
                    ws.Range("A6:G" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A6:G" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A6:G" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A6:G" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    
                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 10).AdjustToContents();
                        ws.Column("B").Width = 50;
                        ws.Column("C").Width = 20;
                        ws.Column("D").Width = 20;
                        ws.Column("E").Width = 20;
                        ws.Column("F").Width = 20;
                        ws.Column("G").Width = 20;
                        ws.Column("A").Style.Alignment.SetWrapText(true);
                        ws.Column("B").Style.Alignment.SetWrapText(true);
                        ws.Column("C").Style.Alignment.SetWrapText(true);
                        ws.Column("D").Style.Alignment.SetWrapText(true);
                        ws.Column("E").Style.Alignment.SetWrapText(true);
                        ws.Column("F").Style.Alignment.SetWrapText(true);
                        ws.Column("G").Style.Alignment.SetWrapText(true);
                        
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file_name);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";

            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }

            return columnName;
        }

        public static int ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }

        /// Gán dữ liệu cho cell
        /// </summary>
        /// <param name="ws"></param>
        private void InsertDataCell(IXLWorksheet ws, string cellName, int row, string value, bool bold,
            XLAlignmentHorizontalValues horizontal, XLAlignmentVerticalValues vertical, bool isNumber = true)
        {
            if (isNumber == true && string.IsNullOrEmpty(value))
                value = "0";

            ws.Cell(cellName + row).Value = isNumber == false ? (object)value : Convert.ToDecimal(value);
            ws.Cell(cellName + row).Style.Font.Bold = bold;
            ws.Cell(cellName + row).Style.Alignment.Horizontal = horizontal;
            ws.Cell(cellName + row).Style.Alignment.Vertical = vertical;
            ws.Cell(cellName + row).Style.Alignment.WrapText = true;
            if (isNumber)
                ws.Cell(cellName + row).Style.NumberFormat.Format = "#,##0";//#,##0.00
        }
        /// Gán dữ liệu cho cell có gộp cell
        /// </summary>
        /// <param name="ws"></param>
        private void InsertDataCell_Merge(IXLWorksheet ws, string cellName, int row, string value, bool bold,
            XLAlignmentHorizontalValues horizontal, XLAlignmentVerticalValues vertical, string cellStartMerge, string cellEndMerge)
        {
            ws.Cell(cellName + row).Value = value;
            ws.Cell(cellName + row).Style.Font.Bold = bold;
            ws.Cell(cellName + row).Style.Alignment.Horizontal = horizontal;
            ws.Cell(cellName + row).Style.Alignment.Vertical = vertical;
            ws.Cell(cellName + row).Style.Alignment.WrapText = true;
            //ws.Cell(cellName + row).Style.Alignment.h = true;
            ws.Range(cellStartMerge + ":" + cellEndMerge).Row(1).Merge();
        }

        /// Gán dữ liệu cho cell có gộp cell
        /// </summary>
        /// <param name="ws"></param>
        private void CreateHeader(IXLWorksheet ws, string tileReport, UserLogin user, string tenToChucCaNhan, string sThoiGian, string tenNhomTBHs)
        {
            #region header
            // 
            ws.Cell("A1").Value = "BÁO CÁO XUẤT NHẬP TỒN VẬT PHẨM";
            ws.Range("A1:G1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 16;
            ws.Row(1).Height = 40;

            // 
            ws.Cell("B2").Value = "Mã dự án:";
            ws.Cell("C2").Value = tileReport;
            //ws.Range("A2:J2").Row(1).Merge();
            ws.Cell("B2").Style.Font.Bold = true;
            ws.Cell("B2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("B2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B2").Style.Font.FontName = "Times New Roman";
            ws.Cell("B2").Style.Font.FontSize = 13;

            // 
            ws.Cell("B3").Value = "Tên tổ chức/Cá nhân:";
            ws.Cell("C3").Value = tenToChucCaNhan;
            //ws.Range("A3:J3").Row(1).Merge();
            ws.Cell("B3").Style.Font.Bold = true;
            ws.Cell("B3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("B3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B3").Style.Font.FontName = "Times New Roman";
            ws.Cell("B3").Style.Font.FontSize = 13;

            ws.Cell("B4").Value = "Thời gian:";
            ws.Cell("C4").Value = sThoiGian;
            //ws.Range("A3:J3").Row(1).Merge();
            ws.Cell("B4").Style.Font.Bold = true;
            ws.Cell("B4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("B4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B4").Style.Font.FontName = "Times New Roman";
            ws.Cell("B4").Style.Font.FontSize = 13;

            ws.Cell("D4").Value = "Nhóm:";
            ws.Cell("E4").Value = tenNhomTBHs;
            //ws.Range("A3:J3").Row(1).Merge();
            ws.Cell("D4").Style.Font.Bold = true;
            ws.Cell("D4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("D4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("D4").Style.Font.FontName = "Times New Roman";
            ws.Cell("D4").Style.Font.FontSize = 13;

            var row = 6;

            //header table
            ws.Row(row).Height = 40;
            ws.Cell("A" + row).Value = "STT";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Tên vật phẩm";
            //ws.Range("B" + row + ":C" + row).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("C" + row).Value = "ĐVT";
            ws.Cell("C" + row).Style.Font.Bold = true;
            ws.Cell("C" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Tồn kho đầu kỳ";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.WrapText = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "Nhập kho";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.WrapText = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "Xuất kho";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "Tồn kho cuối kỳ";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.WrapText = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            
            #endregion
        }

        private void CreateFooter(IXLWorksheet ws, string tileReport, UserLogin user, int row)
        {
            row = row + 2;
            ws.Cell("B"+row).Value = "Trưởng nhóm";
            ws.Cell("B"+row).Style.Font.Bold = true;
            ws.Cell("B"+row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B"+row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B"+row).Style.Font.FontName = "Times New Roman";
            ws.Cell("B"+row).Style.Font.FontSize = 13;

            ws.Cell("B" + (row + 1)).Value = "(Ký, họ tên)";
            ws.Cell("B" + (row + 1)).Style.Font.Italic = true;
            ws.Cell("B" + (row + 1)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell("F" + row).Value = "Người lập";
            //ws.Range("F65:I65").Row(1).Merge();
            ws.Cell("F"+row).Style.Font.Bold = true;
            ws.Cell("F"+row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F"+row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("F"+row).Style.Font.FontName = "Times New Roman";
            ws.Cell("F"+row).Style.Font.FontSize = 13;

            ws.Cell("F" + (row+1)).Value = "(Ký, họ tên)";
            ws.Cell("F" + (row+1)).Style.Font.Italic = true;
            ws.Cell("F" + (row+1)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        }

        #endregion 
    }
}