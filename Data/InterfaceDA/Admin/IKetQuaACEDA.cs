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
    public interface IKetQuaACEDA
    {

        /// <summary>
        /// Lấy kết quả ACE theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KetQuaACEPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả ACE theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<KetQuaACEPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
