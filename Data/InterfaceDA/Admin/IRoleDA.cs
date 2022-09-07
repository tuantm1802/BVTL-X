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
    public interface IRoleDA
    {

        BVTL_QT_QUYEN GetItemByBVTL_QT_QUYENName(string name);

        BVTL_QT_QUYEN GetItemById(string id);

        /// <summary>
        /// Lấy tất cả
        /// </summary>
        /// <returns></returns>
        List<BVTL_QT_QUYEN> GetAll();

        List<RolePageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize);

        ObjectMessage Add(BVTL_QT_QUYEN model, List<TreeModel> pageMenus);
        ObjectMessage Edit(BVTL_QT_QUYEN model, List<TreeModel> pageMenus);
        ObjectMessage Delete(string Id);
    }
}
