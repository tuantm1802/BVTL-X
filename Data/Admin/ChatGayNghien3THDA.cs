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
using System.Data.SqlClient;
using Common.ICommon;
using Common.Common;

namespace Data.Admin
{
    public class ChatGayNghien3THDA : IChatGayNghien3THDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy thông tin theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BVTL_CHAT_GAY_NGHIEN_3TH GetItemById(int id)
        {
            return db.BVTL_CHAT_GAY_NGHIEN_3TH.FirstOrDefault(x => x.id == id);
        }

        /// <summary>
        /// Lấy dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<ChatGayNghien3THPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<ChatGayNghien3THPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<ChatGayNghien3THPageModel>(Constants.SP_ChatGayNghien3TH_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "ChatGayNghien3THDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách ChatGayNghien3TH theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<ChatGayNghien3THPageModel>();
            }
            return result;
        }
        /// <summary>
        /// Lấy tất cả dữ liệu
        /// </summary>
        /// <returns></returns>
        public List<BVTL_CHAT_GAY_NGHIEN_3TH> GetAll()
        {
            return db.BVTL_CHAT_GAY_NGHIEN_3TH.ToList();
        }
    }
}
