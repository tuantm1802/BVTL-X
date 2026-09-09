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
                SELECT 
                    tcv.ID,
                    tcv.MA_NHOM,
                    tcv.TEN_NHOM,
                    tcv.CITY_CODE,
                    ISNULL(c.Name, tcv.CITY_CODE) AS CityName,
                    tcv.MA_TCV,
                    tcv.TEN_TCV,
                    tcv.MADUAN,
                    tcv.IsActive,
                    tcv.CreatedDate
                FROM CD45_NHOM_TCV tcv
                LEFT JOIN BVTL_CITES c ON tcv.CITY_CODE = c.Code
                WHERE tcv.MADUAN = 'CD45'";

            var pList = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(cityCode))
            {
                sql += " AND tcv.CITY_CODE = @CityCode";
                pList.Add(new SqlParameter("@CityCode", cityCode));
            }
            if (!string.IsNullOrEmpty(maNhom))
            {
                sql += " AND tcv.MA_NHOM = @MaNhom";
                pList.Add(new SqlParameter("@MaNhom", maNhom));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (tcv.TEN_TCV LIKE @Keyword OR tcv.MA_TCV LIKE @Keyword OR tcv.TEN_NHOM LIKE @Keyword)";
                pList.Add(new SqlParameter("@Keyword", "%" + keyword.Trim() + "%"));
            }
            sql += " ORDER BY tcv.CITY_CODE, tcv.MA_NHOM, TRY_CAST(tcv.MA_TCV AS INT), tcv.TEN_TCV";

            return db.Database.SqlQuery<CD45_NhomTcvViewModel>(sql, pList.ToArray()).ToList();
        }

        public CD45_NhomTcvKpiModel GetKpiStats()
        {
            var sql = @"
                SELECT 
                    COUNT(*) AS TongTCV,
                    COUNT(DISTINCT MA_NHOM) AS TongNhom,
                    COUNT(DISTINCT CITY_CODE) AS TongTinh,
                    ISNULL(SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END), 0) AS TcvActive
                FROM CD45_NHOM_TCV
                WHERE MADUAN = 'CD45'";

            return db.Database.SqlQuery<CD45_NhomTcvKpiModel>(sql).FirstOrDefault() ?? new CD45_NhomTcvKpiModel();
        }

        public List<CD45_NhomTcvViewModel> GetAllForExport(string cityCode, string maNhom)
        {
            return GetListNhomTcv(cityCode, maNhom, null);
        }
    }
}
