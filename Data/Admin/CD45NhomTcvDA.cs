using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class CD45NhomTcvDA : ICD45NhomTcvDA
    {
        private BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        public List<CD45_NhomTcvViewModel> GetListNhomTcv(string cityCode, string maNhom, string keyword)
        {
            var sql = @"
                WITH AllNhomTcv AS (
                    SELECT 
                        tcv.ID,
                        RTRIM(tcv.MA_NHOM) AS MA_NHOM,
                        tcv.TEN_NHOM,
                        RTRIM(tcv.CITY_CODE) AS CITY_CODE,
                        ISNULL(c.Name, tcv.CITY_CODE) AS CityName,
                        RTRIM(tcv.MA_TCV) AS MA_TCV,
                        tcv.TEN_TCV,
                        tcv.MADUAN,
                        tcv.IsActive,
                        ISNULL(tcv.CreatedDate, GETDATE()) AS CreatedDate,
                        ISNULL(n.PREFIX, ISNULL(tcv.PREFIX, N'Nhóm')) AS PREFIX,
                        ISNULL(n.SHORT_PREFIX, ISNULL(tcv.SHORT_PREFIX, N'Nhóm')) AS SHORT_PREFIX,
                        ISNULL(n.CHUC_DANH, ISNULL(tcv.CHUC_DANH, N'Trưởng nhóm')) AS CHUC_DANH
                    FROM CD45_NHOM_TCV tcv
                    LEFT JOIN BVTL_CITES c ON tcv.CITY_CODE = c.Code
                    LEFT JOIN BVTL_NHOM_TBH n ON (tcv.MA_NHOM = n.manhom_tbh OR (n.manhom_tbh_map IS NOT NULL AND tcv.MA_NHOM = n.manhom_tbh_map))
                    WHERE tcv.MADUAN = 'CD45'

                    UNION ALL

                    SELECT 
                        -CAST(ROW_NUMBER() OVER (ORDER BY n.manhom_tbh) AS INT) AS ID,
                        RTRIM(n.manhom_tbh) AS MA_NHOM,
                        n.tennhom_tbh AS TEN_NHOM,
                        RTRIM(n.city_code) AS CITY_CODE,
                        ISNULL(c.Name, n.city_code) AS CityName,
                        CAST(NULL AS VARCHAR(50)) AS MA_TCV,
                        CAST(NULL AS NVARCHAR(150)) AS TEN_TCV,
                        n.maduan AS MADUAN,
                        CAST(0 AS BIT) AS IsActive,
                        CAST(GETDATE() AS DATETIME) AS CreatedDate,
                        ISNULL(n.PREFIX, N'Nhóm') AS PREFIX,
                        ISNULL(n.SHORT_PREFIX, N'Nhóm') AS SHORT_PREFIX,
                        ISNULL(n.CHUC_DANH, N'Trưởng nhóm') AS CHUC_DANH
                    FROM BVTL_NHOM_TBH n
                    LEFT JOIN BVTL_CITES c ON n.city_code = c.Code
                    WHERE n.maduan = 'CD45'
                      AND NOT EXISTS (
                          SELECT 1 FROM CD45_NHOM_TCV t 
                          WHERE t.MADUAN = 'CD45' 
                            AND (t.MA_NHOM = n.manhom_tbh OR (n.manhom_tbh_map IS NOT NULL AND t.MA_NHOM = n.manhom_tbh_map))
                      )
                )
                SELECT * FROM AllNhomTcv tcv WHERE 1=1";

            var pList = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(cityCode))
            {
                sql += " AND tcv.CITY_CODE = @CityCode";
                pList.Add(new SqlParameter("@CityCode", cityCode));
            }
            if (!string.IsNullOrEmpty(maNhom))
            {
                var nhom = db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == maNhom || x.manhom_tbh_map == maNhom);
                string map = nhom != null && !string.IsNullOrEmpty(nhom.manhom_tbh_map) ? nhom.manhom_tbh_map : maNhom;
                string std = nhom != null && !string.IsNullOrEmpty(nhom.manhom_tbh) ? nhom.manhom_tbh : maNhom;
                sql += " AND tcv.MA_NHOM IN (@MaNhom, @MaNhomMap, @MaNhomStd)";
                pList.Add(new SqlParameter("@MaNhom", maNhom));
                pList.Add(new SqlParameter("@MaNhomMap", map));
                pList.Add(new SqlParameter("@MaNhomStd", std));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (tcv.TEN_TCV LIKE @Keyword OR tcv.MA_TCV LIKE @Keyword OR tcv.TEN_NHOM LIKE @Keyword OR tcv.PREFIX LIKE @Keyword)";
                pList.Add(new SqlParameter("@Keyword", "%" + keyword.Trim() + "%"));
            }
            sql += " ORDER BY tcv.CITY_CODE, tcv.MA_NHOM, CASE WHEN tcv.MA_TCV IS NULL THEN 1 ELSE 0 END, TRY_CAST(tcv.MA_TCV AS INT), tcv.TEN_TCV";

            return db.Database.SqlQuery<CD45_NhomTcvViewModel>(sql, pList.ToArray()).ToList();
        }

        public CD45_NhomTcvKpiModel GetKpiStats()
        {
            var sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45') AS TongTCV,
                    (SELECT COUNT(DISTINCT manhom_tbh) FROM BVTL_NHOM_TBH WHERE maduan = 'CD45') AS TongNhom,
                    (SELECT COUNT(DISTINCT city_code) FROM BVTL_NHOM_TBH WHERE maduan = 'CD45') AS TongTinh,
                    (SELECT ISNULL(SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END), 0) FROM CD45_NHOM_TCV WHERE MADUAN = 'CD45') AS TcvActive";

            return db.Database.SqlQuery<CD45_NhomTcvKpiModel>(sql).FirstOrDefault() ?? new CD45_NhomTcvKpiModel();
        }

        public List<CD45_NhomTcvViewModel> GetAllForExport(string cityCode, string maNhom)
        {
            return GetListNhomTcv(cityCode, maNhom, null);
        }
    }
}
