using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.Report
{
    public class KetQuaSangLocModel
    {
        public string NoiDung { get; set; }
        public int SoNguoi { get; set; }
        public decimal PhanTram { get; set; }
    }

    public class KetQuaSangLocProModel: BVTL_BO_BIEU_MAU_KH_BAO_CAO
    {
        public string NoiDung { get; set; }
        public int SoLuong { get; set; }
        public int ketqua { get; set; }
        public string NoiDung_QK { get; set; }
        public string NoiDung_HT { get; set; }

    }
    public class KetQuaQSTModel
    {
        public string KetQua { get; set; }
        public int SoLuong { get; set; }
        public decimal TyLe { get; set; }
        public string GhiChu { get; set; }
        public string IsShow { get; set; }
        public int? SubRowspan { get; set; }
        

    }

    public class DuongSuDungMaTuyDaModel
    {
        public string NoiDung { get; set; }
        public decimal HutHit { get; set; }
        public decimal DangBot { get; set; }
        public decimal UongNuot { get; set; }
        public decimal TiemChich { get; set; }
        public decimal KBKTL { get; set; }
    }

    public class TanSuatSuDungMaTuyDaModel
    {
        public string NoiDung { get; set; }
        public decimal VaiLan1Ngay { get; set; }
        public decimal HangNgay { get; set; }
        public decimal VaiLan1Tuan { get; set; }
        public decimal VaiNgayRoiTamNghi { get; set; }
        public decimal DungCuoiTuan { get; set; }
        public decimal VaiLan1Thang { get; set; }
        public decimal ItHon1Lan1Thang { get; set; }
        public decimal KBKTL { get; set; }
    }

    public class LanDauSuDungMaTuyDaModel
    {
        public string NoiDung { get; set; }
        public decimal _13 { get; set; }
        public decimal _14 { get; set; }
        public decimal _15 { get; set; }
        public decimal _16 { get; set; }
        public decimal _17 { get; set; }
        public decimal _18 { get; set; }
        public decimal _19 { get; set; }
        public decimal _20 { get; set; }
        public decimal _21 { get; set; }
        public decimal _22 { get; set; }
        public decimal _23 { get; set; }
    }

    public class LoaiMaTuyDaSuDungDTModel
    {
        public string NoiDung { get; set; }
        public decimal Da { get; set; }
        public decimal Keo { get; set; }
        public decimal CanCo { get; set; }
        public decimal Ketamin { get; set; }
        public decimal BongCuoi { get; set; }
        public decimal Heroin { get; set; }
        public decimal CacChatHit { get; set; }
    }
    public class NguyCoKhiSDMTDModel
    {
        public string NoiDung { get; set; }
        public decimal ChuaBaoGio { get; set; }
        public decimal DaTungTiemChich { get; set; }
        public decimal VanDangTiemChich { get; set; }
    }

    public class ChungBKTModel
    {
        public string NoiDung { get; set; }
        public decimal ChuaBaoGio { get; set; }
        public decimal DaTungDungChung { get; set; }
    }

    public class NguyCoTinhDucModel
    {
        public string NoiDung { get; set; }
        public decimal ChuaBaoGio { get; set; }
        public decimal DongGioi { get; set; }
        public decimal KhacGioi { get; set; }
        public decimal CaHai { get; set; }
    }

    public class DungBCSModel
    {
        public string NoiDung { get; set; }
        public decimal LuonLuon { get; set; }
        public decimal ThuongXuyen { get; set; }
        public decimal ThiThoang { get; set; }
        public decimal HiemKhi { get; set; }
        public decimal KhongBaoGio { get; set; }
    }

    public class SuDungMTDKhiQHTDModel
    {
        public string NoiDung { get; set; }
        public decimal Co { get; set; }
        public decimal Khong { get; set; }
        public decimal KBKTL { get; set; }
    }

    public class NhieuNguyCoTinhDucModel
    {
        public string NoiDung { get; set; }
        public decimal Mot { get; set; }
        public decimal Hai { get; set; }
        public decimal Ba { get; set; }
        public decimal BonNam { get; set; }
    }

    public class BenhSTIModel
    {
        public string NoiDung { get; set; }
        public decimal Lau { get; set; }
        public decimal SuiMaoGa { get; set; }
        public decimal KhongMac { get; set; }
        public decimal Khac { get; set; }
        public decimal GiangMai { get; set; }
        public decimal KBKTL { get; set; }
    }

    public class BenhLao_VGCModel
    {
        public string NoiDung { get; set; }
        public decimal HienTai_SoLuong { get; set; }
        public decimal HienTai_PhanTram { get; set; }
        public decimal QuaKhu_SoLuong { get; set; }
        public decimal QuaKhu_PhanTram { get; set; }
    }


    public class CacChatGayNghienModel
    {
        public string NoiDung { get; set; }
        public decimal NguyCoThap_SoLuong { get; set; }
        public decimal NguyCoThap_PhanTram { get; set; }
        public decimal NguyCoTrungBinh_SoLuong { get; set; }
        public decimal NguyCoTrungBinh_PhanTram { get; set; }
        public decimal NguyCoCao_SoLuong { get; set; }
        public decimal NguyCoCao_PhanTram { get; set; }
        public decimal Tong_SoLuong { get; set; }
    }
    public class CacChatGayNghienAssistModel
    {
        public string LoaiDiem  { get; set; }
        public int NguyCo_Thap { get; set; }
        public decimal TyLe_NguyCo_Thap { get; set; }
        public int NguyCo_TrungBinh { get; set; }
        public decimal TyLe_NguyCo_TrungBinh { get; set; }
        public int NguyCo_Cao { get; set; }
        public decimal TyLe_NguyCo_Cao { get; set; }
        
    }

    public class MucDoGapVanDeSKTTModel
    {
        public string NoiDung { get; set; }
        public decimal KhongChutNao_SoLuong { get; set; }
        public decimal KhongChutNao_PhanTram { get; set; }
        public decimal Tu1Den7Ngay_SoLuong { get; set; }
        public decimal Tu1Den7Ngay_PhanTram { get; set; }
        public decimal Tu8NgayTroLen_SoLuong { get; set; }
        public decimal Tu8NgayTroLen_PhanTram { get; set; }
        public decimal GanNhuHangNgay_SoLuong { get; set; }
        public decimal GanNhuHangNgay_PhanTram { get; set; }
    }

    public class LoanThanModel
    {
        public string NoiDung { get; set; }
        public decimal TheoDoiRinhRap_SoLuong { get; set; }
        public decimal TheoDoiRinhRap_PhanTram { get; set; }
        public decimal YNghi_SoLuong { get; set; }
        public decimal YNghi_PhanTram { get; set; }
        public decimal NgheThuMaNKKNT_SoLuong { get; set; }
        public decimal NgheThuMaNKKNT_PhanTram { get; set; }
    }
}
