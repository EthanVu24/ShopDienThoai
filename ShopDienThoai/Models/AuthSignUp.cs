using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopDienThoai.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    public class AuthSignUp
    {
        [DisplayName("Họ tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string ho_ten { get; set; }
        [DisplayName("Email")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [RegularExpression(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email không hợp lệ")]
        public string email { get; set; }
        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Mật khẩu tối thiểu 4 ký tự")]
        public string mat_khau { get; set; }
        [DisplayName("Xác nhận mật khẩu")]
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("mat_khau", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string xac_nhan_mat_khau { get; set; }
    }
}