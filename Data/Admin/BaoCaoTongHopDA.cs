using log4net;
using Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Data.InterfaceDA.Admin;
using Model.ModelExtend.Base;
using Common.Common;
using Common.ICommon;
using System.Data.SqlClient;
using Model.ModelExtend.Report;
using Model.ModelExtend;

namespace Data.Admin
{
    public class BaoCaoTongHopDA : IBaoCaoTongHopDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy báo cáo
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<BaoCaoModel> GetDataReport(ReportSearchModel modelSearch)
        {
            var result = new List<BaoCaoModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Months", string.IsNullOrEmpty(modelSearch.Months) ? DBNull.Value : (object)modelSearch.Months),
                    new SqlParameter("Year", modelSearch.Year == null ? 0 : (object)modelSearch.Year),
                    new SqlParameter("CityCodes", string.IsNullOrEmpty(modelSearch.CityCodes) ? DBNull.Value : (object)modelSearch.CityCodes),
                    new SqlParameter("TypeReport", modelSearch.TypeReport)
                };
                result = _DatabaseSql.ExecuteProcToList<BaoCaoModel>(Constants.SP_Report_Get_All_Data, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "BaoCaoTongHopDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy tổng hợp báo cáo theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<BaoCaoModel>();
            }
            return result;
        }


        /// <summary>
        /// Lấy dữ liệu báo cáo tổng hợp theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<BaoCaoTongHopPageModel> GetBaoCaoTongHopByPage(ModelSearch modelSearch)
        {
            var result = new List<BaoCaoTongHopPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Months", string.IsNullOrEmpty(modelSearch.Months) ? DBNull.Value : (object)modelSearch.Months),
                    new SqlParameter("Year", modelSearch.Year == null ? 0 : (object)modelSearch.Year),
                    new SqlParameter("CityCodes", string.IsNullOrEmpty(modelSearch.CityCodes) ? DBNull.Value : (object)modelSearch.CityCodes),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<BaoCaoTongHopPageModel>(Constants.SP_BaoCaoTongHop_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "BaoCaoTongHopDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách dữ liệu báo cáo tổng hợp theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<BaoCaoTongHopPageModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy báo cáo tổng hợp theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaoCaoTongHopPageModel GetItemById(int id)
        {
            var result = new BaoCaoTongHopPageModel();

            var param = new List<SqlParameter>
                {
                    new SqlParameter("Id", id),
                };
            var resultPro = _DatabaseSql.ExecuteProcToList<BaoCaoTongHopPageModel>(Constants.SP_BaoCaoTongHop_Get_By_Id, param).ToList();
            if (resultPro != null && resultPro.Count > 0)
                result = resultPro.FirstOrDefault();
            return result;
        }

    }
}
