using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class MenuModel : PageMenu
    {
        public string Actions { get; set; }
        public int CountChild { get; set; }
    }
}
