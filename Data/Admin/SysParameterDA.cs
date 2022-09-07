using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class SysParameterDA: ISysParameterDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public BVTL_QT_THAM_SO GetItemById(int id)
        {
            return db.BVTL_QT_THAM_SO.FirstOrDefault(x => x.ID == id);
        }

        public List<SysParameterPageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize)
        {
            var result = new List<SysParameterPageModel>();
            pageSize = 10;
            try
            {
                var param = db.BVTL_QT_THAM_SO.FirstOrDefault(x => x.ParamCode == "PageSize");
                if (param != null)
                    pageSize = Convert.ToInt32(param.ParamValue);

                var sqlString = "SELECT *, count(ID) over() as TotalRow FROM [BVTL_QT_THAM_SO] WHERE IsActive =1";
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
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "BVTL_QT_THAM_SODA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách tham số theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<SysParameterPageModel>();
            }
            return result;
        }

        public List<BVTL_QT_THAM_SO> GetAll()
        {
            return db.BVTL_QT_THAM_SO.ToList();
        }


        public ObjectMessage Add(BVTL_QT_THAM_SO model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                model.IsActive = true;
                db.BVTL_QT_THAM_SO.Add(model);
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
        public ObjectMessage Edit(BVTL_QT_THAM_SO model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (BVTL_REPORTINGEntities context = new BVTL_REPORTINGEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {

                        var data = context.BVTL_QT_THAM_SO.FirstOrDefault(x => x.ID == model.ID);
                        data.ParamCode = model.ParamCode;
                        data.ParamValue = model.ParamValue;
                        data.Desctiption = model.Desctiption;

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
                var itemDelete = db.BVTL_QT_THAM_SO.Find(Id);
                db.BVTL_QT_THAM_SO.Remove(itemDelete);
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
