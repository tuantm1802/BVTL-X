using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.User
{
    public class ViewProfile
    {
        public long ID { get; set; }
        public string UserName { get; set; }
        public string UserGroup { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool Status { get; set; }
        public string Avartar { get; set; }
    }
}
