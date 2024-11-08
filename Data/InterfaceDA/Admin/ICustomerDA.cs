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

         List<CustomerPageModel> GetAllByPage(ModelSearch modelSearch);
         int GetTotalCustomersCount();
         Customer GetCustomerById(int id);
         Customer GetCustomerChuyenGuiById(int id);
         Customer GetCustomerSuDungChatById(int id);
         Customer GetCustomerQHTDById(int id);
         Customer GetDichVuChuyenGuiByCustomerId(int id, string recordid);
         Customer GetSinhHoatNhomByCustomerId(int id, string recordid);
         Customer GetPhieuTuVanCustomerId(int id, string recordid);
         List<PhieuTuVan> GetListPhieuTuVanCustomerId(int id, string recordid);
        List<KhamDieuTriSKTT> GetListKhamVaDieuTriSKTTCustomerId(int id, string recordid);
        Customer GetKhamVaDieuTriCustomerId(int id, string recordid);
         List<BVTL_KHACH_HANG> GetAll();
        DataTableResponse<Customer> GetAllCustomers(DataTableRequest request);


         ObjectMessage Add(BVTL_KHACH_HANG Customer);

         ObjectMessage Delete(int Id);
    }
}
