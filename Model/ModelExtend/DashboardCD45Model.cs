using System;
using System.Collections.Generic;

namespace Model.ModelExtend
{
    public class DashboardCD45OverviewModel
    {
        public int TongKhachHang { get; set; }
        public int TongSangLocQST { get; set; }
        public int QSTNguyCoCao { get; set; }
        public double TyLeQSTNguyCoCao { get; set; }
        public int TongKhamSKTT { get; set; }
        public int TongLuotKhamSKTT { get; set; }
        public int TongTuVanL1 { get; set; }
        public int TongHoTroXH { get; set; }
        public int TongTaiLieuPhat { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public string LastSyncTimeString { get; set; }
    }

    public class DashboardCD45ByTargetGroupModel
    {
        public byte? DoiTuongId { get; set; }
        public string TenDoiTuong { get; set; }
        public int TongKH { get; set; }
        public int SoKHSangLoc { get; set; }
        public int Muc1_RatCao { get; set; }
        public int Muc2_Cao { get; set; }
        public int Muc3_TrungBinh { get; set; }
        public int Muc4_Thap { get; set; }
        public double TyLeNguyCoCao { get; set; }
    }

    public class DashboardCD45ByAgeGroupModel
    {
        public string NhomTuoi { get; set; }
        public int TongKH { get; set; }
        public int SoKHSangLoc { get; set; }
        public int Muc1 { get; set; }
        public int Muc2 { get; set; }
        public int Muc3 { get; set; }
        public int Muc4 { get; set; }
        public double TyLeNguyCoCao { get; set; }
    }

    public class DashboardCD45MentalHealthModel
    {
        public byte? DoiTuongId { get; set; }
        public string TenDoiTuong { get; set; }
        public int SoCaTuVan { get; set; }
        public int PCL5_DuongTinh { get; set; }
        public int PCL5_AmTinh { get; set; }
        public double TyLePCL5DuongTinh { get; set; }
        public double DiemAuditCTB { get; set; }
        public double DiemKyThiTB { get; set; }
    }

    public class DashboardCD45ByProvinceModel
    {
        public string CityCode { get; set; }
        public string CityName { get; set; }
        public int TongKH { get; set; }
        public int SangLocQST { get; set; }
        public int KhamSKTT { get; set; }
        public int TuVanL1 { get; set; }
    }

    public class DashboardCD45CascadeFunnelModel
    {
        public int Step1_TiepCanTruyenThong { get; set; }
        public int Step2_SangLocQST { get; set; }
        public int Step3_NguyCoCaoQST { get; set; }
        public int Step4_TuVanTamLy { get; set; }
        public int Step5_KhamChuyenKhoa { get; set; }
        public int Step6_TaiKhamSKTT { get; set; }
    }

    public class DashboardCD45SocialSupportModel
    {
        public int TongNhanHoTro { get; set; }
        public int HoTroBHYT { get; set; }
        public int HoTroMethadone { get; set; }
        public int XetNghiemHIV { get; set; }
        public int STIs { get; set; }
        public int ViemGan { get; set; }
    }

    public class DashboardCD45FullDataModel
    {
        public DashboardCD45OverviewModel Overview { get; set; }
        public List<DashboardCD45ByTargetGroupModel> ByTargetGroup { get; set; }
        public List<DashboardCD45ByAgeGroupModel> ByAgeGroup { get; set; }
        public List<DashboardCD45MentalHealthModel> MentalHealth { get; set; }
        public List<DashboardCD45ByProvinceModel> ByProvince { get; set; }
        public DashboardCD45CascadeFunnelModel CascadeFunnel { get; set; }
        public DashboardCD45SocialSupportModel SocialSupport { get; set; }
    }
}
