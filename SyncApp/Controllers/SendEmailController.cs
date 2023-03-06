using ClosedXML.Excel;
using Common;
using Data.Admin;
using Data.API;
using Data.InterfaceDA.Admin;
using Data.InterfaceDA.API;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.API;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using Model.ModelExtend.User;
using Newtonsoft.Json;
using SyncBVTL.Push.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SyncBVTL.Push.Controllers.PA
{
    public class SendEmailController : BaseController
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string accessToken = "";
        public IUserDA userDA = new UserDA();
        private string AddressEmail = "";
        private string PassEmail = "";
        private string NotifiBCT = "";
        private string NotifiBCQ = "";
        ISysParameterDA _sysParameterDA = new SysParameterDA();
        IBaoCaoTongHopDA _BaoCaoTongHopDA = new BaoCaoTongHopDA();
        IBVTL_NHOM_TBHDA _BVTL_NHOM_TBHDA = new BVTL_NHOM_TBHDA();
        IDuAnDA _DuAnDA = new DuAnDA();

        public ActionResult Index()
        {

            return View();
        }

        /// <summary>
        /// Gửi danh sách khách hàng sắp đến hạn khám lại
        /// </summary>
        /// <returns></returns>
        public async Task SendNotification()
        {
            AddressEmail = _sysParameterDA.GetByParamCode("EmailSend").FirstOrDefault().ParamValue;
            PassEmail = _sysParameterDA.GetByParamCode("PassEmail").FirstOrDefault().ParamValue;
            // Lấy danh sách người dùng có email
            var users = userDA.GetAllUserByEmailNotNull();
            if (users != null && users.Count > 0)
            {
                var notificationSearch = new ReportSearchModel();
                var notifications = new List<NotificationModel>();

                string subject = "Danh sách khách hàng sắp đến hẹn khám lại";
                string body = "<div class=\"container\"><h2>Danh sách khách hàng sắp đến hẹn khám lại</h2> " +
                    "<style>" +
                    ".container {background-color:#E7E9EB; width: 100%;overflow: auto;position: absolute;top: 144px;bottom: 0;height: auto;}" +
                    ".container.horizontal{min-height:200px; margin-left:0;}" +
                    ".table {  width: 100%; max-width: 100%; margin-bottom: 20px; background-color: transparent;border-collapse: collapse; border-spacing: 0;}" +
                    "tr{display: table-row;vertical-align: inherit;border-color: inherit;}" +
                    ".table>thead>tr>th {vertical-align: bottom;border: 1x solid #ddd; text-align: center;}" +
                    ".table>tbody>tr>td {padding: 8px;line-height: 1.42857143;vertical-align: top;border: 1px solid #ddd;}" +
                    ".text-left{text-align: left;}" +
                    "</style>" +
                    "<table class=\"table\" width=\"100%\" cellspacing=\"0\">" +
                    "<thead>" +
                        "<tr>" +
                            "<th style =\"width: auto;\"> Nội dung</th> " +
                            "<th style =\"width: auto;\"> Mã KH</th> " +
                            "<th style =\"width: auto;\"> Họ tên</th> " +
                            "<th style =\"width:auto;\"> Giới tính</th> " +
                            "<th style =\"width: auto;\"> Năm sinh</th> " +
                            "<th style =\"width:auto;\"> Số điện thoại</th> " +
                        "</tr> " +
                    "</thead> ";
                foreach (var user in users)
                {
                    // Lấy danh sách thông báo theo người dùng
                    notificationSearch = new ReportSearchModel
                    {
                        MaDuAn = user.MaDuAn,
                        CityCodes = user.CityCodes
                    };
                    notifications = userDA.GetNotification(notificationSearch);
                    // Gửi thông báo đến người dùng
                    //if (notifications != null && notifications.Count > 0)
                    {
                        foreach (var notification in notifications)
                        {
                            body += "<tr>" +
                            "<td class=\"text-left\">" + notification.NoiDung + "</td>" +
                            "<td class=\"text-left\">" + notification.MaKH + " </td>" +
                            "<td class=\"text-left\">" + notification.HoTen + " </td>" +
                            "<td class=\"text-left\">" + notification.GioiTinh + " </td>" +
                            "<td class=\"text-left\">" + notification.NamSinh + " </td>" +
                            "<td class=\"text-left\">" + notification.SoDienThoai + " </td>" +
                        "</tr>";
                        }

                        body += "</table></div>";
                        try
                        {
                            using (MailMessage mail = new MailMessage())
                            {
                                mail.From = new MailAddress(AddressEmail);
                                mail.To.Add(user.Email);
                                mail.Subject = subject;
                                mail.Body = body;
                                mail.IsBodyHtml = true;
                                //mail.Attachments.Add(new Attachment("C:\\file.zip"));

                                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                {
                                    smtp.Credentials = new NetworkCredential(AddressEmail, PassEmail);
                                    smtp.EnableSsl = true;
                                    smtp.Send(mail);
                                }
                            }

                            log.Info("Gửi email notification(" + user.Email + ") thành công!");
                        }
                        catch (Exception ex)
                        {
                            log.Error("Lỗi gửi email notification(" + user.Email + "): " + ex.Message);
                        }

                    }
                }

            }
        }

        /// <summary>
        /// Gửi báo cáo tháng
        /// </summary>
        /// <returns></returns>
        public async Task SendReportMonth()
        {
            AddressEmail = _sysParameterDA.GetByParamCode("EmailSend").FirstOrDefault().ParamValue;
            PassEmail = _sysParameterDA.GetByParamCode("PassEmail").FirstOrDefault().ParamValue;
            NotifiBCT = _sysParameterDA.GetByParamCode("NotifiBCT").FirstOrDefault().ParamValue;
            var currentDate = DateTime.Now;
            if (NotifiBCT == currentDate.Day.ToString() && currentDate.Hour == 1)
            {
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

                        maDuAns = new List<string>();

                        if (!string.IsNullOrEmpty(user.MaDuAn))
                        {
                            maDuAns = user.MaDuAn.Split(',').ToList();
                            foreach (var maDuAn in maDuAns)
                            {
                                // Tạo file báo cáo
                                fileName = ExportDataReportMonth(year, month.ToString(), user.CityCodes, maNhomTBHs, maDuAn, fileName);
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    try
                                    {
                                        using (MailMessage mail = new MailMessage())
                                        {
                                            mail.From = new MailAddress(AddressEmail);
                                            mail.To.Add(user.Email);
                                            mail.Subject = subject;
                                            mail.Body = body;
                                            mail.IsBodyHtml = true;
                                            mail.Attachments.Add(new Attachment(fileName));

                                            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                            {
                                                smtp.Credentials = new NetworkCredential(AddressEmail, PassEmail);
                                                smtp.EnableSsl = true;
                                                smtp.Send(mail);
                                            }
                                        }

                                        log.Info("Gửi email báo cáo tháng(" + user.Email + ") thành công!");
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error("Lỗi gửi email báo cáo tháng(" + user.Email + "): " + ex.Message);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Gửi báo cáo quý
        /// </summary>
        /// <returns></returns>
        public async Task SendReportQuater()
        {
            AddressEmail = _sysParameterDA.GetByParamCode("EmailSend").FirstOrDefault().ParamValue;
            PassEmail = _sysParameterDA.GetByParamCode("PassEmail").FirstOrDefault().ParamValue;
            NotifiBCQ = _sysParameterDA.GetByParamCode("NotifiBCQ").FirstOrDefault().ParamValue;
            var currentDate = DateTime.Now;
            if (NotifiBCQ == currentDate.Day.ToString() && currentDate.Hour == 1 &&
                (currentDate.Month == 1 || currentDate.Month == 4 || currentDate.Month == 7 || currentDate.Month == 10))
            {
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

                        maDuAns = new List<string>();

                        if (!string.IsNullOrEmpty(user.MaDuAn))
                        {
                            maDuAns = user.MaDuAn.Split(',').ToList();
                            foreach (var maDuAn in maDuAns)
                            {
                                // Tạo file báo cáo
                                fileName = ExportDataReportQuater(year, months, user.CityCodes, quater, maNhomTBHs, maDuAn, fileName);
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    try
                                    {
                                        using (MailMessage mail = new MailMessage())
                                        {
                                            mail.From = new MailAddress(AddressEmail);
                                            mail.To.Add(user.Email);
                                            mail.Subject = subject;
                                            mail.Body = body;
                                            mail.IsBodyHtml = true;
                                            mail.Attachments.Add(new Attachment(fileName));

                                            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                            {
                                                smtp.Credentials = new NetworkCredential(AddressEmail, PassEmail);
                                                smtp.EnableSsl = true;
                                                smtp.Send(mail);
                                            }
                                        }

                                        log.Info("Gửi email báo cáo quý(" + user.Email + ") thành công!");
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error("Lỗi gửi email báo cáo quý(" + user.Email + "): " + ex.Message);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Gửi báo cáo quý
        /// </summary>
        /// <returns></returns>
        public async Task SendReportYear()
        {
            AddressEmail = _sysParameterDA.GetByParamCode("EmailSend").FirstOrDefault().ParamValue;
            PassEmail = _sysParameterDA.GetByParamCode("PassEmail").FirstOrDefault().ParamValue;
            NotifiBCQ = _sysParameterDA.GetByParamCode("NotifiBCQ").FirstOrDefault().ParamValue;
            var currentDate = DateTime.Now;
            if (NotifiBCQ == currentDate.Day.ToString() && currentDate.Hour == 1 &&
                (currentDate.Month == 1))
            {
                // Lấy danh sách người dùng có email
                var users = userDA.GetAllUserByEmailNotNull();
                if (users != null && users.Count > 0)
                {
                    var months = "1,2,3,4,5,6,7,8,9,10,11,12";
                    var year = currentDate.Year -1;

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

                        maDuAns = new List<string>();

                        if (!string.IsNullOrEmpty(user.MaDuAn))
                        {
                            maDuAns = user.MaDuAn.Split(',').ToList();
                            foreach (var maDuAn in maDuAns)
                            {
                                // Tạo file báo cáo
                                fileName = ExportDataReportYear(year, months, user.CityCodes, maNhomTBHs, maDuAn, fileName);
                                if (!string.IsNullOrEmpty(fileName))
                                {
                                    try
                                    {
                                        using (MailMessage mail = new MailMessage())
                                        {
                                            mail.From = new MailAddress(AddressEmail);
                                            mail.To.Add(user.Email);
                                            mail.Subject = subject;
                                            mail.Body = body;
                                            mail.IsBodyHtml = true;
                                            mail.Attachments.Add(new Attachment(fileName));

                                            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                            {
                                                smtp.Credentials = new NetworkCredential(AddressEmail, PassEmail);
                                                smtp.EnableSsl = true;
                                                smtp.Send(mail);
                                            }
                                        }

                                        log.Info("Gửi email báo cáo năm(" + user.Email + ") thành công!");
                                    }
                                    catch (Exception ex)
                                    {
                                        log.Error("Lỗi gửi email báo cáo năm(" + user.Email + "): " + ex.Message);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        #region Xuất dữ liệu ra excel
        /// <summary>
        /// Export Báo cáo tháng
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Months"></param>
        /// <param name="CityCodes"></param>
        /// <param name="maNhomTBHs"></param>
        /// <param name="maDuAn"></param>
        /// <param name="file_name"></param>
        /// <returns></returns>
        [HttpGet]
        public string ExportDataReportMonth(int Year, string Months, string CityCodes, string maNhomTBHs, string maDuAn, string file_name)
        {
            try
            {
                var modelSearch = new ReportSearchModel() { Year = Year, Months = Months, CityCodes = CityCodes, TypeReport = 1, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };

                modelSearch.MaDuAn = maDuAn;

                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);

                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
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
                    file_name += "_" + sNhomFilename;
                }
                else
                {
                    file_name += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                }

                var lsM = Months.Split(',');
                string sMFilenam = lsM[0];
                if (lsM.Count() > 1)
                {
                    sMFilenam = lsM.First() + "-" + lsM.Last();
                }

                file_name += "_BC_THANG_" + sMFilenam + "-" + Year + ".xlsx";
                if (!System.IO.File.Exists(file_name))
                {
                    System.IO.File.Create(file_name).Dispose();
                }
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo tháng " + Months + " năm " + Year);
                    var titleReport = "Kỳ báo cáo: Báo cáo Tháng " + Months + " - " + Year;
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

                    wb.SaveAs(file_name);
                }
            }
            catch (Exception ex)
            {
                log.Error("Tạo file báo cáo tháng lỗi: " + ex.Message);
                file_name = null;
            }

            return file_name;
        }

        /// <summary>
        /// Export báo cáo quý
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Months"></param>
        /// <param name="CityCodes"></param>
        /// <param name="quy"></param>
        /// <param name="maNhomTBHs"></param>
        /// <param name="maDuAn"></param>
        /// <returns></returns>
        [HttpGet]
        public string ExportDataReportQuater(int Year, string Months, string CityCodes, string quy, string maNhomTBHs, string maDuAn, string file_name)
        {
            try
            {

                var modelSearch = new ReportSearchModel() { Year = Year, Months = Months, CityCodes = CityCodes, TypeReport = 2, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };
                modelSearch.MaDuAn = maDuAn;

                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);

                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
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
                    file_name += "_" + sNhomFilename;
                }
                else
                {
                    file_name += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                }
                file_name += "_BC_QUY_" + quy + "-" + Year + ".xlsx";
                if (!System.IO.File.Exists(file_name))
                {
                    System.IO.File.Create(file_name).Dispose();
                }
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo quý " + quy + " năm " + Year);
                    var titleReport = "Kỳ báo cáo: Báo cáo Quý " + quy + " - " + Year;
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
                    wb.SaveAs(file_name);
                }
            }
            catch (Exception ex)
            {
                log.Error("Tạo file báo cáo quý lỗi: " + ex.Message);
                file_name = "";
            }
            return file_name;
        }

        /// <summary>
        /// Export báo cáo năm
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Months"></param>
        /// <param name="CityCodes"></param>
        /// <param name="maNhomTBHs"></param>
        /// <param name="maDuAn"></param>
        /// <returns></returns>
        [HttpGet]
        public string ExportDataReportYear(int Year, string Months, string CityCodes, string maNhomTBHs, string maDuAn, string file_name)
        {
            try
            {

                var modelSearch = new ReportSearchModel() { Year = Year, Months = Months, CityCodes = CityCodes, TypeReport = 4, MaNhomTBH = maNhomTBHs, MaDuAn = maDuAn };
                modelSearch.MaDuAn = maDuAn;
                var data = _BaoCaoTongHopDA.GetDataReport(modelSearch);
                // Lấy danh sách nhóm TBH theo tỉnh
                var nhomTBHs = new List<NhomTBHPageModel>();
                if (string.IsNullOrEmpty(maNhomTBHs))
                    nhomTBHs = _BVTL_NHOM_TBHDA.GetItemByCityCodes(CityCodes);
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
                    file_name += "_" + sNhomFilename;
                }
                else
                {
                    file_name += "_" + nhomTBHs.Select(x => x.CityName).FirstOrDefault();
                }

                //var file_name = maDuAn + "_" + string.Join("-", nhomTBHs.Select(x => x.manhom_tbh)) + "_BAO_CAO_NAM_" + Year + ".xlsx";
                file_name += "_BC_NAM_" + Year + ".xlsx";
                if (!System.IO.File.Exists(file_name))
                {
                    System.IO.File.Create(file_name).Dispose();
                }
                using (XLWorkbook wb = new XLWorkbook())
                {

                    var ws = wb.Worksheets.Add("Báo cáo năm " + Year);
                    var titleReport = "Kỳ báo cáo: Báo cáo Năm " + Year;
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
                    wb.SaveAs(file_name);
                }
            }
            catch (Exception ex)
            {
                log.Error("Tạo file báo cáo năm lỗi: "+ex.Message);
                file_name = "";
            }
            return file_name;
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
        private void CreateHeader(IXLWorksheet ws, string tileReport, string tenNhomTBHs)
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

        private void CreateFooter(IXLWorksheet ws)
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