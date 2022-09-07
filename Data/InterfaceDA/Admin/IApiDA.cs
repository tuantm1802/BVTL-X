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
    public interface IApiDA
    {

        /// <summary>
        /// Lấy thông tin api theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        ApiPageModel GetItemByCode(string code);

        /// <summary>
        /// Tìm kiếm api theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<ApiPageModel> GetAllByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy tất cả đầu api
        /// </summary>
        /// <returns></returns>
        List<BVTL_API> GetAll();
    }
}
