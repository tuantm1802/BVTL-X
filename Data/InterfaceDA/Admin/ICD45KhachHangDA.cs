using Model.ModelExtend;
using Model.ModelExtend.Base;
using System.Collections.Generic;

namespace Data.InterfaceDA.Admin
{
    public interface ICD45KhachHangDA
    {
        DataTableResponse<CD45KhachHangViewModel> GetPagingCustomers(CD45KhachHangFilterModel filter);
        CD45KhachHangDetailModel GetCustomerDetail(string recordId);
        List<CD45KhachHangViewModel> GetAllForExport(CD45KhachHangFilterModel filter);
    }
}
