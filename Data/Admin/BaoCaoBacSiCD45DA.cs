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

        private static readonly Dictionary<string, string> DictHospital = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "1", "Hà Nội - Bệnh viện Lão khoa" },
            { "2", "Hưng Yên - BV SKTT Thái Bình" },
            { "3", "Hưng Yên - PK Meheal" },
            { "4", "Hà Nội - Phòng khám Dr Phi" },
            { "5", "Ninh Bình - BV SKTT Ninh Bình" },
            { "6", "Bệnh viện tâm thần Nghệ An" },
            { "7", "Bệnh viện SKTT Hải Phòng" },
            { "8", "Bệnh viện tâm thần TP.HCM" },
            { "9", "Bệnh viện Thủ Đức" }
        };

        private static readonly Dictionary<string, string> DictDiagnose = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "1", "F00- Sa sút trí tuệ" },
            { "2", "F10- Rối loạn tâm thần do rượu" },
            { "3", "F10.0- Nhiễm độc rượu cấp" },
            { "4", "F10.1- Sử dụng rượu gây hại" },
            { "5", "F10.2- Hội chứng nghiện rượu" },
            { "6", "F10.3- Trạng thái cai rượu" },
            { "7", "F10.4- Trạng thái cai với mê sảng" },
            { "8", "F10.5- Rối loạn tâm thần do rượu" },
            { "9", "F10.6- Hội chứng quên do rượu" },
            { "10", "F10.7- Di chứng loạn thần rượu" },
            { "11", "F10.8- Rối loạn tâm thần rượu khác" },
            { "12", "F10.9- Rối loạn tâm thần rượu KBD" },
            { "13", "F11- Rối loạn tâm thần do thuốc phiện" },
            { "14", "F11.0- Nhiễm độc thuốc phiện cấp" },
            { "15", "F11.1- Sử dụng thuốc phiện gây hại" },
            { "16", "F11.2- Hội chứng nghiện thuốc phiện" },
            { "17", "F11.3- Trạng thái cai thuốc phiện" },
            { "18", "F11.4- Cai thuốc phiện với mê sảng" },
            { "19", "F11.5- Rối loạn tâm thần do thuốc phiện" },
            { "20", "F11.6- Hội chứng quên do thuốc phiện" },
            { "21", "F11.7- Di chứng loạn thần thuốc phiện" },
            { "22", "F11.8- Rối loạn khác do thuốc phiện" },
            { "23", "F11.9- Rối loạn thuốc phiện KBD" },
            { "24", "F12- Rối loạn tâm thần do cần sa" },
            { "25", "F12.0- Nhiễm độc cần sa cấp" },
            { "26", "F12.1- Sử dụng cần sa gây hại" },
            { "27", "F12.2- Hội chứng nghiện cần sa" },
            { "28", "F12.3- Trạng thái cai cần sa" },
            { "29", "F12.4- Cai cần sa với mê sảng" },
            { "30", "F12.5- Rối loạn tâm thần do cần sa" },
            { "31", "F12.6- Hội chứng quên do cần sa" },
            { "32", "F12.7- Di chứng loạn thần cần sa" },
            { "33", "F12.8- Rối loạn cần sa khác" },
            { "34", "F12.9- Rối loạn cần sa KBD" },
            { "35", "F13- Rối loạn do chất an thần/ngủ" },
            { "36", "F13.0- Nhiễm độc chất an thần cấp" },
            { "37", "F13.1- Sử dụng chất an thần gây hại" },
            { "38", "F13.2- Nghiện chất an thần" },
            { "39", "F13.3- Trạng thái cai chất an thần" },
            { "40", "F13.4- Cai chất an thần với mê sảng" },
            { "41", "F13.5- Rối loạn tâm thần do an thần" },
            { "42", "F13.6- Hội chứng quên do an thần" },
            { "43", "F13.7- Di chứng loạn thần an thần" },
            { "44", "F13.8- Rối loạn an thần khác" },
            { "45", "F13.9- Rối loạn an thần KBD" },
            { "46", "F14- Rối loạn tâm thần do cocain" },
            { "57", "F15- Rối loạn tâm thần do chất kích thích khác (ATS/ma túy đá)" },
            { "68", "F16- Rối loạn tâm thần do chất gây ảo giác" },
            { "79", "F18- Rối loạn do dung môi bay hơi" },
            { "90", "F19- Rối loạn do sử dụng nhiều loại ma túy" },
            { "101", "F20- Tâm thần phân liệt" },
            { "102", "F21- Rối loạn loại phân liệt" },
            { "103", "F22- Rối loạn hoang tưởng dai dẳng" },
            { "104", "F23- Rối loạn loạn thần cấp" },
            { "105", "F24- Rối loạn hoang tưởng cảm ứng" },
            { "106", "F25- Rối loạn phân liệt cảm xúc" },
            { "107", "F29- Loạn thần không thực tổn KBD" },
            { "108", "F30- Giai đoạn hưng cảm" },
            { "112", "F31- Rối loạn cảm xúc lưỡng cực" },
            { "113", "F32- Giai đoạn trầm cảm" },
            { "114", "F33- Rối loạn trầm cảm tái diễn" },
            { "115", "F34- Rối loạn khí sắc dai dẳng" },
            { "116", "F38- Rối loạn khí sắc khác" },
            { "117", "F39- Rối loạn khí sắc KBD" },
            { "118", "F40- Rối loạn lo âu ám ảnh sợ hãi" },
            { "119", "F41- Các rối loạn lo âu khác" },
            { "120", "F42- Rối loạn ám ảnh nghi thức (OCD)" },
            { "121", "F43- Phản ứng với stress và rối loạn thích ứng" },
            { "122", "F43.1- Rối loạn stress sau sang chấn (PTSD)" },
            { "123", "F44- Rối loạn phân ly" },
            { "124", "F45- Rối loạn dạng cơ thể" },
            { "125", "F50- Rối loạn ăn uống" },
            { "126", "F51- Rối loạn giấc ngủ không thực tổn" },
            { "127", "F52- Loạn chức năng tình dục" },
            { "128", "F60- Rối loạn nhân cách đặc hiệu" },
            { "129", "F70- Chậm phát triển tâm thần nhẹ" },
            { "130", "F90- Rối loạn tăng động" },
            { "131", "F41.2- Rối loạn hỗn hợp lo âu và trầm cảm" },
            { "132", "Khác" }
        };

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
