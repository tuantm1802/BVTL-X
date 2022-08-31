using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ICommon
{
    public interface IUltil
    {

        string LocDau(string str);

        string ConvertDate2(string dateString);
        string ConvertDate4(string dateString);
        DateTime ConvertStringToDate(string dateString, string format = "yyyyMMdd");
        string ListInt2String(List<int> data);
        /// <summary>
        /// Convert xâu có dạng 1,2,3,4 sang list int
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns> 
        List<int> ListIntConvertToString(string source);

        string ConvertToUnAsign(string str);

        /// <summary>
        /// Convert ngày tháng: 16/06/2017 -> 20170616
        /// </summary>
        /// <param name="date">16/06/2017</param>
        /// <returns>20170616</returns>
        string GetDate(string date);

        /// <summary>
        /// Convert ngày tháng: 20170616 -> 16/06/2017
        /// </summary>
        /// <param name="date8">20170616</param>
        /// <returns>16/06/2017</returns>
        string LoadDate(string date8);

        /// <summary>
        /// input: 20170620151200
        /// </summary>
        /// <param name="date14"></param>
        /// <returns>20/06/2017 15:12:00</returns>
        string LoadDateTime(string date14);
        /// Chuỗi kết quả chuyển từ số
        string NumberToText(double inputNumber, bool suffix = true);
        string ConvertToBaseUri(string uri);


        void DeleteFile(string pathFolder);
    }
}
