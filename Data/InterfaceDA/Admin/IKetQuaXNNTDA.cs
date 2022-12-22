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
    public interface IKetQuaXNNTDA
    {

        /// <summary>
        /// Lấy kết quả XN Nước tiểu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KetQuaXNNTPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả XN Nước tiểu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<KetQuaXNNTPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
