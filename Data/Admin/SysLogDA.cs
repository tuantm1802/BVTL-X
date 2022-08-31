using Data.InterfaceDA.Admin;
using Model.Model;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class SysLogDA: ISysLogDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        /// <summary>
        /// Lấy danh sách log theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<BVTL_QT_LOG> GetLogByPage(ModelSearch modelSearch)
        {
            if (string.IsNullOrEmpty(modelSearch.KeyWord))
            {
                var queryResultPage = db.BVTL_QT_LOG
              .Skip(modelSearch.pageSize * modelSearch.currentPage)
              .Take(modelSearch.pageSize);

                return queryResultPage.ToList();
            }
            else
            {
                var queryResultPage = db.BVTL_QT_LOG.Where(x => x.Content.Contains(modelSearch.KeyWord) || x.ControllerName.Contains(modelSearch.KeyWord))
              .Skip(modelSearch.pageSize * modelSearch.currentPage)
              .Take(modelSearch.pageSize);

                return queryResultPage.ToList();
            }
        }

        public ObjectMessage Add(BVTL_QT_LOG model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.BVTL_QT_LOG.Add(model);
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
        public ObjectMessage Edit(BVTL_QT_LOG model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_QT_LOG.FirstOrDefault(x => x.ID == model.ID);
                data.ControllerName = model.ControllerName;
                data.UserName = model.UserName;
                data.DateLog = model.DateLog;
                data.Content = model.Content;

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

    }
}
