using Model.ModelExtend;
using System.Collections.Generic;

namespace Data.InterfaceDA
{
    public interface IBaoCaoCD45DA
    {
        List<BaoCaoCD45Model> GetBaoCao(string fromDate, string toDate, string cityCode, string maNhom, string maTCV);
        List<CD45_TCV_ItemModel> GetListTCV(string cityCode, string maNhom);
        List<CD45_DrillDown_ItemModel> GetDrillDown(string chiTieuCode, string fromDate, string toDate, string cityCode, string maNhom, string maTCV, int? doiTuong);
    }
}
