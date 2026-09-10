using System;

namespace Model.ModelExtend.Report
{
    public class ExportedReportLogModel
    {
        public long Id { get; set; }
        public string ReportType { get; set; }
        public string ReportName { get; set; }
        public string PeriodType { get; set; }
        public string PeriodValue { get; set; }
        public int Year { get; set; }
        public int? Month { get; set; }
        public string MaDuAn { get; set; }
        public string CityCode { get; set; }
        public string MaNhom { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSizeKb { get; set; }
        public int TotalRecords { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public int ExecutionTimeMs { get; set; }
        public bool TelegramSent { get; set; }
        public string TriggerType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        // Helper properties for display
        public string CreatedDateFormatted => CreatedDate.ToString("dd/MM/yyyy HH:mm:ss");
        public string FileSizeFormatted => FileSizeKb > 1024 ? $"{(FileSizeKb / 1024.0):F2} MB" : $"{FileSizeKb} KB";
    }

    public class ScheduledReportSettingModel
    {
        public int RunDay { get; set; } = 5;
        public int RunHour { get; set; } = 8;
        public bool IsActive { get; set; } = true;
        public string TelegramChatId { get; set; } = "589101034";
        public DateTime? LastRunTime { get; set; }
        public DateTime? NextRunTime { get; set; }
        public int TotalExportedFiles { get; set; }
    }

    public class ReportExportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSizeBytes { get; set; }
        public int TotalItems { get; set; }
        public int ExecutionTimeMs { get; set; }
    }
}
