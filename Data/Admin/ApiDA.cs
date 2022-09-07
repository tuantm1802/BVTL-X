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
using Common.ICommon;
using Common.Common;
using System.Data.SqlClient;

namespace Data.Admin
{
    public class ApiDA : IApiDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy thông tin api theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ApiPageModel GetItemByCode(string code)
        {
            var result = new ApiPageModel();
            var api = db.BVTL_API.FirstOrDefault(x => x.Api_Code == code);
            var tables = db.BVTL_MASTER_TABLE.Where(x => x.Api_Id == api.Api_Id).Select(x => x.table_name).ToList();

            result.Api_Id = api.Api_Id;
            result.Api_Code = api.Api_Code;
            result.NameSyncdata = api.NameSyncdata;
            result.HrefApi = api.HrefApi;
            result.TypeApi = api.TypeApi;
            result.TokenApi = api.TokenApi;
            result.TableNameSaveData = api.TableNameSaveData;
            result.IsActive = api.IsActive;
            result.ReportId = api.ReportId;
            result.TimeReCall = api.TimeReCall;
            result.TableNames = tables;
            return result;
        }

        /// <summary>
        /// Tìm kiếm api theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<ApiPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<ApiPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<ApiPageModel>(Constants.SP_Api_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "ApiDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách Api theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<ApiPageModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả đầu api
        /// </summary>
        /// <returns></returns>
        public List<BVTL_API> GetAll()
        {
            return db.BVTL_API.ToList();
        }
    }
}
