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
        private readonly string apiCreateFileReport = ConfigurationManager.AppSettings["apiCreateFileReport"].ToString();
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
        IGetDataFromAPI getDataFromAPI = new GetDataFromAPI();

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
            if(!string.IsNullOrEmpty(AddressEmail) && !string.IsNullOrEmpty(PassEmail))
            {
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
                        if (notifications != null && notifications.Count > 0)
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
            else
            {
                log.Error("Lỗi Không có cấu hình Email để gửi thông báo!");
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
            if (string.IsNullOrEmpty(NotifiBCT))
                NotifiBCT = "1";
            if (!string.IsNullOrEmpty(AddressEmail) && !string.IsNullOrEmpty(PassEmail))
            {
                var currentDate = DateTime.Now;
                if (NotifiBCT == currentDate.Day.ToString() && currentDate.Hour == 1)
                {
                    // Lấy danh sách người dùng có email
                    var users = userDA.GetAllUserByEmailNotNull();
                    if (users != null && users.Count > 0)
                    {
                        var listFileReport = getDataFromAPI.GetListFileReport(apiCreateFileReport+ "api/ExportReportFile/CreateFileReportMonth").Result;
                        if(listFileReport != null && listFileReport.Count > 0)
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
                            foreach (var FileReport in listFileReport)
                            {
                                try
                                {
                                    using (MailMessage mail = new MailMessage())
                                    {
                                        mail.From = new MailAddress(AddressEmail);
                                        mail.To.Add(FileReport.Email);
                                        mail.Subject = subject;
                                        mail.Body = body;
                                        mail.IsBodyHtml = true;
                                        mail.Attachments.Add(new Attachment(FileReport.PathFileExcel));

                                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                        {
                                            smtp.Credentials = new NetworkCredential(AddressEmail, PassEmail);
                                            smtp.EnableSsl = true;
                                            smtp.Send(mail);
                                        }
                                    }

                                    log.Info("Gửi email báo cáo tháng(" + FileReport.Email + ") thành công!");
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Lỗi gửi email báo cáo tháng(" + FileReport.Email + "): " + ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                log.Error("Lỗi Không có cấu hình Email để gửi báo báo!");
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
            if (string.IsNullOrEmpty(NotifiBCQ))
                NotifiBCQ = "1";
            if (!string.IsNullOrEmpty(AddressEmail) && !string.IsNullOrEmpty(PassEmail))
            {
                var currentDate = DateTime.Now;
                if (NotifiBCQ == currentDate.Day.ToString() && currentDate.Hour == 1 &&
                    (currentDate.Month == 1 || currentDate.Month == 4 || currentDate.Month == 7 || currentDate.Month == 10))
                {
                    // Lấy danh sách người dùng có email
                    var users = userDA.GetAllUserByEmailNotNull();
                    if (users != null && users.Count > 0)
                    {
                        var listFileReport = getDataFromAPI.GetListFileReport(apiCreateFileReport + "api/ExportReportFile/CreateFileReportQuater").Result;
                        if (listFileReport != null && listFileReport.Count > 0)
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
                            foreach (var FileReport in listFileReport)
                            {
                                try
                                {
                                    using (MailMessage mail = new MailMessage())
                                    {
                                        mail.From = new MailAddress(AddressEmail);
                                        mail.To.Add(FileReport.Email);
                                        mail.Subject = subject;
                                        mail.Body = body;
                                        mail.IsBodyHtml = true;
                                        mail.Attachments.Add(new Attachment(FileReport.PathFileExcel));

                                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                        {
                                            smtp.Credentials = new NetworkCredential(AddressEmail, PassEmail);
                                            smtp.EnableSsl = true;
                                            smtp.Send(mail);
                                        }
                                    }

                                    log.Info("Gửi email báo cáo quý(" + FileReport.Email + ") thành công!");
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Lỗi gửi email báo cáo quý(" + FileReport.Email + "): " + ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                log.Error("Lỗi Không có cấu hình Email để gửi báo báo!");
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
            if (string.IsNullOrEmpty(NotifiBCQ))
                NotifiBCQ = "1";
            if (!string.IsNullOrEmpty(AddressEmail) && !string.IsNullOrEmpty(PassEmail))
            {
                var currentDate = DateTime.Now;
                if (NotifiBCQ == currentDate.Day.ToString() && currentDate.Hour == 1 &&
                    (currentDate.Month == 1))
                {
                    // Lấy danh sách người dùng có email
                    var users = userDA.GetAllUserByEmailNotNull();
                    if (users != null && users.Count > 0)
                    {

                        var listFileReport = getDataFromAPI.GetListFileReport(apiCreateFileReport + "api/ExportReportFile/CreateFileReportYear").Result;
                        if (listFileReport != null && listFileReport.Count > 0)
                        {
                            var months = "1,2,3,4,5,6,7,8,9,10,11,12";
                            var year = currentDate.Year - 1;

                            string subject = "Báo cáo năm năm " + year;
                            string body = "";
                            foreach (var FileReport in listFileReport)
                            {
                                try
                                {
                                    using (MailMessage mail = new MailMessage())
                                    {
                                        mail.From = new MailAddress(AddressEmail);
                                        mail.To.Add(FileReport.Email);
                                        mail.Subject = subject;
                                        mail.Body = body;
                                        mail.IsBodyHtml = true;
                                        mail.Attachments.Add(new Attachment(FileReport.PathFileExcel));

                                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                                        {
                                            smtp.Credentials = new NetworkCredential(AddressEmail, PassEmail);
                                            smtp.EnableSsl = true;
                                            smtp.Send(mail);
                                        }
                                    }

                                    log.Info("Gửi email báo cáo năm(" + FileReport.Email + ") thành công!");
                                }
                                catch (Exception ex)
                                {
                                    log.Error("Lỗi gửi email báo cáo năm(" + FileReport.Email + "): " + ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                log.Error("Lỗi Không có cấu hình Email để gửi báo báo!");
            }
            
        }
    }
}