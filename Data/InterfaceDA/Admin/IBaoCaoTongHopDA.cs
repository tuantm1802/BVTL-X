using System.Collections.Generic;
using Model.ModelExtend;
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

        /// <summary>
        /// Lấy dữ liệu báo cáo tổng hợp theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
         List<BaoCaoTongHopPageModel> GetBaoCaoTongHopByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy báo cáo tổng hợp theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
         BaoCaoTongHopPageModel GetItemById(int id);
    }
}
