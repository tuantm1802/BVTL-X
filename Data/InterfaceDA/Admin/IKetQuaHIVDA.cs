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
    public interface IKetQuaHIVDA
    {

        /// <summary>
        /// Lấy kết quả HIV theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KetQuaHIVPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả HIV theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<KetQuaHIVPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
