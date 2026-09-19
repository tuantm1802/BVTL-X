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

            var list = db.Database.SqlQuery<DataQualityStatsByNhomModel>(sql, pDuAn).ToList();
            EnrichStatsWithNames(list);
            return list;
        }

        private void EnrichStatsWithNames(List<DataQualityStatsByNhomModel> list)
        {
            if (list == null || list.Count == 0) return;

            var dictCity = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "HNO", "Hà Nội" }, { "HN", "Hà Nội" },
                { "HPG", "Hải Phòng" }, { "HP", "Hải Phòng" },
                { "HCM", "TP Hồ Chí Minh" }, { "HC", "TP Hồ Chí Minh" },
                { "NAN", "Nghệ An" }, { "NA", "Nghệ An" },
                { "HYE", "Hưng Yên" }, { "HY", "Hưng Yên" },
                { "NBI", "Ninh Bình" }, { "NB", "Ninh Bình" },
                { "KHA", "Khánh Hòa" }, { "NT", "Nha Trang" },
                { "LU", "Cụm hồ sơ TCV" },
                { "AT", "Gom nhóm tự động" }
            };

            try
            {
                var cities = db.Database.SqlQuery<LookupItem>("SELECT RTRIM(Code) AS Code, RTRIM(Name) AS Name FROM BVTL_CITES").ToList();
                foreach (var c in cities)
                {
                    if (!string.IsNullOrEmpty(c.Code) && !string.IsNullOrEmpty(c.Name) && !dictCity.ContainsKey(c.Code))
                    {
                        dictCity[c.Code] = c.Name;
                    }
                }
            }
            catch { }

            var dictNhom = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "UNKNOWN", "Chưa phân nhóm" }
            };

            try
            {
                var groups = db.Database.SqlQuery<NhomLookupItem>("SELECT RTRIM(manhom_tbh) AS Code1, RTRIM(manhom_tbh_map) AS Code2, RTRIM(tennhom_tbh) AS Name FROM BVTL_NHOM_TBH WHERE maduan = 'CD45'").ToList();
                foreach (var g in groups)
                {
                    if (!string.IsNullOrEmpty(g.Name))
                    {
                        if (!string.IsNullOrEmpty(g.Code1)) dictNhom[g.Code1] = g.Name;
                        if (!string.IsNullOrEmpty(g.Code2)) dictNhom[g.Code2] = g.Name;
                    }
                }

                var tcvGroups = db.Database.SqlQuery<LookupItem>("SELECT DISTINCT RTRIM(MA_NHOM) AS Code, RTRIM(TEN_NHOM) AS Name FROM CD45_NHOM_TCV WHERE TEN_NHOM IS NOT NULL AND TEN_NHOM <> ''").ToList();
                foreach (var tg in tcvGroups)
                {
                    if (!string.IsNullOrEmpty(tg.Code) && !string.IsNullOrEmpty(tg.Name) && !dictNhom.ContainsKey(tg.Code))
                    {
                        dictNhom[tg.Code] = tg.Name;
                    }
                }
            }
            catch { }

            foreach (var item in list)
            {
                string cityCode = (item.CITY_CODE ?? "").Trim();
                string nhomCode = (item.MA_NHOM ?? "").Trim();

                if (dictCity.TryGetValue(cityCode, out var cityName))
                {
                    item.TEN_TINH = cityName;
                }
                else
                {
                    item.TEN_TINH = !string.IsNullOrEmpty(cityCode) ? ("Mã tỉnh " + cityCode) : "Chưa xác định";
                }

                if (dictNhom.TryGetValue(nhomCode, out var nhomName))
                {
                    item.TEN_NHOM = nhomName;
                }
                else
                {
                    item.TEN_NHOM = nhomCode != "UNKNOWN" && !string.IsNullOrEmpty(nhomCode) ? ("Nhóm " + nhomCode) : "Chưa phân nhóm";
                }
            }
        }

        private class LookupItem
        {
            public string Code { get; set; }
            public string Name { get; set; }
        }

        private class NhomLookupItem
        {
            public string Code1 { get; set; }
            public string Code2 { get; set; }
            public string Name { get; set; }
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

        public List<BVTL_DATA_STANDARDIZATION_LOG_Entity> GetLogsByUnit(string maDuAn, string cityCode, string maNhom, string metricType)
        {
            var pDuAn = new SqlParameter("@MaDuAn", string.IsNullOrEmpty(maDuAn) ? DBNull.Value : (object)maDuAn);
            var pCity = new SqlParameter("@CityCode", string.IsNullOrEmpty(cityCode) || cityCode == "-" ? DBNull.Value : (object)cityCode);
            var pNhom = new SqlParameter("@MaNhom", string.IsNullOrEmpty(maNhom) ? DBNull.Value : (object)maNhom);
            var pMetric = new SqlParameter("@MetricType", string.IsNullOrEmpty(metricType) ? DBNull.Value : (object)metricType.ToUpper());

            var sql = @"
                ;WITH CTE_LogNhom AS (
                    SELECT 
                        log.ID,
                        log.MADUAN,
                        log.REPORT_ID,
                        log.API_CODE,
                        log.TABLE_NAME,
                        log.RECORD_ID,
                        log.FIELD_NAME,
                        log.OLD_VALUE,
                        log.NEW_VALUE,
                        log.RULE_CODE,
                        log.SEVERITY,
                        log.ACTION_TAKEN,
                        log.MESSAGE,
                        log.CREATED_DATE,
                        log.IS_RESOLVED,
                        log.RESOLVED_NOTE,
                        ISNULL(kh.MA_NHOM, 'UNKNOWN') AS MA_NHOM,
                        ISNULL(kh.CITY_CODE, SUBSTRING(log.RECORD_ID, 2, 2)) AS CITY_CODE
                    FROM BVTL_DATA_STANDARDIZATION_LOG log
                    LEFT JOIN CD45_KH kh ON log.RECORD_ID = kh.RECORD_ID AND log.MADUAN = kh.MADUAN
                    WHERE (@MaDuAn IS NULL OR log.MADUAN = @MaDuAn)
                )
                SELECT 
                    ID, MADUAN, REPORT_ID, API_CODE, TABLE_NAME, RECORD_ID, FIELD_NAME, 
                    OLD_VALUE, NEW_VALUE, RULE_CODE, SEVERITY, ACTION_TAKEN, MESSAGE, 
                    CREATED_DATE, IS_RESOLVED, RESOLVED_NOTE
                FROM CTE_LogNhom
                WHERE (@CityCode IS NULL OR CITY_CODE = @CityCode)
                  AND (@MaNhom IS NULL OR MA_NHOM = @MaNhom)
                  AND (
                      (@MetricType = 'PENDING' AND IS_RESOLVED = 0 AND (SEVERITY = 'WARNING' OR SEVERITY = 'ERROR'))
                      OR (@MetricType = 'WARNING' AND SEVERITY = 'WARNING')
                      OR (@MetricType = 'ERROR' AND SEVERITY = 'ERROR')
                      OR (@MetricType = 'RESOLVED' AND IS_RESOLVED = 1)
                      OR (@MetricType IS NULL OR @MetricType = '' OR @MetricType = 'ALL')
                  )
                ORDER BY CREATED_DATE DESC, ID DESC";

            return db.Database.SqlQuery<BVTL_DATA_STANDARDIZATION_LOG_Entity>(sql, pDuAn, pCity, pNhom, pMetric).ToList();
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
