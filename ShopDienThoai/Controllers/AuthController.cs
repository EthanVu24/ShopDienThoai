using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ShopDienThoai.Models;

namespace ShopDienThoai.Controllers
{
    public class AuthController : Controller
    {
        private ShopDienThoaiEntities db = new ShopDienThoaiEntities();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AuthLogin _loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = db.tai_khoan.FirstOrDefault(m => m.email.Equals(_loginModel.email) && m.mat_khau.Equals(_loginModel.mat_khau));
                if(user == null)
                {
                    ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
                }
                else if (user.trang_thai == false)
                {
                    ModelState.AddModelError("", "Tài khoản của bạn hiện tại đang bị khoá!");
                }
                else
                {
                    Session["user"] = user;
                    if(user.id_vai_tro != 3)
                    {
                        return RedirectToAction("Dashboard","Admin");
                    }
                    else
                    {
                        TempData["SweetAlert"] = "success|Xin chào " + user.ho_ten;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(_loginModel);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(AuthSignUp _signUpModel)
        {
            if (ModelState.IsValid)
            {
                if(db.tai_khoan.Any(m => m.email.Equals(_signUpModel.email)))
                {
                    ModelState.AddModelError("", "Email đã được đăng ký");
                }
                else
                {
                    var account = new tai_khoan
                    {
                        ho_ten = _signUpModel.ho_ten,
                        email = _signUpModel.email,
                        mat_khau = _signUpModel.mat_khau,
                        id_vai_tro = 3,
                        trang_thai = true
                    };
                    db.tai_khoan.Add(account);
                    db.SaveChanges();
                    TempData["SweetAlert"] = "success|Đăng ký tài khoản thành công";
                    return RedirectToAction("Login");
                }
            }
            return View(_signUpModel);
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }
    }
}