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
        public string MaDuAn { get; set; }
        public List<string> TableNames { get; set; }
        public string Start_Time_Sync { get; set; }
        public string End_Time_Syc { get; set; }
        public string Message { get; set; }
        public string RawOrLabel { get; set; }
    }
}
