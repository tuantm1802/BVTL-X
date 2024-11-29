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

namespace SyncBVTL.Push.ScheduleTasks
{
    public static class JobScheduleChangeTimeloop
    {
        static ISysLogDA _sysLogDA = new SysLogDA();

        public static async Task ChangeTimeloop(ProcessModel item)
        {
            NameValueCollection properties = new NameValueCollection();
            properties["quartz.threadPool.threadCount"] = "100";

            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler scheduler = sf.GetScheduler().Result;

            //IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            await scheduler.Start();

            await scheduler.DeleteJob(new JobKey(item.ReportId + "_Job"));
            IJobDetail jobDetail = CreateJob(item.Code, item.ReportId);

            var log = new BVTL_QT_LOG
            {
                ControllerName = "JobScheduleChangeTimeloop",
                UserName = "",
                DateLog = DateTime.Now,
                Content = "Gọi hàm SyncBVTL.Push.JobScheduleChangeTimeloop.ChangeTimeloop | " + item.Code
            };
            _sysLogDA.Add(log);


            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger_" + item.ReportId + "Job")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(item.TimeLoop)
                    .RepeatForever())
                .Build();
            await scheduler.ScheduleJob(jobDetail, trigger).ConfigureAwait(true);

            //var currentlyExecuting = scheduler.GetCurrentlyExecutingJobs().Result;


            //foreach (var job in currentlyExecuting)
            //{
            //    if (string.Equals(job.JobDetail.JobType.Name, item.ReportId + "_Job"))
            //    {
            //        await scheduler.UnscheduleJob(job.Trigger.Key);
            //        await scheduler.DeleteJob(job.JobDetail.Key);

            //        IJobDetail jobDetail = CreateJob(item.Code, item.ReportId);
            //        ITrigger trigger = TriggerBuilder.Create()
            //            .WithIdentity("trigger_" + item.ReportId + "Job")
            //            .StartNow()
            //            .WithSimpleSchedule(x => x
            //                .WithIntervalInSeconds(item.TimeLoop)
            //                .RepeatForever())
            //            .Build();
            //        await scheduler.ScheduleJob(jobDetail, trigger).ConfigureAwait(true);
            //        break;
            //    }
            //}

        }

        public static IJobDetail CreateJob(string code, string reportId)
        {
            switch (code)
            {
                case Constants.GetDataFromAPI:
                    return JobBuilder.Create<GetDataAPIJob>().WithIdentity(reportId + "_Job").Build();

                default:
                    return null;
            }
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