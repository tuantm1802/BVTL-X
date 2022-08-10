using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Base
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Mời nhập tên đăng nhập.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Mời nhập mật khẩu.")]
        public string Password { get; set; }
        public string RememberMe { get; set; }
    }
}
