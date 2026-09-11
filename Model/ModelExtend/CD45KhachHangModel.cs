using System;
using System.Collections.Generic;

namespace Model.ModelExtend
{
    public class CD45KhachHangViewModel
    {
        public long ID { get; set; }
        public string RECORD_ID { get; set; }
        public string MA_KH_NGHIEN_CUU { get; set; }
        public string CITY_CODE { get; set; }
        public string CityName { get; set; }
        public string MA_NHOM { get; set; }
        public string TenNhom { get; set; }
        public DateTime? NGAY_THAM_GIA { get; set; }
        public byte? DOI_TUONG { get; set; }
        public string TenDoiTuong { get; set; }
        public byte? GIOI_TINH_TU_XD { get; set; }
        public string TenGioiTinh { get; set; }
        public short? NAM_SINH { get; set; }
        public int? Tuoi { get; set; }
        public bool? CO_CCCD { get; set; }
        public bool? CO_BHYT { get; set; }
        public bool? CO_THUONG_TRU { get; set; }
        public bool? DANG_VO_GIA_CU { get; set; }
        public bool? BI_TAM_GIU_6T { get; set; }
        public string COMPLETE_STATUS { get; set; }
        public DateTime NGAY_SYNC { get; set; }
    }

    public class CD45KhachHangFilterModel
    {
        public string Keyword { get; set; }
        public string CityCode { get; set; }
        public string MaNhom { get; set; }
        public byte? DoiTuong { get; set; }
        public bool? CoBHYT { get; set; }
        public bool? CoCCCD { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class CD45KhachHangDetailModel
    {
        public CD45KhachHangViewModel ThongTinChung { get; set; }
        public byte? HON_NHAN { get; set; }
        public string TenHonNhan { get; set; }
        public byte? SO_CON { get; set; }
        public bool? VO_GIA_CU_6T { get; set; }
        public bool? THAM_GIA_NGHIEN_CUU { get; set; }
        public string DOI_TUONG_KHAC { get; set; }

        // KPI tóm tắt hoạt động
        public int TongHoatDong { get; set; }
        public int TongQst { get; set; }
        public int TongChanDoan { get; set; }
        public int TongHoTroXh { get; set; }
        public int TongTuanThu { get; set; }
        public int TongTuVan { get; set; }
        public bool CoVanTay { get; set; }

        // Danh sách hoạt động chi tiết các Tab
        public List<CD45_HoatDongItemModel> ListHoatDong { get; set; } = new List<CD45_HoatDongItemModel>();
        public List<CD45_QstItemModel> ListQst { get; set; } = new List<CD45_QstItemModel>();
        public List<CD45_ChanDoanItemModel> ListChanDoan { get; set; } = new List<CD45_ChanDoanItemModel>();
        public List<CD45_HoTroXhItemModel> ListHoTroXh { get; set; } = new List<CD45_HoTroXhItemModel>();
        public List<CD45_TuanThuItemModel> ListTuanThu { get; set; } = new List<CD45_TuanThuItemModel>();
        public List<CD45_TuVanItemModel> ListTuVan { get; set; } = new List<CD45_TuVanItemModel>();
    }

    public class CD45_HoatDongItemModel
    {
        public int? REPEAT_INSTANCE { get; set; }
        public DateTime? NGAY_HOAT_DONG { get; set; }
        public byte? LOAI_DV { get; set; }
        public string TenLoaiDv { get; set; }
        public string MA_TCV { get; set; }
        public string TenTCV { get; set; }
        public byte? DIA_DIEM { get; set; }
        public string CHU_DE { get; set; }
        public int? SO_TAI_LIEU { get; set; }
        public int? SO_BAO_CAO_SU { get; set; }
        public int? SO_CHAT_BOI_TRAN { get; set; }
        public int? SO_BOM_KIM { get; set; }
        public string GHI_CHU { get; set; }
    }

    public class CD45_QstItemModel
    {
        public int? REPEAT_INSTANCE { get; set; }
        public DateTime? NGAY_SANG_LOC { get; set; }
        public string MA_TCV { get; set; }
        public string TenTCV { get; set; }
        public byte? LY_DO_DANH_GIA_LAI { get; set; }
        public int? DIEM_QST { get; set; }
        public byte? MUC_QST { get; set; }
        public string TenMucQst { get; set; }
        public byte? Q1A { get; set; }
        public byte? Q1B { get; set; }
        public byte? Q1C { get; set; }
        public byte? Q1D { get; set; }
        public byte? Q1E { get; set; }
        public byte? Q1F { get; set; }
        public byte? Q2_TU_HARM { get; set; }
        public byte? Q3_NGHE { get; set; }
    }

    public class CD45_ChanDoanItemModel
    {
        public int? REPEAT_INSTANCE { get; set; }
        public DateTime? NGAY_KHAM { get; set; }
        public string CO_SO_Y_TE { get; set; }
        public string BAC_SI { get; set; }
        public byte? LAN_KHAM { get; set; }
        public string TRIEU_CHUNG { get; set; }
        public string CHAN_DOAN_CHINH { get; set; }
        public string HINH_THUC_DIEU_TRI { get; set; }
        public DateTime? NGAY_HEN_TAI_KHAM { get; set; }
    }

    public class CD45_HoTroXhItemModel
    {
        public int? REPEAT_INSTANCE { get; set; }
        public DateTime? NGAY_HO_TRO { get; set; }
        public string MA_TCV { get; set; }
        public string TenTCV { get; set; }
        public string DICH_VU { get; set; }
        public string TenDichVu { get; set; }
        public byte? KET_QUA_HIV { get; set; }
    }

    public class CD45_TuanThuItemModel
    {
        public int? REPEAT_INSTANCE { get; set; }
        public DateTime? NGAY_HO_TRO { get; set; }
        public string MA_TCV { get; set; }
        public string TenTCV { get; set; }
        public byte? HINH_THUC_DIEU_TRI { get; set; }
        public bool? CO_KE_DON_THUOC { get; set; }
        public byte? TUAN_THU { get; set; }
    }

    public class CD45_TuVanItemModel
    {
        public int LanTuVan { get; set; } // 1 hoặc 2+
        public int? REPEAT_INSTANCE { get; set; }
        public DateTime? NGAY_TU_VAN { get; set; }
        public string MA_TCV { get; set; }
        public string TenTCV { get; set; }
        public byte? DIA_DIEM { get; set; }
        public int? AUDIT_C_SCORE { get; set; }
        public int? PCL5_SCORE { get; set; }
        public bool? PCL5_POSITIVE { get; set; }
        public int? STIGMA_SCORE { get; set; }
        public byte? TINH_TRANG_SKTT { get; set; }
        public string NHU_CAU_HO_TRO { get; set; }
        public string DANH_GIA_HIEN_TAI { get; set; }
        public string CAN_THIEP_AP_DUNG { get; set; }
        public DateTime? NGAY_HEN_TIEP { get; set; }
    }

    public class CD45_NhomTcvViewModel
    {
        public int ID { get; set; }
        public string MA_NHOM { get; set; }
        public string TEN_NHOM { get; set; }
        public string CITY_CODE { get; set; }
        public string CityName { get; set; }
        public string MA_TCV { get; set; }
        public string TEN_TCV { get; set; }
        public string MADUAN { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CD45_NhomTcvKpiModel
    {
        public int TongTCV { get; set; }
        public int TongNhom { get; set; }
        public int TongTinh { get; set; }
        public int TcvActive { get; set; }
    }
}
