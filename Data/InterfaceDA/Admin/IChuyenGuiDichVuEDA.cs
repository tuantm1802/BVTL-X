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
    public interface IChuyenGuiDichVuDA
    {

        /// <summary>
        /// Lấy chuyển gửi dịch vụ theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ChuyenGuiDichVuPageModel GetItemById(int id);

        /// <summary>
        /// Lấy chuyển gửi dịch vụ theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<ChuyenGuiDichVuPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
