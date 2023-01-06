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
    public interface ITTTruyenThongVIIVDA
    {

        /// <summary>
        /// Lấy kết quả Thông tin truyền thông
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TTTruyenThongVIIVPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả Thông tin truyền thông
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<TTTruyenThongVIIVPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
