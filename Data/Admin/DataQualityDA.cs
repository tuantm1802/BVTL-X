using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Data.InterfaceDA;
using Model.Model;
using Model.ModelExtend.API.CD45;

namespace Data.Admin
{
    public class DataQualityDA : IDataQualityDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        public List<BVTL_DATA_STANDARDIZATION_LOG_Entity> GetLogs(string maDuAn, string apiCode, string severity, string keyword, bool? isResolved, int pageIndex, int pageSize, out int totalRows)
        {
            var pDuAn1 = new SqlParameter("@MaDuAn", string.IsNullOrEmpty(maDuAn) ? DBNull.Value : (object)maDuAn);
            var pApi1 = new SqlParameter("@ApiCode", string.IsNullOrEmpty(apiCode) ? DBNull.Value : (object)apiCode);
            var pSev1 = new SqlParameter("@Severity", string.IsNullOrEmpty(severity) ? DBNull.Value : (object)severity);
            var pKey1 = new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword);
            var pRes1 = new SqlParameter("@IsResolved", (object)isResolved ?? DBNull.Value);

            var sqlCount = @"
                SELECT COUNT(1)
                FROM BVTL_DATA_STANDARDIZATION_LOG
                WHERE (@MaDuAn IS NULL OR MADUAN = @MaDuAn)
                  AND (@ApiCode IS NULL OR API_CODE = @ApiCode)
                  AND (@Severity IS NULL OR SEVERITY = @Severity)
                  AND (@IsResolved IS NULL OR IS_RESOLVED = @IsResolved)
                  AND (@Keyword IS NULL OR RECORD_ID LIKE '%' + @Keyword + '%' OR MESSAGE LIKE '%' + @Keyword + '%')";

            totalRows = db.Database.SqlQuery<int>(sqlCount, pDuAn1, pApi1, pSev1, pRes1, pKey1).FirstOrDefault();

            var pDuAn2 = new SqlParameter("@MaDuAn", string.IsNullOrEmpty(maDuAn) ? DBNull.Value : (object)maDuAn);
            var pApi2 = new SqlParameter("@ApiCode", string.IsNullOrEmpty(apiCode) ? DBNull.Value : (object)apiCode);
            var pSev2 = new SqlParameter("@Severity", string.IsNullOrEmpty(severity) ? DBNull.Value : (object)severity);
            var pKey2 = new SqlParameter("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : (object)keyword);
            var pRes2 = new SqlParameter("@IsResolved", (object)isResolved ?? DBNull.Value);
            var pOffset = new SqlParameter("@Offset", (pageIndex - 1) * pageSize);
            var pSize = new SqlParameter("@PageSize", pageSize);

            var sql = @"
                SELECT ID, MADUAN, REPORT_ID, API_CODE, TABLE_NAME, RECORD_ID, FIELD_NAME, 
                       OLD_VALUE, NEW_VALUE, RULE_CODE, SEVERITY, ACTION_TAKEN, MESSAGE, 
                       CREATED_DATE, IS_RESOLVED, RESOLVED_NOTE
                FROM BVTL_DATA_STANDARDIZATION_LOG
                WHERE (@MaDuAn IS NULL OR MADUAN = @MaDuAn)
                  AND (@ApiCode IS NULL OR API_CODE = @ApiCode)
                  AND (@Severity IS NULL OR SEVERITY = @Severity)
                  AND (@IsResolved IS NULL OR IS_RESOLVED = @IsResolved)
                  AND (@Keyword IS NULL OR RECORD_ID LIKE '%' + @Keyword + '%' OR MESSAGE LIKE '%' + @Keyword + '%')
                ORDER BY CREATED_DATE DESC, ID DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            return db.Database.SqlQuery<BVTL_DATA_STANDARDIZATION_LOG_Entity>(sql, pDuAn2, pApi2, pSev2, pRes2, pKey2, pOffset, pSize).ToList();
        }

        public dynamic GetStats(string maDuAn)
        {
            var pDuAn = new SqlParameter("@MaDuAn", string.IsNullOrEmpty(maDuAn) ? DBNull.Value : (object)maDuAn);
            var sql = @"
                SELECT 
                    COUNT(*) AS TotalLogs,
                    SUM(CASE WHEN SEVERITY = 'INFO' THEN 1 ELSE 0 END) AS TotalAutoCleaned,
                    SUM(CASE WHEN SEVERITY = 'WARNING' THEN 1 ELSE 0 END) AS TotalWarnings,
                    SUM(CASE WHEN SEVERITY = 'ERROR' THEN 1 ELSE 0 END) AS TotalErrors,
                    SUM(CASE WHEN IS_RESOLVED = 1 THEN 1 ELSE 0 END) AS TotalResolved,
                    SUM(CASE WHEN IS_RESOLVED = 0 AND (SEVERITY = 'WARNING' OR SEVERITY = 'ERROR') THEN 1 ELSE 0 END) AS TotalPendingAction
                FROM BVTL_DATA_STANDARDIZATION_LOG
                WHERE (@MaDuAn IS NULL OR MADUAN = @MaDuAn)";

            var res = db.Database.SqlQuery<DataQualityStatsModel>(sql, pDuAn).FirstOrDefault();
            return res ?? new DataQualityStatsModel();
        }

