using Common;
using Common.Common;
using Common.ICommon;
using Data.Admin;
using Data.API;
using Data.InterfaceDA.Admin;
using Data.InterfaceDA.API;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.API;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebApp.Services.Jobs
{
    public class GetDataAPIJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GetDataAPIJob));
        private readonly ISyncDataFromApi_SaveToDB _syncService = new SyncDataFromApi_SaveToDB();
        private readonly ISysLogDA _sysLogDA = new SysLogDA();

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                var data = (ProcessModel)dataMap["Data"];

                if (data != null && data.Active)
                {
                    log.Info($"[GetDataAPIJob] Bắt đầu tự động đồng bộ API: {data.ReportId} ({data.Code})...");

                    BaseResult result = await _syncService.GetDataFromApi_SaveToDB(
                        data.Url, 
                        data.Token, 
                        data.ReportId, 
                        data.MaDuAn, 
                        data.TableNames, 
                        data.Code, 
                        data.RawOrLabel
                    );

                    // Ghi sys log
                    var sysLog = new BVTL_QT_LOG
                    {
                        ControllerName = "AutoSyncJob",
                        UserName = "SYSTEM_SCHEDULER",
                        DateLog = DateTime.Now,
                        Content = $"Tự động đồng bộ API [{data.ReportId} - {data.Code}]: {(result.Success ? "Thành công" : "Lỗi - " + result.Message)}"
                    };
                    _sysLogDA.Add(sysLog);

                    // Gửi Telegram
                    try
                    {
                        var notifier = new TelegramNotifier();
                        string tgMsg = $"🤖 [AutoSync] API {data.ReportId} ({data.Code})\nKết quả: {(result.Success ? "✅ Thành công" : "❌ Lỗi: " + result.Message)}\nThời gian: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                        await notifier.SendMessageAsync(tgMsg);
                    }
                    catch (Exception exTg)
                    {
                        log.Warn($"Không thể gửi Telegram: {exTg.Message}");
                    }

                    // Ghi file log
                    try
                    {
                        string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log", "AutoSync");
                        if (!Directory.Exists(logDir))
                        {
                            Directory.CreateDirectory(logDir);
                        }
                        string logFileName = $"Sync_{(string.IsNullOrEmpty(data.Code) ? "API" : data.Code)}_{DateTime.Now:yyyyMMdd}.log";
                        string logPath = Path.Combine(logDir, logFileName);
                        string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{(result.Success ? "SUCCESS" : "ERROR")}] {data.ReportId}: {result.Message}\r\n";
                        File.AppendAllText(logPath, logLine);
                    }
                    catch (Exception exFile)
                    {
                        log.Warn($"Không thể ghi file log: {exFile.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception trong GetDataAPIJob.Execute: {ex.Message}", ex);
            }
        }
    }
}
