using Model.ModelExtend;
using System.Collections.Generic;

namespace Data.InterfaceDA.Admin
{
    public interface IBaoCaoBacSiCD45DA
    {
        List<DoctorItemModel> GetDoctors(string cityCode = null);
        List<BaoCaoBacSiSummaryModel> GetBaoCaoTongHop(string fromDate, string toDate, string cityCode = null, string doctorId = null, string maNhom = null);
        List<BaoCaoBacSiDetailModel> GetBaoCaoChiTiet(string fromDate, string toDate, string cityCode = null, string doctorId = null, string maNhom = null);
    }
}
