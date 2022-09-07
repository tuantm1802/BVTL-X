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
    public class DuongSuDungDA : IDuongSuDungDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BVTL_DUONG_SU_DUNG GetItemById(int id)
        {
            return db.BVTL_DUONG_SU_DUNG.FirstOrDefault(x => x.id == id);
        }

        /// <summary>
        /// Lấy dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<DuongSuDungPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<DuongSuDungPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                   new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<DuongSuDungPageModel>(Constants.SP_DuongSuDung_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "DuongSuDungDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách DuongSuDung theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<DuongSuDungPageModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả dữ liệu
        /// </summary>
        /// <returns></returns>
        public List<BVTL_DUONG_SU_DUNG> GetAll()
        {
            return db.BVTL_DUONG_SU_DUNG.ToList();
        }
    }
}
