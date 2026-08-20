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
                int scheduledCount = 0;
                foreach (ProcessModel item in processModels)
                {
                    if (item.Active)
                    {
                        JobKey jobKey = new JobKey(item.ReportId + "_Job", "AutoSyncGroup");
                        TriggerKey triggerKey = new TriggerKey("trigger_" + item.ReportId + "Job", "AutoSyncGroup");

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
                }

                var sysLog = new BVTL_QT_LOG
                {
                    ControllerName = "JobScheduler",
                    UserName = "SYSTEM",
                    DateLog = DateTime.Now,
                    Content = $"Khởi động Quartz.NET Scheduler thành công với {scheduledCount} tiến trình tự động."
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

        public static async Task TriggerJobNow(string reportId)
        {
            if (Scheduler != null)
            {
                JobKey jobKey = new JobKey(reportId + "_Job", "AutoSyncGroup");
                if (await Scheduler.CheckExists(jobKey))
                {
                    await Scheduler.TriggerJob(jobKey);
                }
                else
                {
                    // Tạo job tạm thời để chạy ngay
                    ProcessService processService = new ProcessService();
                    var list = processService.GetListProcess();
                    var item = list.Find(x => x.ReportId == reportId);
                    if (item != null)
                    {
                        IJobDetail job = JobBuilder.Create<GetDataAPIJob>()
                            .WithIdentity(jobKey)
                            .Build();
                        job.JobDataMap["Data"] = item;

                        ITrigger trigger = TriggerBuilder.Create()
                            .WithIdentity("temp_trigger_" + reportId, "AutoSyncGroup")
                            .StartNow()
                            .Build();

                        await Scheduler.ScheduleJob(job, trigger);
                    }
                }
            }
        }
    }
}
