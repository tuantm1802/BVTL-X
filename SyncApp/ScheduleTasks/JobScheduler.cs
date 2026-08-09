using Data.Admin;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend.API;
using Quartz;
using Quartz.Impl;
using SyncBVTL.Push.Jobs;
using SyncBVTL.Push.Jobs.PAJobs;
using SyncBVTL.Push.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SyncBVTL.Push.ScheduleTasks
{
    public class JobScheduler
    {
        static string logDirectory = ConfigurationManager.AppSettings.Get("LogDirectory");
        static ISysLogDA _sysLogDA = new SysLogDA();
        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IScheduler Scheduler { get; set; }

        public static async Task StartAll()
        {
            if (!string.IsNullOrEmpty(logDirectory))
            {
                var directory = logDirectory + "\\Sync-log";
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                if (Directory.GetCurrentDirectory() != directory)
                {
                    Directory.SetCurrentDirectory(logDirectory);

                    if (!Directory.Exists("Sync-log"))
                    {
                        Directory.CreateDirectory("Sync-log");
                    }
                    Directory.SetCurrentDirectory(directory);
                }
            }

            ProcessService processService = new ProcessService();
            List<ProcessModel> processModels = processService.GetListProcess();
           
            NameValueCollection properties = new NameValueCollection();
            properties["quartz.threadPool.threadCount"] = "100";

            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            Scheduler = sf.GetScheduler().Result;

            // Xóa hết các job cũ
            var currentlyExecuting = Scheduler.GetCurrentlyExecutingJobs().Result;

            foreach (var job in currentlyExecuting)
            {
                await Scheduler.UnscheduleJob(job.Trigger.Key);
                await Scheduler.DeleteJob(job.JobDetail.Key);
            }

            await Scheduler.Start();

            var log = new BVTL_QT_LOG
            {
                ControllerName = "JobScheduler",
                UserName = "",
                DateLog = DateTime.Now,
                Content = "Gọi hàm SyncBVTL.Push.ScheduleTasks.JobScheduler.StartAll | GetDataAPIJob"
            };
            _sysLogDA.Add(log);
            

            #region Các job thực thi các tiến trình đồng bộ dữ liệu
            foreach (ProcessModel item in processModels)
            {
                if (item.Active)
                {
                    IJobDetail job_GetDataAPIJob = JobBuilder.Create<GetDataAPIJob>().WithIdentity(item.ReportId + "_Job").Build();
                    job_GetDataAPIJob.JobDataMap["Data"] = item;
                    ITrigger trigger_GetDataAPIJob;
                    if (item.TimeLoop <= 23 && item.TimeLoop >= 0)
                    {
                        // Chạy theo giờ cố định mỗi ngày (ví dụ: TimeLoop = 23 -> chạy lúc 23:00)
                        trigger_GetDataAPIJob = TriggerBuilder.Create()
                            .WithIdentity("trigger_" + item.ReportId + "Job")
                            .StartNow()
                            .WithCronSchedule($"0 0 {item.TimeLoop} * * ?")
                            .Build();
                    }
                    else
                    {
                        // Chạy lặp lại theo khoảng thời gian giây
                        trigger_GetDataAPIJob = TriggerBuilder.Create()
                            .WithIdentity("trigger_" + item.ReportId + "Job")
                            .StartNow()
                            .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(item.TimeLoop)
                            .RepeatForever())
                            .Build();
                    }
                    await Scheduler.ScheduleJob(job_GetDataAPIJob, trigger_GetDataAPIJob);
                }

            }
            
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //await SendTelegramMessage("589101034", "🎉 Tất cả các job đã được khởi chạy thành công!");


            // Gửi thông báo qua Telegram - https://t.me/SCDISyncBot
            //https://api.telegram.org/bot7553997923:AAFBabzEfRLdluri42vy3VixZwrLfv2BHPs/getUpdates
            
            string botToken = "7553997923:AAFBabzEfRLdluri42vy3VixZwrLfv2BHPs";
            string chatId = "589101034";
            string chatId1 = "-1002496745464";
            //string chatId2 = "-4734041041";
            TelegramNotifier notifier = new TelegramNotifier(botToken, chatId);
            TelegramNotifier notifier1 = new TelegramNotifier(botToken, chatId1);
            //TelegramNotifier notifier2 = new TelegramNotifier(botToken, chatId2);

            await notifier.SendMessageAsync("🎉 Tất cả các job đã được khởi chạy thành công! [" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][JobScheduler:StartAll]");
            await notifier1.SendMessageAsync("🎉 Tất cả các job đã được khởi chạy thành công! [" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][JobScheduler:StartAll]");
            //await notifier2.SendMessageAsync("🎉 Tất cả các job đã được khởi chạy thành công! [" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "][JobScheduler:StartAll]");

            #endregion
        }

        public static async Task SendTelegramMessage(string chatId, string message)
        {
            string botToken = "7553997923:AAFBabzEfRLdluri42vy3VixZwrLfv2BHPs";
            string url = $"https://api.telegram.org/bot{botToken}/sendMessage";
            try { 
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30); // Set timeout
                    var parameters = new Dictionary<string, string>
                    {
                        { "chat_id", chatId },
                        { "text", message }
                    };

                    var content = new FormUrlEncodedContent(parameters);
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    //HttpResponseMessage response = await client.PostAsync(url, content);
                    HttpResponseMessage _response = await client.PostAsync(url, content).ConfigureAwait(false);

                    if (!_response.IsSuccessStatusCode)
                    {
                        string error = await _response.Content.ReadAsStringAsync();
                        throw new Exception($"Error sending Telegram message: {error}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception [JobScheduler]: {ex.Message}\n ");

                Console.WriteLine($"Exception: {ex.Message}");
                // Log thêm stack trace nếu cần
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }


    }
}