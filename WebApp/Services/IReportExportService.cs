using Model.ModelExtend.Report;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IReportExportService
    {
        Task<List<ReportExportResult>> ExecuteAllMonthlyReportsAsync(int year, int month, string triggerType = "AutoSchedule", string createdBy = "QuartzScheduler", string format = "EXCEL");
        Task<ReportExportResult> ExportTCVCD45ZipAsync(int year, int month, string cityCode = null, string maNhom = null, string triggerType = "Manual", string createdBy = "User", string periodType = "Month", string customFromDate = null, string customToDate = null, string periodValue = null, string format = "EXCEL");
        Task<ReportExportResult> ExportHoatDongCD45ExcelAsync(int year, int month, string cityCode = null, string maNhom = null, string triggerType = "Manual", string createdBy = "User", string periodType = "Month", string customFromDate = null, string customToDate = null, string periodValue = null, string format = "EXCEL");
        Task<ReportExportResult> ExportBacSiCD45ExcelAsync(int year, int month, string cityCode = null, string triggerType = "Manual", string createdBy = "User", string periodType = "Month", string customFromDate = null, string customToDate = null, string periodValue = null, string format = "EXCEL");
        Task<ReportExportResult> ExportTongHopBVTLExcelAsync(int year, int month, string triggerType = "Manual", string createdBy = "User");
    }
}
