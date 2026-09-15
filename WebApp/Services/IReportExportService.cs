using Model.ModelExtend.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IReportExportService
    {
        Task<List<ReportExportResult>> ExecuteAllMonthlyReportsAsync(int year, int month, string triggerType = "AutoSchedule", string createdBy = "QuartzScheduler");
        Task<ReportExportResult> ExportTCVCD45ZipAsync(int year, int month, string cityCode = null, string maNhom = null, string triggerType = "Manual", string createdBy = "User", string periodType = "Month", string customFromDate = null, string customToDate = null, string periodValue = null);
        Task<ReportExportResult> ExportHoatDongCD45ExcelAsync(int year, int month, string cityCode = null, string maNhom = null, string triggerType = "Manual", string createdBy = "User", string periodType = "Month", string customFromDate = null, string customToDate = null, string periodValue = null);
        Task<ReportExportResult> ExportTongHopBVTLExcelAsync(int year, int month, string triggerType = "Manual", string createdBy = "User");
    }
}
