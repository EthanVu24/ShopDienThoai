using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopDienThoai.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    public class AuthLogin
    {
        [DisplayName("Email")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [RegularExpression(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; }
        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Mật khẩu tối thiểu 4 ký tự")]
        public string mat_khau { get; set; }
    }
}