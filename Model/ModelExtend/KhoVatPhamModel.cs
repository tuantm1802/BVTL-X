using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class ChiTietPhieuXuatNhapModel
    {
        public int Id { get; set; }
        public int SoLuong { get; set; }
        public string MaPhieu { get; set; }
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public int? LoaiPhieu { get; set; }
        public DateTime? NgayLap { get; set; }
        public string GhiChu { get; set; }
        public DateTime? sync_date { get; set; }
        public string manhom_tbh { get; set; }
        public string city_code { get; set; }
    }
    public class TonKhoModel
    {
        public string MaSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string DVT { get; set; }
        public int? TonDauKy { get; set; }
        public int? TonCuoiKy { get; set; }
        public int? NhapKho { get; set; }
        public int? XuatKho { get; set; }        
        
    }

}
