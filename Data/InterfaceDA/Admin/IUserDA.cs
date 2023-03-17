using Model.Model;
using Model.ModelExtend;
using Model.ModelExtend.Base;
using Model.ModelExtend.Report;
using Model.ModelExtend.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.Admin
{
    public interface IUserDA
    {
         int Login(string userName, string password);
         BVTL_QT_NGUOI_DUNG GetItemByUserName(string userName);
         UserPageModel GetItemById(int Id);

        /// <summary>
        /// Lấy dữ liệu theo trang
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
         List<UserPageModel> GetAllByPage(ModelSearch modelSearch);

         List<string> GetListCredentials(string userName);

         ObjectMessage Add(BVTL_QT_NGUOI_DUNG model, List<string> maNhomTBHs);
         ObjectMessage Edit(BVTL_QT_NGUOI_DUNG model, List<string> maNhomTBHs);

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <param name="passwordOd"></param>
        /// <param name="passwordNew"></param>
        /// <returns></returns>
         ObjectMessage ChangePassword(long nguoiDungId, string passwordOd, string passwordNew);

         ObjectMessage Delete(int Id);

        ObjectMessage ActiveUser(int Id);

        /// <summary>
        /// Kiểm tra xem BVTL_QT_NGUOI_DUNG có bị khóa không
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <returns></returns>
         bool CheckLock(int nguoiDungId);

        /// <summary>
        /// Lấy dữ liệu thông báo
        /// </summary>
        /// <param name="modelSearch"></param>
        /// <returns></returns>
        List<NotificationModel> GetNotification(ReportSearchModel modelSearch);

        /// <summary>
        /// Lấy danh sách người dùng có email not null
        /// </summary>
        /// <returns></returns>
        List<BVTL_QT_NGUOI_DUNG> GetAllUserByEmailNotNull();

        /// <summary>
        /// Lấy danh sách nhóm TBH theo người dùng
        /// </summary>
        /// <returns></returns>
        string GetMaNhomTBHByUser(int userId);

        /// <summary>
        /// đặt lại mật khẩu mới
        /// </summary>
        /// <param name="nguoiDungId"></param>
        /// <param name="passwordOd"></param>
        /// <param name="passwordNew"></param>
        /// <returns></returns>
         ObjectMessage ResetPassword(long nguoiDungId, string passwordNew);
    }
}
