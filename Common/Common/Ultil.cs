using Common.ICommon;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Common.Common
{
    public class Ultil: IUltil
    {
        private  readonly string[] VietNamChar = new string[]
        {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        };
        public  string LocDau(string str)
        {
            //Thay thế và lọc dấu từng char      
            for (int i = 1; i < VietNamChar.Length; i++)
            {
                for (int j = 0; j < VietNamChar[i].Length; j++)
                    str = str.Replace(VietNamChar[i][j], VietNamChar[0][i - 1]);
            }
            return str;
        }

        public  string ConvertDate2(string dateString)
        {
            try
            {
                var date = DateTime.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                return date.ToString("yyyyMMdd");
            }
            catch
            {
                return "";
            }
        }
        public DateTime ConvertDateFromUtc(DateTime? dateTime)
        {
            
            try
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                DateTime dNgay = TimeZoneInfo.ConvertTimeFromUtc((DateTime)dateTime, cstZone);
                
                return dNgay;
            }
            catch
            {
                return new DateTime();
            }
        }
        public  string ConvertDate4(string dateString)
        {
            try
            {
                var date = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
                return date.ToString("dd/MM/yyyy");
            }
            catch
            {
                return "";
            }
        }
        public  DateTime ConvertStringToDate(string dateString,string format = "yyyyMMdd")
        {
            try
            {
                return DateTime.ParseExact(dateString, format, CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        public  string ListInt2String(List<int> data)
        {
            string dReturn = string.Empty;
            if (data != null)
            {
                foreach (var item in data)
                {
                    if (string.IsNullOrEmpty(dReturn))
                        dReturn += item;
                    else
                        dReturn += "," + item;
                }
            }
            return dReturn;
        }
        /// <summary>
        /// Convert xâu có dạng 1,2,3,4 sang list int
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns> 
        public  List<int> ListIntConvertToString(string source)
        {
            try
            {
                var ltsValue = new List<int>();
                if (string.IsNullOrEmpty(source)) return ltsValue;
                if (source.EndsWith(","))
                    source = source.Substring(0, source.LastIndexOf(",", StringComparison.Ordinal));
                if (source.Contains(','))
                {
                    foreach (var item in source.Split(','))
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            int id;
                            if (int.TryParse(item, out id))
                            {
                                if (!ltsValue.Contains(id))
                                    ltsValue.Add(id);
                            }
                        }
                    }
                }
                else
                    ltsValue.Add(Convert.ToInt32(source));
                return ltsValue;
            }
            catch
            {
                return new List<int>();
            }
        }

        public  string ConvertToUnAsign(string str)
        {
            string[] signs = new string[] {"aAeEoOuUiIdDyY","áàạảãâấầậẩẫăắằặẳẵ","ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ","éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ","óòọỏõôốồộổỗơớờợởỡ","ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ","úùụủũưứừựửữ","ÚÙỤỦŨƯỨỪỰỬỮ","íìịỉĩ","ÍÌỊỈĨ","đ","Đ","ýỳỵỷỹ","ÝỲỴỶỸ"
        };
            for (int i = 1; i < signs.Length; i++)
            {
                for (int j = 0; j < signs[i].Length; j++)
                {
                    str = str.Replace(signs[i][j], signs[0][i - 1]);
                }
            }
            return str;
        }

        /// <summary>
        /// Convert ngày tháng: 16/06/2017 -> 20170616
        /// </summary>
        /// <param name="date">16/06/2017</param>
        /// <returns>20170616</returns>
        public  string GetDate(string date)
        {
            string date8 = "";
            try
            {
                string[] datea = date.Split('/');
                if (datea.Length == 3)
                    date8 = datea[2] + datea[1] + datea[0];
                if (datea.Length == 2)
                    date8 = datea[1] + datea[0];
                if (datea.Length == 1)
                    date8 = datea[0];
            }
            catch { }
            return date8;
        }

        /// <summary>
        /// Convert ngày tháng: 20170616 -> 16/06/2017
        /// </summary>
        /// <param name="date8">20170616</param>
        /// <returns>16/06/2017</returns>
        public  string LoadDate(string date8)
        {
            string date = "";
            try
            {
                date = date8.Substring(6, 2) + "/" + date8.Substring(4, 2) + "/" + date8.Substring(0, 4);
            }
            catch { }
            return date;
        }

        /// <summary>
        /// input: 20170620151200
        /// </summary>
        /// <param name="date14"></param>
        /// <returns>20/06/2017 15:12:00</returns>
        public  string LoadDateTime(string date14)
        {
            string date = "";
            try
            {
                if (date14.Length == 8)
                    date = date14.Substring(6, 2) + "/" + date14.Substring(4, 2) + "/" + date14.Substring(0, 4);
                if (date14.Length == 14)
                    date = date14.Substring(6, 2) + "/" + date14.Substring(4, 2) + "/" + date14.Substring(0, 4) + " " + date14.Substring(8, 2) + ":" + date14.Substring(10, 2) + ":" + date14.Substring(12, 2);
            }
            catch { }
            return date;
        }
        /// Chuỗi kết quả chuyển từ số
        public  string NumberToText(double inputNumber, bool suffix = true)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result + (suffix ? " đồng chẵn" : "");
        }
        public  string ConvertToBaseUri(string uri)
        {
            var baseUri = new Uri(uri);
            return baseUri.GetLeftPart(System.UriPartial.Authority);
        }
        

        public void DeleteFile(string pathFolder)
        {
            if (Directory.Exists(pathFolder))
            {
                DirectoryInfo di = new DirectoryInfo(pathFolder);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            
        }
    }

    public class Commonnumbers
    {
        public  string NumberToText(double inputNumber, bool suffix = true)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result + (suffix ? " đồng chẵn" : "");
        }
    }
}
