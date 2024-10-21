using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopDienThoai.Models;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.IO;

namespace ShopDienThoai.Controllers
{
    public class HomeController : Controller
    {
        private ShopDienThoaiEntities db = new ShopDienThoaiEntities();
        public ActionResult Index()
        {
            var cate = db.hang_san_xuat.ToList();
            var listProduct = new List<ListProducts>();
            foreach (var item in cate)
            {
                var products = db.san_pham.Where(m => m.id_hang_san_xuat == item.id).ToList();
                listProduct.Add(new ListProducts
                {
                    CateId = item.id,
                    CateName = item.ten_hang,
                    Products = products
                });
            }
            return View(listProduct);
        }

        public ActionResult GetProductCategories()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var categories = db.hang_san_xuat.ToList();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductFollowCategory(int? cate)
        {
            var products = db.san_pham.Where(m => m.id_hang_san_xuat == cate).ToList();
            return View(products);
        }

        public ActionResult ProductDetail(int? id)
        {
            var product = db.san_pham.Where(m => m.id == id).SingleOrDefault();
            return View(product);
        }

        public ActionResult AddToCart(int id)
        {
            var sanPham = db.san_pham.SingleOrDefault(m => m.id == id);
            List<Cart> list = Session["Cart"] as List<Cart> ?? new List<Cart>();
            Cart cart = list.FirstOrDefault(m => m.id == id);
            bool status = false;
            //Nếu sản phẩm chưa có trong giỏ hàng
            if(cart == null)
            {
                list.Add(new Cart
                {
                    id = sanPham.id,
                    ten_san_pham = sanPham.ten_san_pham,
                    anh_san_pham = sanPham.anh_san_pham,
                    so_luong = 1,
                    don_gia = (long)(sanPham.don_gia - (sanPham.don_gia * sanPham.giam_gia / 100))
                });
                status = true;
            }
            else
            {
                cart.so_luong += 1;
            }
            Session["Cart"] = list;
            return Json(new { status  }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewCart()
        {
            List<Cart> cart = Session["Cart"] as List<Cart> ?? new List<Cart>();
            return View(cart);
        }

        public ActionResult IncreQtyCart(int id)
        {
            var sanPham = db.san_pham.SingleOrDefault(m => m.id == id);
            List<Cart> list = Session["Cart"] as List<Cart>;
            Cart cart = list.FirstOrDefault(m => m.id == id);
            bool status = false;
            if (cart.so_luong < sanPham.so_luong)
            {
                cart.so_luong += 1;
                status = true;
            }
            var soLuong = cart.so_luong;
            CultureInfo culture = new CultureInfo("vi-VN");
            var thanhTien = string.Format(culture, "{0:0,0}", cart.so_luong * cart.don_gia);
            thanhTien = thanhTien + " VNĐ";
            return Json(new { status, soLuong, thanhTien }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DecreQtyCart(int id)
        {
            var sanPham = db.san_pham.SingleOrDefault(m => m.id == id);
            List<Cart> list = Session["Cart"] as List<Cart>;
            Cart cart = list.FirstOrDefault(m => m.id == id);
            bool status = true;
            cart.so_luong -= 1;
            if (cart.so_luong == 0)
            {
                list.Remove(cart);
                status = false;
            }
            var soLuong = cart.so_luong;
            CultureInfo culture = new CultureInfo("vi-VN");
            var thanhTien = string.Format(culture, "{0:0,0}", cart.so_luong * cart.don_gia);
            thanhTien = thanhTien + " VNĐ";
            return Json(new { status, soLuong, thanhTien }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteItemCart(int id)
        {
            List<Cart> list = Session["Cart"] as List<Cart>;
            Cart cart = list.FirstOrDefault(m => m.id == id);
            list.Remove(cart);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalPrice()
        {
            List<Cart> list = Session["Cart"] as List<Cart>;
            long total = 0;
            foreach(var item in list)
            {
                total += item.so_luong * item.don_gia;
            }
            CultureInfo culture = new CultureInfo("vi-VN");
            var thanhTien = string.Format(culture, "{0:0,0}", total);
            thanhTien = thanhTien + " VNĐ";
            return Json(new { thanhTien }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalQtyCart()
        {
            List<Cart> list = Session["Cart"] as List<Cart>;

            int total = list != null ? list.Count : 0;

            return Json(new { total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Checkout()
        {
            var user = Session["user"] as tai_khoan;
            if(user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            List<Cart> list = Session["Cart"] as List<Cart>;
            ViewBag.cart = list;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(hoa_don hoaDonModel)
        {
            if (ModelState.IsValid)
            {
                List<Cart> list = Session["Cart"] as List<Cart>;
                var user = Session["user"] as tai_khoan;
                var _hoaDon = new hoa_don
                {
                    id_tai_khoan = user.id,
                    dia_chi = hoaDonModel.dia_chi,
                    so_dien_thoai = hoaDonModel.so_dien_thoai
                };
                db.hoa_don.Add(_hoaDon);
                db.SaveChanges();
                var hoaDonIdMax = db.hoa_don.Max(m => m.id);
                foreach (var item in list)
                {
                    var _chiTietHoaDon = new chi_tiet_hoa_don
                    {
                        id_hoa_don = hoaDonIdMax,
                        id_san_pham = item.id,
                        so_luong = item.so_luong,
                        thanh_tien = (int)(item.so_luong * item.don_gia),
                        ngay_mua = DateTime.Now
                    };
                    db.chi_tiet_hoa_don.Add(_chiTietHoaDon);
                    db.SaveChanges();

                    var productQty = db.san_pham.FirstOrDefault(m => m.id == item.id);
                    productQty.so_luong -= item.so_luong;
                    db.SaveChanges();
                }
                list.Clear();
                TempData["SweetAlert"] = "success|Đơn hàng đã đặt thành công";
                return RedirectToAction("Index");
            }
            return View(hoaDonModel);
        }

        public ActionResult ViewOrders()
        {
            var user = Session["user"] as tai_khoan;
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            var orders = db.chi_tiet_hoa_don
                .Include(m => m.hoa_don)
                .Include(m => m.hoa_don.tai_khoan)
                .Include(m => m.san_pham)
                .Where(m => m.hoa_don.tai_khoan.id == user.id)
                .ToList();
            return View(orders);
        }

        public ActionResult ProductSearch(string name)
        {
            var _products = db.san_pham.Where(m => m.ten_san_pham.Contains(name)).ToList();
            return View(_products);
        }

        public ActionResult UserDetail()
        {
            var userCurrent = Session["user"] as tai_khoan;
            var id = userCurrent.id;
            var user = db.tai_khoan.Find(id);
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserDetail(tai_khoan _taiKhoanModel, HttpPostedFileBase avatar, string new_password)
        {
            var userCurrent = Session["user"] as tai_khoan;
            var id = userCurrent.id;
            if (ModelState.IsValid)
            {
                var _client = db.tai_khoan.Find(id);
                if (_client != null)
                {
                    _client.ho_ten = _taiKhoanModel.ho_ten;
                    _client.email = _taiKhoanModel.email;
                    _client.mat_khau = _taiKhoanModel.mat_khau;

                    if (avatar != null)
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(avatar.InputStream))
                        {
                            bytes = br.ReadBytes(avatar.ContentLength);
                        }
                        _client.anh_dai_dien = bytes;
                    }
                    if (!string.IsNullOrEmpty(new_password))
                    {
                        _client.mat_khau = new_password;
                    }
                    db.SaveChanges();
                    TempData["SweetAlert"] = "success|Cập nhật tài khoản thành công";
                    return RedirectToAction("Index");
                }
            }
            return View(_taiKhoanModel);
        }
    }
}