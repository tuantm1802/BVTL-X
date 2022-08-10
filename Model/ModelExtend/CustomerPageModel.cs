using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ModelExtend
{
    public class CustomerPageModel : Customer
    {
        public int TotalRow { get; set; }
        public string CityName { get; set; }
        public string GenderText { get; set; }
        public string DateOfBirthText { get; set; }
    }
}
