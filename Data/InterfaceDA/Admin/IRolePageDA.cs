using Model.Model;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.Admin
{
    public interface IRolePageDA
    {
        List<BVTL_QT_QUYEN_PAGE> GetAllByRole(string roleID);
        ObjectMessage Add(BVTL_QT_QUYEN_PAGE rolePage);
        ObjectMessage Edit(BVTL_QT_QUYEN_PAGE rolePage);
        ObjectMessage Delete(int Id);
    }
}
