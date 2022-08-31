using log4net;
using Model.Model;
using Model.ModelExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.InterfaceDA.Admin;
using Model.ModelExtend.Base;

namespace Data.Admin
{
    public class CustomerDA: ICustomerDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public BVTL_KHACH_HANG GetItemByCode(string code)
        {
            return db.BVTL_KHACH_HANG.FirstOrDefault(x => x.makh == code);
        }

        public List<CustomerPageModel> GetAllByPage(ModelSearch modelSearch, ref int pageSize)
        {
            var result = new List<CustomerPageModel>();
            pageSize = 10;
            try
            {
                var param = db.BVTL_QT_THAM_SO.FirstOrDefault(x => x.ParamCode == "PageSize");
                if (param != null)
                    pageSize = Convert.ToInt32(param.ParamValue);

                var sqlString = "SELECT c.*, ci.Name as CityName, " +
                    "(case when c.gioitinh  = 'M' then N'Nam' when c.gioitinh  = 'F' then N'Nữ'  else '' end) as GioiTinhText, " +
                     "dt.name as LoaiDoiTuong, " +
                     "(case when c.ngaytiepcan is not null then CONVERT(varchar, c.ngaytiepcan, 103) else '' end) as ngaytiepcantext, " +
                    "count(c.khachhang_id) over() as TotalRow FROM [BVTL_KHACH_HANG] c" +
                    " inner join BVTL_CITES ci on ci.Code = c.city_code" +
                    " inner join BVTL_LOAI_DOI_TUONG dt on dt.id = c.loai_doi_tuong_id" +
                    " WHERE 1 =1";
                if (!string.IsNullOrEmpty(modelSearch.KeyWord))
                {
                    sqlString += " AND (c.makh LIKE N'%" + modelSearch.KeyWord + "%' OR c.hoten LIKE N'%" + modelSearch.KeyWord + "%')";
                }
                if (!string.IsNullOrEmpty(modelSearch.SortColumn))
                    sqlString += " ORDER BY " + modelSearch.SortColumn;
                sqlString += " OFFSET " + ((modelSearch.currentPage - 1) * modelSearch.pageSize) + " ROWS FETCH NEXT " + modelSearch.pageSize + " ROWS ONLY;";
                result = db.Database.SqlQuery<CustomerPageModel>(sqlString).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "CustomerDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách khách hàng theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<CustomerPageModel>();
            }
            return result;
        }

        public List<BVTL_KHACH_HANG> GetAll()
        {
            return db.BVTL_KHACH_HANG.ToList();
        }


        public ObjectMessage Add(BVTL_KHACH_HANG Customer)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.BVTL_KHACH_HANG.Add(Customer);
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
        
        public ObjectMessage Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var itemDelete = db.BVTL_KHACH_HANG.Find(Id);
                db.BVTL_KHACH_HANG.Remove(itemDelete);
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
