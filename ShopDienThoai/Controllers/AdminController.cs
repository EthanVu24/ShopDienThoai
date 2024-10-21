using ShopDienThoai.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.IO;
using System.Web.Mvc;
using ShopDienThoai.Models;
using PagedList;

namespace ShopDienThoai.Controllers
{
    [isAdmin]
    public class AdminController : Controller
    {
        private ShopDienThoaiEntities db = new ShopDienThoaiEntities();
        private int pageSize = 2;
        public ActionResult Dashboard()
        {
            var products = db.san_pham.Count();
            var accounts = db.tai_khoan.Where(m => m.id_vai_tro != 3).Count();
            var clients = db.tai_khoan.Where(m => m.id_vai_tro == 3).Count();
            var orders = db.chi_tiet_hoa_don.Count();
            var suppliers = db.nha_cung_cap.Count();

            ViewBag.products = products;
            ViewBag.accounts = accounts;
            ViewBag.orders = orders;
            ViewBag.clients = clients;
            ViewBag.suppliers = suppliers;

            return View();
        }

        [AdminAuthorize(RequiredRole = 2)]
        public ActionResult Product()
        {
            var products = db.san_pham
                .Include(m => m.chi_tiet_san_pham)
                .Include(m => m.hang_san_xuat)
                .Include(m => m.nha_cung_cap)
                .ToList();
            var pageData = products.ToPagedList(1, pageSize);
            List<hang_san_xuat> hangSanXuatList = db.hang_san_xuat.ToList();
            hangSanXuatList.Insert(0, new hang_san_xuat { id = 0, ten_hang = "Hãng sản xuất" });
            ViewBag.inputSearchProductManufacturer = new SelectList(hangSanXuatList, "id", "ten_hang");
            return View(pageData);
        }

        public ActionResult ProductSearch(string name, int page, int? minPrice, int? maxPrice, int? manufacturer, int? ram, int? storage)
        {
            var productQuery = db.san_pham.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                productQuery = productQuery.Where(m => m.ten_san_pham.Contains(name));
            }
            if (minPrice.HasValue)
            {
                productQuery = productQuery.Where(m => m.don_gia >= minPrice);
            }
            if (maxPrice.HasValue)
            {
                productQuery = productQuery.Where(m => m.don_gia <= maxPrice);
            }
            if (manufacturer != 0)
            {
                productQuery = productQuery.Where(m => m.id_hang_san_xuat == manufacturer);
            }
            if (ram.HasValue)
            {
                productQuery = productQuery.Where(m => m.chi_tiet_san_pham.ram == ram);
            }
            if (storage.HasValue)
            {
                productQuery = productQuery.Where(m => m.chi_tiet_san_pham.rom == storage);
            }
            var products = productQuery.ToList();
            var pageData = products.ToPagedList(page, pageSize);
            return PartialView("ProductTable", pageData);
        }

        public ActionResult ProductCreate()
        {
            ViewBag.id_hang_san_xuat = new SelectList(db.hang_san_xuat, "id", "ten_hang");
            ViewBag.id_nha_cung_cap = new SelectList(db.nha_cung_cap, "id", "ten_nha_cung_cap");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductCreate(san_pham _sanPhamModel, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var _product = new san_pham
                {
                    ten_san_pham = _sanPhamModel.ten_san_pham,
                    so_luong = _sanPhamModel.so_luong,
                    don_gia = _sanPhamModel.don_gia,
                    giam_gia = _sanPhamModel.giam_gia,
                    id_hang_san_xuat = _sanPhamModel.id_hang_san_xuat,
                    id_nha_cung_cap = _sanPhamModel.id_nha_cung_cap
                };

                if (image != null)
                {
                    byte[] bytes;
                    using (BinaryReader br = new BinaryReader(image.InputStream))
                    {
                        bytes = br.ReadBytes(image.ContentLength);
                    }
                    _product.anh_san_pham = bytes;
                }
                else
                {
                    _product.anh_san_pham = null;
                }

                db.san_pham.Add(_product);
                db.SaveChanges();

                var _productIdMax = db.san_pham.Max(m => m.id);

                var _productDetail = new chi_tiet_san_pham
                {
                    id_san_pham = _productIdMax,
                    man_hinh = _sanPhamModel.chi_tiet_san_pham.man_hinh,
                    do_phan_giai = _sanPhamModel.chi_tiet_san_pham.do_phan_giai,
                    tan_so_quet = _sanPhamModel.chi_tiet_san_pham.tan_so_quet,
                    cam_sau = _sanPhamModel.chi_tiet_san_pham.cam_sau,
                    cam_truoc = _sanPhamModel.chi_tiet_san_pham.cam_truoc,
                    cpu = _sanPhamModel.chi_tiet_san_pham.cpu,
                    ram = _sanPhamModel.chi_tiet_san_pham.ram,
                    rom = _sanPhamModel.chi_tiet_san_pham.rom,
                    sim = _sanPhamModel.chi_tiet_san_pham.sim,
                    chan_sac = _sanPhamModel.chi_tiet_san_pham.chan_sac,
                    dung_luong_pin = _sanPhamModel.chi_tiet_san_pham.dung_luong_pin
                };

                db.chi_tiet_san_pham.Add(_productDetail);
                db.SaveChanges();

                TempData["SweetAlert"] = "success|Thêm sản phẩm thành công";
                return RedirectToAction("Product");
            }
            return View(_sanPhamModel);
        }

