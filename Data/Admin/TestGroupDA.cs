using Common;
using log4net;
using Model.Model;
using Model.ModelExtend;
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
    public class TestGroupDA
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        BaoCaoBVTLEntities db = new BaoCaoBVTLEntities();

        /// <summary>
        /// Lấy tất cả Nhóm thu thập dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        public List<TestGroup> GetAllByPage(ModelSearch modelSearch, ref int totalRow)
        {
            int skipRows = (modelSearch.currentPage - 1) * modelSearch.pageSize;
            if (string.IsNullOrEmpty(modelSearch.KeyWord))
            {
                var queryResultPage = db.TestGroups.ToList();

                totalRow = queryResultPage.Count();

                queryResultPage = queryResultPage.Skip(skipRows)
              .Take(modelSearch.pageSize).ToList();

                return queryResultPage.ToList();
            }
            else
            {
                var queryResultPage = db.TestGroups.Where(x => x.Name.Contains(modelSearch.KeyWord)).ToList();
                totalRow = queryResultPage.Count();

                queryResultPage = queryResultPage.Skip(skipRows)
              .Take(modelSearch.pageSize).ToList();

                return queryResultPage.ToList();
            }

        }

        /// <summary>
        /// Lấy Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public TestGroup GetItemById(int Id)
        {
            return db.TestGroups.FirstOrDefault(x => x.Id == Id);
        }

        /// <summary>
        /// Lấy danh sách người dùng thep Nhóm thu thập dữ liệu theo id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public List<User> GetAllUserById(int Id)
        {
            return (from tg in db.TestGroups
                    join utg in db.User_TestGroup on tg.Id equals utg.TestGroupId
                    join u in db.Users on utg.UserId equals u.ID
                    where tg.Id == Id
                    select u).ToList();
        }

        /// <summary>
        /// Thêm mới
        /// </summary>
        /// <param name="TestGroup"></param>
        /// <returns></returns>
        public ObjectMessage Add(TestGroup TestGroup)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                TestGroup.CreatedDate = DateTime.Now;
                TestGroup.IsActive = true;
                db.TestGroups.Add(TestGroup);
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
        /// <param name="TestGroup"></param>
        /// <returns></returns>
        public ObjectMessage Edit(TestGroup TestGroup)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.TestGroups.FirstOrDefault(x => x.Id == TestGroup.Id);
                data.Code = TestGroup.Code;
                data.Name = TestGroup.Name;
                data.LastUpdateDate = DateTime.Now;
                data.LastUpdateBy = TestGroup.LastUpdateBy;
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
        /// <param name="Id"></param>
        /// <returns></returns>
        public ObjectMessage Delete(int Id, int userId)
        {
            ObjectMessage obj = new ObjectMessage();
            try
            {
                var data = db.TestGroups.FirstOrDefault(x => x.Id == Id);
                data.IsActive = false;
                data.LastUpdateDate = DateTime.Now;
                data.LastUpdateBy = userId;
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

    }
}
