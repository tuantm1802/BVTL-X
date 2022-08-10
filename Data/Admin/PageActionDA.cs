using Common;
using log4net;
using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class PageActionDA
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public PageAction GetItemByPage(int pageId)
        {
            return db.PageActions.FirstOrDefault(x => x.PAGE_ID == pageId);
        }

        public List<PageAction> GetAllByPage(int page)
        {
            var pageSize = 10;
            var param = db.SysParameters.FirstOrDefault(x => x.ParamCode == "PageSize");
            if (param != null)
                pageSize = Convert.ToInt32(param.ParamValue);
            return db.PageActions.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
        }

        /// <summary>
        /// Lấy danh sách action trên 1 page menu
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public List<PageAction> GetAllByPageMenu(int page)
        {
            return db.PageActions.Where(x=>x.PAGE_ID == page).ToList();
        }

        public List<PageAction> GetAll()
        {
            return db.PageActions.ToList();
        }

        public ObjectMessage Add(PageAction pageAction)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.PageActions.Add(pageAction);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }catch(Exception ex)
            {
                log.Error("PageActionDA - Thêm action cho 1 trang(" + pageAction.CONTROL_NAME + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
        public ObjectMessage Edit(PageAction pageAction)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.PageActions.FirstOrDefault(x => x.PAGE_ID == pageAction.PAGE_ID);
                data.CONTROL_NAME = pageAction.CONTROL_NAME;
                data.CONTROL_DESC = pageAction.CONTROL_DESC;

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("PageActionDA - Sửa action cho 1 trang(" + pageAction.CONTROL_NAME + ") lỗi: " + ex.Message);
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
                var itemDelete = db.PageActions.Find(Id);
                db.PageActions.Remove(itemDelete);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("PageActionDA - Xóa action cho 1 trang(" + Id + ") lỗi: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }
    }
}
