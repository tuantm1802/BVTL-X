using Common;
using Common.Common;
using Common.ICommon;
using Data.InterfaceDA.Admin;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using Model.ModelExtend.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class UserDA : IUserDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        BVTL_REPORTINGEntities db = new BVTL_REPORTINGEntities();
        IDatabaseSql _DatabaseSql = new DatabaseSql();
        IEncryptor _encryptor = new Encryptor();
        public int Login(string userName, string password)
        {
            using (var context = new BVTL_REPORTINGEntities())
            {
                var result = context.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.UserName == userName);
                if (result == null)
                    return 0;

                if (!result.Status)
                    return -1;

                string savedHash = result.Password;
                string md5Hash = _encryptor.MD5Hash(password);

                if (savedHash != null && (savedHash.Equals(md5Hash, StringComparison.OrdinalIgnoreCase) || _encryptor.VerifyPassword(password, savedHash)))
                {
                    return 1;
                }
                else
                {
                    return -2;
                }
            }
        }
        public BVTL_QT_NGUOI_DUNG GetItemByUserName(string userName)
        {
            return db.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.UserName == userName);
        }
        public UserPageModel GetItemById(int Id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var result = (from u in db.BVTL_QT_NGUOI_DUNG
                          join r in db.BVTL_QT_QUYEN on u.GroupID equals r.ID
                          where u.ID == Id
                          select new UserPageModel
                          {
                              ID = u.ID,
                              Name = u.Name,
                              Phone = u.Phone,
                              Status = u.Status,
                              UserName = u.UserName,
                              RoleName = r.Name,
                              UserGroupID = u.GroupID,
                              GroupID = u.GroupID,
                              Address = u.Address,
                              Avartar = u.Avartar,
                              Email = u.Email,
                              CreatedDate = u.CreatedDate,
                              DateOfBirth = u.DateOfBirth,
                              Gender = u.Gender,
                              IdNumber = u.IdNumber,
                              Possition = u.Possition,
                              OperativeLevel = u.OperativeLevel,
                              OriginId = u.OriginId,
                              CityCodes = u.CityCodes,
                              MaDuAn = u.MaDuAn
                          }).FirstOrDefault();


            result.RoleName = db.BVTL_QT_QUYEN.FirstOrDefault(x => x.ID == result.GroupID).Name;

            var nhomTBHs = db.BVTL_NHOM_TBH.ToList()
               .Select(x => new BVTL_NHOM_TBH
               {
                   manhom_tbh = x.manhom_tbh,
                   tennhom_tbh = x.tennhom_tbh,
                   city_code = x.city_code
               }).ToList();

            // Lấy danh sách nhóm thu thập dữ liệu
            result.TestGroups = new List<BVTL_NHOM_TBH>();
            result.TestGroups = (from tg in nhomTBHs
                                 join utg in db.BVTL_QT_NGUOI_DUNG_NHOM_TBH on tg.manhom_tbh equals utg.NhomTBHMa
                                 where utg.NguoiDungId == Id && utg.IsActive == true
                                 select tg).ToList();



            // Lấy danh sách tỉnh quản lý
            result.Citys = new List<BVTL_CITES>();
            if (!string.IsNullOrEmpty(result.CityCodes))
            {
                var citys = db.BVTL_CITES.ToList()
               .Select(x => new BVTL_CITES
               {
                   Code = x.Code,
                   Name = x.Name,
                   IsActive = x.IsActive,
                   CreatedDate = x.CreatedDate,
                   CreatedBy = x.CreatedBy,
                   LastUpdateDate = x.LastUpdateDate,
                   LastUpdateBy = x.LastUpdateBy
               }).ToList();


                var cityCodes = result.CityCodes.Split(',').ToList();
                result.Citys = citys.Where(x => cityCodes.Contains(x.Code)).ToList();
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<UserPageModel> GetAllByPage(ModelSearch modelSearch)
        {
            var result = new List<UserPageModel>();
            try
            {
                bool isDeleted = modelSearch.IsDeleted ?? (modelSearch.Status == "deleted");
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),
                    new SqlParameter("OrderByName", string.IsNullOrEmpty(modelSearch.SortColumn) ? "UserName" : (object)modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage <= 0 ? 1 : modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize <= 0 ? 10 : modelSearch.pageSize),
                    new SqlParameter("IsDeleted", isDeleted)
                };
                result = _DatabaseSql.ExecuteProcToList<UserPageModel>(Constants.SP_User_Get_By_Page, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "UserDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách người dùng theo trang lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<UserPageModel>();
            }
            return result;
        }

        public List<string> GetListCredentials(string userName)
        {
            var query = from pm in db.BVTL_QT_PAGE_MENU
                        join rp in db.BVTL_QT_QUYEN_PAGE on pm.ID equals rp.PageID
                        join u in db.BVTL_QT_NGUOI_DUNG on rp.RoleID equals u.GroupID
                        where u.UserName == userName && !string.IsNullOrEmpty(pm.CONTROLLER_NAME)
                        select new
                        {
                            ID = pm.CONTROLLER_NAME
                        };

            return query.Select(x => x.ID).ToList();
        }

        public ObjectMessage Add(BVTL_QT_NGUOI_DUNG model, List<string> maNhomTBHs)
        {
            ObjectMessage obj = new ObjectMessage();

            using (BVTL_REPORTINGEntities context = new BVTL_REPORTINGEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Thêm người dùng
                        if (!string.IsNullOrEmpty(model.Password))
                            model.Password = _encryptor.MD5Hash(model.Password);
                        else
                            model.Password = _encryptor.MD5Hash("123456789a@");
                        model.IsActive = true;
                        model = context.BVTL_QT_NGUOI_DUNG.Add(model);
                        context.SaveChanges();

                        // Thêm người dùng vào nhóm CBO (không ảnh hưởng CityCodes)
                        if (model.ID > 0 && maNhomTBHs.Count > 0)
                        {
                            for (int i = 0; i < maNhomTBHs.Count; i++)
                            {
                                context.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Add(new BVTL_QT_NGUOI_DUNG_NHOM_TBH
                                {
                                    NguoiDungId = model.ID,
                                    NhomTBHMa = maNhomTBHs[i],
                                    IsActive = true
                                });
                            }
                            context.SaveChanges();
                        }

                        // Đồng bộ phân quyền địa bàn vào bảng liên kết chuẩn BVTL_QT_NGUOI_DUNG_CITY (Task 2.2)
                        if (!string.IsNullOrEmpty(model.CityCodes))
                        {
                            var cityList = model.CityCodes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var c in cityList)
                            {
                                var cleanCode = c.Trim();
                                if (!string.IsNullOrEmpty(cleanCode))
                                {
                                    context.Database.ExecuteSqlCommand(
                                        "INSERT INTO BVTL_QT_NGUOI_DUNG_CITY (NguoiDungId, CityCode, IsActive, CreatedDate) VALUES (@userId, @cityCode, 1, GETDATE())",
                                        new SqlParameter("@userId", model.ID),
                                        new SqlParameter("@cityCode", cleanCode));
                                }
                            }
                        }

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                        obj.Error = false;
                        obj.Title = "Thêm mới thành công!";

                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        obj.Error = true;
                        obj.Title = ex.Message;
                    }

                }
            }
            return obj;
        }
        public ObjectMessage Edit(BVTL_QT_NGUOI_DUNG model, List<string> maNhomTBHs)
        {
            ObjectMessage obj = new ObjectMessage();
            using (BVTL_REPORTINGEntities context = new BVTL_REPORTINGEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var data = context.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.ID == model.ID);

                        if (model.ID > 0 && maNhomTBHs.Count > 0)
                        {
                            // Vô hiệu hoá tất cả nhóm CBO cũ
                            var allTestGroup = context.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Where(x => x.NguoiDungId == model.ID).ToList();
                            if (allTestGroup != null && allTestGroup.Count > 0)
                            {
                                for (int i = 0; i < allTestGroup.Count; i++)
                                    allTestGroup[i].IsActive = false;
                                context.SaveChanges();
                            }

                            // Thêm/kích hoạt nhóm CBO mới được chọn
                            var checkTGs = new List<BVTL_QT_NGUOI_DUNG_NHOM_TBH>();
                            for (int i = 0; i < maNhomTBHs.Count; i++)
                            {
                                var check = allTestGroup.Where(x => x.NguoiDungId == model.ID && x.NhomTBHMa == maNhomTBHs[i]).Count();
                                if (check == 0)
                                    context.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Add(new BVTL_QT_NGUOI_DUNG_NHOM_TBH { NguoiDungId = model.ID, NhomTBHMa = maNhomTBHs[i], IsActive = true });
                                else
                                {
                                    checkTGs = allTestGroup.Where(x => x.NguoiDungId == model.ID && x.NhomTBHMa == maNhomTBHs[i]).ToList();
                                    for (int j = 0; j < checkTGs.Count; j++)
                                        checkTGs[j].IsActive = true;
                                }
                            }
                            context.SaveChanges();
                        }

                        // CityCodes được admin chọn trực tiếp (từ UserController.Edit),
                        // không ghi đè tự động từ nhóm CBO nữa
                        data.CityCodes = model.CityCodes;
                        data.UserName = model.UserName;
                        data.Address = model.Address;
                        data.Name = model.Name;
                        data.Email = model.Email;
                        data.Phone = model.Phone;
                        data.Avartar = model.Avartar;
                        data.Status = model.Status;
                        data.DateOfBirth = model.DateOfBirth;
                        data.Gender = model.Gender;
                        data.IdNumber = model.IdNumber;
                        data.Possition = model.Possition;
                        data.GroupID = model.GroupID;
                        data.MaDuAn = model.MaDuAn;
                        context.SaveChanges();

                        // Đồng bộ phân quyền địa bàn vào bảng liên kết chuẩn BVTL_QT_NGUOI_DUNG_CITY (Task 2.2)
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM BVTL_QT_NGUOI_DUNG_CITY WHERE NguoiDungId = @userId",
                            new SqlParameter("@userId", data.ID));

                        if (!string.IsNullOrEmpty(model.CityCodes))
                        {
                            var cityList = model.CityCodes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var c in cityList)
                            {
                                var cleanCode = c.Trim();
                                if (!string.IsNullOrEmpty(cleanCode))
                                {
                                    context.Database.ExecuteSqlCommand(
                                        "INSERT INTO BVTL_QT_NGUOI_DUNG_CITY (NguoiDungId, CityCode, IsActive, CreatedDate) VALUES (@userId, @cityCode, 1, GETDATE())",
                                        new SqlParameter("@userId", data.ID),
                                        new SqlParameter("@cityCode", cleanCode));
                                }
                            }
                        }

                        obj.Error = false;
                        obj.Title = "Cập nhật thành công!";
                        dbContextTransaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        obj.Error = true;
                        obj.Title = ex.Message;
                        dbContextTransaction.Rollback();
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// thay đổi mật khẩu
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <param name="passwordOd"></param>
        /// <param name="passwordNew"></param>
        /// <returns></returns>
        public ObjectMessage ChangePassword(long nguoiDungId, string passwordOd, string passwordNew)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (var context = new BVTL_REPORTINGEntities())
                {
                    var data = context.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.ID == nguoiDungId);
                    if (data == null)
                    {
                        obj.Error = true;
                        obj.Title = "Không tìm thấy thông tin người dùng.";
                        return obj;
                    }

                    var passwordOldb = data.Password;
                    string md5Old = _encryptor.MD5Hash(passwordOd);
                    bool isCorrect = (passwordOldb != null && (passwordOldb.Equals(md5Old, StringComparison.OrdinalIgnoreCase) || _encryptor.VerifyPassword(passwordOd, passwordOldb)));

                    if (!isCorrect)
                    {
                        obj.Error = true;
                        obj.Title = "Bạn nhập mật khẩu cũ không đúng.";
                    }
                    else
                    {
                        data.Password = _encryptor.MD5Hash(passwordNew);
                        context.SaveChanges();
                        obj.Error = false;
                        obj.Title = "Thay đổi mật khẩu thành công!";
                    }

                    return obj;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.PropertyName + ": " + x.ErrorMessage);
                var fullError = string.Join("; ", errorMessages);
                log.Error("ChangePassword validation error: " + fullError);
                obj.Error = true;
                obj.Title = "Lỗi xác thực dữ liệu: " + fullError;
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("ChangePassword error: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }
        }

        /// <summary>
        /// đặt lại mật khẩu mới
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <param name="passwordNew"></param>
        /// <returns></returns>
        public ObjectMessage ResetPassword(long nguoiDungId, string passwordNew)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                using (var context = new BVTL_REPORTINGEntities())
                {
                    var data = context.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.ID == nguoiDungId);
                    if (data == null)
                    {
                        obj.Error = true;
                        obj.Title = "Không tìm thấy thông tin người dùng.";
                        return obj;
                    }

                    obj.Email = data.Email;
                    data.Password = _encryptor.MD5Hash(passwordNew);
                    context.SaveChanges();
                    obj.Error = false;
                    obj.Title = "Reset mật khẩu thành công!";

                    return obj;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.PropertyName + ": " + x.ErrorMessage);
                var fullError = string.Join("; ", errorMessages);
                log.Error("ResetPassword validation error: " + fullError);
                obj.Error = true;
                obj.Title = "Lỗi xác thực dữ liệu: " + fullError;
                return obj;
            }
            catch (Exception ex)
            {
                log.Error("ResetPassword error: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }
        }

        public ObjectMessage Delete(int Id)
        {
            return DoDeleteUser(Id, 0, "System");
        }

        public ObjectMessage ActiveUser(int Id)
        {
            return ToggleLockUser(Id, false, "System");
        }

        public ObjectMessage CheckCanDeleteUser(int id, long currentUserId)
        {
            var obj = new ObjectMessage { Error = false };
            try
            {
                using (var context = new BVTL_REPORTINGEntities())
                {
                    var user = context.BVTL_QT_NGUOI_DUNG.AsNoTracking().FirstOrDefault(x => x.ID == id);
                    if (user == null)
                    {
                        obj.Error = true;
                        obj.Title = "Không tìm thấy người dùng yêu cầu!";
                        return obj;
                    }

                    if (user.IsAdmin || string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
                    {
                        obj.Error = true;
                        obj.Title = "Không thể xóa tài khoản Quản trị viên hệ thống (Admin)!";
                        return obj;
                    }

                    if (user.ID == currentUserId)
                    {
                        obj.Error = true;
                        obj.Title = "Bạn không thể tự xóa tài khoản của chính mình!";
                        return obj;
                    }

                    int loginCount = 0;
                    try { loginCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_LOGIN_HISTORY WHERE UserId = @id OR UserName = @uName", new SqlParameter("@id", id), new SqlParameter("@uName", user.UserName ?? "")).FirstOrDefault(); } catch {}

                    int logCount = 0;
                    try { logCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_QT_LOG WHERE UserName = @uName", new SqlParameter("@uName", user.UserName ?? "")).FirstOrDefault(); } catch {}

                    int featCount = 0;
                    try { featCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_FEATURE_USAGE_LOG WHERE UserId = @id OR UserName = @uName", new SqlParameter("@id", id), new SqlParameter("@uName", user.UserName ?? "")).FirstOrDefault(); } catch {}

                    int exportCount = 0;
                    try { exportCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_EXPORTED_REPORT_LOG WHERE CreatedBy = @uName", new SqlParameter("@uName", user.UserName ?? "")).FirstOrDefault(); } catch {}

                    int groupCount = context.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Count(x => x.NguoiDungId == id);
                    int cityCount = 0;
                    try { cityCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_QT_NGUOI_DUNG_CITY WHERE NguoiDungId = @id", new SqlParameter("@id", id)).FirstOrDefault(); } catch {}

                    int totalActivity = loginCount + logCount + featCount + exportCount;
                    string deleteType = totalActivity > 0 ? "soft" : "hard";

                    obj.Error = false;
                    obj.Title = "Kiểm tra thành công";
                    obj.Content = deleteType;
                    obj.Object = new
                    {
                        ID = user.ID,
                        UserName = user.UserName,
                        FullName = user.Name,
                        DeleteType = deleteType,
                        TotalActivity = totalActivity,
                        LoginCount = loginCount,
                        LogCount = logCount,
                        FeatureCount = featCount,
                        ExportCount = exportCount,
                        GroupCount = groupCount,
                        CityCount = cityCount,
                        IsActive = user.IsActive
                    };
                    return obj;
                }
            }
            catch (Exception ex)
            {
                log.Error("CheckCanDeleteUser error: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }
        }

        public ObjectMessage DoDeleteUser(int id, long currentUserId, string currentUserName)
        {
            var obj = new ObjectMessage { Error = false };
            try
            {
                using (var context = new BVTL_REPORTINGEntities())
                {
                    var user = context.BVTL_QT_NGUOI_DUNG.AsNoTracking().FirstOrDefault(x => x.ID == id);
                    if (user == null)
                    {
                        obj.Error = true;
                        obj.Title = "Không tìm thấy người dùng cần xóa!";
                        return obj;
                    }

                    if (user.IsAdmin || string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
                    {
                        obj.Error = true;
                        obj.Title = "Không thể xóa tài khoản Quản trị viên hệ thống (Admin)!";
                        return obj;
                    }

                    if (currentUserId > 0 && user.ID == currentUserId)
                    {
                        obj.Error = true;
                        obj.Title = "Bạn không thể tự xóa tài khoản của chính mình!";
                        return obj;
                    }

                    string userName = user.UserName;
                    string fullName = user.Name;

                    int loginCount = 0;
                    try { loginCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_LOGIN_HISTORY WHERE UserId = @id OR UserName = @uName", new SqlParameter("@id", id), new SqlParameter("@uName", userName ?? "")).FirstOrDefault(); } catch {}

                    int logCount = 0;
                    try { logCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_QT_LOG WHERE UserName = @uName", new SqlParameter("@uName", userName ?? "")).FirstOrDefault(); } catch {}

                    int featCount = 0;
                    try { featCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_FEATURE_USAGE_LOG WHERE UserId = @id OR UserName = @uName", new SqlParameter("@id", id), new SqlParameter("@uName", userName ?? "")).FirstOrDefault(); } catch {}

                    int exportCount = 0;
                    try { exportCount = context.Database.SqlQuery<int>("SELECT COUNT(*) FROM BVTL_EXPORTED_REPORT_LOG WHERE CreatedBy = @uName", new SqlParameter("@uName", userName ?? "")).FirstOrDefault(); } catch {}

                    int totalActivity = loginCount + logCount + featCount + exportCount;

                    if (totalActivity == 0)
                    {
                        // === TRƯỜNG HỢP 1: XÓA CỨNG (HARD DELETE) ===
                        using (var trans = context.Database.BeginTransaction())
                        {
                            context.Database.ExecuteSqlCommand("DELETE FROM BVTL_QT_NGUOI_DUNG_NHOM_TBH WHERE NguoiDungId = @id", new SqlParameter("@id", id));
                            context.Database.ExecuteSqlCommand("DELETE FROM BVTL_QT_NGUOI_DUNG_CITY WHERE NguoiDungId = @id", new SqlParameter("@id", id));
                            context.Database.ExecuteSqlCommand("DELETE FROM BVTL_USER_ONLINE WHERE UserId = @id OR UserName = @uName", new SqlParameter("@id", id), new SqlParameter("@uName", userName ?? ""));

                            if (!string.IsNullOrEmpty(user.Avartar))
                            {
                                try
                                {
                                    var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(user.Avartar);
                                    if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
                                } catch {}
                            }

                            context.Database.ExecuteSqlCommand("DELETE FROM BVTL_QT_NGUOI_DUNG WHERE ID = @id", new SqlParameter("@id", id));
                            trans.Commit();
                        }

                        try
                        {
                            context.BVTL_QT_LOG.Add(new BVTL_QT_LOG
                            {
                                ControllerName = "User",
                                UserName = string.IsNullOrEmpty(currentUserName) ? "System" : currentUserName,
                                DateLog = DateTime.Now,
                                Content = "Xóa vĩnh viễn tài khoản người dùng " + userName + " (" + fullName + ", ID: " + id + ") do tài khoản chưa phát sinh dữ liệu."
                            });
                            context.SaveChanges();
                        } catch {}

                        obj.Error = false;
                        obj.Content = "hard";
                        obj.Title = "Đã xóa vĩnh viễn tài khoản " + userName + " thành công!";
                        return obj;
                    }
                    else
                    {
                        // === TRƯỜNG HỢP 2: XÓA MỀM (SOFT DELETE) ===
                        using (var trans = context.Database.BeginTransaction())
                        {
                            context.Database.ExecuteSqlCommand(
                                "UPDATE BVTL_QT_NGUOI_DUNG SET IsActive = 0, Status = 0, ModifiedDate = GETDATE(), ModifiedBy = @modifiedBy WHERE ID = @id",
                                new SqlParameter("@modifiedBy", (object)currentUserName ?? DBNull.Value),
                                new SqlParameter("@id", id)
                            );

                            context.Database.ExecuteSqlCommand("DELETE FROM BVTL_USER_ONLINE WHERE UserId = @id OR UserName = @uName", new SqlParameter("@id", id), new SqlParameter("@uName", userName ?? ""));
                            trans.Commit();
                        }

                        try
                        {
                            context.BVTL_QT_LOG.Add(new BVTL_QT_LOG
                            {
                                ControllerName = "User",
                                UserName = string.IsNullOrEmpty(currentUserName) ? "System" : currentUserName,
                                DateLog = DateTime.Now,
                                Content = "Xóa mềm tài khoản người dùng " + userName + " (" + fullName + ", ID: " + id + ") - Đã khóa và ẩn khỏi danh sách để bảo toàn " + totalActivity + " bản ghi kiểm toán/nhật ký."
                            });
                            context.SaveChanges();
                        } catch {}

                        obj.Error = false;
                        obj.Content = "soft";
                        obj.Title = "Đã xóa mềm tài khoản " + userName + " thành công (ẩn khỏi danh sách và khóa vĩnh viễn để bảo toàn lịch sử dữ liệu).";
                        return obj;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("DoDeleteUser error: " + ex.ToString());
                obj.Error = true;
                obj.Title = "Lỗi khi xóa người dùng: " + ex.Message;
                return obj;
            }
        }

        public ObjectMessage RestoreUser(int id, long currentUserId, string currentUserName)
        {
            var obj = new ObjectMessage { Error = false };
            try
            {
                using (var context = new BVTL_REPORTINGEntities())
                {
                    var user = context.BVTL_QT_NGUOI_DUNG.AsNoTracking().FirstOrDefault(x => x.ID == id);
                    if (user == null)
                    {
                        obj.Error = true;
                        obj.Title = "Không tìm thấy người dùng cần khôi phục!";
                        return obj;
                    }

                    string userName = user.UserName;

                    context.Database.ExecuteSqlCommand(
                        "UPDATE BVTL_QT_NGUOI_DUNG SET IsActive = 1, Status = 1, ModifiedDate = GETDATE(), ModifiedBy = @modifiedBy WHERE ID = @id",
                        new SqlParameter("@modifiedBy", (object)currentUserName ?? DBNull.Value),
                        new SqlParameter("@id", id)
                    );

                    try
                    {
                        context.BVTL_QT_LOG.Add(new BVTL_QT_LOG
                        {
                            ControllerName = "User",
                            UserName = string.IsNullOrEmpty(currentUserName) ? "System" : currentUserName,
                            DateLog = DateTime.Now,
                            Content = "Khôi phục tài khoản người dùng " + userName + " (ID: " + id + ") từ thùng rác."
                        });
                        context.SaveChanges();
                    } catch {}

                    obj.Error = false;
                    obj.Title = "Khôi phục tài khoản " + userName + " thành công!";
                    return obj;
                }
            }
            catch (Exception ex)
            {
                log.Error("RestoreUser error: " + ex.Message);
                obj.Error = true;
                obj.Title = "Lỗi khi khôi phục người dùng: " + ex.Message;
                return obj;
            }
        }

        public ObjectMessage ToggleLockUser(int id, bool isLock, string currentUserName)
        {
            var obj = new ObjectMessage { Error = false };
            try
            {
                using (var context = new BVTL_REPORTINGEntities())
                {
                    var user = context.BVTL_QT_NGUOI_DUNG.AsNoTracking().FirstOrDefault(x => x.ID == id);
                    if (user == null)
                    {
                        obj.Error = true;
                        obj.Title = "Không tìm thấy người dùng!";
                        return obj;
                    }

                    if (user.IsAdmin || string.Equals(user.UserName, "admin", StringComparison.OrdinalIgnoreCase))
                    {
                        obj.Error = true;
                        obj.Title = "Không thể khóa tài khoản Quản trị viên hệ thống!";
                        return obj;
                    }

                    string userName = user.UserName;

                    context.Database.ExecuteSqlCommand(
                        "UPDATE BVTL_QT_NGUOI_DUNG SET Status = @status, IsActive = 1, ModifiedDate = GETDATE(), ModifiedBy = @modifiedBy WHERE ID = @id",
                        new SqlParameter("@status", !isLock),
                        new SqlParameter("@modifiedBy", (object)currentUserName ?? DBNull.Value),
                        new SqlParameter("@id", id)
                    );

                    if (isLock)
                    {
                        try { context.Database.ExecuteSqlCommand("DELETE FROM BVTL_USER_ONLINE WHERE UserId = @id OR UserName = @uName", new SqlParameter("@id", id), new SqlParameter("@uName", userName ?? "")); } catch {}
                    }

                    try
                    {
                        context.BVTL_QT_LOG.Add(new BVTL_QT_LOG
                        {
                            ControllerName = "User",
                            UserName = string.IsNullOrEmpty(currentUserName) ? "System" : currentUserName,
                            DateLog = DateTime.Now,
                            Content = (isLock ? "Khóa tài khoản người dùng " : "Kích hoạt lại tài khoản người dùng ") + userName + " (ID: " + id + ")."
                        });
                        context.SaveChanges();
                    } catch {}

                    obj.Error = false;
                    obj.Title = isLock ? "Khóa tài khoản thành công!" : "Kích hoạt tài khoản thành công!";
                    return obj;
                }
            }
            catch (Exception ex)
            {
                log.Error("ToggleLockUser error: " + ex.Message);
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }
        }

        /// <summary>
        /// Kiểm tra xem BVTL_QT_NGUOI_DUNG có bị khóa không
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <returns></returns>
        public bool CheckLock(int nguoiDungId)
        {
            var result = false;

            using (var context = new BVTL_REPORTINGEntities())
            {

                var resultPro = context.BVTL_QT_NGUOI_DUNG.Where(x => x.ID == nguoiDungId &&  x.IsActive == false).ToList();
                if (resultPro != null && resultPro.Count > 0)
                    result = true;
                else
                    result = false;
            }

            return result;
        }

        /// <summary>
        /// Lấy dữ liệu thông báo
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<NotificationModel> GetNotification(ReportSearchModel modelSearch)
        {
            var result = new List<NotificationModel>();
            try
            {
                var param = new List<SqlParameter>
                {
                    new SqlParameter("CityCodes", string.IsNullOrEmpty(modelSearch.CityCodes) ? DBNull.Value : (object)modelSearch.CityCodes),
                    new SqlParameter("MaNhomTBHs", string.IsNullOrEmpty(modelSearch.MaNhomTBH) ? DBNull.Value : (object)modelSearch.MaNhomTBH),
                    new SqlParameter("MaDuAn", string.IsNullOrEmpty(modelSearch.MaDuAn) ? DBNull.Value : (object)modelSearch.MaDuAn),
                    //new SqlParameter("Page", modelSearch.currentPage),
                    //new SqlParameter("PageSize", modelSearch.pageSize)
                };
                result = _DatabaseSql.ExecuteProcToList<NotificationModel>(Constants.SP_Notification_Search_Data, param).ToList();
            }
            catch (Exception ex)
            {
                var log = new BVTL_QT_LOG
                {
                    ControllerName = "UserDA",
                    UserName = "",
                    DateLog = DateTime.Now,
                    Content = "Lấy danh sách thông báo lỗi:" + ex.Message
                };
                db.BVTL_QT_LOG.Add(log);
                result = new List<NotificationModel>();
            }
            return result;
        }

        /// <summary>
        /// Lấy danh sách người dùng có email not null
        /// </summary>
        /// <returns></returns>
        public List<BVTL_QT_NGUOI_DUNG> GetAllUserByEmailNotNull()
        {
            return db.BVTL_QT_NGUOI_DUNG.Where(x => x.Email != null && x.Email != "").ToList();
        }

        /// <summary>
        /// Lấy danh sách nhóm TBH theo người dùng
        /// </summary>
        /// <returns></returns>
        public string GetMaNhomTBHByUser(int userId)
        {
            var result = "";
            var nhomTBHs = db.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Where(x => x.NguoiDungId == userId).ToList();
            if (nhomTBHs != null && nhomTBHs.Count > 0)
                result = string.Join(",", nhomTBHs.Select(x => x.NhomTBHMa));
            return result;
        }
    }
    public class DataSelect
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
