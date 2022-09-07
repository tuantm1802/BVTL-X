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
    public interface IDuongSuDungDA
    {
        /// <summary>
        /// Lấy theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BVTL_DUONG_SU_DUNG GetItemById(int id);

        /// <summary>
        /// Lấy dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        List<DuongSuDungPageModel> GetAllByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy tất cả dữ liệu
        /// </summary>
        /// <returns></returns>
        List<BVTL_DUONG_SU_DUNG> GetAll();
    }
}
