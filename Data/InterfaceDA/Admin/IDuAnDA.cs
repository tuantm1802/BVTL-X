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
    public interface IDuAnDA
    {

        /// <summary>
        /// Lấy thông tin dự án theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        BVTL_DU_AN GetItemByCode(string maduan);

        /// <summary>
        /// Lấy danh sách dự án theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        List<DuAnPageModel> GetAllByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy danh sách dự án
        /// </summary>
        /// <returns></returns>
        List<BVTL_DU_AN> GetAll();

        /// <summary>
        /// Lấy danh sách dự án theo người dùng
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         List<BVTL_DU_AN> GetDuAnReport(int userId);
    }
}
