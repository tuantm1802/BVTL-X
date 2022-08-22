using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class PageMenuModel: BVTL_QT_PAGE_MENU
    {
        public string APP_CODE_TEXT { get; set; }
        public string PARENT_NAME { get; set; }
        public string PERMISSION_REQUIRE_TEXT { get; set; }
    }
}
