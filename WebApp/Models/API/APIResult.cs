using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.API
{
    public class APIResult
    {
        /// <summary>
        /// 200: thành công
        /// 201: thành công( Được  trả  về  khi  insert  các  bản    ghi  thành công)
        /// 400:  Tham số không hợp lệ(Được  trả  về  khi  insert/update/delete  các bản  ghi hoặc  truy  vấn  dữ   liệu  và  tham  số truyền trong API không hợp lệ)
        /// 403:  Không   cho  phép thực  thi(Người  dùng  không  được  cấp  phép  thực thi các API hoặc  các thực thể hoặc  các trường dữ liệu)
        /// 404: Nội  dung  hoặc  dữ   liệu  không tìm thấy
        /// 409: Xung đột(Được  trả  về  khi  có  dữ   liệu  đã  tồn  tại  trên phía  server)
        /// 500: Lỗi hệ   thống
        /// 501: Không thực thi(Được  trả   về  khi  những  yêu  cầu  từ  phía client không được thực thi)
        /// </summary>
        public string code { get; set; }
        public string message { get; set; }
    }
}