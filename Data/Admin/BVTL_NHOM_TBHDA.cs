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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),
                    new SqlParameter("OrderByName", string.IsNullOrEmpty(modelSearch.SortColumn) ? "manhom_tbh" : (object)modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage <= 0 ? 1 : modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize <= 0 ? 10 : modelSearch.pageSize),
                    new SqlParameter("CityCode", string.IsNullOrEmpty(modelSearch.CityCode) ? DBNull.Value : (object)modelSearch.CityCode)
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
                if (model == null)
                {
                    obj.Error = true;
                    obj.Title = "Dữ liệu thêm mới không hợp lệ!";
                    return obj;
                }

                if (string.IsNullOrWhiteSpace(model.manhom_tbh))
                {
                    obj.Error = true;
                    obj.Title = "Vui lòng nhập Mã nhóm!";
                    return obj;
                }

                model.manhom_tbh = model.manhom_tbh.Trim().ToUpper();

                if (model.manhom_tbh.Length > 6)
                {
                    obj.Error = true;
                    obj.Title = "Mã nhóm tối đa 6 ký tự (Ví dụ: HN_01, HCM01, TTDL01)!";
                    return obj;
                }

                if (!Regex.IsMatch(model.manhom_tbh, @"^[A-Z0-9_]+$"))
                {
                    obj.Error = true;
                    obj.Title = "Mã nhóm chỉ chứa chữ cái không dấu, chữ số và dấu gạch dưới (Ví dụ: HN_01, HCM01)!";
                    return obj;
                }

                if (string.IsNullOrWhiteSpace(model.tennhom_tbh))
                {
                    obj.Error = true;
                    obj.Title = "Vui lòng nhập Tên nhóm thu thập dữ liệu!";
                    return obj;
                }
                model.tennhom_tbh = model.tennhom_tbh.Trim();

                if (string.IsNullOrWhiteSpace(model.city_code))
                {
                    obj.Error = true;
                    obj.Title = "Vui lòng chọn Tỉnh / Thành phố quản lý!";
                    return obj;
                }
                model.city_code = model.city_code.Trim();

                // Kiểm tra trùng mã nhóm
                if (db.BVTL_NHOM_TBH.Any(x => x.manhom_tbh == model.manhom_tbh))
                {
                    obj.Error = true;
                    obj.Title = $"Mã nhóm '{model.manhom_tbh}' đã tồn tại trong hệ thống. Vui lòng chọn mã khác!";
                    return obj;
                }

                // Thiết lập các trường mặc định nếu chưa có
                model.manhom_tbh_map = model.manhom_tbh;
                if (string.IsNullOrWhiteSpace(model.maduan)) model.maduan = "CD45";
                if (string.IsNullOrWhiteSpace(model.PREFIX)) model.PREFIX = "Nhóm";
                if (string.IsNullOrWhiteSpace(model.SHORT_PREFIX)) model.SHORT_PREFIX = "Nhóm";
                if (string.IsNullOrWhiteSpace(model.CHUC_DANH)) model.CHUC_DANH = "Trưởng nhóm";

                db.BVTL_NHOM_TBH.Add(model);
                db.SaveChanges();

                obj.Error = false;
                obj.Title = $"Thêm mới nhóm '{model.tennhom_tbh}' thành công!";
                return obj;
            }
            catch (DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErr = string.Join("; ", errors);
                log.Error("Lỗi validate khi thêm nhóm: " + fullErr, ex);
                obj.Error = true;
                obj.Title = "Dữ liệu không hợp lệ: " + (string.IsNullOrWhiteSpace(fullErr) ? ex.Message : fullErr);
                return obj;
            }
            catch (DbUpdateException ex)
            {
                log.Error("Lỗi DbUpdateException khi thêm nhóm: " + ex.Message, ex);
                obj.Error = true;
                obj.Title = "Không thể thêm nhóm: Mã nhóm đã tồn tại hoặc vi phạm ràng buộc dữ liệu!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Lỗi ngoại lệ khi thêm nhóm: " + ex.Message, ex);
                obj.Error = true;
                obj.Title = "Lỗi khi thêm nhóm: " + ex.Message;
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
                if (model == null || string.IsNullOrWhiteSpace(model.manhom_tbh))
                {
                    obj.Error = true;
                    obj.Title = "Dữ liệu cập nhật không hợp lệ!";
                    return obj;
                }

                var data = db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == model.manhom_tbh);
                if (data == null)
                {
                    obj.Error = true;
                    obj.Title = "Không tìm thấy thông tin nhóm cần cập nhật!";
                    return obj;
                }

                if (string.IsNullOrWhiteSpace(model.tennhom_tbh))
                {
                    obj.Error = true;
                    obj.Title = "Tên nhóm không được để trống!";
                    return obj;
                }

                if (string.IsNullOrWhiteSpace(model.city_code))
                {
                    obj.Error = true;
                    obj.Title = "Vui lòng chọn Tỉnh / Thành phố quản lý!";
                    return obj;
                }

                data.tennhom_tbh = model.tennhom_tbh.Trim();
                data.city_code = model.city_code.Trim();
                if (string.IsNullOrWhiteSpace(data.maduan)) data.maduan = "CD45";
                if (!string.IsNullOrWhiteSpace(model.PREFIX)) data.PREFIX = model.PREFIX.Trim();
                if (!string.IsNullOrWhiteSpace(model.SHORT_PREFIX)) data.SHORT_PREFIX = model.SHORT_PREFIX.Trim();
                if (!string.IsNullOrWhiteSpace(model.CHUC_DANH)) data.CHUC_DANH = model.CHUC_DANH.Trim();

                db.SaveChanges();
                obj.Error = false;
                obj.Title = "Cập nhật thông tin nhóm thành công!";
                return obj;
            }
            catch (DbEntityValidationException ex)
            {
                var errors = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                var fullErr = string.Join("; ", errors);
                log.Error("Lỗi validate khi cập nhật nhóm: " + fullErr, ex);
                obj.Error = true;
                obj.Title = "Dữ liệu không hợp lệ: " + (string.IsNullOrWhiteSpace(fullErr) ? ex.Message : fullErr);
                return obj;
            }
            catch (DbUpdateException ex)
            {
                log.Error("Lỗi DbUpdateException khi cập nhật nhóm: " + ex.Message, ex);
                obj.Error = true;
                obj.Title = "Lỗi cập nhật: Vi phạm ràng buộc cơ sở dữ liệu!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("Lỗi khi cập nhật nhóm: " + ex.Message, ex);
                obj.Error = true;
                obj.Title = "Lỗi khi cập nhật nhóm: " + ex.Message;
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
                if (string.IsNullOrWhiteSpace(maNhom))
                {
                    obj.Error = true;
                    obj.Title = "Mã nhóm cần xóa không hợp lệ!";
                    return obj;
                }

                var data = db.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == maNhom);
                if (data == null)
                {
                    obj.Error = true;
                    obj.Title = "Không tìm thấy nhóm cần xóa trên hệ thống!";
                    return obj;
                }

                // 1. Kiểm tra các bảng dữ liệu nghiệp vụ (xét nghiệm, báo cáo, theo dõi, biểu mẫu...)
                var relatedModules = new List<string>();
                if (db.BVTL_KQ_XN_HIV.Any(x => x.manhom_tbh == maNhom)) relatedModules.Add("Xét nghiệm HIV");
                if (db.BVTL_KQ_SL_ACE.Any(x => x.manhom_tbh == maNhom)) relatedModules.Add("Sàng lọc ACE");
                if (db.BVTL_KQ_XN_NUOC_TIEU.Any(x => x.manhom_tbh == maNhom)) relatedModules.Add("Xét nghiệm Nước tiểu");
                if (db.BVTL_BO_BIEU_MAU_KH_BAO_CAO.Any(x => x.manhom_tbh == maNhom)) relatedModules.Add("Biểu mẫu báo cáo KH");
                if (db.BVTL_THEO_DAU_KH.Any(x => x.manhom_tbh == maNhom)) relatedModules.Add("Theo dõi khách hàng");
                if (db.BVTL_THONG_TIN_TRUYEN_THONG.Any(x => x.manhom_tbh == maNhom) || db.VIIV_THONG_TIN_TRUYEN_THONG.Any(x => x.manhom_tbh == maNhom)) relatedModules.Add("Thông tin truyền thông");
                if (db.VIIV_TRAINING_DATA_COLLECTION.Any(x => x.manhom_tbh == maNhom)) relatedModules.Add("Tập huấn thu thập");
                if (db.VIIV_TT_KH_MAT_DAU.Any(x => x.manhom_tbh == maNhom)) relatedModules.Add("Khách hàng mất dấu");

                if (relatedModules.Count > 0)
                {
                    obj.Error = true;
                    obj.Title = $"Không thể xóa nhóm '{data.tennhom_tbh}' ({maNhom}) do đã phát sinh dữ liệu trong các phân hệ: {string.Join(", ", relatedModules)}!";
                    return obj;
                }

                // 2. Nếu không có dữ liệu nghiệp vụ, nhưng có liên kết người dùng trong BVTL_QT_NGUOI_DUNG_NHOM_TBH:
                // Tự động giải phóng liên kết người dùng trước khi xóa nhóm
                var userLinks = db.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Where(x => x.NhomTBHMa == maNhom).ToList();
                if (userLinks.Count > 0)
                {
                    db.BVTL_QT_NGUOI_DUNG_NHOM_TBH.RemoveRange(userLinks);
                }

                // 3. Xóa nhóm
                db.BVTL_NHOM_TBH.Remove(data);
                db.SaveChanges();

                obj.Error = false;
                obj.Title = $"Đã xóa nhóm '{data.tennhom_tbh}' ({maNhom}) thành công!";
                return obj;
            }
            catch (DbUpdateException ex)
            {
                log.Error($"Lỗi DbUpdateException khi xóa nhóm {maNhom}: {ex.Message}", ex);
                obj.Error = true;
                obj.Title = $"Không thể xóa nhóm '{maNhom}' do đang có dữ liệu liên kết trên hệ thống!";
                return obj;
            }
            catch (Exception ex)
            {
                log.Error($"Lỗi khi xóa nhóm {maNhom}: {ex.Message}", ex);
                obj.Error = true;
                obj.Title = "Lỗi khi xóa nhóm: " + ex.Message;
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
