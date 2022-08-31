using Common;
using log4net;
using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.InterfaceDA.Admin;
using Model.ModelExtend.Base;

namespace Data.Admin
{
    public class PageActionDA: IPageActionDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public BVTL_QT_PAGE_ACTION GetItemByPage(int pageId)
        {
            return db.BVTL_QT_PAGE_ACTION.FirstOrDefault(x => x.PAGE_ID == pageId);
        }

        public List<BVTL_QT_PAGE_ACTION> GetAllByPage(int page)
        {
            var pageSize = 10;
            var param = db.BVTL_QT_THAM_SO.FirstOrDefault(x => x.ParamCode == "PageSize");
            if (param != null)
                pageSize = Convert.ToInt32(param.ParamValue);
            return db.BVTL_QT_PAGE_ACTION.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<BVTL_QT_PAGE_ACTION> GetAllByPageMenu(int page)
        {
            return db.BVTL_QT_PAGE_ACTION.Where(x=>x.PAGE_ID == page).ToList();
        }

        public List<BVTL_QT_PAGE_ACTION> GetAll()
        {
            return db.BVTL_QT_PAGE_ACTION.ToList();
        }

        public ObjectMessage Add(BVTL_QT_PAGE_ACTION model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.BVTL_QT_PAGE_ACTION.Add(model);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }catch(Exception ex)
            {
                log.Error("BVTL_QT_PAGE_ACTIONDA - Thêm action cho 1 trang(" + model.CONTROL_NAME + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(BVTL_QT_PAGE_ACTION model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_QT_PAGE_ACTION.FirstOrDefault(x => x.PAGE_ID == model.PAGE_ID);
                data.CONTROL_NAME = model.CONTROL_NAME;
                data.CONTROL_DESC = model.CONTROL_DESC;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("BVTL_QT_PAGE_ACTIONDA - Sửa action cho 1 trang(" + model.CONTROL_NAME + ") lỗi: " + ex.Message);
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
                var itemDelete = db.BVTL_QT_PAGE_ACTION.Find(Id);
                db.BVTL_QT_PAGE_ACTION.Remove(itemDelete);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("BVTL_QT_PAGE_ACTIONDA - Xóa action cho 1 trang(" + Id + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
