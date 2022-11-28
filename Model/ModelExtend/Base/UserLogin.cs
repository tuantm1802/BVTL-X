 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.Base
{
    [Serializable]
    public class UserLogin
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public string Avartar { get; set; }
        public int EmployeeId { get; set; }
        public string GroupId { get; set; }
        public bool ViewHome { get; set; }
        public string Appcode { get; set; }
        public int? UnitId { get; set; }
        public int? KhoId { get; set; }
        public string IsKyDuyet { get; set; }

        public string UserGroup { get; set; }
        public int? CapPhat_UnitId { get; set; }
        public string CityCodes { get; set; }
        public string MaDuAn { get; set; }
    }
}