        public ActionResult ProductEdit(int? id)
        {
            var product = db.san_pham.Include(m => m.chi_tiet_san_pham).FirstOrDefault(m => m.id == id);
            if(product != null)
            {
                ViewBag.id_hang_san_xuat = new SelectList(db.hang_san_xuat, "id", "ten_hang", product.id_hang_san_xuat);
                ViewBag.id_nha_cung_cap = new SelectList(db.nha_cung_cap, "id", "ten_nha_cung_cap", product.id_nha_cung_cap);
                return View(product);
            }
            return RedirectToAction("Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProductEdit(san_pham _sanPhamModel, int id, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                var _product = db.san_pham.Find(id);
                _product.ten_san_pham = _sanPhamModel.ten_san_pham;
                _product.so_luong = _sanPhamModel.so_luong;
                _product.don_gia = _sanPhamModel.don_gia;
                _product.giam_gia = _sanPhamModel.giam_gia;
                _product.id_hang_san_xuat = _sanPhamModel.id_hang_san_xuat;
                _product.id_nha_cung_cap = _sanPhamModel.id_nha_cung_cap;
                if (image != null)
                {
                    byte[] bytes;
                    using (BinaryReader br = new BinaryReader(image.InputStream))
                    {
                        bytes = br.ReadBytes(image.ContentLength);
                    }
                    _product.anh_san_pham = bytes;
                }

                var _productDetail = db.chi_tiet_san_pham.Find(id);
                _productDetail.id_san_pham = id;
                _productDetail.man_hinh = _sanPhamModel.chi_tiet_san_pham.man_hinh;
                _productDetail.do_phan_giai = _sanPhamModel.chi_tiet_san_pham.do_phan_giai;
                _productDetail.tan_so_quet = _sanPhamModel.chi_tiet_san_pham.tan_so_quet;
                _productDetail.cam_sau = _sanPhamModel.chi_tiet_san_pham.cam_sau;
                _productDetail.cam_truoc = _sanPhamModel.chi_tiet_san_pham.cam_truoc;
                _productDetail.cpu = _sanPhamModel.chi_tiet_san_pham.cpu;
                _productDetail.ram = _sanPhamModel.chi_tiet_san_pham.ram;
                _productDetail.rom = _sanPhamModel.chi_tiet_san_pham.rom;
                _productDetail.sim = _sanPhamModel.chi_tiet_san_pham.sim;
                _productDetail.chan_sac = _sanPhamModel.chi_tiet_san_pham.chan_sac;
                _productDetail.dung_luong_pin = _sanPhamModel.chi_tiet_san_pham.dung_luong_pin;

                db.SaveChanges();
                TempData["SweetAlert"] = "success|Cập nhật sản phẩm thành công";
                return RedirectToAction("Product");
            }
            return View(_sanPhamModel);
        }

        public ActionResult ProductDelete(int? id)
        {
            var product = db.san_pham.Find(id);
            if (product != null)
            {
                db.san_pham.Remove(product);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Xoá sản phẩm thành công";
            }
            return RedirectToAction("Product");
        }

