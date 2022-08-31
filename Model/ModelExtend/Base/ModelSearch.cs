using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.Base
{
    public class ModelSearch
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public string SortColumn { get; set; }
        public string AppCode { get; set; }
        public string Permission_Require { get; set; }
        public string KeyWord { get; set; }
    }
}
