using Data.Admin;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend.API;
using Quartz;
using Quartz.Impl;
using WebApp.Services.Jobs;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace WebApp.Services.ScheduleTasks
{
    public static class JobScheduleChangeTimeloop
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JobScheduleChangeTimeloop));
        private static readonly ISysLogDA _sysLogDA = new SysLogDA();

        public static async Task ChangeTimeloop(ProcessModel item)
        {
            if (item == null || string.IsNullOrEmpty(item.ReportId)) return;

            try
            {
                IScheduler scheduler = JobScheduler.Scheduler;
                if (scheduler == null || scheduler.IsShutdown)
                {
                    NameValueCollection properties = new NameValueCollection();
                    properties["quartz.threadPool.threadCount"] = "50";
                    ISchedulerFactory sf = new StdSchedulerFactory(properties);
                    scheduler = await sf.GetScheduler();
                    if (!scheduler.IsStarted)
                    {
                        await scheduler.Start();
                    }
                    JobScheduler.Scheduler = scheduler;
                }

                JobKey jobKey = new JobKey(item.ReportId + "_Job", "AutoSyncGroup");
                TriggerKey triggerKey = new TriggerKey("trigger_" + item.ReportId + "Job", "AutoSyncGroup");

                // Xóa Trigger và Job cũ nếu đã có
                if (await scheduler.CheckExists(triggerKey))
                {
                    await scheduler.UnscheduleJob(triggerKey);
                }
                if (await scheduler.CheckExists(jobKey))
                {
                    await scheduler.DeleteJob(jobKey);
                }

                // Nếu Active == true, khởi tạo lại Job và Trigger mới
                if (item.Active)
                {
                    IJobDetail jobDetail = JobBuilder.Create<GetDataAPIJob>()
                        .WithIdentity(jobKey)
                        .Build();
                    jobDetail.JobDataMap["Data"] = item;

                    ITrigger trigger;
                    if (item.TimeLoop <= 23 && item.TimeLoop >= 0)
                    {
                        // Chạy theo giờ cố định hàng ngày
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(triggerKey)
                            .StartNow()
                            .WithCronSchedule($"0 0 {item.TimeLoop} * * ?")
                            .Build();
                    }
                    else
                    {
                        // Chạy lặp theo khoảng thời gian (giây)
                        int interval = item.TimeLoop >= 30 ? item.TimeLoop : 60;
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(triggerKey)
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithIntervalInSeconds(interval)
                                .RepeatForever())
                            .Build();
                    }

                    await scheduler.ScheduleJob(jobDetail, trigger);
                }

                var sysLog = new BVTL_QT_LOG
                {
                    ControllerName = "JobScheduleChangeTimeloop",
                    UserName = "ADMIN",
                    DateLog = DateTime.Now,
                    Content = $"Cập nhật lịch đồng bộ cho API [{item.ReportId} - {item.Code}]: TimeLoop = {item.TimeLoop}s, Active = {item.Active}"
                };
                _sysLogDA.Add(sysLog);
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi trong ChangeTimeloop cho item {item.Code} (ReportId: {item.ReportId}): {ex.Message}", ex);
                throw;
            }
        }
    }
}
