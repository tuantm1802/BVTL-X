using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data.Admin
{
    public class BaoCaoBacSiCD45DA : IBaoCaoBacSiCD45DA
    {
        private readonly BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        private static readonly Dictionary<string, string> DictHospital = BaoCaoCD45DA.DictHospital;
        private static readonly Dictionary<string, string> DictDiagnose = BaoCaoCD45DA.DictDiagnose;

        public List<DoctorItemModel> GetDoctors(string cityCode = null)
        {
            var sql = "SELECT MA_BAC_SI, TEN_BAC_SI, TINH_THANH, CITY_CODE FROM CD45_DM_BAC_SI WHERE ISNULL(IS_ACTIVE, 1) = 1";
            if (!string.IsNullOrEmpty(cityCode))
            {
                sql += " AND (CITY_CODE = '" + cityCode.Replace("'", "''") + "')";
            }
            sql += " ORDER BY TINH_THANH, TEN_BAC_SI";
            return db.Database.SqlQuery<DoctorItemModel>(sql).ToList();
        }

        public List<BaoCaoBacSiSummaryModel> GetBaoCaoTongHop(string fromDate, string toDate, string cityCode = null, string doctorId = null, string maNhom = null)
        {
            var pFromDate = string.IsNullOrEmpty(fromDate) ? new SqlParameter("@FromDate", DBNull.Value) : new SqlParameter("@FromDate", DateTime.ParseExact(fromDate, "dd/MM/yyyy", null));
            var pToDate = string.IsNullOrEmpty(toDate) ? new SqlParameter("@ToDate", DBNull.Value) : new SqlParameter("@ToDate", DateTime.ParseExact(toDate, "dd/MM/yyyy", null));
            var pCityCode = string.IsNullOrEmpty(cityCode) ? new SqlParameter("@CityCode", DBNull.Value) : new SqlParameter("@CityCode", cityCode);
            var pDoctorId = string.IsNullOrEmpty(doctorId) ? new SqlParameter("@DoctorId", DBNull.Value) : new SqlParameter("@DoctorId", doctorId);
            var pMaNhom = string.IsNullOrEmpty(maNhom) ? new SqlParameter("@MaNhom", DBNull.Value) : new SqlParameter("@MaNhom", maNhom);

            return db.Database.SqlQuery<BaoCaoBacSiSummaryModel>(
                "EXEC SP_CD45_GetBaoCaoBacSi_TongHop @FromDate, @ToDate, @CityCode, @DoctorId, @MaNhom",
                pFromDate, pToDate, pCityCode, pDoctorId, pMaNhom
            ).ToList();
        }

        public List<BaoCaoBacSiDetailModel> GetBaoCaoChiTiet(string fromDate, string toDate, string cityCode = null, string doctorId = null, string maNhom = null)
        {
            var pFromDate = string.IsNullOrEmpty(fromDate) ? new SqlParameter("@FromDate", DBNull.Value) : new SqlParameter("@FromDate", DateTime.ParseExact(fromDate, "dd/MM/yyyy", null));
            var pToDate = string.IsNullOrEmpty(toDate) ? new SqlParameter("@ToDate", DBNull.Value) : new SqlParameter("@ToDate", DateTime.ParseExact(toDate, "dd/MM/yyyy", null));
            var pCityCode = string.IsNullOrEmpty(cityCode) ? new SqlParameter("@CityCode", DBNull.Value) : new SqlParameter("@CityCode", cityCode);
            var pDoctorId = string.IsNullOrEmpty(doctorId) ? new SqlParameter("@DoctorId", DBNull.Value) : new SqlParameter("@DoctorId", doctorId);
            var pMaNhom = string.IsNullOrEmpty(maNhom) ? new SqlParameter("@MaNhom", DBNull.Value) : new SqlParameter("@MaNhom", maNhom);

            var list = db.Database.SqlQuery<BaoCaoBacSiDetailModel>(
                "EXEC SP_CD45_GetBaoCaoBacSi @FromDate, @ToDate, @CityCode, @DoctorId, @MaNhom",
                pFromDate, pToDate, pCityCode, pDoctorId, pMaNhom
            ).ToList();

            // Enrich Hospital and Diagnose names
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.CO_SO_Y_TE) && DictHospital.TryGetValue(item.CO_SO_Y_TE.Trim(), out var hospitalName))
                {
                    item.TEN_CO_SO_Y_TE = hospitalName;
                }
                else
                {
                    item.TEN_CO_SO_Y_TE = item.CO_SO_Y_TE ?? "-";
                }

                if (!string.IsNullOrEmpty(item.CHAN_DOAN_CHINH) && DictDiagnose.TryGetValue(item.CHAN_DOAN_CHINH.Trim(), out var diagName))
                {
                    item.TEN_CHAN_DOAN = diagName;
                }
                else
                {
                    item.TEN_CHAN_DOAN = item.CHAN_DOAN_CHINH ?? "-";
                }
            }

            return list;
        }
    }
}
