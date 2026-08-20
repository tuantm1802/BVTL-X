using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.SystemMonitor
{
    /// <summary>
    /// Model đại diện phiên người dùng đang Online/Hoạt động thời gian thực
    /// </summary>
    public class UserOnlineModel
    {
        public string SessionId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string CityCodes { get; set; }
        public string MaDuAn { get; set; }
        public string IpAddress { get; set; }
        public string Browser { get; set; }
        public string OperatingSystem { get; set; }
        public string DeviceType { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime LastActiveTime { get; set; }
        public string CurrentController { get; set; }
        public string CurrentAction { get; set; }
        public string CurrentUrl { get; set; }
        public string Status { get; set; } // 'Online', 'Idle', 'Offline'
        
        // Calculated helper properties
        public string LoginTimeText => LoginTime.ToString("dd/MM/yyyy HH:mm:ss");
        public string LastActiveTimeText => LastActiveTime.ToString("dd/MM/yyyy HH:mm:ss");
        public double InactiveMinutes => Math.Round((DateTime.Now - LastActiveTime).TotalMinutes, 1);
        public bool IsOnline => InactiveMinutes <= 2.0;
        public bool IsIdle => InactiveMinutes > 2.0 && InactiveMinutes <= 10.0;
    }

    /// <summary>
    /// Model lịch sử đăng nhập
    /// </summary>
    public class LoginHistoryModel
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Browser { get; set; }
        public string OperatingSystem { get; set; }
        public string DeviceType { get; set; }
        public string Status { get; set; } // 'Success', 'WrongPassword', 'AccountLocked', 'UserNotFound'
        public string SessionId { get; set; }

        public string LoginTimeText => LoginTime.ToString("dd/MM/yyyy HH:mm:ss");
        public string LogoutTimeText => LogoutTime.HasValue ? LogoutTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "—";
        public string SessionDurationText
        {
            get
            {
                if (!LogoutTime.HasValue) return "Đang hoạt động / Tự hết hạn";
                var span = LogoutTime.Value - LoginTime;
                if (span.TotalHours >= 1)
                    return $"{Math.Floor(span.TotalHours)} giờ {span.Minutes} phút";
                return $"{span.Minutes} phút {span.Seconds} giây";
            }
        }
    }

    /// <summary>
    /// Model thống kê lượt sử dụng tính năng / module
    /// </summary>
    public class FeatureUsageModel
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ActionType { get; set; } // 'View', 'Search', 'Create', 'Update', 'Delete', 'ExportExcel', 'Sync'
        public int? ExecutionTimeMs { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime AccessDate { get; set; }
        public DateTime CreatedDate { get; set; }

        // Aggregated fields
        public string ModuleName { get; set; }
        public int HitCount { get; set; }
        public int TotalViews { get; set; }
        public int TotalMutations { get; set; } // Thêm, Sửa, Xóa
        public int TotalExports { get; set; }
        public double AvgExecutionTimeMs { get; set; }
        public int ErrorCount { get; set; }
        public DateTime? LastAccessedTime { get; set; }
        public string LastAccessedTimeText => LastAccessedTime.HasValue ? LastAccessedTime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "—";
    }

    /// <summary>
    /// Model tìm kiếm / lọc dữ liệu giám sát hệ thống
    /// </summary>
    public class SystemMonitorSearchModel
    {
        public string Keyword { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Status { get; set; }
        public string ControllerName { get; set; }
        public string ActionType { get; set; }
        public string RoleId { get; set; }
        public string CityCode { get; set; }
        public int currentPage { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string SortColumn { get; set; }
    }

    /// <summary>
    /// Model thống kê tổng quan KPI giám sát
    /// </summary>
    public class SystemMonitorDashboardKPIs
    {
        // KPIs Giám sát Truy cập
        public int OnlineUsersCount { get; set; }
        public int IdleUsersCount { get; set; }
        public int DailyActiveUsers { get; set; } // DAU
        public int TodayPageViews { get; set; }
        public int TodayFailedLogins { get; set; }

        // KPIs Khai thác Module
        public string TopFeatureName { get; set; }
        public int TopFeatureHits { get; set; }
        public int TotalMonthlyMutations { get; set; }
        public int TotalMonthlyExports { get; set; }
        public double SystemAvgResponseTimeMs { get; set; }
        public int TotalUnderutilizedFeaturesCount { get; set; }
    }

    /// <summary>
    /// Model thống kê lưu lượng theo thời gian & thiết bị
    /// </summary>
    public class TrafficStatsModel
    {
        public List<string> TimeLabels { get; set; } = new List<string>();
        public List<int> PageViewsData { get; set; } = new List<int>();
        public List<int> UniqueUsersData { get; set; } = new List<int>();
        
        public Dictionary<string, int> DeviceDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> BrowserDistribution { get; set; } = new Dictionary<string, int>();
    }

    /// <summary>
    /// Model bảng xếp hạng Top chức năng & chức năng ngủ đông
    /// </summary>
    public class FeatureRankModel
    {
        public string ControllerName { get; set; }
        public string FeatureName { get; set; }
        public string Category { get; set; }
        public int TotalHits { get; set; }
        public int TotalUsers { get; set; }
        public int ViewCount { get; set; }
        public int MutationCount { get; set; }
        public int ExportCount { get; set; }
        public double AvgDurationMs { get; set; }
        public int ErrorCount { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public string LastUsedDateText => LastUsedDate.HasValue ? LastUsedDate.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa từng sử dụng";
        public int DaysInactive { get; set; }
    }
}
