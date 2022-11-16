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
        public List<ListQuyModel> ListQuy { get; set; }
        public int? TyLe { get; set; }
    }

    public class ListQuyModel
    {
        public string Quy { get; set; }
        public int Nam { get; set; }
        public int SoLuong { get; set; }
        public int Orderby { get; set; }
    }
}
