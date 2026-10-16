using Data.Admin;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend.API;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using WebApp.Services.Jobs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WebApp.Services.ScheduleTasks
{
    public class JobScheduler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JobScheduler));
        private static readonly ISysLogDA _sysLogDA = new SysLogDA();

        public static IScheduler Scheduler { get; set; }
        public static bool IsAutoSyncEnabled { get; set; } = true;
        public static DateTime? LastStartedTime { get; set; }

        public static async Task StartAll()
        {
            try
            {
                if (!IsAutoSyncEnabled)
                {
                    log.Info("AutoSync is currently DISABLED by configuration.");
                    return;
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                ProcessService processService = new ProcessService();
                List<ProcessModel> processModels = processService.GetListProcess();

                NameValueCollection properties = new NameValueCollection();
                properties["quartz.threadPool.threadCount"] = "50";
                properties["quartz.scheduler.instanceName"] = "WebApp_AutoSyncScheduler";

                ISchedulerFactory sf = new StdSchedulerFactory(properties);
                Scheduler = await sf.GetScheduler();

                if (Scheduler.IsStarted)
                {
                    await Scheduler.Clear();
                }
                else
                {
                    await Scheduler.Start();
                }

                LastStartedTime = DateTime.Now;

                // Lên lịch cho các API active
                // Lên lịch cho các API active
                int scheduledCount = 0;
                foreach (ProcessModel item in processModels)
                {
                    if (item.Active)
                    {
                        try
                        {
                            // Ưu tiên dùng Mã API (Code) duy nhất để tránh xung đột JobKey khi các API có chung ReportId
                            string jobIdentifier = !string.IsNullOrEmpty(item.Code) ? item.Code : item.ReportId;
                            JobKey jobKey = new JobKey(jobIdentifier + "_Job", "AutoSyncGroup");
                            TriggerKey triggerKey = new TriggerKey("trigger_" + jobIdentifier + "Job", "AutoSyncGroup");

                            IJobDetail job = JobBuilder.Create<GetDataAPIJob>()
                                .WithIdentity(jobKey)
                                .Build();
                            job.JobDataMap["Data"] = item;

                            ITrigger trigger;
                            if (item.TimeLoop <= 23 && item.TimeLoop >= 0)
                            {
                                // Chạy theo giờ cố định mỗi ngày (VD: 23 = 23:00)
                                trigger = TriggerBuilder.Create()
                                    .WithIdentity(triggerKey)
                                    .StartNow()
                                    .WithCronSchedule($"0 0 {item.TimeLoop} * * ?")
                                    .Build();
                            }
                            else
                            {
                                // Chạy lặp lại theo giây (hoặc tối thiểu 30s)
                                int interval = item.TimeLoop >= 30 ? item.TimeLoop : 60;
                                trigger = TriggerBuilder.Create()
                                    .WithIdentity(triggerKey)
                                    .StartNow()
                                    .WithSimpleSchedule(x => x
                                        .WithIntervalInSeconds(interval)
                                        .RepeatForever())
                                    .Build();
                            }

                            await Scheduler.ScheduleJob(job, trigger);
                            scheduledCount++;
                        }
                        catch (Exception exItem)
                        {
                            log.Error($"[JobScheduler.StartAll] Lỗi khi đăng ký Job cho API [{item.Code}] (ReportId: {item.ReportId}): {exItem.Message}", exItem);
                        }
                    }
                }

                // Đăng ký Job tự động xuất báo cáo định kỳ (mùng 5 hàng tháng)
                try
                {
                    var scheduledReportDA = new ScheduledReportDA();
                    var settings = scheduledReportDA.GetSettings();

                    IJobDetail job_ExportReport = JobBuilder.Create<PeriodicReportExportJob>()
                        .WithIdentity("Job_PeriodicReportExport", "ReportingGroup")
                        .Build();

                    int day = settings.RunDay > 0 && settings.RunDay <= 28 ? settings.RunDay : 5;
                    int hour = settings.RunHour >= 0 && settings.RunHour <= 23 ? settings.RunHour : 8;
                    string cronExpr = $"0 0 {hour} {day} * ?";

                    ITrigger trigger_ExportReport = TriggerBuilder.Create()
                        .WithIdentity("Trigger_PeriodicReportExport", "ReportingGroup")
                        .WithCronSchedule(cronExpr)
                        .Build();

                    await Scheduler.ScheduleJob(job_ExportReport, trigger_ExportReport);
                    scheduledCount++;
                    log.Info($"Đăng ký PeriodicReportExportJob thành công với Cron: {cronExpr}");
                }
                catch (Exception exReport)
                {
                    log.Error("Lỗi đăng ký PeriodicReportExportJob: " + exReport.Message, exReport);
                }

                var sysLog = new BVTL_QT_LOG
                {
                    ControllerName = "JobScheduler",
                    UserName = "SYSTEM",
                    DateLog = DateTime.Now,
                    Content = $"Khởi động Quartz.NET Scheduler thành công với {scheduledCount} tiến trình tự động (bao gồm xuất báo cáo định kỳ)."
                };
                _sysLogDA.Add(sysLog);
                log.Info($"Quartz Scheduler started with {scheduledCount} jobs.");
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi khởi động JobScheduler.StartAll: {ex.Message}", ex);
            }
        }

        public static async Task Shutdown()
        {
            try
            {
                if (Scheduler != null && !Scheduler.IsShutdown)
                {
                    await Scheduler.Shutdown(true);
                    log.Info("Quartz Scheduler shutdown successfully.");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi Shutdown Scheduler: {ex.Message}", ex);
            }
        }

        public static async Task PauseAll()
        {
            if (Scheduler != null && Scheduler.IsStarted)
            {
                await Scheduler.PauseAll();
                IsAutoSyncEnabled = false;
            }
        }

        public static async Task ResumeAll()
        {
            if (Scheduler != null && Scheduler.IsStarted)
            {
                await Scheduler.ResumeAll();
                IsAutoSyncEnabled = true;
            }
            else
            {
                IsAutoSyncEnabled = true;
                await StartAll();
            }
        }

        public static async Task TriggerJobNow(string key)
        {
            if (Scheduler != null)
            {
                // Thử tìm theo Key chính (Code_Job hoặc ReportId_Job)
                JobKey jobKey = new JobKey(key + "_Job", "AutoSyncGroup");
                if (!await Scheduler.CheckExists(jobKey))
                {
                    jobKey = new JobKey(key, "AutoSyncGroup");
                }

                if (await Scheduler.CheckExists(jobKey))
                {
                    await Scheduler.TriggerJob(jobKey);
                }
                else
                {
                    // Tạo job tạm thời để chạy ngay nếu chưa có trong Scheduler
                    ProcessService processService = new ProcessService();
                    var list = processService.GetListProcess();
                    var item = list.Find(x => x.Code == key || x.ReportId == key);
                    if (item != null)
                    {
                        string jobIdentifier = !string.IsNullOrEmpty(item.Code) ? item.Code : item.ReportId;
                        JobKey tempJobKey = new JobKey(jobIdentifier + "_Job", "AutoSyncGroup");
                        IJobDetail job = JobBuilder.Create<GetDataAPIJob>()
                            .WithIdentity(tempJobKey)
                            .Build();
                        job.JobDataMap["Data"] = item;

                        ITrigger trigger = TriggerBuilder.Create()
                            .WithIdentity("temp_trigger_" + jobIdentifier, "AutoSyncGroup")
                            .StartNow()
                            .Build();

                        await Scheduler.ScheduleJob(job, trigger);
                    }
                }
            }
        }

        public static async Task ReschedulePeriodicReportJob(int runDay, int runHour)
        {
            if (Scheduler != null)
            {
                TriggerKey triggerKey = new TriggerKey("Trigger_PeriodicReportExport", "ReportingGroup");
                JobKey jobKey = new JobKey("Job_PeriodicReportExport", "ReportingGroup");

                int day = runDay > 0 && runDay <= 28 ? runDay : 5;
                int hour = runHour >= 0 && runHour <= 23 ? runHour : 8;
                string cronExpr = $"0 0 {hour} {day} * ?";

                ITrigger newTrigger = TriggerBuilder.Create()
                    .WithIdentity(triggerKey)
                    .WithCronSchedule(cronExpr)
                    .Build();

                if (await Scheduler.CheckExists(triggerKey))
                {
                    await Scheduler.RescheduleJob(triggerKey, newTrigger);
                    log.Info($"Rescheduled PeriodicReportExportJob to Cron: {cronExpr}");
                }
                else
                {
                    IJobDetail job = JobBuilder.Create<PeriodicReportExportJob>()
                        .WithIdentity(jobKey)
                        .Build();
                    await Scheduler.ScheduleJob(job, newTrigger);
                    log.Info($"Created & Scheduled PeriodicReportExportJob to Cron: {cronExpr}");
                }
            }
        }

        public static async Task TriggerExportReportNow()
        {
            if (Scheduler != null)
            {
                JobKey jobKey = new JobKey("Job_PeriodicReportExport", "ReportingGroup");
                if (await Scheduler.CheckExists(jobKey))
                {
                    await Scheduler.TriggerJob(jobKey);
                }
                else
                {
                    IJobDetail job = JobBuilder.Create<PeriodicReportExportJob>()
                        .WithIdentity(jobKey)
                        .Build();

                    ITrigger tempTrigger = TriggerBuilder.Create()
                        .WithIdentity("Temp_Trigger_PeriodicReportExport", "ReportingGroup")
                        .StartNow()
                        .Build();

                    await Scheduler.ScheduleJob(job, tempTrigger);
                }
            }
        }
    }
}
