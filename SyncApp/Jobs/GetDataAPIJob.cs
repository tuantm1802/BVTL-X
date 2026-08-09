using log4net;
using Model.ModelExtend.API;
using Quartz;
using SyncBVTL.Push.Controllers.PA;
using SyncBVTL.Push.Utils;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SyncBVTL.Push.Jobs.PAJobs
{
    public class GetDataAPIJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GetDataAPIJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                var controller = DependencyResolver.Current.GetService<SyncDataController>() ?? new SyncDataController();

                var data = (ProcessModel)dataMap["Data"];
                if (data != null && data.Active)
                {
                    var result = await controller.GetDataFromAPI(data);
                    
                    var notifier = new TelegramNotifier();

                    await notifier.SendMessageAsync("🎉 API " + data.ReportId + " (" + data.Code + ") đã hoàn thành đồng bộ. KQ [" + result.Success + "][" + result.Message + "]! [GetDataAPIJob:Execute]");

                    string logFileName = (string.IsNullOrEmpty(data.Code) ? "" : data.Code + "_") + data.ReportId + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
                    string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                    if (!Directory.Exists(logDir))
                    {
                        Directory.CreateDirectory(logDir);
                    }
                    string logPath = Path.Combine(logDir, logFileName);

                    string logContent = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {(result.Success ? "SUCCESS" : "ERROR")}: {result.Message}\r\n";
                    try
                    {
                        using (var file = new FileStream(logPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            file.Prepend(logContent);
                        }
                    }
                    catch (Exception exFile)
                    {
                        log.Warn($"Không thể ghi log file {logPath}: {exFile.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception [GetDataAPIJob:Execute]: {ex.Message}", ex);
            }
        }
    }
}