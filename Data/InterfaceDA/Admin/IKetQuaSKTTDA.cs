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
    public interface IKetQuaSKTTDA
    {

        /// <summary>
        /// Lấy kết quả SKTT theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KetQuaSKTTPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả SKTT theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<KetQuaSKTTPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
