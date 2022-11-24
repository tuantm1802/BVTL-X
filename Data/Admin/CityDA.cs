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
    public class CityDA : ICityDA
    {
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lấy thông tin tỉnh theo mã
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public BVTL_CITES GetItemByCode(string code)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.BVTL_CITES.FirstOrDefault(x => x.Code == code);
        }

        /// <summary>
        /// Lấy danh sách tỉnh theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<CityPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<CityPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<CityPageModel>(Constants.SP_City_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "CityDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách tỉnh theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<CityPageModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách tỉnh
        /// </summary>
        /// <returns></returns>
        public List<BVTL_CITES> GetAll()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.BVTL_CITES.ToList();
        }

        /// <summary>
        /// Lấy danh sách tỉnh theo người dùng
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<BVTL_CITES> GetCityReport(int userId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var result = new List<BVTL_CITES>();
            try
            {
                var user = db.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x=>x.ID == userId);
                if (string.IsNullOrEmpty(user.CityCodes) || user.IsAdmin)
                {
                    return db.BVTL_CITES.ToList();
                }
                else
                {
                    var cityCodes = user.CityCodes.Split(',').ToList();
                    var citys = db.BVTL_CITES.Where(x => cityCodes.Contains(x.Code)).ToList();
                    return citys != null ? citys : new List<BVTL_CITES>();
                }
            }
            catch (Exception)
            {
                result = new List<BVTL_CITES>();
            }
            return result;
        }

    }
}
