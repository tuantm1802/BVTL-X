using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.Report
{
    public class ReportSearchModel
    {
        public string Months { get; set; }
        public int? Year { get; set; }
        public string CityCodes { get; set; }
        public int? TypeReport { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string MaNhomTBH { get; set; }
        public string MaDuAn { get; set; }
        public int? TuSoMaKH { get; set; }
        public int? DenSoMaKH { get; set; }
        public Nullable<System.DateTime> _FromDate { get; set; }
        public Nullable<System.DateTime> _ToDate { get; set; }

        public int? TuThang { get; set; }
        public int? TuNam { get; set; }
        public int? DenThang { get; set; }
        public int? DenNam { get; set; }
    }
}