        public ActionResult Client()
        {
            var clients = db.tai_khoan.Where(m => m.id_vai_tro == 3).ToList();
            var pageData = clients.ToPagedList(1, pageSize);
            return View(pageData);
        }

        public ActionResult ClientSearch(string name, bool? status, int page)
        {
            var clientQuery = db.tai_khoan.AsQueryable();
            clientQuery = clientQuery.Where(m => m.id_vai_tro == 3);
            if (!string.IsNullOrEmpty(name))
            {
                clientQuery = clientQuery.Where(m => m.ho_ten.Contains(name));
            }
            if (status.HasValue)
            {
                clientQuery = clientQuery.Where(m => m.trang_thai == status);
            }
            var clients = clientQuery.ToList();
            var pageData = clients.ToPagedList(page, pageSize);
            return PartialView("ClientTable", pageData);
        }

        public ActionResult ClientCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClientCreate(tai_khoan _taiKhoanModel, HttpPostedFileBase avatar)
        {
            if (ModelState.IsValid)
            {
                var _client = new tai_khoan
                {
                    ho_ten = _taiKhoanModel.ho_ten,
                    email = _taiKhoanModel.email,
                    mat_khau = _taiKhoanModel.mat_khau,
                    id_vai_tro = 3,
                    trang_thai = true
                };

                if (avatar != null)
                {
                    byte[] bytes;
                    using (BinaryReader br = new BinaryReader(avatar.InputStream))
                    {
                        bytes = br.ReadBytes(avatar.ContentLength);
                    }
                    _client.anh_dai_dien = bytes;
                }
                else
                {
                    _client.anh_dai_dien = null;
                }

                db.tai_khoan.Add(_client);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Thêm khách hàng thành công";
                return RedirectToAction("Client");
            }
            return View(_taiKhoanModel);
        }

        public ActionResult ClientEdit(int? id)
        {
            var client = db.tai_khoan.Find(id);
            if (client != null)
            {
                return View(client);
            }
            return RedirectToAction("Client");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClientEdit(tai_khoan _taiKhoanModel, int id, HttpPostedFileBase avatar)
        {
            if (ModelState.IsValid)
            {
                var _client = db.tai_khoan.Find(id);
                if(_client != null)
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
                    db.SaveChanges();
                    TempData["SweetAlert"] = "success|Cập nhật khách hàng thành công";
                    return RedirectToAction("Client");
                }
            }
            return View(_taiKhoanModel);
        }

        public ActionResult ClientLock(int? id)
        {
            var client = db.tai_khoan.Find(id);
            if(client != null)
            {
                client.trang_thai = client.trang_thai ? false : true;
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Cập nhật khách hàng thành công";
                return RedirectToAction("Client");
            }
            return View("Client");
        }

        public ActionResult ClientDelete(int? id)
        {
            var client = db.tai_khoan.Find(id);
            if (client != null)
            {
                db.tai_khoan.Remove(client);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Xoá khách hàng thành công";
                return RedirectToAction("Client");
            }
            return View("Client");
        }

        [AdminAuthorize(RequiredRole = 2)]
        public ActionResult Account()
        {
            var accounts = db.tai_khoan.Where(m => m.id_vai_tro != 3).ToList();
            var pageData = accounts.ToPagedList(1, pageSize);
            return View(pageData);
        }

        public ActionResult AccountSearch(string name, bool? status, int page)
        {
            var accountQuery = db.tai_khoan.AsQueryable();
            accountQuery = accountQuery.Where(m => m.id_vai_tro != 3);
            if (!string.IsNullOrEmpty(name))
            {
                accountQuery = accountQuery.Where(m => m.ho_ten.Contains(name));
            }
            if (status.HasValue)
            {
                accountQuery = accountQuery.Where(m => m.trang_thai == status);
            }
            var accounts = accountQuery.ToList();
            var pageData = accounts.ToPagedList(page, pageSize);
            return PartialView("AccountTable", pageData);
        }

