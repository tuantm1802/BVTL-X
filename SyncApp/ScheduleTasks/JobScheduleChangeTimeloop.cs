using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using SyncBVTL.Push.Jobs.PAJobs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Model.ModelExtend.API;
using Common.Common;
using System.Collections.Specialized;
using Model.Model;
using Data.Admin;
using Data.InterfaceDA.Admin;
using System;
using log4net;

namespace SyncBVTL.Push.ScheduleTasks
{
    public static class JobScheduleChangeTimeloop
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JobScheduleChangeTimeloop));
        static ISysLogDA _sysLogDA = new SysLogDA();

        public static async Task ChangeTimeloop(ProcessModel item)
        {
            if (item == null || string.IsNullOrEmpty(item.ReportId)) return;

            try
            {
                IScheduler scheduler = JobScheduler.Scheduler;
                if (scheduler == null)
                {
                    NameValueCollection properties = new NameValueCollection();
                    properties["quartz.threadPool.threadCount"] = "100";
                    ISchedulerFactory sf = new StdSchedulerFactory(properties);
                    scheduler = await sf.GetScheduler();
                    await scheduler.Start();
                    JobScheduler.Scheduler = scheduler;
                }

                JobKey jobKey = new JobKey(item.ReportId + "_Job");
                TriggerKey triggerKey = new TriggerKey("trigger_" + item.ReportId + "Job");

                // Xóa Trigger và Job cũ nếu đã có
                if (await scheduler.CheckExists(triggerKey))
                {
                    await scheduler.UnscheduleJob(triggerKey);
                }
                if (await scheduler.CheckExists(jobKey))
                {
                    await scheduler.DeleteJob(jobKey);
                }

                // Nếu Active == true, khởi tạo lại Job và Trigger mới theo TimeLoop mới
                if (item.Active)
                {
                    IJobDetail jobDetail = JobBuilder.Create<GetDataAPIJob>().WithIdentity(jobKey).Build();
                    // Gán ProcessModel dữ liệu vào JobDataMap
                    jobDetail.JobDataMap["Data"] = item;

                    ITrigger trigger;
                    if (item.TimeLoop <= 23 && item.TimeLoop >= 0)
                    {
                        // Chạy theo giờ cố định mỗi ngày
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(triggerKey)
                            .StartNow()
                            .WithCronSchedule($"0 0 {item.TimeLoop} * * ?")
                            .Build();
                    }
                    else
                    {
                        // Chạy lặp theo khoảng thời gian (giây)
                        trigger = TriggerBuilder.Create()
                            .WithIdentity(triggerKey)
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithIntervalInSeconds(item.TimeLoop)
                                .RepeatForever())
                            .Build();
                    }

                    await scheduler.ScheduleJob(jobDetail, trigger);
                }

                var sysLog = new BVTL_QT_LOG
                {
                    ControllerName = "JobScheduleChangeTimeloop",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Gọi hàm ChangeTimeloop | " + item.Code + " | ReportId: " + item.ReportId + " | TimeLoop: " + item.TimeLoop + "s | Active: " + item.Active
                };
                _sysLogDA.Add(sysLog);
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi trong ChangeTimeloop cho item {item.Code} (ReportId: {item.ReportId}): {ex.Message}", ex);
                throw;
            }
        }

        public static IJobDetail CreateJob(string code, string reportId)
        {
            return JobBuilder.Create<GetDataAPIJob>().WithIdentity(reportId + "_Job").Build();
        }

        public static async Task<List<IJobDetail>> GetJobsAsync(IScheduler scheduler)
        {
            List<IJobDetail> jobs = new List<IJobDetail>();
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());
            foreach (JobKey jobKey in jobKeys)
            {
                jobs.Add(await scheduler.GetJobDetail(jobKey));
            }

            return jobs;
        }
    }
}