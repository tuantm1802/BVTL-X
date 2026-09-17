using System;
using System.Collections.Generic;
using Model.ModelExtend.API.CD45;

namespace Data.InterfaceDA
{
    public interface IDataQualityDA
    {
        List<BVTL_DATA_STANDARDIZATION_LOG_Entity> GetLogs(string maDuAn, string apiCode, string severity, string keyword, bool? isResolved, int pageIndex, int pageSize, out int totalRows);
        List<GroupedDataQualityLogModel> GetGroupedLogs(string maDuAn, string severity);
        List<DataQualityStatsByNhomModel> GetStatsByNhom(string maDuAn);
        dynamic GetStats(string maDuAn);
        bool MarkResolved(long id, string note);
        int ScanDuplicateClients(string maDuAn);
    }
}
