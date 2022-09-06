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
using Common.Common;
using Common.ICommon;

namespace Data.Admin
{
    public class LoaiDoiTuongDA : ILoaiDoiTuongDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy loại đối tượng theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BVTL_LOAI_DOI_TUONG GetItemById(int id)
        {
            return db.BVTL_LOAI_DOI_TUONG.FirstOrDefault(x => x.id == id);
        }

        /// <summary>
        /// Lấy danh sách loại đối tượng theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<LoaiDoiTuongPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<LoaiDoiTuongPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                   new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<LoaiDoiTuongPageModel>(Constants.SP_LoaiDoiTuong_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "LoaiDoiTuongDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách loại đối tượng theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<LoaiDoiTuongPageModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy tất cả loại đối tượng
        /// </summary>
        /// <returns></returns>
        public List<BVTL_LOAI_DOI_TUONG> GetAll()
        {
            return db.BVTL_LOAI_DOI_TUONG.ToList();
        }
    }
}
