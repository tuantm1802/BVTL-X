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
    public interface IPageMenuDA
    {
        BVTL_QT_PAGE_MENU GetItemByName(string name);

        List<BVTL_QT_PAGE_MENU> GetAllByPage(ModelSearch modelSearch);

        List<BVTL_QT_PAGE_MENU> GetAll();

        List<BVTL_QT_PAGE_MENU> GetAllByAppCode();

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        string GetAllByBVTL_QT_PAGE_MENU(int page);
        /// <summary>
        /// Lấy danh sách menu theo user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        List<MenuModel> GetMenuByUser(long userID);

        ObjectMessage Add(BVTL_QT_PAGE_MENU modelPage, List<BVTL_QT_PAGE_ACTION> pageFunctions);
        ObjectMessage Edit(BVTL_QT_PAGE_MENU modelPage, List<BVTL_QT_PAGE_ACTION> pageFunctions);
        ObjectMessage Delete(int Id, int updateBy);
    }
}
