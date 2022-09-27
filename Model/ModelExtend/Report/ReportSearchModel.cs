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
    }
}
