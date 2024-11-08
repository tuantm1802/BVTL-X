using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class CustomerPageModel : BVTL_KHACH_HANG
    {
        public int TotalRow { get; set; }
        public string CityName { get; set; }
        public string GioiTinhText { get; set; }
        public string LoaiDoiTuong { get; set; }
        public string ngaytiepcantext { get; set; }
        public string tenduan { get; set; }
        public string tennhom_tbh { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string RecordId { get; set; }
        public string MaNhomTbh { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string CapBacHocVan { get; set; }
        public string NgheNghiep { get; set; }
        public string NgheNghiepKhac { get; set; }
        public DateTime? NgayHoi { get; set; }
        public DateTime? NgayTuVan { get; set; }
        public int? PhiuTVanComplete { get; set; }
        public DateTime? NgayBangHoiNgay { get; set; }
        public DateTime? NgayShn { get; set; }
        public DateTime? NgayTheoDauTime { get; set; }
        public string CityCodeMap { get; set; }
        public string KetQuaXNHiv { get; set; }
        public DateTime? NgayXNTLVR { get; set; }
        public string ChuyenGuiStatus { get; set; }
        public string ChatSuDungChemsex3Thang       {get; set;}
        public string ChatSuDungThuongXuyenNhat     {get; set;}
        public string TanSuatSuDungMTDChemsex3Thang {get; set;}
        public string SuDungDaChat                  {get; set;}
        public string SuDungChatDaChat { get; set; }
        public string TanSuatSuDungDaChat3Thang { get; set; }
        public string DoiTuongQuanHe { get; set; }
        public string QHTDTT3Thang { get; set; }
        public string TinhTrangMoiQHHT { get; set; }
        public string TanSuatSuDungBCSChemsex3Thang { get; set; }
        public string BanDam3Thang { get; set; }
        public string STIs { get; set; }
        public string TinhTrangViemGanC { get; set; }
        public string MucDoNguyCoThuocLa { get; set; }
        public string MucDoNguyCoThucUong { get; set; }
        public string MucDoNguyCoMaTuyDa { get; set; }
        public string QSTCoTuSat { get; set; }
        public string QSTTongDiem { get; set; }
        public string ACESoLuong { get; set; }
        public List<DichVuChuyenGui> DichVuChuyenGuiList { get; set; }
        public string ThamGiaSinhHoatNhom { get; set; }
        public string TuVanVien { get; set; }
        public int? SoLanThamGia { get; set; }
        public int? SoLanTuVan { get; set; }
        public int? SoLanKham { get; set; }
    }

    public class CustomerChuyenGui
    {
        public int Id { get; set; }
        public string RecordId { get; set; }
        public string ChuyeGui1 { get; set; }
        public string ChuyeGui2 { get; set; }
        public string ChuyeGui3 { get; set; }
        public string ChuyeGui4 { get; set; }
        public string ChuyeGui5 { get; set; }
        public string ChuyeGui6 { get; set; }
        public DateTime? F2Q11 { get; set; } // NgayKham cho ChuyeGui1
        public DateTime? F2Q21 { get; set; } // NgayKham cho ChuyeGui2
        public DateTime? F2Q31D { get; set; } // NgayKham cho ChuyeGui3
        public DateTime? F2Q411 { get; set; } // NgayKham cho loaihinh4___1
        public DateTime? F2Q421 { get; set; } // NgayKham cho loaihinh4___2
        public DateTime? F2Q431 { get; set; } // NgayKham cho loaihinh4___3
        public DateTime? F2Q441 { get; set; } // NgayKham cho loaihinh4___4
        public DateTime? F2Q5D { get; set; } // NgayKham cho ChuyeGui5
        public DateTime? F2Q6D { get; set; } // NgayKham cho ChuyeGui6
        public string F2Q13 { get; set; } // LanKham cho ChuyeGui1
        public string F2Q413 { get; set; } // LanKham cho loaihinh4___1
        public string LoaiHinh1 { get; set; } // Loaihinh4___1
        public string LoaiHinh2 { get; set; } // Loaihinh4___2
        public string LoaiHinh3 { get; set; } // Loaihinh4___3
        public string LoaiHinh4 { get; set; } // Loaihinh4___4
        
    }
    public class DichVuChuyenGui
    {
        public string DichVu { get; set; }
        public DateTime? NgayKham { get; set; }
        public string LanKham { get; set; }
        public string LoaiHinh { get; set; } // STI, Viêm gan C, MMT, Lao
    }
    public class PhieuTuVan
    {
        public string RecordId { get; set; }
        public DateTime? NgayTuVan { get; set; }
        public DateTime? ThoiGianTuVanTiep { get; set; }
        public string HanhViNguyCo { get; set; }
        public string SucKhoeTheChat { get; set; } 
        public string SucKhoeTamThan { get; set; } 
        public string SucKhoeTinhDuc { get; set; } 
        public string TuVanGiamHai { get; set; } 
        public string HoTroSKTT { get; set; } 
        public string HoTroSKTD { get; set; } 
    }
    public class KhamDieuTriSKTT
    {
        public string RecordId { get; set; }
        public DateTime? NgayKham { get; set; }
        public DateTime? NgayHenTaiKham { get; set; }
        public string TrieuChung { get; set; }
        public string ChanDoan { get; set; } 
        public string DungTheoDon { get; set; } 
        
    }

    public class DataTableRequest
    {
        public int draw { get; set; } // Số thứ tự yêu cầu, giúp DataTable đồng bộ các yêu cầu
        public int start { get; set; } // Vị trí bắt đầu của dữ liệu (phân trang)
        public int length { get; set; } // Số bản ghi được lấy trên mỗi trang
        public DataTableSearch search { get; set; } // Đối tượng chứa thông tin tìm kiếm
        public List<DataTableOrder> order { get; set; } // Danh sách các thông tin sắp xếp
        public List<DataTableColumn> columns { get; set; } // Danh sách các cột

        public DataTableRequest()
        {
            search = new DataTableSearch();
            order = new List<DataTableOrder>();
            columns = new List<DataTableColumn>();
        }
    }

    public class DataTableSearch
    {
        public string value { get; set; } // Giá trị tìm kiếm
        public bool regex { get; set; } // Sử dụng regex hay không (hiện không sử dụng)
    }

    public class DataTableOrder
    {
        public int column { get; set; } // Chỉ số của cột được sắp xếp
        public string dir { get; set; } // Hướng sắp xếp (asc hoặc desc)
    }

    public class DataTableColumn
    {
        public string data { get; set; } // Tên của cột
        public string name { get; set; } // Tên hiển thị của cột
        public bool searchable { get; set; } // Có thể tìm kiếm trên cột này hay không
        public bool orderable { get; set; } // Có thể sắp xếp trên cột này hay không
        public DataTableSearch search { get; set; } // Đối tượng tìm kiếm riêng cho cột

        public DataTableColumn()
        {
            search = new DataTableSearch();
        }
    }
    public class DataTableResponse<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<T> data { get; set; }
    }

}
