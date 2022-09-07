using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class ApiPageModel : BVTL_API
    {
        public int TotalRow { get; set; }
        public List<string> TableNames { get; set; }
    }
}
