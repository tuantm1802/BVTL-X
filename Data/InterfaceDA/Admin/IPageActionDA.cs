using System;
using Model.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ModelExtend.Base;

namespace Data.InterfaceDA.Admin
{
    public interface IPageActionDA
    {
         BVTL_QT_PAGE_ACTION GetItemByPage(int pageId);

         List<BVTL_QT_PAGE_ACTION> GetAllByPage(int page);

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
         List<BVTL_QT_PAGE_ACTION> GetAllByPageMenu(int page);
         List<BVTL_QT_PAGE_ACTION> GetAll();

         ObjectMessage Add(BVTL_QT_PAGE_ACTION model);
         ObjectMessage Edit(BVTL_QT_PAGE_ACTION model);
         ObjectMessage Delete(int Id);
    }
}
