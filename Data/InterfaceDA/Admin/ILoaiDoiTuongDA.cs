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
    public interface ILoaiDoiTuongDA
    {
        /// <summary>
        /// Lấy loại đối tượng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
         BVTL_LOAI_DOI_TUONG GetItemById(int id);

        /// <summary>
        /// Lấy danh sách loại đối tượng theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         List<LoaiDoiTuongPageModel> GetAllByPage(ModelSearch modelSearch);

        /// <summary>
        /// Lấy tất cả loại đối tượng
        /// </summary>
        /// <returns></returns>
         List<BVTL_LOAI_DOI_TUONG> GetAll();
    }
}
