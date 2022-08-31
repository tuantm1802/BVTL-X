using Common;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class BVTL_NHOM_TBHDA: IBVTL_NHOM_TBHDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<BVTL_NHOM_TBH> GetAllByPage(ModelSearch modelSearch, ref int totalRow)
        {
            db.Configuration.ProxyCreationEnabled = false;
            int skipRows = (modelSearch.currentPage - 1) * modelSearch.pageSize;
            if (string.IsNullOrEmpty(modelSearch.KeyWord))
            {
                var queryResultPage = db.BVTL_NHOM_TBH.ToList();

                totalRow = queryResultPage.Count();

                queryResultPage = queryResultPage.Skip(skipRows)
              .Take(modelSearch.pageSize).ToList();

                return queryResultPage.ToList();
            }
            else
            {
                var queryResultPage = db.BVTL_NHOM_TBH.Where(x => x.tennhom_tbh.Contains(modelSearch.KeyWord)).ToList();
                totalRow = queryResultPage.Count();

                queryResultPage = queryResultPage.Skip(skipRows)
              .Take(modelSearch.pageSize).ToList();

                return queryResultPage.ToList();
            }

        }

        /// <summary>
        /// Lấy Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
        public BVTL_NHOM_TBH GetItemById(string maNhom)
        {
            return db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == maNhom);
        }

        /// <summary>
        /// Lấy danh sách người dùng thep Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
        public List<BVTL_QT_NGUOI_DUNG> GetAllUserById(string maNhom)
        {
            return (from tg in db.BVTL_NHOM_TBH
                    join utg in db.BVTL_QT_NGUOI_DUNG_NHOM_TBH on tg.manhom_tbh equals utg.NhomTBHMa
                    join u in db.BVTL_QT_NGUOI_DUNG on utg.NguoiDungId equals u.ID
                    where tg.manhom_tbh == maNhom
                    select u).ToList();
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="BVTL_NHOM_TBH"></param>
        /// <returns></returns>
        public ObjectMessage Add(BVTL_NHOM_TBH BVTL_NHOM_TBH)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.BVTL_NHOM_TBH.Add(BVTL_NHOM_TBH);
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

        /// <summary>
        /// Chỉnh sửa
        /// </summary>
        /// <param name="BVTL_NHOM_TBH"></param>
        /// <returns></returns>
        public ObjectMessage Edit(BVTL_NHOM_TBH BVTL_NHOM_TBH)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == BVTL_NHOM_TBH.manhom_tbh);
                data.manhom_tbh = BVTL_NHOM_TBH.manhom_tbh;
                data.tennhom_tbh = BVTL_NHOM_TBH.tennhom_tbh;
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

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
        public ObjectMessage Delete(string maNhom, int userId)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == maNhom);
                db.BVTL_NHOM_TBH.Remove(data);
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
