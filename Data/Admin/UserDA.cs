using Common;
using log4net;
using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.User;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Admin
{
    public class UserDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();
        public int Login(string userName, string password)
        {
            var result = db.Users.FirstOrDefault(x => x.UserName == userName);
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
        public User GetItemByUserName(string userName)
        {
            return db.Users.FirstOrDefault(x => x.UserName == userName);
        }
        public UserPageModel GetItemById(int Id)
        {
            var result = new UserPageModel();

            var user = db.Users.FirstOrDefault(x => x.ID == Id);
            result.ID = user.ID;
            result.Name = user.Name;
            result.Phone = user.Phone;
            result.Status = user.Status;
            result.UserName = user.UserName;
            result.Role = user.Role;
            result.RoleName = user.Role.Name;
            result.Address = user.Address;
            result.Avartar = user.Avartar;
            result.Email = user.Email;
            result.CreatedDate = user.CreatedDate;

            // Lấy danh sách nhóm thu thập dữ liệu
            result.TestGroups = (from tg in db.TestGroups
                                 join utg in db.User_TestGroup on tg.Id equals utg.TestGroupId
                                 where utg.UserId == Id
                                 select tg).ToList();

            return result;
        }

        public List<string> GetListCredentials(string userName)
        {
            var query = from pm in db.PageMenus
                        join rp in db.RolePages on pm.ID equals rp.PageID
                        join u in db.Users on rp.RoleID equals u.GroupID
                        where u.UserName == userName && !string.IsNullOrEmpty(pm.CONTROLLER_NAME)
                        select new
                        {
                            ID = pm.CONTROLLER_NAME
                        };

            return query.Select(x => x.ID).ToList();
        }

        public ObjectMessage Add(User user, List<int> testGroupId)
        {
            ObjectMessage obj = new ObjectMessage();

            using (BaoCaoBVTLEntities context = new BaoCaoBVTLEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Thêm người dùng
                        user.Password = Encryptor.MD5Hash("123456789a@");
                        user.IsActive = true;
                        context.Users.Add(user);
                        context.SaveChanges();

                        // Thêm người dùng vào nhóm
                        if (user.ID > 0 && testGroupId.Count > 0)
                        {
                            for (int i = 0; i < testGroupId.Count; i++)
                            {
                                context.User_TestGroup.Add(new User_TestGroup { UserId = (int)user.ID, TestGroupId = testGroupId[i], Is_Active = true });
                            }
                            context.SaveChanges();
                        }
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
        public ObjectMessage Edit(User user, List<int> testGroupId)
        {
            ObjectMessage obj = new ObjectMessage();
            using (BaoCaoBVTLEntities context = new BaoCaoBVTLEntities())
            {
                using (var dbContextTransaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var data = context.Users.FirstOrDefault(x => x.ID == user.ID);
                        data.UserName = user.UserName;
                        data.Address = user.Address;
                        data.Name = user.Name;
                        data.Email = user.Email;
                        data.Phone = user.Phone;
                        data.Avartar = user.Avartar;
                        data.Status = user.Status;
                        data.DateOfBirth = user.DateOfBirth;
                        data.Gender = user.Gender;
                        data.IdNumber = user.IdNumber;
                        data.Possition = user.Possition;
                        data.GroupID = user.GroupID;
                        context.SaveChanges();

                        if (user.ID > 0 && testGroupId.Count > 0)
                        {
                            var allTestGroup = context.User_TestGroup.Where(x => x.UserId == (int)user.ID).ToList();
                            if (allTestGroup != null && allTestGroup.Count > 0)
                            {
                                for (int i = 0; i < allTestGroup.Count; i++)
                                {
                                    allTestGroup[i].Is_Active = false;

                                }
                                context.SaveChanges();
                            }
                            var check = 0;
                            var checkTGs = new List<User_TestGroup>();
                            for (int i = 0; i < testGroupId.Count; i++)
                            {
                                check = allTestGroup.Where(x => x.UserId == user.ID && x.TestGroupId == testGroupId[i]).Count();
                                if (check == 0)
                                    context.User_TestGroup.Add(new User_TestGroup { UserId = (int)user.ID, TestGroupId = testGroupId[i], Is_Active = true });
                                else
                                {
                                    checkTGs = allTestGroup.Where(x => x.UserId == user.ID && x.TestGroupId == testGroupId[i]).ToList();
                                    for (int j = 0; j < checkTGs.Count; j++)
                                    {
                                        checkTGs[j].Is_Active = false;
                                    }
                                }
                            }
                            context.SaveChanges();
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

        public ObjectMessage ChangePassword(long userId, string passwordOd, string passwordNew)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.Users.FirstOrDefault(x => x.ID == userId);
                var passwordOldb = data.Password;
                string passwordOd1 = Encryptor.MD5Hash(passwordOd);
                if (passwordOldb != passwordOd1)
                {
                    obj.Error = true;
                    obj.Title = "Bạn nhập mật khẩu cũ không đúng.";
                }
                else
                {
                    data.Password = Encryptor.MD5Hash(passwordNew);
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
                var data = db.Users.FirstOrDefault(x => x.ID == Id);
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
        /// Kiểm tra xem user có bị khóa không
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckLock(int userId)
        {
            var result = false;

            using (var context = new BaoCaoBVTLEntities())
            {

                var resultPro = context.Users.Where(x => x.IsActive == false).ToList();
                if (resultPro != null && resultPro.Count > 0)
                    result = true;
                else
                    result = false;
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