        public ActionResult AccountCreate()
        {
            ViewBag.id_vai_tro = new SelectList(db.vai_tro, "id", "ten_vai_tro");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountCreate(tai_khoan _taiKhoanModel, HttpPostedFileBase avatar)
        {
            if (ModelState.IsValid)
            {
                var _account = new tai_khoan
                {
                    ho_ten = _taiKhoanModel.ho_ten,
                    email = _taiKhoanModel.email,
                    mat_khau = _taiKhoanModel.mat_khau,
                    id_vai_tro = _taiKhoanModel.id_vai_tro,
                    trang_thai = true
                };

                if (avatar != null)
                {
                    byte[] bytes;
                    using (BinaryReader br = new BinaryReader(avatar.InputStream))
                    {
                        bytes = br.ReadBytes(avatar.ContentLength);
                    }
                    _account.anh_dai_dien = bytes;
                }
                else
                {
                    _account.anh_dai_dien = null;
                }

                db.tai_khoan.Add(_account);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Thêm tài khoản thành công";
                return RedirectToAction("Account");
            }
            return View(_taiKhoanModel);
        }

        public ActionResult AccountEdit(int? id)
        {
            var account = db.tai_khoan.Find(id);
            if (account != null)
            {
                ViewBag.id_vai_tro = new SelectList(db.vai_tro, "id", "ten_vai_tro", account.id_vai_tro);
                return View(account);
            }
            return RedirectToAction("Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountEdit(tai_khoan _taiKhoanModel, int id, HttpPostedFileBase avatar)
        {
            if (ModelState.IsValid)
            {
                var _account = db.tai_khoan.Find(id);
                if (_account != null)
                {
                    _account.ho_ten = _taiKhoanModel.ho_ten;
                    _account.email = _taiKhoanModel.email;
                    _account.mat_khau = _taiKhoanModel.mat_khau;
                    _account.id_vai_tro = _taiKhoanModel.id_vai_tro;
                    if (avatar != null)
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(avatar.InputStream))
                        {
                            bytes = br.ReadBytes(avatar.ContentLength);
                        }
                        _account.anh_dai_dien = bytes;
                    }
                    db.SaveChanges();
                    TempData["SweetAlert"] = "success|Cập nhật khách hàng thành công";
                    return RedirectToAction("Account");
                }
            }
            return View(_taiKhoanModel);
        }

        public ActionResult AccountLock(int? id)
        {
            var account = db.tai_khoan.Find(id);
            if (account != null)
            {
                account.trang_thai = account.trang_thai ? false : true;
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Cập nhật tài khoản thành công";
                return RedirectToAction("Account");
            }
            return View("Account");
        }

        public ActionResult AccountDelete(int? id)
        {
            var account = db.tai_khoan.Find(id);
            if (account != null)
            {
                db.tai_khoan.Remove(account);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Xoá tài khoản thành công";
                return RedirectToAction("Account");
            }
            return View("Account");
        }

        [AdminAuthorize(RequiredRole = 2)]
        public ActionResult Manufacturer()
        {
            var manufacturer = db.hang_san_xuat.ToList();
            var pageData = manufacturer.ToPagedList(1, pageSize);
            return View(pageData);
        }

        public ActionResult ManufacturerSearch(string name, int page)
        {
            var manufacturerQuery = db.hang_san_xuat.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                manufacturerQuery = manufacturerQuery.Where(m => m.ten_hang.Contains(name));
            }
            var manufacturer = manufacturerQuery.ToList();
            var pageData = manufacturer.ToPagedList(1, pageSize);
            return PartialView("ManufacturerTable", pageData);
        }

        public ActionResult ManufacturerCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManufacturerCreate(hang_san_xuat _hangSanXuatModel)
        {
            if (ModelState.IsValid)
            {
                var _manufacturer = new hang_san_xuat
                {
                    ten_hang = _hangSanXuatModel.ten_hang,
                };

                db.hang_san_xuat.Add(_manufacturer);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Thêm hãng sản xuất thành công";
                return RedirectToAction("Manufacturer");
            }
            return View(_hangSanXuatModel);
        }

