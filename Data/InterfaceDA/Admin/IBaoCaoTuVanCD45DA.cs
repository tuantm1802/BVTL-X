using Model.ModelExtend;
using System.Collections.Generic;

namespace Data.InterfaceDA.Admin
{
    public interface IBaoCaoTuVanCD45DA
    {
        List<CD45BaoCaoTuVanL1Model> GetBaoCaoTuVanL1(string fromDate, string toDate, string cityCode = null, string maNhom = null, string maTCV = null);
        List<CD45BaoCaoTuVanL2Model> GetBaoCaoTuVanL2(string fromDate, string toDate, string cityCode = null, string maNhom = null, string maTCV = null);
    }
}
