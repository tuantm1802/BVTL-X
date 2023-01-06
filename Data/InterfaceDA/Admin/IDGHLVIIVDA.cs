using System;
using Model.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.API;

namespace Data.InterfaceDA.Admin
{
    public interface IDGHLVIIVDA
    {

        /// <summary>
        /// Lấy kết quả Đánh giá hài lòng KH
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        DGHLVIIVPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả Đánh giá hài lòng KH
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<DGHLVIIVPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
