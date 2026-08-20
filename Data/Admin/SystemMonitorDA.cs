using Common.Common;
using Common.ICommon;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend.Base;
using Model.ModelExtend.SystemMonitor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class SystemMonitorDA : ISystemMonitorDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString = ConfigurationManager.AppSettings["ConnectionString"];
        private static bool _tablesInitialized = false;
        private static readonly object _initLock = new object();

        public SystemMonitorDA()
        {
            if (!_tablesInitialized)
            {
                lock (_initLock)
                {
                    if (!_tablesInitialized)
                    {
                        InitTablesIfNotExist();
                        _tablesInitialized = true;
                    }
                }
            }
        }

        public void InitTablesIfNotExist()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string script = @"
                    -- 1. Bảng Lịch sử Đăng nhập
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BVTL_LOGIN_HISTORY')
                    BEGIN
                        CREATE TABLE [dbo].[BVTL_LOGIN_HISTORY] (
                            [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [UserId] BIGINT NULL,
                            [UserName] VARCHAR(50) NOT NULL,
                            [FullName] NVARCHAR(100) NULL,
                            [LoginTime] DATETIME NOT NULL DEFAULT GETDATE(),
                            [LogoutTime] DATETIME NULL,
                            [IpAddress] VARCHAR(50) NULL,
                            [UserAgent] NVARCHAR(500) NULL,
                            [Browser] NVARCHAR(50) NULL,
                            [OperatingSystem] NVARCHAR(50) NULL,
                            [DeviceType] NVARCHAR(30) NULL,
                            [Status] NVARCHAR(50) NOT NULL, -- Success, WrongPassword, AccountLocked, UserNotFound
                            [SessionId] VARCHAR(100) NULL
                        );
                        CREATE INDEX IX_LOGIN_HIST_DATE ON [dbo].[BVTL_LOGIN_HISTORY](LoginTime DESC);
                        CREATE INDEX IX_LOGIN_HIST_USER ON [dbo].[BVTL_LOGIN_HISTORY](UserName);
                    END

                    -- 2. Bảng Phiên hoạt động Real-time (Online/Offline)
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BVTL_USER_ONLINE')
                    BEGIN
                        CREATE TABLE [dbo].[BVTL_USER_ONLINE] (
                            [SessionId] VARCHAR(100) NOT NULL PRIMARY KEY,
                            [UserId] BIGINT NOT NULL,
                            [UserName] VARCHAR(50) NOT NULL,
                            [FullName] NVARCHAR(100) NULL,
                            [RoleName] NVARCHAR(100) NULL,
                            [CityCodes] VARCHAR(200) NULL,
                            [MaDuAn] VARCHAR(200) NULL,
                            [IpAddress] VARCHAR(50) NULL,
                            [Browser] NVARCHAR(50) NULL,
                            [OperatingSystem] NVARCHAR(50) NULL,
                            [DeviceType] NVARCHAR(30) NULL,
                            [LoginTime] DATETIME NOT NULL DEFAULT GETDATE(),
                            [LastActiveTime] DATETIME NOT NULL DEFAULT GETDATE(),
                            [CurrentController] VARCHAR(100) NULL,
                            [CurrentAction] VARCHAR(100) NULL,
                            [CurrentUrl] NVARCHAR(500) NULL,
                            [Status] VARCHAR(20) NOT NULL DEFAULT 'Online' -- Online, Idle, Offline
                        );
                        CREATE INDEX IX_USER_ONLINE_ACTIVE ON [dbo].[BVTL_USER_ONLINE](LastActiveTime DESC);
                    END

                    -- 3. Bảng Thống kê Lượt Sử dụng Chức năng & Module
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BVTL_FEATURE_USAGE_LOG')
                    BEGIN
                        CREATE TABLE [dbo].[BVTL_FEATURE_USAGE_LOG] (
                            [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                            [UserId] BIGINT NULL,
                            [UserName] VARCHAR(50) NULL,
                            [FullName] NVARCHAR(100) NULL,
                            [ControllerName] VARCHAR(100) NOT NULL,
                            [ActionName] VARCHAR(100) NOT NULL,
                            [ActionType] VARCHAR(30) NOT NULL, -- View, Search, Create, Update, Delete, ExportExcel, Sync
                            [ExecutionTimeMs] INT NULL,
                            [IsError] BIT NOT NULL DEFAULT 0,
                            [ErrorMessage] NVARCHAR(1000) NULL,
                            [AccessDate] DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
                        );
                        CREATE INDEX IX_FEATURE_USAGE_DATE ON [dbo].[BVTL_FEATURE_USAGE_LOG](AccessDate DESC);
                        CREATE INDEX IX_FEATURE_USAGE_CTRL ON [dbo].[BVTL_FEATURE_USAGE_LOG](ControllerName);
                    END
                    ";

                    using (SqlCommand cmd = new SqlCommand(script, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - InitTablesIfNotExist Error: " + ex.Message, ex);
            }
        }

        #region User-Agent Parser Helper
        private void ParseUserAgent(string userAgent, out string browser, out string os, out string deviceType)
        {
            browser = "Trình duyệt khác";
            os = "Hệ điều hành khác";
            deviceType = "Desktop";

            if (string.IsNullOrEmpty(userAgent))
                return;

            string ua = userAgent.ToLower();

            // Device
            if (ua.Contains("mobile") || ua.Contains("android") || ua.Contains("iphone"))
                deviceType = "Mobile";
            else if (ua.Contains("ipad") || ua.Contains("tablet"))
                deviceType = "Tablet";
            else
                deviceType = "Desktop";

            // OS
            if (ua.Contains("windows nt 10.0")) os = "Windows 10/11";
            else if (ua.Contains("windows nt 6.3")) os = "Windows 8.1";
            else if (ua.Contains("windows nt 6.1")) os = "Windows 7";
            else if (ua.Contains("windows")) os = "Windows";
            else if (ua.Contains("mac os x")) os = "macOS";
            else if (ua.Contains("iphone") || ua.Contains("ipad")) os = "iOS";
            else if (ua.Contains("android")) os = "Android";
            else if (ua.Contains("linux")) os = "Linux";

            // Browser
            if (ua.Contains("coccoc") || ua.Contains("coc_coc")) browser = "Cốc Cốc";
            else if (ua.Contains("edg/") || ua.Contains("edge")) browser = "Microsoft Edge";
            else if (ua.Contains("chrome") && !ua.Contains("edg")) browser = "Google Chrome";
            else if (ua.Contains("safari") && !ua.Contains("chrome")) browser = "Apple Safari";
            else if (ua.Contains("firefox")) browser = "Mozilla Firefox";
            else if (ua.Contains("opera") || ua.Contains("opr")) browser = "Opera";
        }
        #endregion

        #region Recording Methods
        public void RecordLogin(UserLogin user, string ip, string userAgent, string status, string sessionId)
        {
            try
            {
                string browser, os, deviceType;
                ParseUserAgent(userAgent, out browser, out os, out deviceType);

                string userName = user != null ? user.UserName : "Anonymous";
                string fullName = user != null ? user.Name : "";
                long? userId = user != null ? (long?)user.UserID : null;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 1. Insert login history
                    string sqlHistory = @"
                        INSERT INTO [dbo].[BVTL_LOGIN_HISTORY] 
                        (UserId, UserName, FullName, LoginTime, IpAddress, UserAgent, Browser, OperatingSystem, DeviceType, Status, SessionId)
                        VALUES 
                        (@UserId, @UserName, @FullName, GETDATE(), @IpAddress, @UserAgent, @Browser, @OperatingSystem, @DeviceType, @Status, @SessionId)";

                    using (SqlCommand cmd = new SqlCommand(sqlHistory, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", (object)userId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@FullName", (object)fullName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IpAddress", (object)ip ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UserAgent", (object)userAgent ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Browser", browser);
                        cmd.Parameters.AddWithValue("@OperatingSystem", os);
                        cmd.Parameters.AddWithValue("@DeviceType", deviceType);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@SessionId", (object)sessionId ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. If login is successful, upsert into BVTL_USER_ONLINE
                    if (status == "Success" && user != null && !string.IsNullOrEmpty(sessionId))
                    {
                        string roleName = user.IsAdmin ? "Quản trị viên (Admin)" : "Người dùng";
                        string sqlOnline = @"
                            IF EXISTS (SELECT 1 FROM [dbo].[BVTL_USER_ONLINE] WHERE SessionId = @SessionId)
                            BEGIN
                                UPDATE [dbo].[BVTL_USER_ONLINE]
                                SET LastActiveTime = GETDATE(), Status = 'Online', IpAddress = @IpAddress,
                                    Browser = @Browser, OperatingSystem = @OperatingSystem, DeviceType = @DeviceType
                                WHERE SessionId = @SessionId
                            END
                            ELSE
                            BEGIN
                                INSERT INTO [dbo].[BVTL_USER_ONLINE]
                                (SessionId, UserId, UserName, FullName, RoleName, CityCodes, MaDuAn, IpAddress, Browser, OperatingSystem, DeviceType, LoginTime, LastActiveTime, Status)
                                VALUES
                                (@SessionId, @UserId, @UserName, @FullName, @RoleName, @CityCodes, @MaDuAn, @IpAddress, @Browser, @OperatingSystem, @DeviceType, GETDATE(), GETDATE(), 'Online')
                            END";

                        using (SqlCommand cmdOnline = new SqlCommand(sqlOnline, conn))
                        {
                            cmdOnline.Parameters.AddWithValue("@SessionId", sessionId);
                            cmdOnline.Parameters.AddWithValue("@UserId", user.UserID);
                            cmdOnline.Parameters.AddWithValue("@UserName", user.UserName);
                            cmdOnline.Parameters.AddWithValue("@FullName", (object)user.Name ?? DBNull.Value);
                            cmdOnline.Parameters.AddWithValue("@RoleName", roleName);
                            cmdOnline.Parameters.AddWithValue("@CityCodes", (object)user.CityCodes ?? DBNull.Value);
                            cmdOnline.Parameters.AddWithValue("@MaDuAn", (object)user.MaDuAn ?? DBNull.Value);
                            cmdOnline.Parameters.AddWithValue("@IpAddress", (object)ip ?? DBNull.Value);
                            cmdOnline.Parameters.AddWithValue("@Browser", browser);
                            cmdOnline.Parameters.AddWithValue("@OperatingSystem", os);
                            cmdOnline.Parameters.AddWithValue("@DeviceType", deviceType);
                            cmdOnline.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - RecordLogin Error: " + ex.Message, ex);
            }
        }

        public void RecordLogout(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    // Update LogoutTime in history
                    string sqlHistory = @"
                        UPDATE [dbo].[BVTL_LOGIN_HISTORY]
                        SET LogoutTime = GETDATE()
                        WHERE SessionId = @SessionId AND LogoutTime IS NULL";

                    using (SqlCommand cmd = new SqlCommand(sqlHistory, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.ExecuteNonQuery();
                    }

                    // Delete from Online table
                    string sqlOnline = @"DELETE FROM [dbo].[BVTL_USER_ONLINE] WHERE SessionId = @SessionId";
                    using (SqlCommand cmd = new SqlCommand(sqlOnline, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - RecordLogout Error: " + ex.Message, ex);
            }
        }

        public void UpdateHeartbeat(string sessionId, string currentUrl, string controllerName, string actionName)
        {
            if (string.IsNullOrEmpty(sessionId)) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE [dbo].[BVTL_USER_ONLINE]
                        SET LastActiveTime = GETDATE(),
                            Status = 'Online',
                            CurrentUrl = COALESCE(@CurrentUrl, CurrentUrl),
                            CurrentController = COALESCE(@CurrentController, CurrentController),
                            CurrentAction = COALESCE(@CurrentAction, CurrentAction)
                        WHERE SessionId = @SessionId;
                        
                        -- Tự động dọn dẹp các phiên không gửi heartbeat quá 24h
                        DELETE FROM [dbo].[BVTL_USER_ONLINE] WHERE DATEDIFF(hour, LastActiveTime, GETDATE()) > 24;";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@CurrentUrl", (object)currentUrl ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CurrentController", (object)controllerName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CurrentAction", (object)actionName ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - UpdateHeartbeat Error: " + ex.Message, ex);
            }
        }

        public void RecordFeatureUsage(string userName, string fullName, string controller, string action, string actionType, int? executionTimeMs, bool isError, string errorMsg)
        {
            if (string.IsNullOrEmpty(controller)) return;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO [dbo].[BVTL_FEATURE_USAGE_LOG]
                        (UserName, FullName, ControllerName, ActionName, ActionType, ExecutionTimeMs, IsError, ErrorMessage, AccessDate, CreatedDate)
                        VALUES
                        (@UserName, @FullName, @ControllerName, @ActionName, @ActionType, @ExecutionTimeMs, @IsError, @ErrorMessage, CAST(GETDATE() AS DATE), GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserName", (object)userName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FullName", (object)fullName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ControllerName", controller);
                        cmd.Parameters.AddWithValue("@ActionName", action ?? "");
                        cmd.Parameters.AddWithValue("@ActionType", actionType ?? "View");
                        cmd.Parameters.AddWithValue("@ExecutionTimeMs", (object)executionTimeMs ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsError", isError);
                        cmd.Parameters.AddWithValue("@ErrorMessage", (object)errorMsg ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - RecordFeatureUsage Error: " + ex.Message, ex);
            }
        }
        #endregion

        #region Query & Analytics Methods
        public List<UserOnlineModel> GetOnlineUsers(string keyword, int page, int pageSize, ref int totalRow)
        {
            var list = new List<UserOnlineModel>();
            try
            {
                page = page > 0 ? page : 1;
                pageSize = pageSize > 0 ? pageSize : 10;
                int skip = (page - 1) * pageSize;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string whereClause = "WHERE 1=1 ";
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        whereClause += "AND (UserName LIKE @kw OR FullName LIKE @kw OR RoleName LIKE @kw OR IpAddress LIKE @kw OR CurrentController LIKE @kw) ";
                    }

                    string countSql = $"SELECT COUNT(*) FROM [dbo].[BVTL_USER_ONLINE] {whereClause}";
                    using (SqlCommand countCmd = new SqlCommand(countSql, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) countCmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");
                        totalRow = (int)countCmd.ExecuteScalar();
                    }

                    string dataSql = $@"
                        SELECT SessionId, UserId, UserName, FullName, RoleName, CityCodes, MaDuAn, IpAddress,
                               Browser, OperatingSystem, DeviceType, LoginTime, LastActiveTime,
                               CurrentController, CurrentAction, CurrentUrl,
                               CASE 
                                   WHEN DATEDIFF(minute, LastActiveTime, GETDATE()) <= 2 THEN 'Online'
                                   WHEN DATEDIFF(minute, LastActiveTime, GETDATE()) <= 10 THEN 'Idle'
                                   ELSE 'Offline'
                               END AS Status
                        FROM [dbo].[BVTL_USER_ONLINE]
                        {whereClause}
                        ORDER BY LastActiveTime DESC
                        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(dataSql, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword.Trim() + "%");
                        cmd.Parameters.AddWithValue("@skip", skip);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new UserOnlineModel
                                {
                                    SessionId = reader["SessionId"].ToString(),
                                    UserId = Convert.ToInt64(reader["UserId"]),
                                    UserName = reader["UserName"].ToString(),
                                    FullName = reader["FullName"] != DBNull.Value ? reader["FullName"].ToString() : "",
                                    RoleName = reader["RoleName"] != DBNull.Value ? reader["RoleName"].ToString() : "",
                                    CityCodes = reader["CityCodes"] != DBNull.Value ? reader["CityCodes"].ToString() : "",
                                    MaDuAn = reader["MaDuAn"] != DBNull.Value ? reader["MaDuAn"].ToString() : "",
                                    IpAddress = reader["IpAddress"] != DBNull.Value ? reader["IpAddress"].ToString() : "",
                                    Browser = reader["Browser"] != DBNull.Value ? reader["Browser"].ToString() : "",
                                    OperatingSystem = reader["OperatingSystem"] != DBNull.Value ? reader["OperatingSystem"].ToString() : "",
                                    DeviceType = reader["DeviceType"] != DBNull.Value ? reader["DeviceType"].ToString() : "",
                                    LoginTime = Convert.ToDateTime(reader["LoginTime"]),
                                    LastActiveTime = Convert.ToDateTime(reader["LastActiveTime"]),
                                    CurrentController = reader["CurrentController"] != DBNull.Value ? reader["CurrentController"].ToString() : "",
                                    CurrentAction = reader["CurrentAction"] != DBNull.Value ? reader["CurrentAction"].ToString() : "",
                                    CurrentUrl = reader["CurrentUrl"] != DBNull.Value ? reader["CurrentUrl"].ToString() : "",
                                    Status = reader["Status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - GetOnlineUsers Error: " + ex.Message, ex);
            }
            return list;
        }

        public List<LoginHistoryModel> GetLoginHistory(SystemMonitorSearchModel search, ref int totalRow)
        {
            var list = new List<LoginHistoryModel>();
            try
            {
                int page = search.currentPage > 0 ? search.currentPage : 1;
                int pageSize = search.pageSize > 0 ? search.pageSize : 10;
                int skip = (page - 1) * pageSize;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string whereClause = "WHERE 1=1 ";
                    if (!string.IsNullOrEmpty(search.Keyword))
                    {
                        whereClause += "AND (UserName LIKE @kw OR FullName LIKE @kw OR IpAddress LIKE @kw OR Browser LIKE @kw) ";
                    }
                    if (!string.IsNullOrEmpty(search.Status) && search.Status != "all")
                    {
                        whereClause += "AND Status = @status ";
                    }
                    if (!string.IsNullOrEmpty(search.FromDate))
                    {
                        whereClause += "AND LoginTime >= @fromDate ";
                    }
                    if (!string.IsNullOrEmpty(search.ToDate))
                    {
                        whereClause += "AND LoginTime <= @toDate ";
                    }

                    string countSql = $"SELECT COUNT(*) FROM [dbo].[BVTL_LOGIN_HISTORY] {whereClause}";
                    using (SqlCommand countCmd = new SqlCommand(countSql, conn))
                    {
                        if (!string.IsNullOrEmpty(search.Keyword)) countCmd.Parameters.AddWithValue("@kw", "%" + search.Keyword.Trim() + "%");
                        if (!string.IsNullOrEmpty(search.Status) && search.Status != "all") countCmd.Parameters.AddWithValue("@status", search.Status);
                        if (!string.IsNullOrEmpty(search.FromDate)) countCmd.Parameters.AddWithValue("@fromDate", DateTime.Parse(search.FromDate));
                        if (!string.IsNullOrEmpty(search.ToDate)) countCmd.Parameters.AddWithValue("@toDate", DateTime.Parse(search.ToDate).AddDays(1).AddTicks(-1));
                        totalRow = (int)countCmd.ExecuteScalar();
                    }

                    string dataSql = $@"
                        SELECT Id, UserId, UserName, FullName, LoginTime, LogoutTime, IpAddress, UserAgent,
                               Browser, OperatingSystem, DeviceType, Status, SessionId
                        FROM [dbo].[BVTL_LOGIN_HISTORY]
                        {whereClause}
                        ORDER BY LoginTime DESC
                        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(dataSql, conn))
                    {
                        if (!string.IsNullOrEmpty(search.Keyword)) cmd.Parameters.AddWithValue("@kw", "%" + search.Keyword.Trim() + "%");
                        if (!string.IsNullOrEmpty(search.Status) && search.Status != "all") cmd.Parameters.AddWithValue("@status", search.Status);
                        if (!string.IsNullOrEmpty(search.FromDate)) cmd.Parameters.AddWithValue("@fromDate", DateTime.Parse(search.FromDate));
                        if (!string.IsNullOrEmpty(search.ToDate)) cmd.Parameters.AddWithValue("@toDate", DateTime.Parse(search.ToDate).AddDays(1).AddTicks(-1));
                        cmd.Parameters.AddWithValue("@skip", skip);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new LoginHistoryModel
                                {
                                    Id = Convert.ToInt64(reader["Id"]),
                                    UserId = reader["UserId"] != DBNull.Value ? (long?)Convert.ToInt64(reader["UserId"]) : null,
                                    UserName = reader["UserName"].ToString(),
                                    FullName = reader["FullName"] != DBNull.Value ? reader["FullName"].ToString() : "",
                                    LoginTime = Convert.ToDateTime(reader["LoginTime"]),
                                    LogoutTime = reader["LogoutTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["LogoutTime"]) : null,
                                    IpAddress = reader["IpAddress"] != DBNull.Value ? reader["IpAddress"].ToString() : "",
                                    UserAgent = reader["UserAgent"] != DBNull.Value ? reader["UserAgent"].ToString() : "",
                                    Browser = reader["Browser"] != DBNull.Value ? reader["Browser"].ToString() : "",
                                    OperatingSystem = reader["OperatingSystem"] != DBNull.Value ? reader["OperatingSystem"].ToString() : "",
                                    DeviceType = reader["DeviceType"] != DBNull.Value ? reader["DeviceType"].ToString() : "",
                                    Status = reader["Status"].ToString(),
                                    SessionId = reader["SessionId"] != DBNull.Value ? reader["SessionId"].ToString() : ""
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - GetLoginHistory Error: " + ex.Message, ex);
            }
            return list;
        }

        public TrafficStatsModel GetTrafficStats(int days = 7)
        {
            var stats = new TrafficStatsModel();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 1. Lượt Page Views & Unique Users theo 7 ngày qua
                    string sqlDaily = @"
                        SELECT CAST(CreatedDate AS DATE) as LogDate, 
                               COUNT(*) as PageViews, 
                               COUNT(DISTINCT UserName) as UniqueUsers
                        FROM [dbo].[BVTL_FEATURE_USAGE_LOG]
                        WHERE CreatedDate >= DATEADD(day, -@days, GETDATE())
                        GROUP BY CAST(CreatedDate AS DATE)
                        ORDER BY LogDate ASC";

                    using (SqlCommand cmd = new SqlCommand(sqlDaily, conn))
                    {
                        cmd.Parameters.AddWithValue("@days", days);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DateTime dt = Convert.ToDateTime(reader["LogDate"]);
                                stats.TimeLabels.Add(dt.ToString("dd/MM"));
                                stats.PageViewsData.Add(Convert.ToInt32(reader["PageViews"]));
                                stats.UniqueUsersData.Add(Convert.ToInt32(reader["UniqueUsers"]));
                            }
                        }
                    }

                    // Nếu chưa có dữ liệu log, tạo khung mẫu ngày
                    if (stats.TimeLabels.Count == 0)
                    {
                        for (int i = days - 1; i >= 0; i--)
                        {
                            stats.TimeLabels.Add(DateTime.Now.AddDays(-i).ToString("dd/MM"));
                            stats.PageViewsData.Add(0);
                            stats.UniqueUsersData.Add(0);
                        }
                    }

                    // 2. Cơ cấu Thiết bị (Device Distribution)
                    string sqlDevice = @"
                        SELECT ISNULL(DeviceType, 'Desktop') as Device, COUNT(*) as Total
                        FROM [dbo].[BVTL_LOGIN_HISTORY]
                        WHERE LoginTime >= DATEADD(day, -@days, GETDATE())
                        GROUP BY DeviceType";

                    using (SqlCommand cmd = new SqlCommand(sqlDevice, conn))
                    {
                        cmd.Parameters.AddWithValue("@days", days);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                stats.DeviceDistribution[reader["Device"].ToString()] = Convert.ToInt32(reader["Total"]);
                            }
                        }
                    }

                    // 3. Cơ cấu Trình duyệt (Browser Distribution)
                    string sqlBrowser = @"
                        SELECT ISNULL(Browser, 'Khác') as Browser, COUNT(*) as Total
                        FROM [dbo].[BVTL_LOGIN_HISTORY]
                        WHERE LoginTime >= DATEADD(day, -@days, GETDATE())
                        GROUP BY Browser";

                    using (SqlCommand cmd = new SqlCommand(sqlBrowser, conn))
                    {
                        cmd.Parameters.AddWithValue("@days", days);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                stats.BrowserDistribution[reader["Browser"].ToString()] = Convert.ToInt32(reader["Total"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - GetTrafficStats Error: " + ex.Message, ex);
            }
            return stats;
        }

        public List<FeatureUsageModel> GetFeatureUsageStats(SystemMonitorSearchModel search, ref int totalRow)
        {
            var list = new List<FeatureUsageModel>();
            try
            {
                int page = search.currentPage > 0 ? search.currentPage : 1;
                int pageSize = search.pageSize > 0 ? search.pageSize : 10;
                int skip = (page - 1) * pageSize;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string whereClause = "WHERE 1=1 ";
                    if (!string.IsNullOrEmpty(search.Keyword))
                    {
                        whereClause += "AND (ControllerName LIKE @kw OR ActionName LIKE @kw OR UserName LIKE @kw OR FullName LIKE @kw) ";
                    }
                    if (!string.IsNullOrEmpty(search.ControllerName) && search.ControllerName != "all")
                    {
                        whereClause += "AND ControllerName = @ctrl ";
                    }
                    if (!string.IsNullOrEmpty(search.ActionType) && search.ActionType != "all")
                    {
                        whereClause += "AND ActionType = @actionType ";
                    }
                    if (!string.IsNullOrEmpty(search.FromDate))
                    {
                        whereClause += "AND CreatedDate >= @fromDate ";
                    }
                    if (!string.IsNullOrEmpty(search.ToDate))
                    {
                        whereClause += "AND CreatedDate <= @toDate ";
                    }

                    string countSql = $@"
                        SELECT COUNT(*) FROM (
                            SELECT ControllerName, ActionName, ActionType
                            FROM [dbo].[BVTL_FEATURE_USAGE_LOG]
                            {whereClause}
                            GROUP BY ControllerName, ActionName, ActionType
                        ) t";

                    using (SqlCommand countCmd = new SqlCommand(countSql, conn))
                    {
                        if (!string.IsNullOrEmpty(search.Keyword)) countCmd.Parameters.AddWithValue("@kw", "%" + search.Keyword.Trim() + "%");
                        if (!string.IsNullOrEmpty(search.ControllerName) && search.ControllerName != "all") countCmd.Parameters.AddWithValue("@ctrl", search.ControllerName);
                        if (!string.IsNullOrEmpty(search.ActionType) && search.ActionType != "all") countCmd.Parameters.AddWithValue("@actionType", search.ActionType);
                        if (!string.IsNullOrEmpty(search.FromDate)) countCmd.Parameters.AddWithValue("@fromDate", DateTime.Parse(search.FromDate));
                        if (!string.IsNullOrEmpty(search.ToDate)) countCmd.Parameters.AddWithValue("@toDate", DateTime.Parse(search.ToDate).AddDays(1).AddTicks(-1));
                        totalRow = (int)countCmd.ExecuteScalar();
                    }

                    string dataSql = $@"
                        SELECT ControllerName, ActionName, ActionType,
                               COUNT(*) as HitCount,
                               AVG(CAST(ISNULL(ExecutionTimeMs, 0) AS FLOAT)) as AvgTime,
                               SUM(CASE WHEN IsError = 1 THEN 1 ELSE 0 END) as ErrorCount,
                               MAX(CreatedDate) as LastAccessed
                        FROM [dbo].[BVTL_FEATURE_USAGE_LOG]
                        {whereClause}
                        GROUP BY ControllerName, ActionName, ActionType
                        ORDER BY HitCount DESC
                        OFFSET @skip ROWS FETCH NEXT @pageSize ROWS ONLY";

                    using (SqlCommand cmd = new SqlCommand(dataSql, conn))
                    {
                        if (!string.IsNullOrEmpty(search.Keyword)) cmd.Parameters.AddWithValue("@kw", "%" + search.Keyword.Trim() + "%");
                        if (!string.IsNullOrEmpty(search.ControllerName) && search.ControllerName != "all") cmd.Parameters.AddWithValue("@ctrl", search.ControllerName);
                        if (!string.IsNullOrEmpty(search.ActionType) && search.ActionType != "all") cmd.Parameters.AddWithValue("@actionType", search.ActionType);
                        if (!string.IsNullOrEmpty(search.FromDate)) cmd.Parameters.AddWithValue("@fromDate", DateTime.Parse(search.FromDate));
                        if (!string.IsNullOrEmpty(search.ToDate)) cmd.Parameters.AddWithValue("@toDate", DateTime.Parse(search.ToDate).AddDays(1).AddTicks(-1));
                        cmd.Parameters.AddWithValue("@skip", skip);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new FeatureUsageModel
                                {
                                    ControllerName = reader["ControllerName"].ToString(),
                                    ActionName = reader["ActionName"].ToString(),
                                    ActionType = reader["ActionType"].ToString(),
                                    HitCount = Convert.ToInt32(reader["HitCount"]),
                                    AvgExecutionTimeMs = Math.Round(Convert.ToDouble(reader["AvgTime"]), 1),
                                    ErrorCount = Convert.ToInt32(reader["ErrorCount"]),
                                    LastAccessedTime = Convert.ToDateTime(reader["LastAccessed"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - GetFeatureUsageStats Error: " + ex.Message, ex);
            }
            return list;
        }

        public List<FeatureRankModel> GetTopFeatures(int top = 10, int days = 30)
        {
            var list = new List<FeatureRankModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = $@"
                        SELECT TOP (@top) 
                               ControllerName,
                               COUNT(*) as TotalHits,
                               COUNT(DISTINCT UserName) as TotalUsers,
                               SUM(CASE WHEN ActionType = 'View' THEN 1 ELSE 0 END) as ViewCount,
                               SUM(CASE WHEN ActionType IN ('Create', 'Update', 'Delete') THEN 1 ELSE 0 END) as MutationCount,
                               SUM(CASE WHEN ActionType = 'ExportExcel' THEN 1 ELSE 0 END) as ExportCount,
                               AVG(CAST(ISNULL(ExecutionTimeMs, 0) AS FLOAT)) as AvgDuration,
                               SUM(CASE WHEN IsError = 1 THEN 1 ELSE 0 END) as ErrorCount,
                               MAX(CreatedDate) as LastUsed
                        FROM [dbo].[BVTL_FEATURE_USAGE_LOG]
                        WHERE CreatedDate >= DATEADD(day, -@days, GETDATE())
                        GROUP BY ControllerName
                        ORDER BY TotalHits DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@top", top);
                        cmd.Parameters.AddWithValue("@days", days);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string ctrl = reader["ControllerName"].ToString();
                                list.Add(new FeatureRankModel
                                {
                                    ControllerName = ctrl,
                                    FeatureName = GetFeatureDisplayName(ctrl),
                                    Category = GetFeatureCategory(ctrl),
                                    TotalHits = Convert.ToInt32(reader["TotalHits"]),
                                    TotalUsers = Convert.ToInt32(reader["TotalUsers"]),
                                    ViewCount = Convert.ToInt32(reader["ViewCount"]),
                                    MutationCount = Convert.ToInt32(reader["MutationCount"]),
                                    ExportCount = Convert.ToInt32(reader["ExportCount"]),
                                    AvgDurationMs = Math.Round(Convert.ToDouble(reader["AvgDuration"]), 1),
                                    ErrorCount = Convert.ToInt32(reader["ErrorCount"]),
                                    LastUsedDate = Convert.ToDateTime(reader["LastUsed"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - GetTopFeatures Error: " + ex.Message, ex);
            }
            return list;
        }

        public List<FeatureRankModel> GetUnderutilizedFeatures(int days = 30)
        {
            var list = new List<FeatureRankModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    // Lấy tất cả Controller từ BVTL_QT_PAGE_MENU so sánh với log
                    string sql = @"
                        SELECT m.CONTROLLER_NAME, m.NAME, m.DESCRIPTION,
                               ISNULL(l.TotalHits, 0) as TotalHits,
                               l.LastUsed
                        FROM [dbo].[BVTL_QT_PAGE_MENU] m
                        LEFT JOIN (
                            SELECT ControllerName, COUNT(*) as TotalHits, MAX(CreatedDate) as LastUsed
                            FROM [dbo].[BVTL_FEATURE_USAGE_LOG]
                            WHERE CreatedDate >= DATEADD(day, -@days, GETDATE())
                            GROUP BY ControllerName
                        ) l ON m.CONTROLLER_NAME = l.ControllerName
                        WHERE m.CONTROLLER_NAME IS NOT NULL AND m.CONTROLLER_NAME <> ''
                          AND ISNULL(l.TotalHits, 0) <= 5
                        ORDER BY TotalHits ASC, m.ORDER_BY ASC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@days", days);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string ctrl = reader["CONTROLLER_NAME"].ToString();
                                DateTime? lastUsed = reader["LastUsed"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["LastUsed"]) : null;
                                int daysInactive = lastUsed.HasValue ? (int)(DateTime.Now - lastUsed.Value).TotalDays : 999;

                                list.Add(new FeatureRankModel
                                {
                                    ControllerName = ctrl,
                                    FeatureName = reader["NAME"] != DBNull.Value ? reader["NAME"].ToString() : GetFeatureDisplayName(ctrl),
                                    Category = GetFeatureCategory(ctrl),
                                    TotalHits = Convert.ToInt32(reader["TotalHits"]),
                                    LastUsedDate = lastUsed,
                                    DaysInactive = daysInactive
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - GetUnderutilizedFeatures Error: " + ex.Message, ex);
            }
            return list;
        }

        public Dictionary<string, int> GetRoleUsageDistribution(int days = 30)
        {
            var dict = new Dictionary<string, int>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT ISNULL(r.Name, 'Khác') as RoleName, COUNT(*) as Total
                        FROM [dbo].[BVTL_FEATURE_USAGE_LOG] f
                        LEFT JOIN [dbo].[BVTL_QT_NGUOI_DUNG] u ON f.UserName = u.UserName
                        LEFT JOIN [dbo].[BVTL_QT_QUYEN] r ON u.GroupID = r.ID
                        WHERE f.CreatedDate >= DATEADD(day, -@days, GETDATE())
                        GROUP BY r.Name
                        ORDER BY Total DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@days", days);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dict[reader["RoleName"].ToString()] = Convert.ToInt32(reader["Total"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - GetRoleUsageDistribution Error: " + ex.Message, ex);
            }
            return dict;
        }

        public SystemMonitorDashboardKPIs GetDashboardKPIs()
        {
            var kpi = new SystemMonitorDashboardKPIs();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 1. Online & Idle count
                    string sqlOnline = @"
                        SELECT 
                            SUM(CASE WHEN DATEDIFF(minute, LastActiveTime, GETDATE()) <= 2 THEN 1 ELSE 0 END) as OnlineCount,
                            SUM(CASE WHEN DATEDIFF(minute, LastActiveTime, GETDATE()) > 2 AND DATEDIFF(minute, LastActiveTime, GETDATE()) <= 10 THEN 1 ELSE 0 END) as IdleCount
                        FROM [dbo].[BVTL_USER_ONLINE]";

                    using (SqlCommand cmd = new SqlCommand(sqlOnline, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            kpi.OnlineUsersCount = reader["OnlineCount"] != DBNull.Value ? Convert.ToInt32(reader["OnlineCount"]) : 0;
                            kpi.IdleUsersCount = reader["IdleCount"] != DBNull.Value ? Convert.ToInt32(reader["IdleCount"]) : 0;
                        }
                    }

                    // 2. DAU & Today Page Views
                    string sqlToday = @"
                        SELECT 
                            COUNT(DISTINCT UserName) as DAU,
                            COUNT(*) as TodayHits,
                            AVG(CAST(ISNULL(ExecutionTimeMs, 0) AS FLOAT)) as AvgTime
                        FROM [dbo].[BVTL_FEATURE_USAGE_LOG]
                        WHERE AccessDate = CAST(GETDATE() AS DATE)";

                    using (SqlCommand cmd = new SqlCommand(sqlToday, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            kpi.DailyActiveUsers = reader["DAU"] != DBNull.Value ? Convert.ToInt32(reader["DAU"]) : 0;
                            kpi.TodayPageViews = reader["TodayHits"] != DBNull.Value ? Convert.ToInt32(reader["TodayHits"]) : 0;
                            kpi.SystemAvgResponseTimeMs = reader["AvgTime"] != DBNull.Value ? Math.Round(Convert.ToDouble(reader["AvgTime"]), 1) : 0;
                        }
                    }

                    // 3. Failed Logins today
                    string sqlFailed = @"
                        SELECT COUNT(*) 
                        FROM [dbo].[BVTL_LOGIN_HISTORY]
                        WHERE CAST(LoginTime AS DATE) = CAST(GETDATE() AS DATE) AND Status <> 'Success'";

                    using (SqlCommand cmd = new SqlCommand(sqlFailed, conn))
                    {
                        kpi.TodayFailedLogins = (int)cmd.ExecuteScalar();
                    }

                    // 4. Monthly Mutations & Exports
                    string sqlMonth = @"
                        SELECT 
                            SUM(CASE WHEN ActionType IN ('Create', 'Update', 'Delete') THEN 1 ELSE 0 END) as Mutations,
                            SUM(CASE WHEN ActionType = 'ExportExcel' THEN 1 ELSE 0 END) as Exports
                        FROM [dbo].[BVTL_FEATURE_USAGE_LOG]
                        WHERE CreatedDate >= DATEADD(day, -30, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(sqlMonth, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            kpi.TotalMonthlyMutations = reader["Mutations"] != DBNull.Value ? Convert.ToInt32(reader["Mutations"]) : 0;
                            kpi.TotalMonthlyExports = reader["Exports"] != DBNull.Value ? Convert.ToInt32(reader["Exports"]) : 0;
                        }
                    }

                    // 5. Top Feature
                    string sqlTop = @"
                        SELECT TOP 1 ControllerName, COUNT(*) as Hits
                        FROM [dbo].[BVTL_FEATURE_USAGE_LOG]
                        WHERE CreatedDate >= DATEADD(day, -30, GETDATE())
                        GROUP BY ControllerName
                        ORDER BY Hits DESC";

                    using (SqlCommand cmd = new SqlCommand(sqlTop, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string topCtrl = reader["ControllerName"].ToString();
                            kpi.TopFeatureName = GetFeatureDisplayName(topCtrl);
                            kpi.TopFeatureHits = Convert.ToInt32(reader["Hits"]);
                        }
                        else
                        {
                            kpi.TopFeatureName = "Chưa có dữ liệu";
                            kpi.TopFeatureHits = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("SystemMonitorDA - GetDashboardKPIs Error: " + ex.Message, ex);
            }
            return kpi;
        }

        private string GetFeatureDisplayName(string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName)) return "Chưa xác định";
            switch (controllerName.ToLower())
            {
                case "home": return "Bảng điều khiển (Dashboard)";
                case "customer": case "khachhang": return "Quản lý Khách hàng";
                case "phieutuvan": return "Phiếu tư vấn";
                case "chuyenguidichvu": return "Chuyển gửi dịch vụ";
                case "ketquaxnnt": return "Xét nghiệm Nước tiểu";
                case "ketquahiv": return "Xét nghiệm HIV";
                case "ketquaassist": return "Sàng lọc ASSIST";
                case "ketquaace": return "Bảng hỏi ACE";
                case "ketquasktt": return "Sức khỏe Tâm thần (SKTT)";
                case "baocaotuan": return "Báo cáo Tuần";
                case "baocaothang": return "Báo cáo Tháng";
                case "baocao6thang": return "Báo cáo 6 Tháng";
                case "baocaonam": return "Báo cáo Năm";
                case "baocaotonghop": return "Báo cáo Tổng hợp";
                case "syncdata": return "Đồng bộ Dữ liệu (Auto-Sync)";
                case "user": return "Quản lý Người dùng";
                case "role": return "Quản lý Nhóm quyền";
                case "testgroup": return "Nhóm Thu thập Dữ liệu";
                case "city": return "Danh mục Tỉnh/Thành";
                case "sysparameter": return "Tham số Hệ thống";
                case "systemmonitor": return "Giám sát & Thống kê Hệ thống";
                default: return controllerName;
            }
        }

        private string GetFeatureCategory(string controllerName)
        {
            if (string.IsNullOrEmpty(controllerName)) return "Khác";
            string ctrl = controllerName.ToLower();
            if (ctrl.Contains("baocao")) return "Báo cáo & Thống kê";
            if (ctrl.Contains("ketqua") || ctrl.Contains("chatgaynghien")) return "Xét nghiệm & Sàng lọc";
            if (ctrl.Contains("customer") || ctrl.Contains("khachhang") || ctrl.Contains("phieutuvan") || ctrl.Contains("chuyengui")) return "Nghiệp vụ Chăm sóc";
            if (ctrl.Contains("user") || ctrl.Contains("role") || ctrl.Contains("sys") || ctrl.Contains("sync") || ctrl.Contains("testgroup") || ctrl.Contains("city")) return "Quản trị Hệ thống";
            return "Chung";
        }
        #endregion
    }
}
