using Data.Admin;
using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend.API;
using Quartz;
using Quartz.Impl;
using SyncBVTL.Push.Jobs.PAJobs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SyncBVTL.Push.ScheduleTasks
{
    public class JobScheduleSingle
    {
        static ISysLogDA _sysLogDA = new SysLogDA();

        public static async Task StartSingle(ProcessModel item)
        {
            var log = new BVTL_QT_LOG
            {
                ControllerName = "JobScheduleSingle",
                UserName = "",
                DateLog = DateTime.Now,
                Content = "Gọi hàm SyncBVTL.Push.ScheduleTasks.StartSingle | GetDataAPIJob"
            };
            _sysLogDA.Add(log);

            NameValueCollection properties = new NameValueCollection();
            properties["quartz.threadPool.threadCount"] = "100";

            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler scheduler = sf.GetScheduler().Result;

            //IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

            var currentlyExecuting = scheduler.GetCurrentlyExecutingJobs().Result;
            if (item.Active)
            {
                IJobDetail job_GetDataAPIJob = JobBuilder.Create<GetDataAPIJob>().WithIdentity(item.ReportId + "_Job").Build();
                ITrigger trigger_GetDataAPIJob = TriggerBuilder.Create()
                    .WithIdentity("trigger_" + item.ReportId + "Job")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(item.TimeLoop)
                        .RepeatForever())
                    .Build();
                scheduler.ScheduleJob(job_GetDataAPIJob, trigger_GetDataAPIJob).ConfigureAwait(true);
            }
            else
            {
                await scheduler.DeleteJob(new JobKey( item.ReportId + "_Job" ));

                //foreach (var job in currentlyExecuting)
                //{
                //    if (string.Equals(job.JobDetail.JobType.Name, item.ReportId + "_Job"))
                //    {
                //        await scheduler.UnscheduleJob(job.Trigger.Key);
                //        await scheduler.DeleteJob(job.JobDetail.Key);
                //        break;
                //    }
                //}

            }
        }
    }
}