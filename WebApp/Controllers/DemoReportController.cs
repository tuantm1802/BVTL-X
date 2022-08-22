using ClosedXML.Excel;
using Common;
using Data.Admin;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Report;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class DemoReportController : BaseController
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        SysLogDA _sysLogDA = new SysLogDA();
        BaseController _helperController = new BaseController();

        // GET: DemoReport
        [HasCredential(ControllerName = "DemoReport")]
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
                int totalItems = 0;
                int pageSize = 0;
                var data = new List<BaoCaoModel>() {
                    new BaoCaoModel
                    {
                        ThongTinBC = "THÔNG TIN CHUNG",
                        BoldText = "Y",
                        Rowpan = 1,
                        Colpan = 1

                    },
                    new BaoCaoModel
                    {
                        STT = "1",
                        ThongTinBC = "Tổng số KH được hỗ trợ",
                        ThongTinBC_Them = "Tổng số KH được hỗ trợ",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 2,
                        MSM = 30,
                        PUD = 112,
                        SW = 2,
                        Nam = 101,
                        Nu = 43,
                        ChuyenGioi=2,
                        Tong =146
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "2",
                        ThongTinBC = "Số KH mới",
                        ThongTinBC_Them= "Số KH mới",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 2,
                        MSM = 30,
                        PUD = 112,
                        SW = 2,
                        Nam = 101,
                        Nu = 43,
                        ChuyenGioi=2,
                        Tong =146
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "3",
                        ThongTinBC = "Số KH mất dấu ",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 1,
                        MSM = 0,
                        PUD = 0,
                        SW = 0,
                        Nam = 0,
                        Nu = 0,
                        ChuyenGioi=0,
                        Tong =0
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "4",
                        ThongTinBC = "Độ tuổi của KH",
                         ThongTinBC_Them = "16- 18 tuổi",
                        BoldText = "N",
                        Rowpan = 3,
                        Colpan = 1,
                        MSM = 19,
                        PUD = 34,
                        SW = 0,
                        Nam = 38,
                        Nu = 15,
                        ChuyenGioi=1,
                        Tong =54
                    }
                     ,
                    new BaoCaoModel
                    {
                        STT = "",
                         ThongTinBC = "Độ tuổi của KH",
                       ThongTinBC_Them = "19- 22 tuổi",
                        BoldText = "N",
                        Rowpan = 0,
                        Colpan = 1,
                        MSM = 48,
                        PUD = 80,
                        SW = 2,
                        Nam = 86,
                        Nu = 44,
                        ChuyenGioi=2,
                        Tong =132
                    }
                     ,
                    new BaoCaoModel
                    {
                        STT = "",
                         ThongTinBC = "Độ tuổi của KH",
                        ThongTinBC_Them = "23- 24 tuổi",
                        BoldText = "N",
                        Rowpan = 0,
                        Colpan = 1,
                        MSM = 36,
                        PUD = 52,
                        SW = 0,
                        Nam = 75,
                        Nu = 13,
                        ChuyenGioi=6,
                        Tong =94
                    }
                };

                //var data = _DemoReportDA.GetAllByPage(modelSearch, ref pageSize);
                //if (data != null && data.Count > 0)
                //    totalItems = data.FirstOrDefault().TotalRow;
                AddLog("Lấy dữ liệu báo cáo( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") thành công.");
                return Json(new { data = data, totalItems = totalItems, Error = false, Title = "Lấy dữ liệu thành công.", pageSize = pageSize }); ;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message.ToString();
                AddLog("Lấy dữ liệu báo cáo( keyword: " + modelSearch.KeyWord + ", page: " + modelSearch.currentPage + ") lỗi: " + ex.Message);

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
                AddLog("Lấy danh sách các botom được thực hiện trên from Người dùng thành công.");
                return Json(new { Buttoms = bottoms, Error = false, Title = "Lấy dữ liệu thành công." }); ;
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
                        ControllerName = "DemoReport",
                        UserName = user.UserName,
                        DateLog = DateTime.Now,
                        Content = content
                    }
                    );
        }

        #region Xuất dữ liệu ra excel
        [HttpGet]
        public ActionResult ExportData(string listCities, string fromDate, string toDate, string listUnitId, int isThucTe)
        {
            try
            {
                var user = Session["USER_SESSION"] as UserLogin;

                var data = new List<BaoCaoModel>() {
                    new BaoCaoModel
                    {
                        ThongTinBC = "THÔNG TIN CHUNG",
                        BoldText = "Y",
                        Rowpan = 1,
                        Colpan = 1

                    },
                    new BaoCaoModel
                    {
                        STT = "1",
                        ThongTinBC = "Tổng số KH được hỗ trợ",
                        ThongTinBC_Them = "Tổng số KH được hỗ trợ",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 2,
                        MSM = 30,
                        PUD = 112,
                        SW = 2,
                        Nam = 101,
                        Nu = 43,
                        ChuyenGioi=2,
                        Tong =146
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "2",
                        ThongTinBC = "Số KH mới",
                        ThongTinBC_Them= "Số KH mới",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 2,
                        MSM = 30,
                        PUD = 112,
                        SW = 2,
                        Nam = 101,
                        Nu = 43,
                        ChuyenGioi=2,
                        Tong =146
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "3",
                        ThongTinBC = "Số KH mất dấu ",
                        BoldText = "N",
                        Rowpan = 1,
                        Colpan = 1,
                        MSM = 0,
                        PUD = 0,
                        SW = 0,
                        Nam = 0,
                        Nu = 0,
                        ChuyenGioi=0,
                        Tong =0
                    }
                    ,
                    new BaoCaoModel
                    {
                        STT = "4",
                        ThongTinBC = "Độ tuổi của KH",
                         ThongTinBC_Them = "16- 18 tuổi",
                        BoldText = "N",
                        Rowpan = 3,
                        Colpan = 1,
                        MSM = 19,
                        PUD = 34,
                        SW = 0,
                        Nam = 38,
                        Nu = 15,
                        ChuyenGioi=1,
                        Tong =54
                    }
                     ,
                    new BaoCaoModel
                    {
                        STT = "",
                         ThongTinBC = "Độ tuổi của KH",
                       ThongTinBC_Them = "19- 22 tuổi",
                        BoldText = "N",
                        Rowpan = 0,
                        Colpan = 1,
                        MSM = 48,
                        PUD = 80,
                        SW = 2,
                        Nam = 86,
                        Nu = 44,
                        ChuyenGioi=2,
                        Tong =132
                    }
                     ,
                    new BaoCaoModel
                    {
                        STT = "",
                         ThongTinBC = "Độ tuổi của KH",
                        ThongTinBC_Them = "23- 24 tuổi",
                        BoldText = "N",
                        Rowpan = 0,
                        Colpan = 1,
                        MSM = 36,
                        PUD = 52,
                        SW = 0,
                        Nam = 75,
                        Nu = 13,
                        ChuyenGioi=6,
                        Tong =94
                    }
                };

                //var paramList = new List<SqlParameter>
                //    {
                //         new SqlParameter(@"@cityIds", listCities),
                //         new SqlParameter(@"@unitIds", listUnitId),
                //         new SqlParameter(@"@currentId", currentId),
                //         new SqlParameter(@"@fromDate", fromDate),
                //         new SqlParameter(@"@toDate", toDate),
                //         new SqlParameter(@"@isThucTe", isThucTe)
                //    };
                //var data = db.Database.SqlQuery<ExportExplosiveCareer>("exec [GetExportExplosiveByNganhNghe_Export] @cityIds, @unitIds, @currentId, @fromDate, @toDate, @isThucTe", paramList.ToArray()).ToList();
                var file_name = "BaoCaoThang.xlsx";
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo tháng");
                    var titleReport = "BÁO CÁO 6 THÁNG";
                    CreateHeader(ws, titleReport, user);

                    var columnName = "";
                    var columnNumber = 0;
                    var row = 4;
                    if (data.Any())
                    {
                       
                        foreach (var rowReport in data)
                        {
                            // Thêm dữ liệu cột STT
                            InsertDataCell(ws, "A", row, rowReport.STT, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                            // Thêm dữ liệu cột thông tin BC
                            InsertDataCell(ws, "B", row, rowReport.ThongTinBC, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                            if(rowReport.Colpan > 0)
                            {
                                columnNumber = ExcelColumnNameToNumber("B");
                                columnName = GetExcelColumnName(columnNumber + (int)rowReport.Colpan);
                                ws.Range("B" + row + ":" + columnName + row).Row(1).Merge();
                            }
                            else
                            {
                                // Thêm dữ liệu cột thông tin BC - thêm
                                InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);
                            }

                            if(rowReport.Rowpan > 0)
                            {
                                ws.Range("B" + row + ":" + "B" + (row+ rowReport.Rowpan)).Row(1).Merge();
                            }

                            // Thêm dữ liệu cột MSM
                            InsertDataCell(ws, "D", row, rowReport.MSM  > 0 ? rowReport.MSM.ToString() : "", true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, rowReport.MSM > 0 ? true : false);

                            // Thêm dữ liệu cột PUD
                            InsertDataCell(ws, "E", row, rowReport.PUD > 0 ? rowReport.PUD.ToString() : "", true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, rowReport.PUD > 0 ? true : false);

                            // Thêm dữ liệu cột SW
                            InsertDataCell(ws, "F", row, rowReport.SW > 0 ? rowReport.SW.ToString() : "", true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, rowReport.SW > 0 ? true : false);

                            // Thêm dữ liệu cột Nam
                            InsertDataCell(ws, "G", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                            // Thêm dữ liệu cột Nữ
                            InsertDataCell(ws, "H", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);

                            // Thêm dữ liệu cột Chuyển giới
                            InsertDataCell(ws, "I", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);

                            // Thêm dữ liệu cột Tổng
                            InsertDataCell(ws, "J", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);
                            row++;
                        }
                    }

                    ws.Range("A3:J" + row).Style.Font.FontName = "Times New Roman";
                    ws.Range("A3:J" + row).Style.Font.FontSize = 13;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        ws.Columns(1, 30).AdjustToContents();
                        ws.Column("B").Width = 50;
                        ws.Column("C").Width = 20;
                        ws.Column("D").Width = 20;
                        ws.Column("E").Width = 20;
                        ws.Column("F").Width = 20;
                        ws.Column("G").Width = 20;
                        ws.Column("H").Width = 20;
                        ws.Column("I").Width = 20;
                        ws.Column("J").Width = 20;
                        ws.Column("A").Style.Alignment.SetWrapText(true);
                        ws.Column("B").Style.Alignment.SetWrapText(true);
                        ws.Column("C").Style.Alignment.SetWrapText(true);
                        ws.Column("D").Style.Alignment.SetWrapText(true);
                        ws.Column("E").Style.Alignment.SetWrapText(true);
                        ws.Column("F").Style.Alignment.SetWrapText(true);
                        ws.Column("G").Style.Alignment.SetWrapText(true);
                        ws.Column("H").Style.Alignment.SetWrapText(true);
                        ws.Column("I").Style.Alignment.SetWrapText(true);
                        ws.Column("J").Style.Alignment.SetWrapText(true);
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
                ws.Cell(cellName + row).Style.NumberFormat.Format = "#,##0.00";
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
        private void CreateHeader(IXLWorksheet ws, string tileReport, UserLogin user)
        {
            #region header
            // 
            ws.Cell("A1").Value = tileReport;//"BÁO CÁO 6 THÁNG (THÁNG 4,5,6,7,8,9/2021)";
            ws.Range("A1:J1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";

            //header table
            ws.Cell("A3").Value = "#";
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B3").Value = "Thông tin báo cáo";
            ws.Range("B3:C3").Merge();
            ws.Cell("B3").Style.Font.Bold = true;
            ws.Cell("B3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D3").Value = "MSM";
            ws.Cell("D3").Style.Font.Bold = true;
            ws.Cell("D3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            
            ws.Cell("E3").Value = "PUD";
            ws.Cell("E3").Style.Font.Bold = true;
            ws.Cell("E3").Style.Alignment.WrapText = true;
            ws.Cell("E3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F3").Value = "SW";
            ws.Cell("F3").Style.Font.Bold = true;
            ws.Cell("F3").Style.Alignment.WrapText = true;
            ws.Cell("F3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G3").Value = "Nam";
            ws.Cell("G3").Style.Font.Bold = true;
            ws.Cell("G3").Style.Alignment.WrapText = true;
            ws.Cell("G3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H3").Value = "Nữ";
            ws.Cell("H3").Style.Font.Bold = true;
            ws.Cell("H3").Style.Alignment.WrapText = true;
            ws.Cell("H3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("I3").Value = "Chuyển giới";
            ws.Cell("I3").Style.Font.Bold = true;
            ws.Cell("I3").Style.Alignment.WrapText = true;
            ws.Cell("I3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("J3").Value = "Tổng";
            ws.Cell("J3").Style.Font.Bold = true;
            ws.Cell("J3").Style.Alignment.WrapText = true;
            ws.Cell("J3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("J3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion
        }


        #endregion 

    }
}