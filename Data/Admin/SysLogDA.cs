using Model.Model;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class SysLogDA
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();

        /// <summary>
        /// Lấy danh sách log theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<SysLog> GetLogByPage(ModelSearch modelSearch)
        {
            if (string.IsNullOrEmpty(modelSearch.KeyWord))
            {
                var queryResultPage = db.SysLogs
              .Skip(modelSearch.pageSize * modelSearch.currentPage)
              .Take(modelSearch.pageSize);

                return queryResultPage.ToList();
            }
            else
            {
                var queryResultPage = db.SysLogs.Where(x => x.Content.Contains(modelSearch.KeyWord) || x.ControllerName.Contains(modelSearch.KeyWord))
              .Skip(modelSearch.pageSize * modelSearch.currentPage)
              .Take(modelSearch.pageSize);

                return queryResultPage.ToList();
            }
        }

        public ObjectMessage Add(SysLog sysLog)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.SysLogs.Add(sysLog);
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
        public ObjectMessage Edit(SysLog sysLog)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.SysLogs.FirstOrDefault(x => x.ID == sysLog.ID);
                data.ControllerName = sysLog.ControllerName;
                data.UserName = sysLog.UserName;
                data.DateLog = sysLog.DateLog;
                data.Content = sysLog.Content;

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
