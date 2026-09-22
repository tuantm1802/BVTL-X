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
                bool isKeyOnly = (modelSearch.CityCodes == "KEY_ONLY" || modelSearch.AppCode == "KEY_ONLY");
                string cityMode = string.Equals(modelSearch.CityMode, "OLD63", StringComparison.OrdinalIgnoreCase) || modelSearch.MaDuAn == "OLD63" || modelSearch.AppCode == "OLD63" ? "OLD63" : "NEW34";
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),
                    new SqlParameter("OrderByName", string.IsNullOrEmpty(modelSearch.SortColumn) ? "KeyFirst" : (object)modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage <= 0 ? 1 : modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize <= 0 ? 20 : modelSearch.pageSize),
                    new SqlParameter("IsKeyOnly", isKeyOnly),
                    new SqlParameter("CityMode", cityMode)
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
        public List<BVTL_CITES> GetAllByCodeMap()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.BVTL_CITES.Where(x=>x.Code_Map != null).ToList();
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
        public List<BVTL_CITES> GetCityReportCodeMap(int userId)
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

        /// <summary>
        /// Cập nhật trạng thái tỉnh trọng điểm CD45 và mã viết tắt
        /// </summary>
        public ObjectMessage UpdateKeyProvince(string code, string codeMap, bool isKey)
        {
            var obj = new ObjectMessage { Error = false };
            try
            {
                var city = db.BVTL_CITES.FirstOrDefault(x => x.Code == code);
                if (city == null)
                {
                    obj.Error = true;
                    obj.Title = "Không tìm thấy tỉnh/thành phố với mã: " + code;
                    return obj;
                }

                if (isKey)
                {
                    if (string.IsNullOrWhiteSpace(codeMap))
                    {
                        obj.Error = true;
                        obj.Title = "Mã viết tắt (Code_Map) không được để trống khi thiết lập làm tỉnh trọng điểm.";
                        return obj;
                    }
                    codeMap = codeMap.Trim().ToUpper();
                    // Kiểm tra trùng mã viết tắt với tỉnh khác
                    var existing = db.BVTL_CITES.FirstOrDefault(x => x.Code_Map == codeMap && x.Code != code);
                    if (existing != null)
                    {
                        obj.Error = true;
                        obj.Title = string.Format("Mã viết tắt '{0}' đã được sử dụng bởi tỉnh {1} ({2}).", codeMap, existing.Name, existing.Code);
                        return obj;
                    }
                    city.Code_Map = codeMap;
                }
                else
                {
                    city.Code_Map = null;
                }

                city.LastUpdateDate = DateTime.Now;
                db.SaveChanges();
                obj.Title = isKey ? string.Format("Đã thiết lập '{0}' thành tỉnh trọng điểm (Mã ánh xạ: {1}).", city.Name, city.Code_Map) 
                                  : string.Format("Đã gỡ trạng thái trọng điểm của '{0}'.", city.Name);
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = "Lỗi cập nhật tỉnh trọng điểm: " + ex.Message;
            }
            return obj;
        }

        /// <summary>
        /// Lấy toàn bộ danh mục 34 tỉnh mới (NQ 202/2025/QH15)
        /// </summary>
        public List<CityNewModel> GetAllNewCities(bool keyOnly = false)
        {
            try
            {
                string sql = "SELECT Code, Name, Code_Map, IsKeyProvince, OldCount, OldNamesSummary, DisplayOrder, IsActive, CreatedDate FROM BVTL_DM_TINH_MOI WHERE IsActive = 1";
                if (keyOnly)
                {
                    sql += " AND IsKeyProvince = 1";
                }
                sql += " ORDER BY CASE WHEN IsKeyProvince = 1 THEN 0 ELSE 1 END, DisplayOrder ASC, Name ASC";
                return db.Database.SqlQuery<CityNewModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetAllNewCities: " + ex.Message, ex);
                return new List<CityNewModel>();
            }
        }

        /// <summary>
        /// Lấy toàn bộ danh sách ánh xạ 63 tỉnh cũ sang 34 tỉnh mới
        /// </summary>
        public List<CityMappingModel> GetCityMappings()
        {
            try
            {
                string sql = "SELECT OldCityCode, NewCityCode, OldCityName, NewCityName, EffectiveDate FROM BVTL_MAP_TINH_CU_MOI ORDER BY NewCityCode ASC, OldCityCode ASC";
                return db.Database.SqlQuery<CityMappingModel>(sql).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetCityMappings: " + ex.Message, ex);
                return new List<CityMappingModel>();
            }
        }

        /// <summary>
        /// Lấy danh sách các mã tỉnh cũ thuộc về 1 tỉnh mới
        /// </summary>
        public List<string> GetMappedOldCityCodes(string newCityCode)
        {
            try
            {
                if (string.IsNullOrEmpty(newCityCode)) return new List<string>();
                var p = new SqlParameter("@NewCityCode", newCityCode);
                var codes = db.Database.SqlQuery<string>("SELECT OldCityCode FROM BVTL_MAP_TINH_CU_MOI WHERE NewCityCode = @NewCityCode", p).ToList();
                if (codes.Count == 0)
                {
                    codes.Add(newCityCode);
                }
                return codes;
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetMappedOldCityCodes: " + ex.Message, ex);
                return new List<string> { newCityCode };
            }
        }

    }
}
