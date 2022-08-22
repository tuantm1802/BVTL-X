using Common;

using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class RolePageDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();


        public List<BVTL_QT_QUYEN_PAGE> GetAllByRole(string roleID)
        {
            return db.BVTL_QT_QUYEN_PAGE.Where(x => x.RoleID == roleID && x.IS_ACTIVE).ToList();
        }

        public ObjectMessage Add(BVTL_QT_QUYEN_PAGE rolePage)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.BVTL_QT_QUYEN_PAGE.Add(rolePage);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(BVTL_QT_QUYEN_PAGE rolePage)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_QT_QUYEN_PAGE.FirstOrDefault(x => x.ID == rolePage.ID);
                data.RoleID = rolePage.RoleID;
                data.PageID = rolePage.PageID;
                data.CONTROL_STRING = rolePage.CONTROL_STRING;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var itemDelete = db.BVTL_QT_QUYEN_PAGE.Find(Id);
                db.BVTL_QT_QUYEN_PAGE.Remove(itemDelete);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
