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
    public interface IPhieuTuVanDA
    {

        /// <summary>
        /// Lấy phiếu tư vấn theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PhieuTuVanPageModel GetItemById(int id);

        /// <summary>
        /// Lấy phiếu tư vấn theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<PhieuTuVanPageModel> GetAllByPage(ModelSearch modelSearch);
    }
}
