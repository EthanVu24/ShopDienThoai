using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopDienThoai.Models
{
    public class Cart
    {
        public int id { get; set; }
        public string ten_san_pham { get; set; }
        public byte[] anh_san_pham { get; set; }
        public int so_luong { get; set; }
        public long don_gia { get; set; }
    }
}