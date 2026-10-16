using System;

namespace Model.ModelExtend
{
    public class CD45BaoCaoTuVanL1Model
    {
        public long ID { get; set; }
        public string RECORD_ID { get; set; }
        public string MA_KH_NGHIEN_CUU { get; set; }
        public string CITY_CODE { get; set; }
        public string CITY_NAME { get; set; }
        public string MA_NHOM { get; set; }
        public string TEN_NHOM { get; set; }
        public string PREFIX { get; set; }
        public string SHORT_PREFIX { get; set; }
        public string CHUC_DANH { get; set; }
        public string MA_TCV { get; set; }
        public string TEN_TCV { get; set; }
        public DateTime? NGAY_TU_VAN { get; set; }
        public string NGAY_TU_VAN_STR => NGAY_TU_VAN.HasValue ? NGAY_TU_VAN.Value.ToString("dd/MM/yyyy") : "";
        public string DIA_DIEM_TEXT { get; set; }
        public string Q3A_TEXT { get; set; }
        public string Q3A_KHAC { get; set; }
        public string Q3B_TEXT { get; set; }
        public string Q3B_KHAC { get; set; }
        public string Q3C_NHU_CAU { get; set; }
        public string Q41_CAN_THIEP_GTH { get; set; }
        public string Q42_NHU_CAU_TG { get; set; }
        public string Q42_KHAC { get; set; }
        public DateTime? NGAY_HEN_TIEP { get; set; }
        public string NGAY_HEN_TIEP_STR => NGAY_HEN_TIEP.HasValue ? NGAY_HEN_TIEP.Value.ToString("dd/MM/yyyy") : "";
        public string COMPLETE_STATUS { get; set; }
    }

    public class CD45BaoCaoTuVanL2Model
    {
        public long ID { get; set; }
        public string RECORD_ID { get; set; }
        public string MA_KH_NGHIEN_CUU { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string LAN_TU_VAN { get; set; }
        public string CITY_CODE { get; set; }
        public string CITY_NAME { get; set; }
        public string MA_NHOM { get; set; }
        public string TEN_NHOM { get; set; }
        public string PREFIX { get; set; }
        public string SHORT_PREFIX { get; set; }
        public string CHUC_DANH { get; set; }
        public string MA_TCV { get; set; }
        public string TEN_TCV { get; set; }
        public DateTime? NGAY_TU_VAN { get; set; }
        public string NGAY_TU_VAN_STR => NGAY_TU_VAN.HasValue ? NGAY_TU_VAN.Value.ToString("dd/MM/yyyy") : "";
        public string DIA_DIEM_TEXT { get; set; }
        public string Q21A_CAM_THAY { get; set; }
        public string Q21A_KHAC { get; set; }
        public string Q21B_CHIA_SE { get; set; }
        public string Q22_VAN_DE { get; set; }
        public string Q31A_CAM_XUC { get; set; }
        public string Q31B_CANG_THANG_NGAN { get; set; }
        public string Q31C_CANG_THANG_DAI { get; set; }
        public string Q31D_THOI_QUEN { get; set; }
        public string Q31E_NOI_DUNG_KHAC { get; set; }
        public string Q31E1_KHAC { get; set; }
        public string Q32_CAN_THIEP_CHINH { get; set; }
        public string Q41_DONG_Y { get; set; }
        public string Q421_VIEC_LAM { get; set; }
        public string Q422_TAN_SUAT { get; set; }
        public DateTime? NGAY_HEN_TIEP { get; set; }
        public string NGAY_HEN_TIEP_STR => NGAY_HEN_TIEP.HasValue ? NGAY_HEN_TIEP.Value.ToString("dd/MM/yyyy") : "";
        public string COMPLETE_STATUS { get; set; }
    }

    public class CD45BaoCaoTuVanFilterModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string CityCode { get; set; }
        public string MaNhom { get; set; }
        public string MaTCV { get; set; }
        public string LoaiTuVan { get; set; } // "L1", "L2", "ALL"
    }
}
