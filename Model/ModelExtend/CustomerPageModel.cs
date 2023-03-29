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
}
