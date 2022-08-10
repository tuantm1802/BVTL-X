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
    public class CustomerDA
    {
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public Customer GetItemByCode(string code)
        {
            return db.Customers.FirstOrDefault(x => x.Code == code);
        }

        public List<CustomerPageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize)
        {
            var result = new List<CustomerPageModel>();
            pageSize = 10;
            try
            {
                var param = db.SysParameters.FirstOrDefault(x => x.ParamCode == "PageSize");
                if (param != null)
                    pageSize = Convert.ToInt32(param.ParamValue);

                var sqlString = "SELECT c.*, c.Name as CityName, " +
                    "(case when c.Gender  = 'M' then N'Nam' when c.Gender  = 'F' then N'Nữ'  else '' end) as GenderText, " +
                     "(case when c.DateOfBirth  is not null then convert(varchar,CONVERT(date, c.DateOfBirth,112), 103)  else '' end) as DateOfBirthText, " +
                    "count(c.Id) over() as TotalRow FROM [Customer] c" +
                    " inner join Cities ci on ci.Id = c.CityId" +
                    " WHERE 1 =1";
                if (!string.IsNullOrEmpty(modelSearch.KeyWord))
                {
                    sqlString += " AND (c.Code LIKE N'%" + modelSearch.KeyWord + "%' OR c.FullName LIKE N'%" + modelSearch.KeyWord + "%')";
                }
                if (!string.IsNullOrEmpty(modelSearch.SortColumn))
                    sqlString += " ORDER BY " + modelSearch.SortColumn;
                sqlString += " OFFSET " + ((modelSearch.currentPage - 1) * modelSearch.pageSize) + " ROWS FETCH NEXT " + modelSearch.pageSize + " ROWS ONLY;";
                result = db.Database.SqlQuery<CustomerPageModel>(sqlString).ToList();
            }
            catch (Exception ex)
            {
                var log = new SysLog
                {
                    ControllerName = "CustomerDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách khách hàng theo trang lỗi:" + ex.Message
                };
                db.SysLogs.Add(log);
                result = new List<CustomerPageModel>();
            }
            return result;
        }

        public List<Customer> GetAll()
        {
            return db.Customers.ToList();
        }


        public ObjectMessage Add(Customer Customer)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.Customers.Add(Customer);
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
        public ObjectMessage Edit(Customer Customer)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (BaoCaoBVTLEntities context = new BaoCaoBVTLEntities())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {

                        var data = context.Customers.FirstOrDefault(x => x.Id == Customer.Id);
                        data.Code = Customer.Code;
                        data.FullName = Customer.FullName;
                        data.Gender = Customer.Gender;
                        data.DateOfBirth = Customer.DateOfBirth;
                        data.CityId = Customer.CityId;
                        data.TypeObject = Customer.TypeObject;
                        data.Code_TCV = Customer.Code_TCV;

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
                var itemDelete = db.Customers.Find(Id);
                db.Customers.Remove(itemDelete);
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
