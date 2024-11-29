using Data.Admin;
using Data.InterfaceDA.Admin;
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
using System.Threading.Tasks;

namespace SyncBVTL.Push.ScheduleTasks
{
    public class JobScheduler
    {
        static string logDirectory = ConfigurationManager.AppSettings.Get("LogDirectory");
        static ISysLogDA _sysLogDA = new SysLogDA();

        public static async Task StartAll()
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

            ProcessService processService = new ProcessService();
            List<ProcessModel> processModels = processService.GetListProcess();
           
            NameValueCollection properties = new NameValueCollection();
            properties["quartz.threadPool.threadCount"] = "100";

            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler scheduler = sf.GetScheduler().Result;

            //IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

            // Xóa hết các job cũ
            var currentlyExecuting = scheduler.GetCurrentlyExecutingJobs().Result;

            foreach (var job in currentlyExecuting)
            {
                await scheduler.UnscheduleJob(job.Trigger.Key);
                await scheduler.DeleteJob(job.JobDetail.Key);
            }

            _ = scheduler.Start();
            //Job tự động cập nhật các đầu api
            /*
            IJobDetail job_UpdateJob = JobBuilder.Create<UpdateAllApiJob>().WithIdentity("UpdateApiJob").Build();
            job_UpdateJob.JobDataMap["Data"] = new ProcessModel { TableNames = new List<string>() { "UpdateApi" } };
            ITrigger trigger_UpdateJob = TriggerBuilder.Create()
                .WithIdentity("trigger_UpdateApiJob")
                .StartNow()
                .WithCronSchedule("0 0-1 * * * ?") //Tự động chạy sau mỗi 60 phút
                .Build();
            _ = scheduler.ScheduleJob(job_UpdateJob, trigger_UpdateJob).ConfigureAwait(true);
            */

            //Job tự động gửi email notification hàng ngày
            //IJobDetail job_NotifiJob = JobBuilder.Create<SendNotificationJob>().WithIdentity("NotifiJob").Build();
            //ITrigger trigger_NotifiJob = TriggerBuilder.Create()
            //    .WithIdentity("trigger_NotifiJob")
            //    .StartNow()
            //   //.WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever())// chạy khi 1 giờ đêm
            //   .WithCronSchedule("0 0/1 * * * ?") //Tự động chạy sau mỗi 1 phút
            //    .Build();
            //_ = scheduler.ScheduleJob(job_NotifiJob, trigger_NotifiJob).ConfigureAwait(true);

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
                    ITrigger trigger_GetDataAPIJob = TriggerBuilder.Create()
                        .WithIdentity("trigger_" + item.ReportId + "Job")
                        .StartNow()
                        .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(item.TimeLoop)
                        .RepeatForever())
                        .Build();
                    _ = scheduler.ScheduleJob(job_GetDataAPIJob, trigger_GetDataAPIJob).ConfigureAwait(true);
                }

            }
            #endregion
        }

    }
}