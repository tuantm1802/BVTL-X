using Model.ModelExtend.API;
using Quartz;
using Quartz.Impl;
using SyncBVTL.Push.Jobs.PAJobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncBVTL.Push.ScheduleTasks
{
    public class JobScheduleSingle
    {
        public static async Task StartSingle(ProcessModel item)
        {

            
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

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