using Model.ModelExtend.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.InterfaceDA.API
{
   public interface IGetDataFromAPI
    {
        /// <summary>
        /// Lấy dữ liệu từ đầu Api trả lại list T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        Task<List<T>> PostDataFromApiReturnList<T>(string url, string token, string reportId, string rawOrLabel);

        /// <summary>
        /// Lấy dữ liệu từ đầu Api trả lại string json đầu api trả ra
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        Task<string> PostDataFromApiReturnString(string url, string token, string reportId, string rawOrLabel);

        #region Call api tạo các file báo cáo

        /// <summary>
        /// Gọi api tạo danh sách file báo cáo
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
         Task<List<UserSendReportModel>> GetListFileReport(string url);
        #endregion
    }
}
