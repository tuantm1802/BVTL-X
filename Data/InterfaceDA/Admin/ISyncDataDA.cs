using Model.Model;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.Admin
{
    public interface ISyncDataDA
    {

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         List<BVTL_API> GetAllByPage(ModelSearch modelSearch, ref int totalRow);


        /// <summary>
        /// Thêm dữ liệu bảng SKTTTest
        /// </summary>
        /// <param name="dataInsert"></param>
        /// <returns></returns>
         ObjectMessage InsertDataFromApi(DataTable dattableInsert, string tableName, string stringConnect);

        /// <summary>
        ///Xóa dữ liệu
        /// </summary>
        /// <param name="dataInsert"></param>
        /// <returns></returns>
         ObjectMessage DeleteData(string tableName);

    }
}
