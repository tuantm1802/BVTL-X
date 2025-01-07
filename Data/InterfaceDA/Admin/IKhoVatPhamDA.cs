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
    public interface IKhoVatPhamDA
    {

        // BVTL_KHACH_HANG GetItemByCode(string code);

        // List<KhoVatPhamPageModel> GetAllByPage(ModelSearch modelSearch);
        // int GetTotalKhoVatPhamsCount();
        // Customer GetCustomerById(int id);
        // Customer GetCustomerChuyenGuiById(int id);
        // Customer GetCustomerSuDungChatById(int id);
        // Customer GetCustomerQHTDById(int id);
        // Customer GetDichVuChuyenGuiByCustomerId(int id, string recordid);
        // Customer GetSinhHoatNhomByCustomerId(int id, string recordid);
        // Customer GetPhieuTuVanCustomerId(int id, string recordid);
        // List<PhieuTuVan> GetListPhieuTuVanCustomerId(int id, string recordid);
        //List<KhamDieuTriSKTT> GetListKhamVaDieuTriSKTTCustomerId(int id, string recordid);
        //Customer GetKhamVaDieuTriCustomerId(int id, string recordid);
        //List<BVTL_KHACH_HANG> GetAll();
        DataTableResponse<PhieuXuatNhap> GetAllPhieuXuatNhaps(DataTableRequest request);
        DataTableResponse<ChiTietPhieuXuatNhapModel> GetAllChiTietPhieuXuatNhaps(DataTableRequest request);
        bool CapNhatPhieuXuatNhap(ChiTietPhieuXuatNhapModel model);
        List<TonKhoModel> GetTonKhoData(DateTime fromDate, DateTime toDate);

        //DataTableResponse<ChiTietPhieuXuatNhap> GetAllChiTietPhieuXuats(DataTableRequest request);
        //DataTableResponse<ChiTietPhieuXuatNhap> GetAllChiTietPhieuNhaps(DataTableRequest request);


    }
}
