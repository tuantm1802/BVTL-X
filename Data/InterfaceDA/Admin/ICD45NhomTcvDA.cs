using Model.ModelExtend;
using System.Collections.Generic;

namespace Data.InterfaceDA.Admin
{
    public interface ICD45NhomTcvDA
    {
        List<CD45_NhomTcvViewModel> GetListNhomTcv(string cityCode, string maNhom, string keyword);
        CD45_NhomTcvKpiModel GetKpiStats();
        List<CD45_NhomTcvViewModel> GetAllForExport(string cityCode, string maNhom);
    }
}
