using Data.InterfaceDA.Admin;
using log4net;
using Model.ModelExtend.Report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Data.Admin
{
    public class ScheduledReportDA : IScheduledReportDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString = GetEffectiveConnectionString();

        private static string GetEffectiveConnectionString()
        {
            var connStr = ConfigurationManager.AppSettings["ConnectionString"];
            if (!string.IsNullOrEmpty(connStr)) return connStr;

            var efConn = ConfigurationManager.ConnectionStrings["BVTL_REPORTINGEntities"]?.ConnectionString;
            if (!string.IsNullOrEmpty(efConn))
            {
                try
                {
                    var efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder(efConn);
                    if (!string.IsNullOrEmpty(efBuilder.ProviderConnectionString))
                        return efBuilder.ProviderConnectionString;
                }
                catch { }
            }
            return "Data Source=103.77.167.206;Persist Security Info=True;Initial Catalog=BVTL_REPORTING_DEV;User ID=sa;Password=GLy74MwZ;";
        }

        public long AddExportLog(ExportedReportLogModel model)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string sql = @"
                    INSERT INTO [dbo].[BVTL_EXPORTED_REPORT_LOG]
                    (
                        [ReportType], [ReportName], [PeriodType], [PeriodValue], [Year], [Month],
                        [MaDuAn], [CityCode], [MaNhom], [FileName], [FilePath], [FileSizeKb],
                        [TotalRecords], [Status], [ErrorMessage], [ExecutionTimeMs], [TelegramSent],
                        [TriggerType], [CreatedBy], [CreatedDate]
                    )
                    VALUES
                    (
                        @ReportType, @ReportName, @PeriodType, @PeriodValue, @Year, @Month,
                        @MaDuAn, @CityCode, @MaNhom, @FileName, @FilePath, @FileSizeKb,
                        @TotalRecords, @Status, @ErrorMessage, @ExecutionTimeMs, @TelegramSent,
                        @TriggerType, @CreatedBy, @CreatedDate
                    );
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ReportType", (object)model.ReportType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReportName", (object)model.ReportName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@PeriodType", (object)model.PeriodType ?? "Month");
                        cmd.Parameters.AddWithValue("@PeriodValue", (object)model.PeriodValue ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", model.Year);
                        cmd.Parameters.AddWithValue("@Month", (object)model.Month ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaDuAn", (object)model.MaDuAn ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CityCode", (object)model.CityCode ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@MaNhom", (object)model.MaNhom ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FileName", (object)model.FileName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FilePath", (object)model.FilePath ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FileSizeKb", model.FileSizeKb);
                        cmd.Parameters.AddWithValue("@TotalRecords", model.TotalRecords);
                        cmd.Parameters.AddWithValue("@Status", (object)model.Status ?? "Success");
                        cmd.Parameters.AddWithValue("@ErrorMessage", (object)model.ErrorMessage ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExecutionTimeMs", model.ExecutionTimeMs);
                        cmd.Parameters.AddWithValue("@TelegramSent", model.TelegramSent);
                        cmd.Parameters.AddWithValue("@TriggerType", (object)model.TriggerType ?? "AutoSchedule");
                        cmd.Parameters.AddWithValue("@CreatedBy", (object)model.CreatedBy ?? "QuartzScheduler");
                        cmd.Parameters.AddWithValue("@CreatedDate", model.CreatedDate == DateTime.MinValue ? DateTime.Now : model.CreatedDate);

                        var id = Convert.ToInt64(cmd.ExecuteScalar());
                        model.Id = id;
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi AddExportLog: " + ex.Message, ex);
                return 0;
            }
        }

        public List<ExportedReportLogModel> GetExportLogs(string reportType, int? year, int? month, int pageIndex, int pageSize, out int totalRows)
        {
            var list = new List<ExportedReportLogModel>();
            totalRows = 0;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string countSql = "SELECT COUNT(*) FROM [BVTL_EXPORTED_REPORT_LOG] WHERE 1=1";
                    string dataSql = "SELECT * FROM [BVTL_EXPORTED_REPORT_LOG] WHERE 1=1";

                    string filter = "";
                    var parameters = new List<SqlParameter>();

                    if (!string.IsNullOrEmpty(reportType))
                    {
                        filter += " AND ReportType = @ReportType";
                        parameters.Add(new SqlParameter("@ReportType", reportType));
                    }
                    if (year.HasValue && year.Value > 0)
                    {
                        filter += " AND [Year] = @Year";
                        parameters.Add(new SqlParameter("@Year", year.Value));
                    }
                    if (month.HasValue && month.Value > 0)
                    {
                        filter += " AND [Month] = @Month";
                        parameters.Add(new SqlParameter("@Month", month.Value));
                    }

                    countSql += filter;
                    using (var cmdCount = new SqlCommand(countSql, conn))
                    {
                        foreach (var p in parameters) cmdCount.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
                        totalRows = Convert.ToInt32(cmdCount.ExecuteScalar());
                    }

                    dataSql += filter + " ORDER BY Id DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    using (var cmdData = new SqlCommand(dataSql, conn))
                    {
                        foreach (var p in parameters) cmdData.Parameters.Add(new SqlParameter(p.ParameterName, p.Value));
                        cmdData.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
                        cmdData.Parameters.AddWithValue("@PageSize", pageSize);

                        using (var reader = cmdData.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new ExportedReportLogModel
                                {
                                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                                    ReportType = reader["ReportType"]?.ToString(),
                                    ReportName = reader["ReportName"]?.ToString(),
                                    PeriodType = reader["PeriodType"]?.ToString(),
                                    PeriodValue = reader["PeriodValue"]?.ToString(),
                                    Year = reader.GetInt32(reader.GetOrdinal("Year")),
                                    Month = reader["Month"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Month"]) : null,
                                    MaDuAn = reader["MaDuAn"]?.ToString(),
                                    CityCode = reader["CityCode"]?.ToString(),
                                    MaNhom = reader["MaNhom"]?.ToString(),
                                    FileName = reader["FileName"]?.ToString(),
                                    FilePath = reader["FilePath"]?.ToString(),
                                    FileSizeKb = Convert.ToInt64(reader["FileSizeKb"]),
                                    TotalRecords = Convert.ToInt32(reader["TotalRecords"]),
                                    Status = reader["Status"]?.ToString(),
                                    ErrorMessage = reader["ErrorMessage"]?.ToString(),
                                    ExecutionTimeMs = Convert.ToInt32(reader["ExecutionTimeMs"]),
                                    TelegramSent = Convert.ToBoolean(reader["TelegramSent"]),
                                    TriggerType = reader["TriggerType"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetExportLogs: " + ex.Message, ex);
            }

            return list;
        }

        public ExportedReportLogModel GetById(long id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string sql = "SELECT * FROM [BVTL_EXPORTED_REPORT_LOG] WHERE Id = @Id";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ExportedReportLogModel
                                {
                                    Id = reader.GetInt64(reader.GetOrdinal("Id")),
                                    ReportType = reader["ReportType"]?.ToString(),
                                    ReportName = reader["ReportName"]?.ToString(),
                                    PeriodType = reader["PeriodType"]?.ToString(),
                                    PeriodValue = reader["PeriodValue"]?.ToString(),
                                    Year = reader.GetInt32(reader.GetOrdinal("Year")),
                                    Month = reader["Month"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Month"]) : null,
                                    MaDuAn = reader["MaDuAn"]?.ToString(),
                                    CityCode = reader["CityCode"]?.ToString(),
                                    MaNhom = reader["MaNhom"]?.ToString(),
                                    FileName = reader["FileName"]?.ToString(),
                                    FilePath = reader["FilePath"]?.ToString(),
                                    FileSizeKb = Convert.ToInt64(reader["FileSizeKb"]),
                                    TotalRecords = Convert.ToInt32(reader["TotalRecords"]),
                                    Status = reader["Status"]?.ToString(),
                                    ErrorMessage = reader["ErrorMessage"]?.ToString(),
                                    ExecutionTimeMs = Convert.ToInt32(reader["ExecutionTimeMs"]),
                                    TelegramSent = Convert.ToBoolean(reader["TelegramSent"]),
                                    TriggerType = reader["TriggerType"]?.ToString(),
                                    CreatedBy = reader["CreatedBy"]?.ToString(),
                                    CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetById: " + ex.Message, ex);
            }
            return null;
        }

        public ScheduledReportSettingModel GetSettings()
        {
            var model = new ScheduledReportSettingModel();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string sql = "SELECT ParamCode, ParamValue FROM [BVTL_QT_THAM_SO] WHERE ParamCode IN ('NotifiBCT', 'NotifiBCT_Hour', 'AutoExportReport_Active', 'TelegramChatId')";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var code = reader["ParamCode"].ToString();
                                var val = reader["ParamValue"]?.ToString();

                                if (code == "NotifiBCT" && int.TryParse(val, out int day)) model.RunDay = day;
                                if (code == "NotifiBCT_Hour" && int.TryParse(val, out int hour)) model.RunHour = hour;
                                if (code == "AutoExportReport_Active") model.IsActive = (val == "1" || val?.ToLower() == "true");
                                if (code == "TelegramChatId" && !string.IsNullOrEmpty(val)) model.TelegramChatId = val;
                            }
                        }
                    }

                    // Query last run time and count
                    const string statSql = @"
                    SELECT 
                        MAX(CreatedDate) AS LastRun,
                        COUNT(*) AS TotalCount
                    FROM [BVTL_EXPORTED_REPORT_LOG] 
                    WHERE Status = 'Success';";

                    using (var cmdStat = new SqlCommand(statSql, conn))
                    {
                        using (var reader = cmdStat.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["LastRun"] != DBNull.Value) model.LastRunTime = Convert.ToDateTime(reader["LastRun"]);
                                if (reader["TotalCount"] != DBNull.Value) model.TotalExportedFiles = Convert.ToInt32(reader["TotalCount"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetSettings: " + ex.Message, ex);
            }

            // Calculate next run time (5th of next/current month at RunHour)
            var now = DateTime.Now;
            var targetRun = new DateTime(now.Year, now.Month, Math.Min(model.RunDay, DateTime.DaysInMonth(now.Year, now.Month)), model.RunHour, 0, 0);
            if (now > targetRun)
            {
                var nextMonth = now.AddMonths(1);
                model.NextRunTime = new DateTime(nextMonth.Year, nextMonth.Month, Math.Min(model.RunDay, DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month)), model.RunHour, 0, 0);
            }
            else
            {
                model.NextRunTime = targetRun;
            }

            return model;
        }

        public bool UpdateSettings(int runDay, int runHour, bool isActive)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    const string sql = @"
                    UPDATE [BVTL_QT_THAM_SO] SET [ParamValue] = @Day WHERE [ParamCode] = 'NotifiBCT';
                    UPDATE [BVTL_QT_THAM_SO] SET [ParamValue] = @Hour WHERE [ParamCode] = 'NotifiBCT_Hour';
                    UPDATE [BVTL_QT_THAM_SO] SET [ParamValue] = @Active WHERE [ParamCode] = 'AutoExportReport_Active';";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Day", runDay.ToString());
                        cmd.Parameters.AddWithValue("@Hour", runHour.ToString());
                        cmd.Parameters.AddWithValue("@Active", isActive ? "1" : "0");
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Lỗi UpdateSettings: " + ex.Message, ex);
                return false;
            }
        }

        public bool CheckDataAvailability(string reportType, int year, int month)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string fromDateStr = $"01/{month:D2}/{year}";
                    var daysInMonth = DateTime.DaysInMonth(year, month);
                    string toDateStr = $"{daysInMonth:D2}/{month:D2}/{year}";

                    if (reportType == "TCV_CD45" || reportType == "HOATDONG_CD45")
                    {
                        // Kiểm tra bảng CD45_KHACH_HANG hoặc gọi SP_CD45_GetBaoCao xem có tổng > 0 không
                        const string sql = @"
                        SELECT TOP 1 1 
                        FROM [CD45_KHACH_HANG] 
                        WHERE CREATED_DATE >= @FromDate AND CREATED_DATE <= @ToDate";

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@FromDate", new DateTime(year, month, 1));
                            cmd.Parameters.AddWithValue("@ToDate", new DateTime(year, month, daysInMonth, 23, 59, 59));
                            var val = cmd.ExecuteScalar();
                            if (val != null) return true;
                        }

                        // Kiểm tra bổ sung qua SP_CD45_GetBaoCao
                        const string spSql = "EXEC SP_CD45_GetBaoCao @FromDate, @ToDate, NULL, NULL, NULL";
                        using (var cmdSp = new SqlCommand(spSql, conn))
                        {
                            cmdSp.Parameters.AddWithValue("@FromDate", new DateTime(year, month, 1));
                            cmdSp.Parameters.AddWithValue("@ToDate", new DateTime(year, month, daysInMonth, 23, 59, 59));
                            using (var r = cmdSp.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    var tong = r["Tong"] != DBNull.Value ? Convert.ToInt32(r["Tong"]) : 0;
                                    if (tong > 0) return true;
                                }
                            }
                        }
                        return false;
                    }
                    else // TONGHOP_BVTL
                    {
                        const string sqlBvtl = @"
                        SELECT TOP 1 1 
                        FROM [BVTL_PHIEU_TU_VAN] 
                        WHERE CreatedDate >= @FromDate AND CreatedDate <= @ToDate";

                        using (var cmd = new SqlCommand(sqlBvtl, conn))
                        {
                            cmd.Parameters.AddWithValue("@FromDate", new DateTime(year, month, 1));
                            cmd.Parameters.AddWithValue("@ToDate", new DateTime(year, month, daysInMonth, 23, 59, 59));
                            var val = cmd.ExecuteScalar();
                            return val != null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi CheckDataAvailability ({reportType}, {month}/{year}): " + ex.Message);
                // Nếu lỗi query, trả về true để cho phép chạy thay vì chặn hoàn toàn
                return true;
            }
        }
    }
}
