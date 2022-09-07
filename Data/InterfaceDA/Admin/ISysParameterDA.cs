using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.Admin
{
    public interface ISysParameterDA
    {
        BVTL_QT_THAM_SO GetItemById(int id);
        List<SysParameterPageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize);
        List<BVTL_QT_THAM_SO> GetAll();
        ObjectMessage Add(BVTL_QT_THAM_SO model);
        ObjectMessage Edit(BVTL_QT_THAM_SO model);
        ObjectMessage Delete(int Id);
    }
}
