using Common;
using Common.Common;
using Common.ICommon;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class BVTL_NHOM_TBHDA: IBVTL_NHOM_TBHDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<NhomTBHPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<NhomTBHPageModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<NhomTBHPageModel>(Constants.SP_NhomTBH_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "BVTL_NHOM_TBHDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách Nhóm tbh theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<NhomTBHPageModel>();
            }
            return result;

        }

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu
        /// </summary>
        /// <returns></returns>
        public List<BVTL_NHOM_TBH> GetAll()
        {
            try
            {
                var sql = "SELECT manhom_tbh, tennhom_tbh, city_code, manhom_tbh_map, maduan, ISNULL(PREFIX, N'Nhóm') AS PREFIX, ISNULL(SHORT_PREFIX, N'Nhóm') AS SHORT_PREFIX, ISNULL(CHUC_DANH, N'Trưởng nhóm') AS CHUC_DANH FROM BVTL_NHOM_TBH";
                var dtoList = db.Database.SqlQuery<BVTL_NHOM_TBH_DTO>(sql).ToList();
                return dtoList.Select(d => new BVTL_NHOM_TBH
                {
                    manhom_tbh = d.manhom_tbh,
                    tennhom_tbh = d.tennhom_tbh,
                    city_code = d.city_code,
                    manhom_tbh_map = d.manhom_tbh_map,
                    maduan = d.maduan,
                    PREFIX = d.PREFIX,
                    SHORT_PREFIX = d.SHORT_PREFIX,
                    CHUC_DANH = d.CHUC_DANH
                }).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Lỗi GetAll Nhóm: " + ex.Message);
                db.Configuration.ProxyCreationEnabled = false;
                return db.BVTL_NHOM_TBH.ToList();
            }
        }

        /// <summary>
        /// Lấy Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
        public BVTL_NHOM_TBH GetItemByMaNhom(string maNhom)
        {
            try
            {
                var pMa = new SqlParameter("@MaNhom", (object)maNhom ?? DBNull.Value);
                var sql = "SELECT TOP 1 manhom_tbh, tennhom_tbh, city_code, manhom_tbh_map, maduan, ISNULL(PREFIX, N'Nhóm') AS PREFIX, ISNULL(SHORT_PREFIX, N'Nhóm') AS SHORT_PREFIX, ISNULL(CHUC_DANH, N'Trưởng nhóm') AS CHUC_DANH FROM BVTL_NHOM_TBH WHERE manhom_tbh = @MaNhom OR manhom_tbh_map = @MaNhom";
                var dto = db.Database.SqlQuery<BVTL_NHOM_TBH_DTO>(sql, pMa).FirstOrDefault();
                if (dto != null)
                {
                    return new BVTL_NHOM_TBH
                    {
                        manhom_tbh = dto.manhom_tbh,
                        tennhom_tbh = dto.tennhom_tbh,
                        city_code = dto.city_code,
                        manhom_tbh_map = dto.manhom_tbh_map,
                        maduan = dto.maduan,
                        PREFIX = dto.PREFIX,
                        SHORT_PREFIX = dto.SHORT_PREFIX,
                        CHUC_DANH = dto.CHUC_DANH
                    };
                }
                return null;
            }
            catch
            {
                db.Configuration.ProxyCreationEnabled = false;
                return db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == maNhom || x.manhom_tbh_map == maNhom);
            }
        }

        /// <summary>
        /// Lấy Nhóm thu thập dữ liệu theo nhiều mã
        /// </summary>
        /// <param name="maNhoms"></param>
        /// <returns></returns>
        public List<NhomTBHPageModel> GetItemByMaNhoms(string maNhoms)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (!string.IsNullOrEmpty(maNhoms))
            {
                var result = (from ntbh in db.BVTL_NHOM_TBH
                              join c in db.BVTL_CITES on ntbh.city_code equals c.Code
                              where maNhoms.Contains(ntbh.manhom_tbh)
                              select new NhomTBHPageModel
                              {
                                  manhom_tbh = ntbh.manhom_tbh,
                                  tennhom_tbh = ntbh.tennhom_tbh,
                                  city_code = ntbh.city_code,
                                  CityName = c.Name
                              }).ToList();

                return result;
            }
            else
            {
                var result = (from ntbh in db.BVTL_NHOM_TBH
                              join c in db.BVTL_CITES on ntbh.city_code equals c.Code
                              select new NhomTBHPageModel
                              {
                                  manhom_tbh = ntbh.manhom_tbh,
                                  tennhom_tbh = ntbh.tennhom_tbh,
                                  city_code = ntbh.city_code,
                                  CityName = c.Name
                              }).ToList();

                return result;
            }
        }
        
        public List<NhomTBHPageModel> GetItemByMaNhomMap(string maNhom)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (!string.IsNullOrEmpty(maNhom))
            {
                var result = (from ntbh in db.BVTL_NHOM_TBH
                              join c in db.BVTL_CITES on ntbh.city_code equals c.Code
                              where maNhom.Contains(ntbh.manhom_tbh_map)
                                && ntbh.manhom_tbh_map != null
                              select new NhomTBHPageModel
                              {
                                  manhom_tbh = ntbh.manhom_tbh_map,
                                  tennhom_tbh = ntbh.tennhom_tbh,
                                  city_code = ntbh.city_code,
                                  CityName = c.Name
                              }).ToList();

                return result;
            }
            else
            {
                var result = (from ntbh in db.BVTL_NHOM_TBH
                              join c in db.BVTL_CITES on ntbh.city_code equals c.Code
                              where ntbh.manhom_tbh_map != null
                              select new NhomTBHPageModel
                              {
                                  manhom_tbh = ntbh.manhom_tbh_map,
                                  tennhom_tbh = ntbh.tennhom_tbh,
                                  city_code = ntbh.city_code,
                                  CityName = c.Name
                              }).ToList();

                return result;
            }
        }


        /// <summary>
        /// Lấy Nhóm thu thập dữ liệu theo tỉnh
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
        public List<NhomTBHPageModel> GetItemByCityCodes(string cityCodes)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (!string.IsNullOrEmpty(cityCodes))
            {
                var result = (from ntbh in db.BVTL_NHOM_TBH
                              join c in db.BVTL_CITES on ntbh.city_code equals c.Code
                              where cityCodes.Contains(c.Code)
                              select new NhomTBHPageModel
                              {
                                  manhom_tbh = ntbh.manhom_tbh,
                                  tennhom_tbh = ntbh.tennhom_tbh,
                                  city_code = ntbh.city_code,
                                  CityName = c.Name
                              }).ToList();

                return result;
            }
            else
            {
                var result = (from ntbh in db.BVTL_NHOM_TBH
                              join c in db.BVTL_CITES on ntbh.city_code equals c.Code
                              select new NhomTBHPageModel
                              {
                                  manhom_tbh = ntbh.manhom_tbh,
                                  tennhom_tbh = ntbh.tennhom_tbh,
                                  city_code = ntbh.city_code,
                                  CityName = c.Name
                              }).ToList();

                return result;
            }
           
        }

        public List<NhomTBHPageModel> GetItemByCityCodesMaDuAn(string cityCodes,string maDuAn)
        {
            db.Configuration.ProxyCreationEnabled = false;
            if (!string.IsNullOrEmpty(cityCodes))
            {
                var result = (from ntbh in db.BVTL_NHOM_TBH
                              join c in db.BVTL_CITES on ntbh.city_code equals c.Code
                              where cityCodes.Contains(c.Code)
                              select new NhomTBHPageModel
                              {
                                  manhom_tbh = ntbh.manhom_tbh_map,
                                  tennhom_tbh = ntbh.tennhom_tbh,
                                  city_code = ntbh.city_code,
                                  CityName = c.Name
                              }).ToList();

                return result;
            }
            else
            {
                var result = (from ntbh in db.BVTL_NHOM_TBH
                              join c in db.BVTL_CITES on ntbh.city_code equals c.Code
                              where (maDuAn == null || ntbh.maduan.Contains(maDuAn))
                              select new NhomTBHPageModel
                              {
                                  manhom_tbh = ntbh.manhom_tbh_map,
                                  tennhom_tbh = ntbh.tennhom_tbh,
                                  city_code = ntbh.city_code,
                                  CityName = c.Name
                              }).ToList();

                return result;
            }

        }

        /// <summary>
        /// Lấy danh sách người dùng thep Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
        public List<BVTL_QT_NGUOI_DUNG> GetAllUserByMaNhom(string maNhom)
        {
            return (from tg in db.BVTL_NHOM_TBH
                    join utg in db.BVTL_QT_NGUOI_DUNG_NHOM_TBH on tg.manhom_tbh equals utg.NhomTBHMa
                    join u in db.BVTL_QT_NGUOI_DUNG on utg.NguoiDungId equals u.ID
                    where tg.manhom_tbh == maNhom
                    select u).ToList();
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ObjectMessage Add(BVTL_NHOM_TBH model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                db.BVTL_NHOM_TBH.Add(model);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Thêm mới thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }

        /// <summary>
        /// Chỉnh sửa
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ObjectMessage Edit(BVTL_NHOM_TBH model)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == model.manhom_tbh);
                data.manhom_tbh = model.manhom_tbh;
                data.tennhom_tbh = model.tennhom_tbh;
                data.city_code = model.city_code;
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }

        /// <summary>
        /// Xóa
        /// </summary>
        /// <param name="maNhom"></param>
        /// <returns></returns>
        public ObjectMessage Delete(string maNhom, int userId)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == maNhom);
                db.BVTL_NHOM_TBH.Remove(data);
                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Xóa thành công!";
                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }

        /// <summary>
        /// Cập nhật Xưng danh (Prefix), Xưng danh viết tắt (ShortPrefix) và Chức danh người ký (ChucDanh) cho Nhóm CBO
        /// </summary>
        public bool UpdatePrefix(string maNhom, string prefix, string shortPrefix, string chucDanh = null)
        {
            try
            {
                var pMa = new SqlParameter("@MaNhom", (object)maNhom ?? DBNull.Value);
                var pPrefix = new SqlParameter("@Prefix", string.IsNullOrWhiteSpace(prefix) ? "Nhóm" : (object)prefix.Trim());
                var pShort = new SqlParameter("@ShortPrefix", string.IsNullOrWhiteSpace(shortPrefix) ? (object)pPrefix.Value : (object)shortPrefix.Trim());
                var pChuc = new SqlParameter("@ChucDanh", string.IsNullOrWhiteSpace(chucDanh) ? (object)DBNull.Value : (object)chucDanh.Trim());

                db.Database.ExecuteSqlCommand("EXEC dbo.SP_CD45_UpdateNhomPrefix @MaNhom, @Prefix, @ShortPrefix, @ChucDanh", pMa, pPrefix, pShort, pChuc);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Lỗi UpdatePrefix nhóm: " + ex.Message, ex);
                return false;
            }
        }

    }
}
