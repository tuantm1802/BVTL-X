using Common;
using Data.Admin;
using Data.API;
using Data.InterfaceDA.Admin;
using Data.InterfaceDA.API;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.API;
using Model.ModelExtend.Report;
using Model.ModelExtend.User;
using Newtonsoft.Json;
using SyncBVTL.Push.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        public ActionResult Index()
        {
            return View();
        }

        public async Task SendNotification()
        {
            // Lấy danh sách người dùng có email
            var users = userDA.GetAllUserByEmailNotNull();
            if (users != null && users.Count > 0)
            {
                var notificationSearch = new ReportSearchModel();
                var notifications = new List<NotificationModel>();
                var fromAddress = new MailAddress("from@gmail.com", "From Name");
                string fromPassword = "fromPassword";
                var toAddress = new MailAddress("to@example.com", "To Name");

                string subject = "Danh sách khách hàng sắp đến hẹn khám lại";
                string body = "<div class=\"container\"><h2>Danh sách khách hàng sắp đến hẹn khám lại</h2> " +
                    "<style>" +
                    ".container {background-color:#E7E9EB; width: 100%;overflow: auto;position: absolute;top: 144px;bottom: 0;height: auto;}"+
                    ".container.horizontal{min-height:200px; margin-left:0;}"+
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
                var smtp = new SmtpClient();
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
                        toAddress = new MailAddress(user.Email, user.Name);
                        body += "</table></div>";
                        try
                        {
                            smtp = new SmtpClient
                            {
                                Host = "smtp.gmail.com",
                                Port = 587,
                                EnableSsl = true,
                                DeliveryMethod = SmtpDeliveryMethod.Network,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),

                            };
                            using (var message = new MailMessage(fromAddress, toAddress)
                            {
                                Subject = subject,
                                Body = body,
                                IsBodyHtml = true
                            })
                            {
                                smtp.Send(message);
                            }
                            log.Info("Gửi email notification(" + user.Email + ") thành công!");
                        }
                        catch (Exception ex) {
                            log.Error("Lỗi gửi email notification("+ user.Email + "): "+ex.Message);
                        }
                        
                    }
                }

            }
        }

    }
}