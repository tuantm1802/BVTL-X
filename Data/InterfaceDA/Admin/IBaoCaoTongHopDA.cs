using System.Collections.Generic;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;

namespace Data.InterfaceDA.Admin
{
    public interface IBaoCaoTongHopDA
    {

        
        /// <summary>
        /// Lấy kết quả ACE theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<BaoCaoModel> GetDataReport(ReportSearchModel modelSearch);
    }
}
