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
using Model.ModelExtend.API;

namespace Data.Admin
{
    public class TrainingDataCollVIIVDA : ITrainingDataCollVIIVDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Lấy kết quả Training Data Collection theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TrainingDataCollVIIVPageModel GetItemById(int id)
        {
            var result = new TrainingDataCollVIIVPageModel();

            var param = new List<SqlParameter>
                {
                    new SqlParameter("Id", id),
                };
            var resultPro = _DatabaseSql.ExecuteProcToList<TrainingDataCollVIIVPageModel>(Constants.SP_TrainingDataCollVIIV_Get_By_Id, param).ToList();
            if (resultPro != null && resultPro.Count > 0)
                result = resultPro.FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Lấy kết quả Training Data Collection theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<TrainingDataCollVIIVPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<TrainingDataCollVIIVPageModel>();
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
                result = _DatabaseSql.ExecuteProcToList<TrainingDataCollVIIVPageModel>(Constants.SP_TrainingDataCollVIIV_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "TrainingDataCollVIIVDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách kết quả Training Data Collection VIIV theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<TrainingDataCollVIIVPageModel>();
            }
            return result;
        }
    }
}
