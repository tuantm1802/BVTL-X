using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.API
{
    public class ProcessModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int TimeLoop { get; set; }
        public bool Active { get; set; } = true;

        public string Url { get; set; }
        public string Token { get; set; }
        public string ReportId { get; set; }
        public List<string> TableNames { get; set; }
    }
}
