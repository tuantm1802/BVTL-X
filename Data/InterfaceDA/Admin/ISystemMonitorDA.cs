using Model.ModelExtend.Base;
using Model.ModelExtend.SystemMonitor;
using System.Collections.Generic;

namespace Data.InterfaceDA.Admin
{
    public interface ISystemMonitorDA
    {
        void InitTablesIfNotExist();
        void RecordLogin(UserLogin user, string ip, string userAgent, string status, string sessionId);
        void RecordLogout(string sessionId);
        void UpdateHeartbeat(string sessionId, string currentUrl, string controllerName, string actionName);
        void RecordFeatureUsage(string userName, string fullName, string controller, string action, string actionType, int? executionTimeMs, bool isError, string errorMsg);
        
        List<UserOnlineModel> GetOnlineUsers(string keyword, int page, int pageSize, ref int totalRow);
        List<LoginHistoryModel> GetLoginHistory(SystemMonitorSearchModel search, ref int totalRow);
        TrafficStatsModel GetTrafficStats(int days = 7);
        
        List<FeatureUsageModel> GetFeatureUsageStats(SystemMonitorSearchModel search, ref int totalRow);
        List<FeatureRankModel> GetTopFeatures(int top = 10, int days = 30);
        List<FeatureRankModel> GetUnderutilizedFeatures(int days = 30);
        Dictionary<string, int> GetRoleUsageDistribution(int days = 30);
        SystemMonitorDashboardKPIs GetDashboardKPIs();
    }
}
