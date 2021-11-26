using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Areas.Admin.Functions;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

namespace OnlineShop.Areas.UserLogin.Controllers
{
    public class MainController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        private static Random random = new Random();
        #region Functions
        public void checkLogin()
        {
            if (Session["userLogin"] == null)
            {
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["OnlineShopUserAuthCookie"];
                if (authCookie != null)
                {
                    string rnd = authCookie.Value;
                    var user = db.Users.Where(r => r.token == rnd).FirstOrDefault();
                    if (user != null)
                    {
                        Session["userLogin"] = user.id;
                    }
                }
            }

        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz$#@!%^&*()";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion
        #region Login
        public ActionResult login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(string mobile, string password, bool? remember)
        {
            var setting = db.Settings.FirstOrDefault();
            #region check captcha
            //if (this.IsCaptchaValid("Captcha is not valid") == false)
            //{
            //    ViewBag.error = "کد امنیتی اشتباه است";
            //    return View();

            //}
            #endregion
            // string ps = CreateMD5(password);
            var oldUser = db.Users.SingleOrDefault(q => q.Username.Trim() == mobile || q.mobile.Trim() == mobile || q.email.Trim() == mobile);
            string ps="";
            if (oldUser != null)
             ps = Security.RsaEncryptDecrypt.RSACls.RSADecryption(oldUser.password, Properties.Settings.Default.PrivateKey);
            if (oldUser != null && ps==password)
            {
                //var user = db.Users.Where(r => (r.mobile.Trim() == mobile.Trim() && r.password == ps) || (r.email.Trim() == mobile && r.password == ps)).FirstOrDefault();
                Session["userLogin"] = oldUser.id;
                if (remember != null && remember == true)
                {
                    string rnd = RandomString(40);
                    oldUser.token = rnd;
                    db.SaveChanges();
                    HttpCookie authCookie = new HttpCookie("OnlineShopUserAuthCookie", rnd)
                    {
                        Expires = DateTime.Now.AddDays(10)
                    };
                    Response.Cookies.Add(authCookie);

                }

                //if (user.mobieVerify != true)
                //{
                //    user.mobieVerify = false;
                //    user.smsToken = GenerateRandomNo().ToString();
                //    db.SaveChanges();
                //    #region send sms to user
                //    SMS sms = new SMS();
                //    sms.Send(user.mobile, user.smsToken, Convert.ToInt32(setting.smsToken_welcome));
                //    #endregion
                //}
                return RedirectToAction("index", "Home", new { area = "" });
            }
            else
            {
                ViewBag.error = "اطلاعات کاربری نادرست است.";
                return View();

            }
        }
        #endregion
        #region verifyMobileForget
        [ValidateAntiForgeryToken]
        public ActionResult verifyMobileForget(string mobile, string sms)
        {
            if (mobile == null || sms == null || string.IsNullOrEmpty(mobile.Trim()) || string.IsNullOrEmpty(sms.Trim()))
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });

            }
            var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();

            if (mobile != null && user.mobileForgetDateTime != null && user.mobileForgetDateTime < DateTime.Now.AddMinutes(5))
            {
                if (sms != null && sms == user.forgetSmsToken)
                {
                    ViewBag.mobile = mobile;
                    ViewBag.sms = sms;
                    return View();
                }
                else
                {
                    return RedirectToAction("verifyForgetPassword", new { id = user.id, error = "کد بازیابی رمز عبور وارد شده اشتباه است" });

                }


            }
            else
            {
                ViewBag.error = "کد بازیابی رمز عبور وارد شده اشتباه است";
                return View();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult verifyMobileForget(string Password, string rePassword, string sms, string mobile)
        {
            ViewBag.mobile = mobile;
            ViewBag.sms = sms;
            var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();
            if (user != null)
            {
                if (user.forgetSmsToken == sms)
                {
                    if (Password == rePassword)
                    {
                        if (Password.Length > 5)
                        {
                            if (Password.Trim() == Password)
                            {

                                user.password = CreateMD5(Password);
                                user.forgetSmsToken = null;
                                user.mobileForgetDateTime = null;
                                db.SaveChanges();
                                ViewBag.success = 1;
                                return View();
                            }
                            else
                            {
                                ViewBag.error = "رمز عبور نمیتواند شامل space باشد";
                                return View();
                            }

                        }
                        else
                        {
                            ViewBag.error = "رمز عبور باید بیشتر از 5 حرف باشد";
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.error = "رمز عبور با تکرار آن همخوانی ندارد";
                        return View();
                    }
                }

            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }


        #endregion
        #region fastLogin

        [HttpGet]
        public ActionResult FastLogin()
        {
            if (Session["userLogin"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        #endregion
        #region Forget password
        public ActionResult verifyForgetPassword(int id, string error)
        {
            var user = db.Users.Find(id);
            if (!string.IsNullOrEmpty(error))
            {
                ViewBag.error = error;
            }
            if (user != null)
            {
                if (user.mobileForgetDateTime < DateTime.Now.AddMinutes(5))
                {
                    ViewBag.mobile = user.mobile;
                    return View();
                }
                else
                {
                    return RedirectToAction("NotFound", "Home", new { area = "" });
                }
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }

        public ActionResult Forget()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Forget(string mobile)
        {
            var setting = db.Settings.FirstOrDefault();

            #region check captcha
            //if (this.IsCaptchaValid("Captcha is not valid") == false)
            //{
            //    ViewBag.error += "کد امنیتی اشتباه است";
            //    return View();
            //}
            #endregion
            #region check Mobile format
            if (mobile != null && !string.IsNullOrEmpty(mobile) && mobile.Substring(0, 2) == "09" && mobile.Length == 11 && Regex.IsMatch(mobile, @"^\d+$"))
            {
                var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();
                if (user != null)
                {
                    user.forgetSmsToken = GenerateRandomNo().ToString();
                    user.mobileForgetDateTime = DateTime.Now;
                    db.SaveChanges();

                    #region send sms to user
                    SMS sms = new SMS();
                    sms.Send(user.mobile, user.forgetSmsToken,Convert.ToInt32(setting.smsToken_forgetPassword));
                    #endregion

                    return RedirectToAction("verifyForgetPassword", new { id = user.id });
                }
                else
                {
                    ViewBag.error = "حساب کاربری با این شماره موبایل ثبت نشده است";
                    return View();

                }

            }
            else
            {
                ViewBag.error = "فرمت وارد شده برای شماره موبایل اشتباه است";
                return View();

            }
            #endregion
            return View();
        }
        #endregion
        #region validate mobile number
        public ActionResult verify()
        {
            checkLogin();

            if (Session["userLogin"] == null)
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
            else
            {
                int id = Convert.ToInt16(Session["userLogin"]);
                var user = db.Users.Find(id);
                if (user != null && user.mobieVerify != true)
                {
                    return View();
                }
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult verify(string sms)
        {
            checkLogin();

            if (Session["userLogin"] == null)
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
            else
            {
                int id = Convert.ToInt16(Session["userLogin"]);
                var user = db.Users.Find(id);
                if (user != null && user.mobieVerify != true)
                {
                    if (user.smsToken == sms.Trim())
                    {
                        user.smsToken = "done";
                        user.mobieVerify = true;
                        db.SaveChanges();
                        return RedirectToAction("index", "Home", new { area = "" });
                    }
                    else
                    {
                        ViewBag.error = "کد وارد شده اشتباه است ، لطفا دوباره امتحان کنید.";
                        return View();
                    }
                }
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });

        }


        #endregion
        #region reciving password for Easy login 

        [HttpPost]
        public JsonResult easyLoginPassword(string code)
        {
            string mobile = (Session["mobile"]).ToString();

            var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();
            if (user.easyLogin == code)
            {
                Session.Remove("mobile");
                Session["userLogin"] = user.id;
                string rnd = RandomString(40);
                user.token = rnd;
                db.SaveChanges();
                HttpCookie authCookie = new HttpCookie("OnlineShopUserAuthCookie", rnd)
                {
                    Expires = DateTime.Now.AddDays(10)
                };
                Response.Cookies.Add(authCookie);
                return Json("Success");
            }
            else
            {
                return Json("Wrong");
            }


            return Json(string.Empty);
        }
        #endregion
        #region Log out
        public ActionResult logOut()
        {
            if (Session["userLogin"] != null)
            {
                int id = Convert.ToInt16(Session["userLogin"]);
                db.Users.Find(id).token = string.Empty;
                db.SaveChanges();
                Session.RemoveAll();
            }
            return RedirectToAction("index", "Home", new { area = "" });
        }

        #endregion
        #region reciving phone number for easy login
        [HttpPost]
        public JsonResult easyLogin(string mobile)
        {
            if (mobile != null && !string.IsNullOrEmpty(mobile) && mobile.Substring(0, 2) == "09" && mobile.Length == 11 && Regex.IsMatch(mobile, @"^\d+$"))
            {
                var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();
                Session["mobile"] = mobile;
                if (user != null)
                {
                    user.easyLogin = GenerateRandomNo().ToString();
                    user.easyLoginTime = DateTime.Now;
                    db.SaveChanges();
                    #region sending sms to user ...
                    var setting = db.Settings.FirstOrDefault();

                    SMS sms = new SMS();
                    if (sms.Send(user.mobile, user.easyLogin, Convert.ToInt32(setting.smsToken_fastLogin)))
                    {
                        return Json("done");
                    }
                    #endregion

                }
            }

            return Json(string.Empty);
        }


        [HttpPost]
        public JsonResult LoginOrRegister(string mobile)
        {
            if (mobile != null && !string.IsNullOrEmpty(mobile) && mobile.Substring(0, 2) == "09" && mobile.Length == 11 && Regex.IsMatch(mobile, @"^\d+$"))
            {
                var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();
                if (user== null)
                {
                    var newUser = new User();
                    newUser.mobile = mobile;
                    newUser.Username = mobile;
                    newUser.CreationDate = DateTime.Now;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    user = newUser; 
                }
                Session["mobile"] = mobile;
                if (user != null)
                {
                    user.easyLogin = GenerateRandomNo().ToString();
                    user.easyLoginTime = DateTime.Now;
                    db.SaveChanges();
                    #region sending sms to user ...
                    SMS sms = new SMS();
                    var setting = db.Settings.FirstOrDefault();

                    if (sms.Send(user.mobile, user.easyLogin, Convert.ToInt32(setting.smsToken_fastLogin)))
                    {
                        return Json("done");
                    }
                    #endregion

                }
            }

            return Json("");
        }


        #endregion
        #region ثبت نام کاربر
        [HttpPost]
        public JsonResult register(string mobile)
        {
            mobile = mobile.Trim();
            if (mobile != null && !string.IsNullOrEmpty(mobile) && mobile.Substring(0, 2) == "09" && mobile.Length == 11 && Regex.IsMatch(mobile, @"^\d+$"))
            {
                var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();
                if (user != null)
                {
                    return Json(new { dublicate = true });
                }
                else
                {
                    var setting = db.Settings.FirstOrDefault();

                    var newUser = new User();
                    newUser.mobile = mobile;
                    newUser.forgetSmsToken = GenerateRandomNo().ToString();
                    newUser.mobileForgetDateTime = DateTime.Now;
                    newUser.Username = mobile;
                    Session["mobile"] = mobile;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    #region send sms to user
                    SMS sms = new SMS();
                    sms.Send(newUser.mobile, newUser.forgetSmsToken, Convert.ToInt32(setting.smsToken_forgetPassword));
                    #endregion
                    return Json(new { Success = true });
                }
            }
            else
            {
                return Json(new { error = "شماره موبایل وارد شده اشتباه است" });
            }
        }

        public JsonResult RegisteSubmit(string code)
        {
            string mobile = (Session["mobile"]).ToString();

            var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();
            if (user.forgetSmsToken == code)
            {
                Session.Remove("mobile");
                Session["userLogin"] = user.id;
                string rnd = RandomString(40);
                user.token = rnd;
                db.SaveChanges();
                HttpCookie authCookie = new HttpCookie("OnlineShopUserAuthCookie", rnd)
                {
                    Expires = DateTime.Now.AddDays(10)
                };
                Response.Cookies.Add(authCookie);
                return Json("Success");
            }
            else
            {
                return Json("Wrong");
            }
            return Json(string.Empty);
        }



        public ActionResult register()
        {
            return View();
        }

        #endregion
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public ActionResult LoginModal()
        {
            return View();
        }
    }
}