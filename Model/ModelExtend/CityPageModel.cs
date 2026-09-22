using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class CityPageModel : BVTL_CITES
    {
        public int TotalRow { get; set; }
        public new bool? IsKeyProvince { get; set; }
        public int? OldCount { get; set; }
        public string OldNamesSummary { get; set; }
        public string CityMode { get; set; }
    }
}
