using log4net;
using Model.ModelExtend.API;
using Quartz;
using Quartz.Impl;
using SyncBVTL.Push.Controllers.PA;
using SyncBVTL.Push.Services;
using SyncBVTL.Push.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SyncBVTL.Push.Jobs.PAJobs
{
    public class UpdateAllApiJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var data = (ProcessModel)dataMap["Data"];

            if (DateTime.Now.Hour == 0)
            {
                log.Info("************************Bắt đầu cập nhật lại các job theo DB************************");

                log.Info("************************Bắt đầu xóa tất cả job************************");
                // Xóa hết các job 
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
                await scheduler.Start();

                await scheduler.DeleteJob(new JobKey(data.ReportId + "_Job"));
                //var currentlyExecuting = scheduler.GetCurrentlyExecutingJobs().Result;
                //foreach (var job in currentlyExecuting)
                //{
                //    if (!string.Equals(job.JobDetail.JobType.Name, data.ReportId + "Job"))
                //    {
                //        await scheduler.UnscheduleJob(job.Trigger.Key);
                //        await scheduler.DeleteJob(job.JobDetail.Key);
                //        break;
                //    }
                //}

                log.Info("************************Kết thúc xóa tất cả job************************");

                // Tạo lại các job
                ProcessService processService = new ProcessService();
                List<ProcessModel> processModels = processService.GetListProcess();
                #region Các job thực thi các tiến trình đồng bộ dữ liệu

                log.Info("************************Bắt đầu tạo lại tất cả job: "+ processModels.Count + "************************");
                int apiActiveCount = 0;
                foreach (ProcessModel item in processModels)
                {
                    if (item.Active)
                    {
                        ++ apiActiveCount;
                        IJobDetail job_GetDataAPIJob = JobBuilder.Create<GetDataAPIJob>().Build();
                        job_GetDataAPIJob.JobDataMap["Data"] = item;
                        ITrigger trigger_GetDataAPIJob = TriggerBuilder.Create()
                            .WithIdentity("trigger_" + item.TableNames + "Job")
                            .StartNow()
                            .WithSimpleSchedule(x => x
                                .WithIntervalInSeconds(item.TimeLoop)
                                .RepeatForever())
                            .Build();
                        _ = scheduler.ScheduleJob(job_GetDataAPIJob, trigger_GetDataAPIJob);
                    }

                }
                #endregion
                log.Info("************************Kết thúc tạo lại tất cả job, API Active:"+ apiActiveCount + "************************");
                string logPath = "Log\\" + data.ReportId + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                if (!Directory.Exists("Log"))
                {
                    Directory.CreateDirectory("Log");
                }

                if (!File.Exists(logPath))
                {
                    File.Create(logPath);
                }
                log.Info("************************Kết thúc cập nhật lại các job theo DB************************");
            }

        }
    }
}