using System;

namespace Model.ModelExtend
{
    public class CD45_BcTieuCauHinhModel
    {
        public int ID { get; set; }
        public string ChiTieuCode { get; set; }
        public string ChiTieuName { get; set; }
        public string SectionCode { get; set; }
        public bool IsSection { get; set; }
        public bool HienThi_Thang { get; set; }
        public bool HienThi_Quy { get; set; }
        public bool HienThi_6T { get; set; }
        public bool HienThi_12T { get; set; }
        public bool Default_Thang { get; set; }
        public bool Default_Quy { get; set; }
        public bool Default_6T { get; set; }
        public bool Default_12T { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class BaoCaoCD45Model
    {
        public string STT { get; set; }
        public string ChiTieu { get; set; }
        public int? Tong { get; set; }
        public int? PUD { get; set; }
        public int? PLHIV { get; set; }
        public int? TG { get; set; }
        public int? SW { get; set; }
        public int? MSM { get; set; }
        public bool IsBold { get; set; }
        public int? IndentLevel { get; set; }
        public string Code { get; set; }
    }

    public class CD45_DrillDown_ItemModel
    {
        public string RECORD_ID { get; set; }
        public string CITY_CODE { get; set; }
        public string MA_NHOM { get; set; }
        public string MA_TCV { get; set; }
        public string DOI_TUONG_TEXT { get; set; }
        public string NGAY_THUC_HIEN { get; set; }
        public string CHI_TIET { get; set; }
        public string TEN_TINH { get; set; }
        public string TEN_NHOM { get; set; }
        public string TEN_TCV { get; set; }
        public int? SO_LUONG { get; set; }
    }

    public class CD45_TCV_ItemModel
    {
        public int ID { get; set; }
        public string MA_NHOM { get; set; }
        public string TEN_NHOM { get; set; }
        public string CITY_CODE { get; set; }
        public string MA_TCV { get; set; }
        public string TEN_TCV { get; set; }
        public string PREFIX { get; set; }
        public string SHORT_PREFIX { get; set; }
        public string CHUC_DANH { get; set; }

        public string GetXungDanh() => !string.IsNullOrWhiteSpace(PREFIX) ? PREFIX.Trim() : "Nhóm";
        public string GetShortXungDanh() => !string.IsNullOrWhiteSpace(SHORT_PREFIX) ? SHORT_PREFIX.Trim() : GetXungDanh();
        public string GetChucDanh() => !string.IsNullOrWhiteSpace(CHUC_DANH) ? CHUC_DANH.Trim() : (CITY_CODE == "HCM" || MA_NHOM == "HN_TT" || MA_NHOM == "tt" || (TEN_NHOM != null && TEN_NHOM.IndexOf("Time", StringComparison.OrdinalIgnoreCase) >= 0) ? "Giám đốc" : "Trưởng nhóm");
    }
}
