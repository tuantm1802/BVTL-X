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
        /// Lấy danh sách dự án
        /// </summary>
        /// <returns></returns>
        public List<BVTL_DU_AN> GetAll()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.BVTL_DU_AN.ToList();
        }

    }
}
