using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class UserPageModel : BVTL_QT_NGUOI_DUNG
    {
        public string UNIT_NAME { get; set; }
        public string ROLE_DESC { get; set; }
        public int TotalRow { get; set; }
        public int? IsLock { get; set; }

        public string CreatedDate_Text
        {
            get
            {
                var date = "";
                if (CreatedDate != null)
                {
                    date = Convert.ToDateTime(CreatedDate).ToString("dd/MM/yyyy");
                }
                return date;
            }
        }
        public string CreatedName { get; set; }
        public string GanderName { get; set; }
        public string RoleName { get; set; }
        public string ModifiedDate_Text
        {
            get
            {
                var date = "";
                if (ModifiedDate != null)
                {
                    date = Convert.ToDateTime(ModifiedDate).ToString("dd/MM/yyyy");
                }
                return date;
            }
        }
        public string ModifiedName { get; set; }
        public string DateOfBirth_Text
        {
            get
            {
                var date = "";
                if (CreatedDate != null)
                {
                    date = Convert.ToDateTime(DateOfBirth).ToString("dd/MM/yyyy");
                }
                return date;
            }
        }

        public List<BVTL_NHOM_TBH> TestGroups { get; set; }

    }
}
