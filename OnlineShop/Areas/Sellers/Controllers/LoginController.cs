using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Areas.Admin.Functions;
using System.Text.RegularExpressions;

namespace OnlineShop.Areas.Sellers.Controllers
{
    public class LoginController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Admin/Login
        public ActionResult Index(int? id)
        {
            if (id!=null && Session["login"]!=null)
            {
                Session["seller"] = id;
                return RedirectToAction("index", "SellerDashboard", new { newlogin = "true" });
            }
            HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["OnlineShopSellerAuthCookie"];
            if (authCookie != null)
            {
                try
                {
                    string rnd = authCookie.Value;
                    var seller = db.Sellers.Where(r => r.token == rnd).FirstOrDefault();
                    if (seller != null)
                    {
                        Session["seller"] = seller.id;
                        return RedirectToAction("index", "SellerDashboard", new { newlogin = "true" });
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Index", "LoginController");
                    throw;
                }
               
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string email, string password, int? remmember)
        {
            try
            {
                var seller = db.Sellers.Where(r => r.email.ToLower() == email.ToLower() && r.password == password).FirstOrDefault();
                if (seller != null && seller.isValid != true)
                {
                    ViewBag.error = "حساب کاربری شما هنوز توسط مدیر سایت تائید نشده است";
                    return View();

                }
                if (seller != null)
                {
                    Session["seller"] = seller.id;
                    if (remmember != null)
                    {
                        string rnd = RandomString(40);
                        seller.token = rnd;
                        db.SaveChanges();
                        HttpCookie authCookie = new HttpCookie("OnlineShopSellerAuthCookie", rnd)
                        {
                            Expires = DateTime.Now.AddDays(10)
                        };
                        Response.Cookies.Add(authCookie);
                    }
                    return RedirectToAction("index", "SellerDashboard", new { newlogin = "true" });
                }
                else
                {
                    ViewBag.error = "نام کاربری و یا رمز عبور اشتباه است";
                }
                return View();
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Index", "LoginController");
                throw;
            }
           
        }
        public ActionResult logOut()
        {
            if (Session["seller"] != null)
            {
                try
                {
                    int id = Convert.ToInt16(Session["seller"]);
                    db.Sellers.Find(id).token = string.Empty;
                    db.SaveChanges();
                    Session.Remove("seller");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "logOut", "LoginController");
                    throw;
                }
               
            }
            return RedirectToAction("index");
        }

        public ActionResult easyLogin() {


            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult easyLogin(string mobile)
        {
            if (mobile != null && !string.IsNullOrEmpty(mobile) && mobile.Substring(0, 2) == "09" && mobile.Length == 11 && Regex.IsMatch(mobile, @"^\d+$"))
            {
                try
                {
                    var seller = db.Sellers.Where(r => r.phone == mobile).FirstOrDefault();
                    Session["mobileSeller"] = mobile;
                    if (seller != null)
                    {
                        if (seller.isValid == true)
                        {
                            var setting = db.Settings.FirstOrDefault();

                            seller.easyLogin = GenerateRandomNo().ToString();
                            seller.easyLoginTime = DateTime.Now;
                            db.SaveChanges();
                            #region sending sms to user ...
                            SMS sms = new SMS();
                            if (sms.Send(seller.phone, seller.easyLogin, Convert.ToInt32(setting.smsToken_fastLogin)))
                            {
                                ViewBag.suuccessSend = 1;
                            }
                            #endregion

                            return View();
                        }
                        else
                        {
                            ViewBag.error = "ثبت نام فروشنده در سایت تائید نشده است";
                            return View();

                        }

                    }
                    else
                    {
                        ViewBag.error = "شماره موبایل وارد شده در سایت وجود ندارد";
                        return View();

                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "easyLogin", "LoginController");
                    throw;
                }
         
            }
            else
            {
                ViewBag.error = "شماره موبایل وارد شده اشتباه است ";
                return View();
            }
            return Json(string.Empty);
        }

        [ValidateAntiForgeryToken]
        public ActionResult reciveCode(string code , int? remmember)
        {
            try
            {
                string phone = (Session["mobileSeller"]).ToString();
                var seller = db.Sellers.Where(r => r.phone == phone.Trim()).FirstOrDefault();
                if (seller != null)
                {
                    if (seller.easyLogin == code)
                    {
                        Session["seller"] = seller.id;
                        if (remmember != null)
                        {
                            string rnd = RandomString(40);
                            seller.token = rnd;
                            db.SaveChanges();
                            HttpCookie authCookie = new HttpCookie("OnlineShopSellerAuthCookie", rnd)
                            {
                                Expires = DateTime.Now.AddDays(10)
                            };
                            Response.Cookies.Add(authCookie);
                        }
                        return RedirectToAction("index", "SellerDashboard", new { newlogin = "true" });
                    }
                    else
                    {
                        ViewBag.error = "کد وارد شده اشتباه است دوباره امتحان کنید";
                    }
                }
                return View("/Areas/Sellers/Views/Login/easyLogin.cshtml");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "reciveCode", "LoginController");
                throw;
            }
           
        }
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz$#@!%^&*()";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}