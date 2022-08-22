using Quartz;
using Quartz.Impl;
using SyncBVTL.Push.Jobs.PAJobs;
using SyncBVTL.Push.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SyncBVTL.Push.Utils.Constants;

namespace SyncBVTL.Push.ScheduleTasks
{
    public class JobScheduleSingle
    {
        public static void StartSingle(ProcessModel item)
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            var currentlyExecuting = scheduler.GetCurrentlyExecutingJobs().Result;
            if (item.Active)
            {
                IJobDetail job_GetDataAPIJob = JobBuilder.Create<GetDataAPIJob>().Build();
                ITrigger trigger_GetDataAPIJob = TriggerBuilder.Create()
                    .WithIdentity("trigger_" + item.TableNames + "Job")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(item.TimeLoop)
                        .RepeatForever())
                    .Build();
                scheduler.ScheduleJob(job_GetDataAPIJob, trigger_GetDataAPIJob);
            }
            else
            {
                foreach (var job in currentlyExecuting)
                {
                    if (string.Equals(job.JobDetail.JobType.Name, item.TableNames + "Job"))
                    {
                        scheduler.DeleteJob(job.JobDetail.Key);
                        break;
                    }
                }

            }
        }
    }
}