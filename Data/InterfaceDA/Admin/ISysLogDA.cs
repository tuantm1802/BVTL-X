using Model.Model;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.Admin
{
    public interface ISysLogDA
    {
        /// <summary>
        /// Lấy danh sách log theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         List<BVTL_QT_LOG> GetLogByPage(ModelSearch modelSearch);
         ObjectMessage Add(BVTL_QT_LOG model);
         ObjectMessage Edit(BVTL_QT_LOG model);
    }
}
