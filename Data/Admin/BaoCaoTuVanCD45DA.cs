using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class BaoCaoTuVanCD45DA : IBaoCaoTuVanCD45DA
    {
        private readonly BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        public List<CD45BaoCaoTuVanL1Model> GetBaoCaoTuVanL1(string fromDate, string toDate, string cityCode = null, string maNhom = null, string maTCV = null)
        {
            var pFromDate = new SqlParameter("@FromDate", string.IsNullOrWhiteSpace(fromDate) ? (object)DBNull.Value : fromDate.Trim());
            var pToDate = new SqlParameter("@ToDate", string.IsNullOrWhiteSpace(toDate) ? (object)DBNull.Value : toDate.Trim());
            var pCityCode = new SqlParameter("@CityCode", string.IsNullOrWhiteSpace(cityCode) ? (object)DBNull.Value : cityCode.Trim());
            var pMaNhom = new SqlParameter("@MaNhom", string.IsNullOrWhiteSpace(maNhom) ? (object)DBNull.Value : maNhom.Trim());
            var pMaTCV = new SqlParameter("@MaTCV", string.IsNullOrWhiteSpace(maTCV) ? (object)DBNull.Value : maTCV.Trim());

            return db.Database.SqlQuery<CD45BaoCaoTuVanL1Model>(
                "EXEC SP_CD45_GetBaoCaoTuVan_L1 @FromDate, @ToDate, @CityCode, @MaNhom, @MaTCV",
                pFromDate, pToDate, pCityCode, pMaNhom, pMaTCV
            ).ToList();
        }

        public List<CD45BaoCaoTuVanL2Model> GetBaoCaoTuVanL2(string fromDate, string toDate, string cityCode = null, string maNhom = null, string maTCV = null)
        {
            var pFromDate = new SqlParameter("@FromDate", string.IsNullOrWhiteSpace(fromDate) ? (object)DBNull.Value : fromDate.Trim());
            var pToDate = new SqlParameter("@ToDate", string.IsNullOrWhiteSpace(toDate) ? (object)DBNull.Value : toDate.Trim());
            var pCityCode = new SqlParameter("@CityCode", string.IsNullOrWhiteSpace(cityCode) ? (object)DBNull.Value : cityCode.Trim());
            var pMaNhom = new SqlParameter("@MaNhom", string.IsNullOrWhiteSpace(maNhom) ? (object)DBNull.Value : maNhom.Trim());
            var pMaTCV = new SqlParameter("@MaTCV", string.IsNullOrWhiteSpace(maTCV) ? (object)DBNull.Value : maTCV.Trim());

            return db.Database.SqlQuery<CD45BaoCaoTuVanL2Model>(
                "EXEC SP_CD45_GetBaoCaoTuVan_L2 @FromDate, @ToDate, @CityCode, @MaNhom, @MaTCV",
                pFromDate, pToDate, pCityCode, pMaNhom, pMaTCV
            ).ToList();
        }
    }
}
