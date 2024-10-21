using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopDienThoai.Models
{
    public class ListProducts
    {
        public int CateId { get; set; }
        public string CateName { get; set; }
        public List<san_pham> Products { get; set; }
    }
}