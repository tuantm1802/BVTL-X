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
            var result = db.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.UserName == userName);
            if (result == null)
                return 0;
            else
            {
                if (!result.Status)
                    return -1;
                else
                {
                    if (result.Password == password)
                        return 1;
                    else
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
                            CityCodes = u.CityCodes
                        }).FirstOrDefault();


            result.RoleName = db.BVTL_QT_QUYEN.FirstOrDefault(x => x.ID == result.GroupID).Name;

            // Lấy danh sách nhóm thu thập dữ liệu
            result.TestGroups = new List<BVTL_NHOM_TBH>();
            result.TestGroups = (from tg in db.BVTL_NHOM_TBH
                                 join utg in db.BVTL_QT_NGUOI_DUNG_NHOM_TBH on tg.manhom_tbh equals utg.NhomTBHMa
                                 where utg.NguoiDungId == Id
                                 select tg).ToList();

            // Lấy danh sách tỉnh quản lý
            result.Citys = new List<BVTL_CITES>();
            if (!string.IsNullOrEmpty(result.CityCodes))
            {
                var cityCodes = result.CityCodes.Split(',').ToList();
                result.Citys = db.BVTL_CITES.Where(x=> cityCodes.Contains(x.Code)).ToList();
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
                var param = new List<SqlParameter>
                {
                    new SqlParameter("Keyword", string.IsNullOrEmpty(modelSearch.KeyWord) ? DBNull.Value : (object)modelSearch.KeyWord),//System.Data.SqlDbType.NVarChar,250,
                    new SqlParameter("OrderByName", modelSearch.SortColumn),
                    new SqlParameter("Page", modelSearch.currentPage),
                    new SqlParameter("PageSize", modelSearch.pageSize)
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
                        model.Password = _encryptor.MD5Hash("123456789a@");
                        model.IsActive = true;
                        context.BVTL_QT_NGUOI_DUNG.Add(model);
                        context.SaveChanges();

                        var cityCodes = "";

                        // Thêm người dùng vào nhóm
                        if (model.ID > 0 && maNhomTBHs.Count > 0)
                        {
                            var nhomTBH = new BVTL_NHOM_TBH();
                            for (int i = 0; i < maNhomTBHs.Count; i++)
                            {
                                context.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Add(new BVTL_QT_NGUOI_DUNG_NHOM_TBH { NguoiDungId = (int)model.ID, NhomTBHMa = maNhomTBHs[i], IsActive = true });

                                // Lấy nhóm tbh
                                nhomTBH = context.BVTL_NHOM_TBH.FirstOrDefault(x=>x.manhom_tbh == maNhomTBHs[i]);
                                if(nhomTBH != null && !string.IsNullOrEmpty(nhomTBH.city_code))
                                {
                                    if (string.IsNullOrEmpty(cityCodes))
                                        cityCodes = nhomTBH.city_code;
                                    else
                                    {
                                        if(!cityCodes.Contains(nhomTBH.city_code))
                                            cityCodes +=','+ nhomTBH.city_code;
                                    }
                                }
                            }
                            context.SaveChanges();

                        }
                        // thêm danh sách tỉnh quản lý
                        model.CityCodes = cityCodes;
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
                        
                        var cityCodes = "";
                        if (model.ID > 0 && maNhomTBHs.Count > 0)
                        {
                            var nhomTBH = new BVTL_NHOM_TBH();
                            var allTestGroup = context.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Where(x => x.NguoiDungId == (int)model.ID).ToList();
                            if (allTestGroup != null && allTestGroup.Count > 0)
                            {
                                for (int i = 0; i < allTestGroup.Count; i++)
                                {
                                    allTestGroup[i].IsActive = false;

                                }
                                context.SaveChanges();
                            }
                            var check = 0;
                            var checkTGs = new List<BVTL_QT_NGUOI_DUNG_NHOM_TBH>();
                            for (int i = 0; i < maNhomTBHs.Count; i++)
                            {
                                check = allTestGroup.Where(x => x.NguoiDungId == model.ID && x.NhomTBHMa == maNhomTBHs[i]).Count();
                                if (check == 0)
                                    context.BVTL_QT_NGUOI_DUNG_NHOM_TBH.Add(new BVTL_QT_NGUOI_DUNG_NHOM_TBH { NguoiDungId = (int)model.ID, NhomTBHMa = maNhomTBHs[i], IsActive = true });
                                else
                                {
                                    checkTGs = allTestGroup.Where(x => x.NguoiDungId == model.ID && x.NhomTBHMa == maNhomTBHs[i]).ToList();
                                    for (int j = 0; j < checkTGs.Count; j++)
                                    {
                                        checkTGs[j].IsActive = false;
                                    }
                                }

                                // Lấy nhóm tbh
                                nhomTBH = context.BVTL_NHOM_TBH.FirstOrDefault(x => x.manhom_tbh == maNhomTBHs[i]);
                                if (nhomTBH != null && !string.IsNullOrEmpty(nhomTBH.city_code))
                                {
                                    if (string.IsNullOrEmpty(cityCodes))
                                        cityCodes = nhomTBH.city_code;
                                    else
                                    {
                                        if (!cityCodes.Contains(nhomTBH.city_code))
                                            cityCodes += ',' + nhomTBH.city_code;
                                    }
                                }
                            }
                            context.SaveChanges();
                        }

                        // thêm danh sách tỉnh quản lý
                        model.CityCodes = cityCodes;
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
                        //data.CityCodes = model.CityCodes;
                        context.SaveChanges();

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

        public ObjectMessage ChangePassword(long nguoiDungId, string passwordOd, string passwordNew)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.ID == nguoiDungId);
                var passwordOldb = data.Password;
                string passwordOd1 = _encryptor.MD5Hash(passwordOd);
                if (passwordOldb != passwordOd1)
                {
                    obj.Error = true;
                    obj.Title = "Bạn nhập mật khẩu cũ không đúng.";
                }
                else
                {
                    data.Password = _encryptor.MD5Hash(passwordNew);
                    db.SaveChanges();
                    obj.Error = false;
                    obj.Title = "Thêm mới thành công!";
                }

                return obj;
            }
            catch (Exception ex)
            {
                obj.Error = true;
                obj.Title = ex.Message;
                return obj;
            }

        }

        public ObjectMessage Delete(int Id)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.BVTL_QT_NGUOI_DUNG.FirstOrDefault(x => x.ID == Id);
                data.IsActive = false;
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
        /// Kiểm tra xem BVTL_QT_NGUOI_DUNG có bị khóa không
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <returns></returns>
        public bool CheckLock(int nguoiDungId)
        {
            var result = false;

            using (var context = new BVTL_REPORTINGEntities())
            {

                var resultPro = context.BVTL_QT_NGUOI_DUNG.Where(x => x.IsActive == false).ToList();
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
    }
    public class DataSelect
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