        public bool MarkResolved(long id, string note)
        {
            var pId = new SqlParameter("@ID", id);
            var pNote = new SqlParameter("@Note", (object)note ?? DBNull.Value);
            var sql = "UPDATE BVTL_DATA_STANDARDIZATION_LOG SET IS_RESOLVED = 1, RESOLVED_NOTE = @Note WHERE ID = @ID";
            return db.Database.ExecuteSqlCommand(sql, pId, pNote) > 0;
        }

        public List<GroupedDataQualityLogModel> GetGroupedLogs(string maDuAn, string severity)
        {
            var pDuAn = new SqlParameter("@MaDuAn", string.IsNullOrEmpty(maDuAn) ? DBNull.Value : (object)maDuAn);
            var pSev = new SqlParameter("@Severity", string.IsNullOrEmpty(severity) ? DBNull.Value : (object)severity);

            var sql = @"
                SELECT 
                    RULE_CODE,
                    SEVERITY,
                    TABLE_NAME,
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN IS_RESOLVED = 0 THEN 1 ELSE 0 END) AS UnresolvedCount,
                    MIN(CREATED_DATE) AS FirstSeen,
                    MAX(CREATED_DATE) AS LastSeen,
                    MAX(MESSAGE) AS SampleMessage
                FROM BVTL_DATA_STANDARDIZATION_LOG
                WHERE (@MaDuAn IS NULL OR MADUAN = @MaDuAn)
                  AND (@Severity IS NULL OR SEVERITY = @Severity)
                GROUP BY RULE_CODE, SEVERITY, TABLE_NAME
                ORDER BY UnresolvedCount DESC, TotalCount DESC";

            return db.Database.SqlQuery<GroupedDataQualityLogModel>(sql, pDuAn, pSev).ToList();
        }

        public List<DataQualityStatsByNhomModel> GetStatsByNhom(string maDuAn)
        {
            var pDuAn = new SqlParameter("@MaDuAn", string.IsNullOrEmpty(maDuAn) ? DBNull.Value : (object)maDuAn);

            var sql = @"
                ;WITH CTE_LogNhom AS (
                    SELECT 
                        log.ID,
                        log.SEVERITY,
                        log.IS_RESOLVED,
                        ISNULL(kh.MA_NHOM, 'UNKNOWN') AS MA_NHOM,
                        ISNULL(kh.CITY_CODE, SUBSTRING(log.RECORD_ID, 2, 2)) AS CITY_CODE
                    FROM BVTL_DATA_STANDARDIZATION_LOG log
                    LEFT JOIN CD45_KH kh ON log.RECORD_ID = kh.RECORD_ID AND log.MADUAN = kh.MADUAN
                    WHERE (@MaDuAn IS NULL OR log.MADUAN = @MaDuAn)
                )
                SELECT 
                    MA_NHOM,
                    CITY_CODE,
                    SUM(CASE WHEN SEVERITY = 'ERROR' THEN 1 ELSE 0 END) AS TotalErrors,
                    SUM(CASE WHEN SEVERITY = 'WARNING' THEN 1 ELSE 0 END) AS TotalWarnings,
                    SUM(CASE WHEN IS_RESOLVED = 1 THEN 1 ELSE 0 END) AS TotalResolved,
                    SUM(CASE WHEN IS_RESOLVED = 0 AND (SEVERITY = 'WARNING' OR SEVERITY = 'ERROR') THEN 1 ELSE 0 END) AS TotalPending
                FROM CTE_LogNhom
                GROUP BY MA_NHOM, CITY_CODE
                ORDER BY TotalPending DESC, TotalWarnings DESC";

            return db.Database.SqlQuery<DataQualityStatsByNhomModel>(sql, pDuAn).ToList();
        }

        public int ScanDuplicateClients(string maDuAn)
        {
            var pDuAn = new SqlParameter("@MaDuAn", string.IsNullOrEmpty(maDuAn) ? "CD45" : (object)maDuAn);
            try
            {
                return db.Database.SqlQuery<int>("EXEC SP_CD45_Scan_Duplicate_Clients @MaDuAn", pDuAn).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
    }

    public class DataQualityStatsModel
    {
        public int TotalLogs { get; set; }
        public int TotalAutoCleaned { get; set; }
        public int TotalWarnings { get; set; }
        public int TotalErrors { get; set; }
        public int TotalResolved { get; set; }
        public int TotalPendingAction { get; set; }
    }
}
