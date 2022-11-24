using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class KetQuaSKTTPageModel : BVTL_KQ_SL_SKTT
    {
        public int TotalRow { get; set; }
        public string hoten { get; set; }
        public string makh { get; set; }
        public string CityName { get; set; }
        public string ngaysltext { get; set; }
        public string tennhom_tbh { get; set; }
        public string tenduan { get; set; }
    }
}
