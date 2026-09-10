using Data.Admin;
using Data.InterfaceDA.Admin;
using log4net;
using Quartz;
using System;
using System.Threading.Tasks;

namespace WebApp.Services.Jobs
{
    [DisallowConcurrentExecution]
    public class PeriodicReportExportJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IReportExportService _reportExportService;
        private readonly IScheduledReportDA _scheduledReportDA;

        public PeriodicReportExportJob()
        {
            _scheduledReportDA = new ScheduledReportDA();
            _reportExportService = new ReportExportService(_scheduledReportDA);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                log.Info("[PeriodicReportExportJob] Bắt đầu kích hoạt kiểm tra và xuất báo cáo tự động định kỳ...");
                var settings = _scheduledReportDA.GetSettings();

                if (!settings.IsActive)
                {
                    log.Info("[PeriodicReportExportJob] Tính năng tự động xuất báo cáo đang TẮT (AutoExportReport_Active = 0). Bỏ qua thực thi.");
                    return;
                }

                // Tính toán tháng cần xuất: Xuất số liệu của tháng trước
                var now = DateTime.Now;
                int targetMonth = now.Month == 1 ? 12 : now.Month - 1;
                int targetYear = now.Month == 1 ? now.Year - 1 : now.Year;

                log.Info($"[PeriodicReportExportJob] Mục tiêu xuất báo cáo: Tháng {targetMonth:D2}/{targetYear}...");

                var results = await _reportExportService.ExecuteAllMonthlyReportsAsync(targetYear, targetMonth, "AutoSchedule", "QuartzScheduler");
                log.Info($"[PeriodicReportExportJob] Hoàn thành job xuất báo cáo tháng {targetMonth:D2}/{targetYear}. Số báo cáo đã xử lý: {results.Count}");
            }
            catch (Exception ex)
            {
                log.Error("[PeriodicReportExportJob] Lỗi không xử lý được trong quá trình chạy Job: " + ex.Message, ex);
            }
        }
    }
}
