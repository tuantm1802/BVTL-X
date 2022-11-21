using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.Report
{
    public class BaoCaoTongHopQuyVIIVModel
    {
        public string STT { get; set; }
        public string HoatDong { get; set; }
        public string ChiTieu { get; set; }
        public string DonVi { get; set; }
        public bool BoldText { get; set; }
        public List<ListQuyModel> ListQuy { get; set; }
        public int? TyLe { get; set; }
    }

    public class ListQuyModel
    {
        public string Quy { get; set; }
        public int IntQuy { get; set; }
        public int Nam { get; set; }
        public int SoLuong { get; set; }
        public int Orderby { get; set; }
    }

    public class BaoCaoTongHopQuyVIIVProModel
    {
        public int OrderBy { get; set; }
        public string STT { get; set; }
        public string HoatDong { get; set; }
        public string ChiTieu { get; set; }
        public string DonVi { get; set; }
        public bool BoldText { get; set; }
        public int? Quy { get; set; }
        public int? Nam { get; set; }
        public int? SoLuong { get; set; }
        public int? TyLe { get; set; }
    }
}
