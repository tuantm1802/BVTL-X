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
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();


        public List<RolePage> GetAllByRole(string roleID)
        {
            return db.RolePages.Where(x => x.RoleID == roleID && x.IS_ACTIVE).ToList();
        }

        public ObjectMessage Add(RolePage rolePage)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.RolePages.Add(rolePage);
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
        public ObjectMessage Edit(RolePage rolePage)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.RolePages.FirstOrDefault(x => x.ID == rolePage.ID);
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
                var itemDelete = db.RolePages.Find(Id);
                db.RolePages.Remove(itemDelete);
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