        public ActionResult ManufacturerEdit(int? id)
        {
            var manufacturer = db.hang_san_xuat.Find(id);
            if (manufacturer != null)
            {
                return View(manufacturer);
            }
            return RedirectToAction("Manufacturer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManufacturerEdit(hang_san_xuat _hangSanXuatModel, int id)
        {
            if (ModelState.IsValid)
            {
                var _manufacturer = db.hang_san_xuat.Find(id);
                if (_manufacturer != null)
                {
                    _manufacturer.ten_hang = _hangSanXuatModel.ten_hang;
                    db.SaveChanges();
                    TempData["SweetAlert"] = "success|Cập nhật hãng sản xuất thành công";
                    return RedirectToAction("Manufacturer");
                }
            }
            return View(_hangSanXuatModel);
        }

        public ActionResult ManufacturerDelete(int? id)
        {
            var manufacturer = db.hang_san_xuat.Find(id);
            if (manufacturer != null)
            {
                db.hang_san_xuat.Remove(manufacturer);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Xoá hãng sản xuất thành công";
                return RedirectToAction("Manufacturer");
            }
            return View("Manufacturer");
        }

        [AdminAuthorize(RequiredRole = 2)]
        public ActionResult Supplier()
        {
            var suppliers = db.nha_cung_cap.ToList();
            var pageData = suppliers.ToPagedList(1, pageSize);
            return View(pageData);
        }

        public ActionResult SupplierSearch(string name, int page)
        {
            var supplierQuery = db.nha_cung_cap.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                supplierQuery = supplierQuery.Where(m => m.ten_nha_cung_cap.Contains(name));
            }
            var supplier = supplierQuery.ToList();
            var pageData = supplier.ToPagedList(1, pageSize);
            return PartialView("SupplierTable", pageData);
        }

        public ActionResult SupplierCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierCreate(nha_cung_cap _nhaCungCapModel)
        {
            if (ModelState.IsValid)
            {
                var _supplier = new nha_cung_cap
                {
                    ten_nha_cung_cap = _nhaCungCapModel.ten_nha_cung_cap,
                    ten_nguoi_dai_dien = _nhaCungCapModel.ten_nguoi_dai_dien,
                    dia_chi = _nhaCungCapModel.dia_chi,
                    so_dien_thoai = _nhaCungCapModel.so_dien_thoai,
                    email = _nhaCungCapModel.email,
                    fax = _nhaCungCapModel.fax
                };

                db.nha_cung_cap.Add(_supplier);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Thêm nhà cung cấp thành công";
                return RedirectToAction("Supplier");
            }
            return View(_nhaCungCapModel);
        }

        public ActionResult SupplierEdit(int? id)
        {
            var supplier = db.nha_cung_cap.Find(id);
            if (supplier != null)
            {
                return View(supplier);
            }
            return RedirectToAction("Supplier");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SupplierEdit(nha_cung_cap _nhaCungCapModel, int id)
        {
            if (ModelState.IsValid)
            {
                var _supplier = db.nha_cung_cap.Find(id);
                if (_supplier != null)
                {
                    _supplier.ten_nha_cung_cap = _nhaCungCapModel.ten_nha_cung_cap;
                    _supplier.ten_nguoi_dai_dien = _nhaCungCapModel.ten_nguoi_dai_dien;
                    _supplier.dia_chi = _nhaCungCapModel.dia_chi;
                    _supplier.so_dien_thoai = _nhaCungCapModel.so_dien_thoai;
                    _supplier.email = _nhaCungCapModel.email;
                    _supplier.fax = _nhaCungCapModel.fax;

                    db.SaveChanges();
                    TempData["SweetAlert"] = "success|Cập nhật nhà cung cấp thành công";
                    return RedirectToAction("Supplier");
                }
            }
            return View(_nhaCungCapModel);
        }

        public ActionResult SupplierDelete(int? id)
        {
            var supplier = db.nha_cung_cap.Find(id);
            if (supplier != null)
            {
                db.nha_cung_cap.Remove(supplier);
                db.SaveChanges();
                TempData["SweetAlert"] = "success|Xoá nhà cung cấp thành công";
                return RedirectToAction("Supplier");
            }
            return View("Supplier");
        }

        public ActionResult Order()
        {
            var orders = db.chi_tiet_hoa_don
                .Include(m => m.hoa_don)
                .Include(m => m.hoa_don.tai_khoan)
                .Include(m => m.san_pham)
                .ToList();
            return View(orders);
        }
    }
}