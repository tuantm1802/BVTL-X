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
    public class KetQuaSKTTDA : IKetQuaSKTTDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy kết quả SKTT theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public KetQuaSKTTPageModel GetItemById(int id)
        {
            var result = new KetQuaSKTTPageModel();

            var param = new List<SqlParameter>
                {
                    new SqlParameter("Id", id),
                };
            var resultPro = _DatabaseSql.ExecuteProcToList<KetQuaSKTTPageModel>(Constants.SP_KetQuaSKTT_Get_By_Id, param).ToList();
            if (resultPro != null && resultPro.Count > 0)
                result = resultPro.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Lấy kết quả SKTT theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<KetQuaSKTTPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<KetQuaSKTTPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Months", string.IsNullOrEmpty(modelSearch.Months) ? DBNull.Value : (object)modelSearch.Months),
                    new SqlParameter("Year", modelSearch.Year == null ? 0 : (object)modelSearch.Year),
                    new SqlParameter("CityCodes", string.IsNullOrEmpty(modelSearch.CityCodes) ? DBNull.Value : (object)modelSearch.CityCodes),
                    new SqlParameter("MaDuAn", string.IsNullOrEmpty(modelSearch.MaDuAn) ? DBNull.Value : (object)modelSearch.MaDuAn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<KetQuaSKTTPageModel>(Constants.SP_KetQuaSKTT_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "KetQuaSKTTDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách kết quả SKTT theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<KetQuaSKTTPageModel>();
            }
            return result;
        }
    }
}
