using Model.ModelExtend;
using System.Collections.Generic;

namespace Data.InterfaceDA.Admin
{
    public interface IBaoCaoChiSoCD45DA
    {
        List<BaoCaoChiSoCD45Model> GetBaoCaoChiSo(string fromDate, string toDate, string cityCode, string maNhom, bool isCumulative, int? nam, string maKy);
        List<CD45_DrillDown_ItemModel> GetDrillDown(string chiTieuCode, string fromDate, string toDate, string cityCode, string maNhom, bool isCumulative, int? doiTuong, int? gioiTinh, int? nhomTuoi);
        List<CD45_ChiSoDuAnConfigModel> GetConfig(int nam, string maKy);
        bool SaveConfig(int nam, string maKy, List<CD45_ChiSoDuAnConfigModel> items, string updatedBy, out string message);
    }
}
