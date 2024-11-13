using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.Report
{
    public class DashboardModel
    {
        
        public string xValues { get; set; }
        public int yValues { get; set; }
        
    }


    public class DashboardTanSuatSuDungTheoDoTuoi
    {
        public string NoiDung { get; set; }
        public string MucDoTuoi { get; set; }
        public string MucDoNguyCo { get; set; }
        public int? TanSuat1_2Lan { get; set; }
        public int? TanSuatMoiThang { get; set; }
        public int? TanSuatMoiTuan { get; set; }
        public int? TanSuatMoiNgay { get; set; }
        public int? KhongSuDungDaChat { get; set; }
        public int? CoSuDungDaChat { get; set; }
        public int? SoLuongAmTinh { get; set; }
        public int? SoLuongDuongTinh { get; set; }
    }
}
