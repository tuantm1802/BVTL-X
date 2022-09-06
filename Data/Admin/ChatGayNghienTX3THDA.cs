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
    public class ChatGayNghienTX3THDA : IChatGayNghienTX3THDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy dữ liệu theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BVTL_CHAT_GAY_NGHIEN_TX3TH GetItemById(int id)
        {
            return db.BVTL_CHAT_GAY_NGHIEN_TX3TH.FirstOrDefault(x => x.id == id);
        }

        /// <summary>
        /// Lấy dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<ChatGayNghienTX3THPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<ChatGayNghienTX3THPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<ChatGayNghienTX3THPageModel>(Constants.SP_ChatGayNghienTX3TH_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "ChatGayNghienTX3THDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách ChatGayNghienTX3TH theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<ChatGayNghienTX3THPageModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy all dữ liệu
        /// </summary>
        /// <returns></returns>
        public List<BVTL_CHAT_GAY_NGHIEN_TX3TH> GetAll()
        {
            return db.BVTL_CHAT_GAY_NGHIEN_TX3TH.ToList();
        }
    }
}
