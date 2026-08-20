using Common.Common;
using Data.InterfaceDA;
using Model.Model;
using Model.ModelExtend;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class BaoCaoCD45DA : IBaoCaoCD45DA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        public List<BaoCaoCD45Model> GetBaoCao(string fromDate, string toDate, string cityCode, string maNhom, string maTCV)
        {
            var pFromDate = string.IsNullOrEmpty(fromDate) ? new SqlParameter("@FromDate", System.DBNull.Value) : new SqlParameter("@FromDate", System.DateTime.ParseExact(fromDate, "dd/MM/yyyy", null));
            var pToDate = string.IsNullOrEmpty(toDate) ? new SqlParameter("@ToDate", System.DBNull.Value) : new SqlParameter("@ToDate", System.DateTime.ParseExact(toDate, "dd/MM/yyyy", null));
            var pCityCode = string.IsNullOrEmpty(cityCode) ? new SqlParameter("@CityCode", System.DBNull.Value) : new SqlParameter("@CityCode", cityCode);
            var pMaNhom = string.IsNullOrEmpty(maNhom) ? new SqlParameter("@MaNhom", System.DBNull.Value) : new SqlParameter("@MaNhom", maNhom);
            var pMaTCV = string.IsNullOrEmpty(maTCV) ? new SqlParameter("@MaTCV", System.DBNull.Value) : new SqlParameter("@MaTCV", maTCV);

            return db.Database.SqlQuery<BaoCaoCD45Model>(
                "EXEC SP_CD45_GetBaoCao @FromDate, @ToDate, @CityCode, @MaNhom, @MaTCV",
                pFromDate, pToDate, pCityCode, pMaNhom, pMaTCV
            ).ToList();
        }

        public List<CD45_TCV_ItemModel> GetListTCV(string cityCode, string maNhom)
        {
            var sql = "SELECT ID, MA_NHOM, TEN_NHOM, CITY_CODE, MA_TCV, TEN_TCV FROM CD45_NHOM_TCV WHERE 1=1";
            if (!string.IsNullOrEmpty(cityCode))
            {
                sql += " AND (CITY_CODE = '" + cityCode.Replace("'", "''") + "')";
            }
            if (!string.IsNullOrEmpty(maNhom))
            {
                sql += " AND (MA_NHOM = '" + maNhom.Replace("'", "''") + "')";
            }
            sql += " ORDER BY CITY_CODE, MA_NHOM, TRY_CAST(MA_TCV AS INT), TEN_TCV";
            return db.Database.SqlQuery<CD45_TCV_ItemModel>(sql).ToList();
        }
    }
}
