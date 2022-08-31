using System;
using Model.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ModelExtend;
using Model.ModelExtend.Base;

namespace Data.InterfaceDA.Admin
{
    public interface ICustomerDA
    {

         BVTL_KHACH_HANG GetItemByCode(string code);

         List<CustomerPageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize);

         List<BVTL_KHACH_HANG> GetAll();


         ObjectMessage Add(BVTL_KHACH_HANG Customer);

         ObjectMessage Delete(int Id);
    }
}
