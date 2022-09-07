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
    public interface ICityDA
    {

        /// <summary>
        /// Lấy thông tin tỉnh theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        BVTL_CITES GetItemByCode(string code);

        /// <summary>
        /// Lấy danh sách tỉnh theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        List<CityPageModel> GetAllByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy danh sách tỉnh
        /// </summary>
        /// <returns></returns>
        List<BVTL_CITES> GetAll();

        /// <summary>
        /// Lấy danh sách tỉnh theo người dùng
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         List<BVTL_CITES> GetCityReport(int userId);
    }
}
