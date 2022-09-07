using Model.Model;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.Admin
{
    public interface IBVTL_NHOM_TBHDA
    {

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         List<BVTL_NHOM_TBH> GetAllByPage(ModelSearch modelSearch, ref int totalRow);

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         List<BVTL_NHOM_TBH> GetAll();

        /// <summary>
        /// Lấy Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
        BVTL_NHOM_TBH GetItemByMaNhom(string maNhom);

        /// <summary>
        /// Lấy danh sách người dùng thep Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
         List<BVTL_QT_NGUOI_DUNG> GetAllUserByMaNhom(string maNhom);

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="BVTL_NHOM_TBH"></param>
        /// <returns></returns>
         ObjectMessage Add(BVTL_NHOM_TBH BVTL_NHOM_TBH);

        /// <summary>
        /// Chỉnh sửa
        /// </summary>
        /// <param name="BVTL_NHOM_TBH"></param>
        /// <returns></returns>
         ObjectMessage Edit(BVTL_NHOM_TBH BVTL_NHOM_TBH);

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
         ObjectMessage Delete(string maNhom, int userId);

    }
}
