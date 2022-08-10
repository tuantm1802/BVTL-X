using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend.User
{
    public class MY_PROFILE_Model
    {
        public int USER_ID { get; set; }
        public string FULL_NAME { get; set; }
        public string LOGIN_NAME { get; set; }
        public string TEL_NO { get; set; }
        public string UNIT_NAME { get; set; }
        public string USER_DESC { get; set; }
        public string TITLE_NAME { get; set; }
        public Nullable<int> LEVEL_ID { get; set; }
    }
}
