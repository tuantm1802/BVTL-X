using Model.ModelExtend.Report;
using System.Collections.Generic;

namespace Data.InterfaceDA.Admin
{
    public interface IScheduledReportDA
    {
        long AddExportLog(ExportedReportLogModel model);
        List<ExportedReportLogModel> GetExportLogs(string reportType, int? year, int? month, int pageIndex, int pageSize, out int totalRows);
        ExportedReportLogModel GetById(long id);
        ScheduledReportSettingModel GetSettings();
        bool UpdateSettings(int runDay, int runHour, bool isActive);
        bool CheckDataAvailability(string reportType, int year, int month);
    }
}
