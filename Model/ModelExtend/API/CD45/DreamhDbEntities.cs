using System;

namespace Model.ModelExtend.API.CD45
{
    public class CD45_KH_Entity {
        public string RECORD_ID { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public DateTime? NGAY_THAM_GIA { get; set; }
        public bool? THAM_GIA_NGHIEN_CUU { get; set; }
        public string MA_KH_NGHIEN_CUU { get; set; }
        public byte? GIOI_TINH_TU_XD { get; set; }
        public byte? GIOI_TINH_KHAI_SINH { get; set; }
        public short? NAM_SINH { get; set; }
        public byte? DOI_TUONG { get; set; }
        public string DOI_TUONG_KHAC { get; set; }
        public bool? CO_CCCD { get; set; }
        public bool? CO_THUONG_TRU { get; set; }
        public bool? CO_BHYT { get; set; }
        public bool? VO_GIA_CU_6T { get; set; }
        public bool? DANG_VO_GIA_CU { get; set; }
        public bool? BI_TAM_GIU_6T { get; set; }
        public byte? HON_NHAN { get; set; }
        public byte? SO_CON { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_HOAT_DONG_Entity {
        public string RECORD_ID { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public byte? LOAI_DV { get; set; }
        public DateTime? NGAY_HOAT_DONG { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public string MA_TCV { get; set; }
        public byte? DIA_DIEM { get; set; }
        public string DIA_DIEM_NGOAI { get; set; }
        public string DIA_DIEM_KHAC { get; set; }
        public string CHU_DE { get; set; }
        public int? SO_TAI_LIEU { get; set; }
        public bool? DONG_Y_QST { get; set; }
        public int? SO_BAO_CAO_SU { get; set; }
        public int? SO_CHAT_BOI_TRAN { get; set; }
        public int? SO_BOM_KIM { get; set; }
        public string GHI_CHU { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_QST_Entity {
        public string RECORD_ID { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public byte? LY_DO_DANH_GIA_LAI { get; set; }
        public DateTime? NGAY_SANG_LOC { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public string MA_TCV { get; set; }
        public byte? Q1A { get; set; }
        public byte? Q1B { get; set; }
        public byte? Q1C { get; set; }
        public byte? Q1D { get; set; }
        public byte? Q1E { get; set; }
        public byte? Q1F { get; set; }
        public byte? Q2_TU_HARM { get; set; }
        public byte? Q3_NGHE { get; set; }
        public int? DIEM_QST { get; set; }
        public byte? MUC_QST { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_HO_TRO_XH_Entity {
        public string RECORD_ID { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public DateTime? NGAY_HO_TRO { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public string MA_TCV { get; set; }
        public string DICH_VU { get; set; }
        public byte? KET_QUA_HIV { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_TUAN_THU_Entity {
        public string RECORD_ID { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public DateTime? NGAY_HO_TRO { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public string MA_TCV { get; set; }
        public byte? HINH_THUC_DIEU_TRI { get; set; }
        public bool? CO_KE_DON_THUOC { get; set; }
        public byte? TUAN_THU { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_CHAN_DOAN_Entity {
        public string RECORD_ID { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public DateTime? NGAY_KHAM { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public string MA_TCV { get; set; }
        public string CO_SO_Y_TE { get; set; }
        public string BAC_SI { get; set; }
        public byte? LAN_KHAM { get; set; }
        public string TRIEU_CHUNG { get; set; }
        public string CHAN_DOAN_CHINH { get; set; }
        public string NGUY_CO { get; set; }
        public string HINH_THUC_DIEU_TRI { get; set; }
        public DateTime? NGAY_HEN_TAI_KHAM { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_TU_VAN_L1_Entity {
        public string RECORD_ID { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public DateTime? NGAY_TU_VAN { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public string MA_TCV { get; set; }
        public byte? DIA_DIEM { get; set; }
        public int? AUDIT_C_SCORE { get; set; }
        public int? PCL5_SCORE { get; set; }
        public bool? PCL5_POSITIVE { get; set; }
        public int? STIGMA_SCORE { get; set; }
        public byte? TINH_TRANG_SKTT { get; set; }
        public string NHU_CAU_HO_TRO { get; set; }
        public DateTime? LICH_HEN_TIEP { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_TU_VAN_L2_Entity {
        public string RECORD_ID { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public DateTime? NGAY_TU_VAN { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public string MA_TCV { get; set; }
        public byte? DIA_DIEM { get; set; }
        public string DANH_GIA_HIEN_TAI { get; set; }
        public string CAN_THIEP_AP_DUNG { get; set; }
        public DateTime? NGAY_HEN_TIEP { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_THEO_DAU_Entity {
        public string RECORD_ID { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public DateTime? NGAY_THEO_DAU { get; set; }
        public byte? HINH_THUC_LIEN_HE { get; set; }
        public byte? KET_QUA { get; set; }
        public bool? MAT_DAU { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45_VAN_TAY_Entity {
        public string RECORD_ID { get; set; }
        public int? REPEAT_INSTANCE { get; set; }
        public string CITY_CODE { get; set; }
        public string MADUAN { get; set; }
        public string MA_NHOM { get; set; }
        public string REDCAP_DAG { get; set; }
        public DateTime? NGAY_GHI_NHAN { get; set; }
        public byte? DICH_VU { get; set; }
        public byte? VAN_DE { get; set; }
        public byte? PHUONG_AN { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class BVTL_DATA_STANDARDIZATION_LOG_Entity {
        public long ID { get; set; }
        public string MADUAN { get; set; }
        public string REPORT_ID { get; set; }
        public string API_CODE { get; set; }
        public string TABLE_NAME { get; set; }
        public string RECORD_ID { get; set; }
        public string FIELD_NAME { get; set; }
        public string OLD_VALUE { get; set; }
        public string NEW_VALUE { get; set; }
        public string RULE_CODE { get; set; }
        public string SEVERITY { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string MESSAGE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public bool IS_RESOLVED { get; set; }
        public string RESOLVED_NOTE { get; set; }
    }

    public class GroupedDataQualityLogModel {
        public string RULE_CODE { get; set; }
        public string SEVERITY { get; set; }
        public string TABLE_NAME { get; set; }
        public int TotalCount { get; set; }
        public int UnresolvedCount { get; set; }
        public DateTime? FirstSeen { get; set; }
        public DateTime? LastSeen { get; set; }
        public string SampleMessage { get; set; }
    }

    public class DataQualityStatsByNhomModel {
        public string MA_NHOM { get; set; }
        public string CITY_CODE { get; set; }
        public int TotalErrors { get; set; }
        public int TotalWarnings { get; set; }
        public int TotalResolved { get; set; }
        public int TotalPending { get; set; }
    }
}
