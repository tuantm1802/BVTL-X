using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.Base
{
    public class PaginationResponse
    {
        public IEnumerable items { get; set; }
        public int totalNumber { get; set; } = 0;
    }
    public class PaginationRequest
    {
        public string keyword { get; set; }
        public string orderBy { get; set; }
        public int pageSize { get; set; }
        public int pageNumber { get; set; }
    }
}
