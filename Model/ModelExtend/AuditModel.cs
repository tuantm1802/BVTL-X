using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class AuditModel
    {
        //Audit Properties
        public int ACTION_ID { get; set; }
        public int USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string IP_ADDRESS { get; set; }
        public string LINK_PAGE { get; set; }
        public string LINK_ACCESSED { get; set; }
        public string APP_CODE { get; set; }
        public string ACTION { get; set; }
        public string DATA { get; set; }
        public DateTime TIME_STAMP { get; set; }
        public Guid AUDIT_ID { get; set; }
    }
}
