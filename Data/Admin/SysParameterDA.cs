using log4net;
using Model.Model;
using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class SysParameterDA
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public SysParameter GetItemByCode(string code)
        {
            return db.SysParameters.FirstOrDefault(x => x.ParamCode == code);
        }

        public List<SysParameterPageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize)
        {
            var result = new List<SysParameterPageModel>();
            pageSize = 10;
            try
            {
                var param = db.SysParameters.FirstOrDefault(x => x.ParamCode == "PageSize");
                if (param != null)
                    pageSize = Convert.ToInt32(param.ParamValue);

                var sqlString = "SELECT *, count(ID) over() as TotalRow FROM [SysParameters] WHERE IsActive =1";
                if (!string.IsNullOrEmpty(modelSearch.KeyWord))
                {
                    sqlString += " AND (ParamCode LIKE N'%" + modelSearch.KeyWord + "%' OR Desctiption LIKE N'%" + modelSearch.KeyWord + "%')";
                }
                if (!string.IsNullOrEmpty(modelSearch.SortColumn))
                    sqlString += " ORDER BY " + modelSearch.SortColumn;
                sqlString += " OFFSET " + ((modelSearch.currentPage - 1) * modelSearch.pageSize) + " ROWS FETCH NEXT " + modelSearch.pageSize + " ROWS ONLY;";
                result = db.Database.SqlQuery<SysParameterPageModel>(sqlString).ToList();
            }
            catch (Exception ex)
            {
                var log = new SysLog
                {
                    ControllerName = "SysParameterDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách tham số theo trang lỗi:" + ex.Message
                };
                db.SysLogs.Add(log);
                result = new List<SysParameterPageModel>();
            }
            return result;
        }

        public List<SysParameter> GetAll()
        {
            return db.SysParameters.ToList();
        }


        public ObjectMessage Add(SysParameter sysParameter)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                sysParameter.IsActive = true;
                db.SysParameters.Add(sysParameter);
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
        public ObjectMessage Edit(SysParameter sysParameter)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (BaoCaoBVTLEntities context = new BaoCaoBVTLEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {

                        var data = context.SysParameters.FirstOrDefault(x => x.ID == sysParameter.ID);
                        data.ParamCode = sysParameter.ParamCode;
                        data.ParamValue = sysParameter.ParamValue;
                        data.Desctiption = sysParameter.Desctiption;

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        obj.Error = false;
                        obj.Title = "Cập nhật thành công!";

                    }
                }

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
                var itemDelete = db.SysParameters.Find(Id);
                db.SysParameters.Remove(itemDelete);
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
