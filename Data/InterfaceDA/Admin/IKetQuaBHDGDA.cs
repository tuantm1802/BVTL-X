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
    public interface IKetQuaBHDGDA
    {

        /// <summary>
        /// Lấy kết quả BHDG theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        KetQuaBHDGPageModel GetItemById(int id);

        /// <summary>
        /// Lấy kết quả BHDG theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<KetQuaBHDGPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
