using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ClosedXML.Excel;
using Data.Admin;
using Data.InterfaceDA.Admin;
using log4net;
using Model.ModelExtend;
using Model.ModelExtend.Report;
using Model.ModelExtend.User;

namespace WebBVTLAPI.Controllers
{
    public class ExportReportFileController : ApiController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static IUserDA userDA ;
        private static IBaoCaoTongHopDA _BaoCaoTongHopDA ;
        private static IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA ;
        private static IDuAnDA _DuAnDA ;

        public ExportReportFileController()
        {
            userDA = new UserDA();
            _BaoCaoTongHopDA = new BaoCaoTongHopDA();
            _BVTL_NHOM_TBHDA = new BVTL_NHOM_TBHDA();
            _DuAnDA = new DuAnDA();
        }

        /// <summary>
        /// Gửi báo cáo tháng
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ExportReportFile/CreateFileReportMonth")]
        public async Task<IHttpActionResult> CreateFileReportMonth()
        {
        var result = new List<UserSendReportModel>();
            var currentDate = DateTime.Now;
            // Lấy danh sách người dùng có email
            var users = userDA.GetAllUserByEmailNotNull();
            if (users != null && users.Count > 0)
            {
                var month = currentDate.Month;
                var year = currentDate.Year;
                if (month == 1)
                {
                    month = 12;
                    year = year - 1;
                }
                else
                    month = month - 1;

                string subject = "Báo cáo tháng " + month + " năm " + year;
                string body = "";
                var fileName = "C:/BaoCao/";
                var maNhomTBHs = "";
                var maDuAns = new List<string>();
                foreach (var user in users)
                {
                    maNhomTBHs = userDA.GetMaNhomTBHByUser((int)user.ID);
                    fileName += user.UserName + "/" + month + "_" + year;
                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(fileName))
                        Directory.CreateDirectory(fileName);
                    else
                    {
                        // delete all file
                        System.IO.DirectoryInfo di = new DirectoryInfo(fileName);

                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                    maDuAns = new List<string>();

                    if (!string.IsNullOrEmpty(user.MaDuAn))
                    {
                        maDuAns = user.MaDuAn.Split(',').ToList();
                        foreach (var maDuAn in maDuAns)
                        {
                            // Tạo file báo cáo
                            //fileName = await ExportDataReportMonth(year, month.ToString(), user.CityCodes, maNhomTBHs, maDuAn, fileName);
                            try
                            {
                                var modelSearch = new ReportSearchModel() { Year = year, Months = month.ToString(), CityCodes = user.CityCodes, TypeReport = 1, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };

                                modelSearch.MaDuAn = maDuAn;

                                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);

                                // Lấy danh sách nhóm TBH theo tỉnh
                                var nhomTBHs = new List<NhomTBHPageModel>();
                                if (string.IsNullOrEmpty(maNhomTBHs))
                                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(user.CityCodes);
                                else
                                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhoms(maNhomTBHs);

                                var tenDuAn = "";
                                if (!string.IsNullOrEmpty(maDuAn))
                                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);

                                var tenNhomTBHs = "";
                                if (nhomTBHs != null && nhomTBHs.Count > 0)
                                {
                                    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                                }

                                string sNhomFilename = "";
                                if (nhomTBHs != null && nhomTBHs.Count == 1)
                                {
                                    sNhomFilename = string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                                    //fileName += "_" + sNhomFilename;
                                }
                                //else
                                //{
                                //    fileName += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                                //}

                                var lsM = month.ToString().Split(',');
                                string sMFilenam = lsM[0];
                                if (lsM.Count() > 1)
                                {
                                    sMFilenam = lsM.First() + "-" + lsM.Last();
                                }

                                fileName +="/BC_THANG_" + sMFilenam + "-" + year + ".xlsx";
                                if (!System.IO.File.Exists(fileName))
                                {
                                    System.IO.File.Create(fileName).Dispose();
                                }
                                //XLWorkbook wb = new XLWorkbook();
                                using (XLWorkbook wb = new XLWorkbook())
                                {

                                    var ws = wb.Worksheets.Add("Báo cáo tháng " + month.ToString() + " năm " + year);
                                    var titleReport = "Kỳ báo cáo: Báo cáo Tháng " + month.ToString() + " - " + year;
                                    CreateHeader(ws, titleReport, tenNhomTBHs);

                                    var columnName = "";
                                    var columnNumber = 0;
                                    var row = 6;
                                    if (data.Any())
                                    {

                                        foreach (var rowReport in data)
                                        {
                                            if (rowReport.IsShow == "Y")
                                            {
                                                // Thêm dữ liệu cột STT
                                                InsertDataCell(ws, "A", row, rowReport.STT, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                                                if (!string.IsNullOrEmpty(rowReport.ThongTinBC))
                                                {
                                                    // Thêm dữ liệu cột thông tin BC
                                                    InsertDataCell(ws, "B", row, rowReport.ThongTinBC, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                    if (rowReport.Colpan > 1)
                                                    {
                                                        columnNumber = ExcelColumnNameToNumber("B");
                                                        columnName = GetExcelColumnName(columnNumber + (int)rowReport.Colpan);
                                                        ws.Range("B" + row + ":" + columnName + row).Column(1).Merge();
                                                    }
                                                    else
                                                    {
                                                        // Thêm dữ liệu cột thông tin BC - thêm
                                                        InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                    }

                                                    if (rowReport.Rowpan > 1)
                                                    {
                                                        ws.Range("B" + row + ":" + "B" + (row + rowReport.Rowpan - 1)).Merge();
                                                    }
                                                }
                                                else
                                                {
                                                    // Thêm dữ liệu cột thông tin BC - thêm
                                                    InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                }


                                                // Thêm dữ liệu cột Tổng
                                                InsertDataCell(ws, "D", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);

                                                // Thêm dữ liệu cột MSM
                                                InsertDataCell(ws, "E", row, rowReport.MSM > 0 ? rowReport.MSM.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.MSM > 0 ? true : false);

                                                // Thêm dữ liệu cột PUD
                                                InsertDataCell(ws, "F", row, rowReport.PUD > 0 ? rowReport.PUD.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.PUD > 0 ? true : false);

                                                // Thêm dữ liệu cột SW
                                                InsertDataCell(ws, "G", row, rowReport.SW > 0 ? rowReport.SW.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.SW > 0 ? true : false);

                                                // Thêm dữ liệu cột Nam
                                                InsertDataCell(ws, "H", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                                                // Thêm dữ liệu cột Nữ
                                                InsertDataCell(ws, "I", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);

                                                // Thêm dữ liệu cột Chuyển giới
                                                InsertDataCell(ws, "J", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);


                                                row++;
                                            }
                                        }
                                    }

                                    CreateFooter(ws);
                                    ws.Range("A5:J" + row).Style.Font.FontName = "Times New Roman";
                                    ws.Range("A5:J" + row).Style.Font.FontSize = 13;
                                    ws.Range("A5:J" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;

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

                                    wb.SaveAs(fileName);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Tạo file báo cáo tháng lỗi: " + ex.Message);
                                fileName = null;
                            }
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                result.Add(new UserSendReportModel()
                                {
                                    Email = user.Email,
                                    FullName = user.Name,
                                    PathFileExcel = fileName
                                });
                            }
                        }
                    }
                }

            }

            return Ok(result);
        }

        /// <summary>
        /// Gửi báo cáo quý
        /// </summary>
        /// <returns></returns>
         [HttpGet]
        [Route("api/ExportReportFile/CreateFileReportQuater")]
        public async Task<IHttpActionResult> CreateFileReportQuater()
        {
           
            var result = new List<UserSendReportModel>();
            var currentDate = DateTime.Now;
            // Lấy danh sách người dùng có email
            var users = userDA.GetAllUserByEmailNotNull();
            if (users != null && users.Count > 0)
            {
                var months = "";
                var quater = "";
                var month = currentDate.Month;
                var year = currentDate.Year;
                if (month == 1)
                {
                    quater = "IV";
                    months = "10,11,12";
                    year = year - 1;
                }
                else if (month == 4)
                {
                    quater = "I";
                    months = "1,2,3";
                }
                else if (month == 7)
                {
                    quater = "II";
                    months = "4,5,6";
                }
                else if (month == 10)
                {
                    quater = "III";
                    months = "7,8,9";
                }

                string subject = "Báo cáo quý " + quater + " năm " + year;
                string body = "";
                var fileName = "C:/BaoCao/";
                var maNhomTBHs = "";
                var maDuAns = new List<string>();
                foreach (var user in users)
                {
                    maNhomTBHs = userDA.GetMaNhomTBHByUser((int)user.ID);
                    fileName += user.UserName + "/" + quater + "_" + year;
                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(fileName))
                        Directory.CreateDirectory(fileName);
                    else
                    {
                        // delete all file
                        System.IO.DirectoryInfo di = new DirectoryInfo(fileName);

                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                    maDuAns = new List<string>();

                    if (!string.IsNullOrEmpty(user.MaDuAn))
                    {
                        maDuAns = user.MaDuAn.Split(',').ToList();
                        foreach (var maDuAn in maDuAns)
                        {
                            // Tạo file báo cáo
                            //fileName = ExportDataReportQuater(year, months, user.CityCodes, quater, maNhomTBHs, maDuAn, fileName);
                            try
                            {

                                var modelSearch = new ReportSearchModel() { Year = year, Months = months, CityCodes = user.CityCodes, TypeReport = 2, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };
                                modelSearch.MaDuAn = maDuAn;

                                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);

                                // Lấy danh sách nhóm TBH theo tỉnh
                                var nhomTBHs = new List<NhomTBHPageModel>();
                                if (string.IsNullOrEmpty(maNhomTBHs))
                                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(user.CityCodes);
                                else
                                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhoms(maNhomTBHs);
                                var tenDuAn = "";
                                if (!string.IsNullOrEmpty(maDuAn))
                                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);
                                var tenNhomTBHs = "";
                                if (nhomTBHs != null && nhomTBHs.Count > 0)
                                {
                                    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                                }

                                string sNhomFilename = "";
                                if (nhomTBHs != null && nhomTBHs.Count == 1)
                                {
                                    sNhomFilename = string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                                    //fileName += "_" + sNhomFilename;
                                }
                                //else
                                //{
                                //    fileName += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                                //}
                                fileName += "/BC_QUY_" + quater + "-" + year + ".xlsx";
                                if (!System.IO.File.Exists(fileName))
                                {
                                    System.IO.File.Create(fileName).Dispose();
                                }
                                using (XLWorkbook wb = new XLWorkbook())
                                {

                                    var ws = wb.Worksheets.Add("Báo cáo quý " + quater + " năm " + year);
                                    var titleReport = "Kỳ báo cáo: Báo cáo Quý " + quater + " - " + year;
                                    CreateHeader(ws, titleReport, tenNhomTBHs);

                                    var columnName = "";
                                    var columnNumber = 0;
                                    var row = 6;
                                    if (data.Any())
                                    {

                                        foreach (var rowReport in data)
                                        {
                                            if (rowReport.IsShow == "Y")
                                            {
                                                // Thêm dữ liệu cột STT
                                                InsertDataCell(ws, "A", row, rowReport.STT, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                                                if (!string.IsNullOrEmpty(rowReport.ThongTinBC))
                                                {
                                                    // Thêm dữ liệu cột thông tin BC
                                                    InsertDataCell(ws, "B", row, rowReport.ThongTinBC, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                    if (rowReport.Colpan > 1)
                                                    {
                                                        columnNumber = ExcelColumnNameToNumber("B");
                                                        columnName = GetExcelColumnName(columnNumber + (int)rowReport.Colpan);
                                                        ws.Range("B" + row + ":" + columnName + row).Column(1).Merge();
                                                    }
                                                    else
                                                    {
                                                        // Thêm dữ liệu cột thông tin BC - thêm
                                                        InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                    }

                                                    if (rowReport.Rowpan > 1)
                                                    {
                                                        ws.Range("B" + row + ":" + "B" + (row + rowReport.Rowpan - 1)).Merge();
                                                    }
                                                }
                                                else
                                                {
                                                    // Thêm dữ liệu cột thông tin BC - thêm
                                                    InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                }


                                                // Thêm dữ liệu cột Tổng
                                                InsertDataCell(ws, "D", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);

                                                // Thêm dữ liệu cột MSM
                                                InsertDataCell(ws, "E", row, rowReport.MSM > 0 ? rowReport.MSM.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.MSM > 0 ? true : false);

                                                // Thêm dữ liệu cột PUD
                                                InsertDataCell(ws, "F", row, rowReport.PUD > 0 ? rowReport.PUD.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.PUD > 0 ? true : false);

                                                // Thêm dữ liệu cột SW
                                                InsertDataCell(ws, "G", row, rowReport.SW > 0 ? rowReport.SW.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.SW > 0 ? true : false);

                                                // Thêm dữ liệu cột Nam
                                                InsertDataCell(ws, "H", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                                                // Thêm dữ liệu cột Nữ
                                                InsertDataCell(ws, "I", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);

                                                // Thêm dữ liệu cột Chuyển giới
                                                InsertDataCell(ws, "J", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);


                                                row++;
                                            }
                                        }
                                    }

                                    CreateFooter(ws);

                                    ws.Range("A5:J" + row).Style.Font.FontName = "Times New Roman";
                                    ws.Range("A5:J" + row).Style.Font.FontSize = 13;
                                    ws.Range("A5:J" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;


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
                                    wb.SaveAs(fileName);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Tạo file báo cáo quý lỗi: " + ex.Message);
                                fileName = "";
                            }
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                result.Add(new UserSendReportModel()
                                {
                                    Email = user.Email,
                                    FullName = user.Name,
                                    PathFileExcel = fileName
                                });
                            }
                        }
                    }
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// Gửi báo cáo quý
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ExportReportFile/CreateFileReportYear")]
        public async Task<IHttpActionResult> CreateFileReportYear()
        {
            
            var result = new List<UserSendReportModel>();
            var currentDate = DateTime.Now;
            // Lấy danh sách người dùng có email
            var users = userDA.GetAllUserByEmailNotNull();
            if (users != null && users.Count > 0)
            {
                var months = "1,2,3,4,5,6,7,8,9,10,11,12";
                var year = currentDate.Year - 1;

                string subject = "Báo cáo năm năm " + year;
                string body = "";
                var fileName = "C:/BaoCao/";
                var maNhomTBHs = "";
                var maDuAns = new List<string>();
                foreach (var user in users)
                {
                    maNhomTBHs = userDA.GetMaNhomTBHByUser((int)user.ID);
                    fileName += user.UserName + "/" + year;
                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(fileName))
                        Directory.CreateDirectory(fileName);
                    else
                    {
                        // delete all file
                        System.IO.DirectoryInfo di = new DirectoryInfo(fileName);

                        foreach (FileInfo file in di.GetFiles())
                        {
                            file.Delete();
                        }
                    }

                    maDuAns = new List<string>();

                    if (!string.IsNullOrEmpty(user.MaDuAn))
                    {
                        maDuAns = user.MaDuAn.Split(',').ToList();
                        foreach (var maDuAn in maDuAns)
                        {
                            // Tạo file báo cáo
                            //fileName = ExportDataReportYear(year, months, user.CityCodes, maNhomTBHs, maDuAn, fileName);
                            try
                            {

                                var modelSearch = new ReportSearchModel() { Year = year, Months = months, CityCodes = user.CityCodes, TypeReport = 4, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };
                                modelSearch.MaDuAn = maDuAn;
                                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);
                                // Lấy danh sách nhóm TBH theo tỉnh
                                var nhomTBHs = new List<NhomTBHPageModel>();
                                if (string.IsNullOrEmpty(maNhomTBHs))
                                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(user.CityCodes);
                                else
                                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByMaNhoms(maNhomTBHs);
                                var tenDuAn = "";
                                if (!string.IsNullOrEmpty(maDuAn))
                                    tenDuAn = "Dự án: " + _DuAnDA.GetItemByCode(maDuAn);
                                var tenNhomTBHs = "";
                                if (nhomTBHs != null && nhomTBHs.Count > 0)
                                {
                                    tenNhomTBHs = "Nhóm: " + string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                                }

                                string sNhomFilename = "";
                                if (nhomTBHs != null && nhomTBHs.Count == 1)
                                {
                                    sNhomFilename = string.Join("; ", nhomTBHs.Select(x => x.tennhom_tbh + "-" + x.CityName));
                                    //fileName += "_" + sNhomFilename;
                                }
                                //else
                                //{
                                //    fileName += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                                //}

                                //var file_name = maDuAn + "_" + string.Join("-", nhomTBHs.Select(x => x.manhom_tbh)) + "_BAO_CAO_NAM_" + Year + ".xlsx";
                                fileName += "/BC_NAM_" + year + ".xlsx";
                                if (!System.IO.File.Exists(fileName))
                                {
                                    System.IO.File.Create(fileName).Dispose();
                                }
                                using (XLWorkbook wb = new XLWorkbook())
                                {

                                    var ws = wb.Worksheets.Add("Báo cáo năm " + year);
                                    var titleReport = "Kỳ báo cáo: Báo cáo Năm " + year;
                                    CreateHeader(ws, titleReport, tenNhomTBHs);

                                    var columnName = "";
                                    var columnNumber = 0;
                                    var row = 6;
                                    if (data.Any())
                                    {

                                        foreach (var rowReport in data)
                                        {
                                            if (rowReport.IsShow == "Y")
                                            {
                                                // Thêm dữ liệu cột STT
                                                InsertDataCell(ws, "A", row, rowReport.STT, true, XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues.Center, false);

                                                if (!string.IsNullOrEmpty(rowReport.ThongTinBC))
                                                {
                                                    // Thêm dữ liệu cột thông tin BC
                                                    InsertDataCell(ws, "B", row, rowReport.ThongTinBC, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                    if (rowReport.Colpan > 1)
                                                    {
                                                        columnNumber = ExcelColumnNameToNumber("B");
                                                        columnName = GetExcelColumnName(columnNumber + (int)rowReport.Colpan);
                                                        ws.Range("B" + row + ":" + columnName + row).Column(1).Merge();
                                                    }
                                                    else
                                                    {
                                                        // Thêm dữ liệu cột thông tin BC - thêm
                                                        InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                    }

                                                    if (rowReport.Rowpan > 1)
                                                    {
                                                        ws.Range("B" + row + ":" + "B" + (row + rowReport.Rowpan - 1)).Merge();
                                                    }
                                                }
                                                else
                                                {
                                                    // Thêm dữ liệu cột thông tin BC - thêm
                                                    InsertDataCell(ws, "C", row, rowReport.ThongTinBC_Them, true, XLAlignmentHorizontalValues.Left, XLAlignmentVerticalValues.Center, false);
                                                }


                                                // Thêm dữ liệu cột Tổng
                                                InsertDataCell(ws, "D", row, rowReport.Tong > 0 ? rowReport.Tong.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Tong > 0 ? true : false);

                                                // Thêm dữ liệu cột MSM
                                                InsertDataCell(ws, "E", row, rowReport.MSM > 0 ? rowReport.MSM.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.MSM > 0 ? true : false);

                                                // Thêm dữ liệu cột PUD
                                                InsertDataCell(ws, "F", row, rowReport.PUD > 0 ? rowReport.PUD.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.PUD > 0 ? true : false);

                                                // Thêm dữ liệu cột SW
                                                InsertDataCell(ws, "G", row, rowReport.SW > 0 ? rowReport.SW.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.SW > 0 ? true : false);

                                                // Thêm dữ liệu cột Nam
                                                InsertDataCell(ws, "H", row, rowReport.Nam > 0 ? rowReport.Nam.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nam > 0 ? true : false);

                                                // Thêm dữ liệu cột Nữ
                                                InsertDataCell(ws, "I", row, rowReport.Nu > 0 ? rowReport.Nu.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.Nu > 0 ? true : false);

                                                // Thêm dữ liệu cột Chuyển giới
                                                InsertDataCell(ws, "J", row, rowReport.ChuyenGioi > 0 ? rowReport.ChuyenGioi.ToString() : "", true, XLAlignmentHorizontalValues.Right, XLAlignmentVerticalValues.Center, rowReport.ChuyenGioi > 0 ? true : false);


                                                row++;
                                            }

                                        }
                                    }

                                    CreateFooter(ws);

                                    ws.Range("A5:J" + row).Style.Font.FontName = "Times New Roman";
                                    ws.Range("A5:J" + row).Style.Font.FontSize = 13;
                                    ws.Range("A5:J" + row).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    ws.Range("A5:J" + row).Style.Border.BottomBorder = XLBorderStyleValues.Thin;


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
                                    wb.SaveAs(fileName);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Tạo file báo cáo năm lỗi: " + ex.Message);
                                fileName = "";
                            }
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                result.Add(new UserSendReportModel()
                                {
                                    Email = user.Email,
                                    FullName = user.Name,
                                    PathFileExcel = fileName
                                });
                            }
                        }
                    }
                }

            }
            return Ok(result);

        }

        #region Xuất dữ liệu ra excel
       
        public string GetExcelColumnName(int columnNumber)
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
        public void InsertDataCell(IXLWorksheet ws, string cellName, int row, string value, bool bold,
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
        public void InsertDataCell_Merge(IXLWorksheet ws, string cellName, int row, string value, bool bold,
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
        public void CreateHeader(IXLWorksheet ws, string tileReport, string tenNhomTBHs)
        {
            #region header
            // 
            ws.Cell("A1").Value = "BÁO CÁO HOẠT ĐỘNG";//"BÁO CÁO 6 THÁNG (THÁNG 4,5,6,7,8,9/2021)";
            ws.Range("A1:J1").Row(1).Merge();
            ws.Cell("A1").Style.Font.Bold = true;
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A1").Style.Font.FontName = "Times New Roman";
            ws.Cell("A1").Style.Font.FontSize = 13;

            // 
            ws.Cell("A2").Value = tileReport;//"BÁO CÁO 6 THÁNG (THÁNG 4,5,6,7,8,9/2021)";
            ws.Range("A2:J2").Row(1).Merge();
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A2").Style.Font.FontName = "Times New Roman";
            ws.Cell("A2").Style.Font.FontSize = 13;

            // 
            ws.Cell("A3").Value = tenNhomTBHs;
            ws.Range("A3:J3").Row(1).Merge();
            ws.Cell("A3").Style.Font.Bold = true;
            ws.Cell("A3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("A3").Style.Font.FontName = "Times New Roman";
            ws.Cell("A3").Style.Font.FontSize = 13;

            var row = 5;

            //header table
            ws.Cell("A" + row).Value = "#";
            ws.Cell("A" + row).Style.Font.Bold = true;
            ws.Cell("A" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("B" + row).Value = "Thông tin báo cáo";
            ws.Range("B" + row + ":C" + row).Merge();
            ws.Cell("B" + row).Style.Font.Bold = true;
            ws.Cell("B" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("D" + row).Value = "Tổng";
            ws.Cell("D" + row).Style.Font.Bold = true;
            ws.Cell("D" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("D" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("E" + row).Value = "MSM";
            ws.Cell("E" + row).Style.Font.Bold = true;
            ws.Cell("E" + row).Style.Alignment.WrapText = true;
            ws.Cell("E" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("E" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("F" + row).Value = "PUD";
            ws.Cell("F" + row).Style.Font.Bold = true;
            ws.Cell("F" + row).Style.Alignment.WrapText = true;
            ws.Cell("F" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("G" + row).Value = "SW";
            ws.Cell("G" + row).Style.Font.Bold = true;
            ws.Cell("G" + row).Style.Alignment.WrapText = true;
            ws.Cell("G" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("G" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("H" + row).Value = "Nam";
            ws.Cell("H" + row).Style.Font.Bold = true;
            ws.Cell("H" + row).Style.Alignment.WrapText = true;
            ws.Cell("H" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("H" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("I" + row).Value = "Nữ";
            ws.Cell("I" + row).Style.Font.Bold = true;
            ws.Cell("I" + row).Style.Alignment.WrapText = true;
            ws.Cell("I" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("I" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            ws.Cell("J" + row).Value = "Chuyển giới";
            ws.Cell("J" + row).Style.Font.Bold = true;
            ws.Cell("J" + row).Style.Alignment.WrapText = true;
            ws.Cell("J" + row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("J" + row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            #endregion
        }

        public void CreateFooter(IXLWorksheet ws)
        {
            ws.Cell("B42").Value = "Trưởng nhóm";
            ws.Cell("B42").Style.Font.Bold = true;
            ws.Cell("B42").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("B42").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("B42").Style.Font.FontName = "Times New Roman";
            ws.Cell("B42").Style.Font.FontSize = 13;

            ws.Cell("C42").Value = "Cán bộ dự án";
            ws.Range("C42:D42").Row(1).Merge();
            ws.Cell("C42").Style.Font.Bold = true;
            ws.Cell("C42").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("C42").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("C42").Style.Font.FontName = "Times New Roman";
            ws.Cell("C42").Style.Font.FontSize = 13;

            ws.Cell("F42").Value = "Quản lý chương trình";
            ws.Range("F42:I42").Row(1).Merge();
            ws.Cell("F42").Style.Font.Bold = true;
            ws.Cell("F42").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("F42").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            ws.Cell("F42").Style.Font.FontName = "Times New Roman";
            ws.Cell("F42").Style.Font.FontSize = 13;

        }
        #endregion
    }
}
