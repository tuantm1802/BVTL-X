using Model.ModelExtend.API;
using Quartz;
using Quartz.Impl;
using SyncBVTL.Push.Jobs;
using SyncBVTL.Push.Jobs.PAJobs;
using SyncBVTL.Push.Services;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace SyncBVTL.Push.ScheduleTasks
{
    public class JobScheduler
    {
        static string logDirectory = ConfigurationManager.AppSettings.Get("LogDirectory");

        public static void StartAllAsync()
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

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
            scheduler.Start();

            ////Job tự động kiểm tra trạng thái kết nối với đầu api
            //IJobDetail job_CheckConnectionStatus = JobBuilder.Create<CheckConnectAPIJob>().Build();
            //ITrigger trigger_CheckConnectionStatus = TriggerBuilder.Create()
            //    .StartNow()
            //    .WithCronSchedule("0 0/1 * * * ?") //Tự động chạy sau mỗi 1 phút
            //    .Build();
            //scheduler.ScheduleJob(job_CheckConnectionStatus, trigger_CheckConnectionStatus);

            //Job tự động cập nhật các đầu api
            IJobDetail job_UpdateJob = JobBuilder.Create<UpdateAllApiJob>().WithIdentity("UpdateApiJob").Build();
            job_UpdateJob.JobDataMap["Data"] = new ProcessModel { TableNames = new List<string>() { "UpdateApi" } };
            ITrigger trigger_UpdateJob = TriggerBuilder.Create()
                .WithIdentity("trigger_UpdateApiJob")
                .StartNow()
                .WithCronSchedule("0 0-1 * * * ?") //Tự động chạy sau mỗi 60 phút
                .Build();
            scheduler.ScheduleJob(job_UpdateJob, trigger_UpdateJob).ConfigureAwait(true);

            #region Các job thực thi các tiến trình đồng bộ dữ liệu
            foreach (ProcessModel item in processModels)
            {
                if (item.Active)
                {
                    IJobDetail job_GetDataAPIJob = JobBuilder.Create<GetDataAPIJob>().WithIdentity(item.ReportId+"_Job").Build();
                    //job_GetDataAPIJob.JobDataMap["Token"] = item.Token;
                    //job_GetDataAPIJob.JobDataMap["Url"] = item.Url;
                    //job_GetDataAPIJob.JobDataMap["TableName"] = item.TableName;
                    //job_GetDataAPIJob.JobDataMap["ReportId"] = item.ReportId;
                    job_GetDataAPIJob.JobDataMap["Data"] = item;
                    ITrigger trigger_GetDataAPIJob = TriggerBuilder.Create()
                        .WithIdentity("trigger_"+ item.ReportId + "Job")
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(item.TimeLoop)
                            .RepeatForever())
                        .Build();
                    scheduler.ScheduleJob(job_GetDataAPIJob, trigger_GetDataAPIJob).ConfigureAwait(true);
                }

                //switch (item.Code)
                //{
                //    case ProcessCode.GetDataFromAPI:
                //        if (item.Active)
                //        {
                //            IJobDetail job_GetDataAPIJob = JobBuilder.Create<GetDataAPIJob>().Build();
                //            job_GetDataAPIJob.JobDataMap["logPath"] = ProcessCode.GetDataFromAPI;
                //            ITrigger trigger_GetDataAPIJob = TriggerBuilder.Create()
                //                .WithIdentity("trigger_GetDataAPIJob")
                //                .StartNow()
                //                .WithSimpleSchedule(x => x
                //                    .WithIntervalInSeconds(item.TimeLoop)
                //                    .RepeatForever())
                //                .Build();
                //            scheduler.ScheduleJob(job_GetDataAPIJob, trigger_GetDataAPIJob);
                //            break;
                //        }
                //        else
                //        {
                //            break;
                //        }

                //    default:
                //        break;
                //}
            }
            #endregion
        }

    }
}