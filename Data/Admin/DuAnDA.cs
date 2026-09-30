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
using Common.Common;
using Common.ICommon;
using System.Data.SqlClient;

namespace Data.Admin
{
    public class DuAnDA : IDuAnDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy thông tin dự án theo mã
        /// </summary>
        /// <param name="maduan"></param>
        /// <returns></returns>
        public BVTL_DU_AN GetItemByCode(string maduan)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.BVTL_DU_AN.FirstOrDefault(x => x.maduan == maduan);
        }

        /// <summary>
        /// Lấy danh sách dự án theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<DuAnPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<DuAnPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<DuAnPageModel>(Constants.SP_DuAn_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "DuAnDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách dự án theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<DuAnPageModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách dự án (kèm trạng thái IsActive)
        /// </summary>
        /// <returns></returns>
        public List<BVTL_DU_AN> GetAll()
        {
            try
            {
                var sql = "SELECT maduan, tenduan, ISNULL(IsActive, 1) AS IsActive FROM BVTL_DU_AN";
                var dtoList = db.Database.SqlQuery<BVTL_DU_AN_DTO>(sql).ToList();
                return dtoList.Select(d => new BVTL_DU_AN
                {
                    maduan = d.maduan,
                    tenduan = d.tenduan,
                    IsActive = d.IsActive
                }).ToList();
            }
            catch (Exception ex)
            {
                log.Warn("Lỗi lấy danh sách dự án với IsActive: " + ex.Message);
                db.Configuration.ProxyCreationEnabled = false;
                return db.BVTL_DU_AN.ToList();
            }
        }

        /// <summary>
        /// Lấy danh sách dự án theo người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<BVTL_DU_AN> GetDuAnReport(int userId)
        {
            var result = new List<BVTL_DU_AN>();
            try
            {
                var allDuAn = GetAll();
                var user = db.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.ID == userId);
                if (user == null) return result;

                // Admin hệ thống: xem tất cả dự án
                if (user.IsAdmin)
                    return allDuAn;

                // MaDuAn rỗng: trả về rỗng, không để lộ toàn bộ dự án
                if (string.IsNullOrEmpty(user.MaDuAn))
                    return result;

                var listMaDuAn = user.MaDuAn.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                return allDuAn.Where(x => listMaDuAn.Contains(x.maduan)).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetDuAnReport: " + ex.Message, ex);
                result = new List<BVTL_DU_AN>();
            }
            return result;
        }
    }
}
