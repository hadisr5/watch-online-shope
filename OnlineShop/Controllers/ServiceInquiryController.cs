using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Areas.Admin.Functions;
using OnlineShop.Models;
using OnlineShop.Security;
using OnlineShop.Security.RsaEncryptDecrypt;
using OnlineShop.Security.ViewModels;

namespace OnlineShop.Controllers
{
    
    public class ServiceInquiryController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Admin/Login
        public ActionResult Index()
        {
            HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["OnlineShopAdminAuthCookie"];
            if (authCookie != null)
            {
                string rnd = authCookie.Value;
                var user = db.AdminUsers.Where(r => r.token == rnd).FirstOrDefault();
                if (user != null)
                {
                    Session["login"] = user.id;
                    return RedirectToAction("index", "AdminDashboard", new { newlogin = "true" });
                }
            }
            return View();
        }
        //[HttpPost]
        //public ActionResult Index(string email, string password, int? remmember)
        //{
        //    var user = db.AdminUsers.Where(r => r.username.ToLower() == email.ToLower() && r.password == password).FirstOrDefault();

        //    if (user != null)
        //    {
        //        if(user.Active==true)
        //        {
        //            Session["login"] = user.id;
        //            if (remmember != null)
        //            {
        //                string rnd = RandomString(40);
        //                user.token = rnd;
        //                db.SaveChanges();
        //                HttpCookie authCookie = new HttpCookie("OnlineShopAdminAuthCookie", rnd)
        //                {
        //                    Expires = DateTime.Now.AddDays(10)
        //                };
        //                Response.Cookies.Add(authCookie);
        //            }
        //            return RedirectToAction("index", "AdminDashboard", new { newlogin = "true" });
        //        }
        //        else
        //            ViewBag.error = "حساب کاربری شما غیر فعال است.";

        //    }
        //    else
        //    {
        //        ViewBag.error = "نام کاربری و یا رمز عبور اشتباه است";
        //    }
        //    return View();
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult forgetpassword(string username)
        {
            try
            {
                List<string> lstError = new List<string>();
                var olduser = db.Users.SingleOrDefault(q => q.Username == username);
                if (olduser == null)
                {
                    lstError.Add("نام کاربری یافت نشد");
                    TempData["Error"] = "نام کاربری یافت نشد";
                    return RedirectToAction("Index");
                }
                //olduser.Token = Security.Utility.CreateToke();
                //db.SaveChanges();
                //ارسال ایمیل برای تایید ایمیل کاربری
                var body = "<p>پسورد شما :</p>" +
                "<p>" + OnlineShop.Security.RsaEncryptDecrypt.RSACls.RSADecryption(olduser.password, Properties.Settings.Default.PrivateKey) + "</p>";

                MessageSender _messageSender = new MessageSender();
                _messageSender.SendEmailAsync(olduser.email, "فراموشی رمز عبور", body, true);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "forgetpassword", "ServiceInquiryController");
                throw;
            }

        }

        [HttpGet]
        public ActionResult FastLogin()
        {
            if (Session["Username"] == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }

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
        public JsonResult easyLoginPassword(string code)
        {
            string mobile = (Session["mobile"]).ToString();

            var user = db.Users.Where(r => r.mobile == mobile).FirstOrDefault();
            if (user.easyLogin == code)
            {
                var oldUserSettings = db.UserSettings.SingleOrDefault(q => q.Key == "TimeoutLogin");
                // string cookievalue;
                if (Request.Cookies["OnlineShopAdminAuthCookie"] != null)
                {
                    List<string> lst = new List<string>();
                    lst.Add("امکان نشست همزمان برای یک کاربر وجود ندارد");
                    ViewBag.Error = lst;
                    db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                    {
                        AuthenticationLogType = (byte)AuthenticationLogType.SimultaneousSession,
                        CreateDate = DateTime.Now,
                        Hostname = Utility.GetHostName(),
                        IP = Utility.GetLocalIPAddress(),
                        UserId = user.id,
                        UserName = user.Username,
                        Description = "امکان نشست همزمان برای یک کاربر وجود ندارد"
                    });
                    db.SaveChanges();
                    return Json("Wrong");
                }
                else
                {

                    var ticket = user.Username + ">>>" + $"{user.firstName} {user.lastName}" + ">>>" + user.id + ">>>" + DateTime.Now;

                    Response.Cookies["OnlineShopAdminAuthCookie"].Value = RSACls.RSAEncryption(ticket, Properties.Settings.Default.PublicKey);



                }
                //}
                //var encryptConnectionString= Security.RsaEncryptDecrypt.RSACls.RSAEncryption(Properties.Settings.Default.ConnectionString,Properties.Settings.Default.PublicKey);

                // var encryptConnectionStrin = Security.RsaEncryptDecrypt.RSACls.RSADecryption(Properties.Settings.Default.ConnectionString, Properties.Settings.Default.PrivateKey);

                var oldUser = db.Users.SingleOrDefault(q => q.id == user.id);
                oldUser.Hostname = Utility.GetHostName();
                db.SaveChanges();

                fillConfigSetting();
                Session["Username"] = user.Username;
                Session["FullName"] = $"{user.firstName} {user.lastName}";
                Session["UserId"] = user.id;
                Session["login"] = user.id;
                var RolesName = db.UserRoles.Where(q => q.UserId == user.id).Select(q => q.Role.RoleName).ToList();
                Session["Roles"] = RolesName.ToArray();
                Session["DateCreated"] = DateTime.Now;

                db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                {
                    AuthenticationLogType = (byte)AuthenticationLogType.SuccessfullLogin,
                    CreateDate = DateTime.Now,
                    Hostname = Utility.GetHostName(),
                    IP = Utility.GetLocalIPAddress(),
                    UserId = user.id,
                    UserName = user.Username,
                    Description = "ورود موفق"
                });
                db.SaveChanges();

                return Json("Success");
            }
            else
            {
                db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                {
                    AuthenticationLogType = (byte)AuthenticationLogType.WrongEasyCode,
                    CreateDate = DateTime.Now,
                    Hostname = Utility.GetHostName(),
                    IP = Utility.GetLocalIPAddress(),
                    UserId = user.id,
                    UserName = user.Username,
                    Description = "کد پیامکی وارد شده اشتباه است"
                });
                db.SaveChanges();
                return Json("Wrong");
            }


            return Json(string.Empty);
        }
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        [HttpPost]
        [PreventSpam(DelayRequest: 1, ErrorMessage: "You can send login request every 20 seconds.")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AccountViewModel avm)
        {
            try
            {
                var captcha = Session["captcha"].ToString();
                if (captcha != avm.captcha)
                {
                    List<string> lst = new List<string>();
                    lst.Add("پسورد امنیتی درست وارد نشده است");
                    ViewBag.Error = lst;
                  
                    db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                    {
                        AuthenticationLogType=(byte)AuthenticationLogType.WrongCaptcha,
                        CreateDate=DateTime.Now,
                        Hostname=Utility.GetHostName(),
                        IP=Utility.GetLocalIPAddress(),
                        UserId= (avm.UserId != 0) ? avm.UserId:(int?)null ,
                        UserName=avm.Username,
                        Description= "پسورد امنیتی درست وارد نشده است"
                    });
                    db.SaveChanges();
                    return View();

                }
                Account account = new Account()
                {
                    Username = avm.Username,
                    Password = avm.Password
                };
                avm.Account = account;
                AccountModel accountModel = new AccountModel();

                ValidationEntity validationEntity = new ValidationEntity();
                var result = validationEntity.ValidationLogin(avm);
                if (result.lstString != null)
                {
                    ViewBag.Error = result.lstString;
                    ViewBag.IsResetPassword = avm.IsResetPassword;
                    return View("Index");
                }
                else
                {
                    /// ورود من را به خاطر بسپار

                    //if (avm.remmember == true)
                    //{

                    var oldUserSettings = db.UserSettings.SingleOrDefault(q => q.Key == "TimeoutLogin");
                    // string cookievalue;
                    if (Request.Cookies["OnlineShopAdminAuthCookie"] != null)
                    {
                        List<string> lst = new List<string>();
                        lst.Add("امکان نشست همزمان برای یک کاربر وجود ندارد");
                        ViewBag.Error = lst;
                        db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                        {
                            AuthenticationLogType = (byte)AuthenticationLogType.SimultaneousSession,
                            CreateDate = DateTime.Now,
                            Hostname = Utility.GetHostName(),
                            IP = Utility.GetLocalIPAddress(),
                            UserId = (avm.UserId != 0) ? avm.UserId : (int?)null,
                            UserName = avm.Username,
                            Description= "امکان نشست همزمان برای یک کاربر وجود ندارد"
                        });
                        db.SaveChanges();
                        return View("Index");

                        // cookievalue = Request.Cookies["ioocCookie"].ToString();
                    }
                    else
                    {

                        var ticket = avm.Username + ">>>" + avm.Account.FullName + ">>>" + avm.UserId + ">>>" + DateTime.Now;

                        Response.Cookies["OnlineShopAdminAuthCookie"].Value = RSACls.RSAEncryption(ticket, Properties.Settings.Default.PublicKey);



                    }
                    //}
                    //var encryptConnectionString= Security.RsaEncryptDecrypt.RSACls.RSAEncryption(Properties.Settings.Default.ConnectionString,Properties.Settings.Default.PublicKey);

                    // var encryptConnectionStrin = Security.RsaEncryptDecrypt.RSACls.RSADecryption(Properties.Settings.Default.ConnectionString, Properties.Settings.Default.PrivateKey);

                    var oldUser = db.Users.SingleOrDefault(q => q.id == avm.Account.UserId);
                    oldUser.Hostname = Utility.GetHostName();
                    db.SaveChanges();

                    fillConfigSetting();
                    Session["Username"] = avm.Account.Username;
                    Session["FullName"] = avm.Account.FullName;
                    Session["UserId"] = avm.Account.UserId;
                    Session["login"] = avm.Account.UserId;
                    var RolesName = db.UserRoles.Where(q => q.UserId == avm.Account.UserId).Select(q => q.Role.RoleName).ToList();
                    Session["Roles"] = RolesName.ToArray();
                    Session["DateCreated"] = DateTime.Now;

                    if (validationEntity.checkPasswordMustChanges())
                    {
                        db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                        {
                            AuthenticationLogType = (byte)AuthenticationLogType.PasswordMustChange,
                            CreateDate = DateTime.Now,
                            Hostname = Utility.GetHostName(),
                            IP = Utility.GetLocalIPAddress(),
                            UserId = (avm.UserId != 0) ? avm.UserId : (int?)null,
                            UserName = avm.Username,
                            Description= "رمز عبور باید تغییر کند"
                        });
                        db.SaveChanges();
                        return Redirect("/Account/ChangePassword");

                    }

                    db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                    {
                        AuthenticationLogType = (byte)AuthenticationLogType.SuccessfullLogin,
                        CreateDate = DateTime.Now,
                        Hostname = Utility.GetHostName(),
                        IP = Utility.GetLocalIPAddress(),
                        UserId = (avm.UserId != 0) ? avm.UserId : (int?)null,
                        UserName = avm.Username,
                        Description = "ورود موفق"
                    });
                    db.SaveChanges();
                    return Redirect("/Admin/AdminDashboard/Index?newlogin=true");
                    // return RedirectToAction("index", "dashboard", new { area = "Admin" });

                    //return RedirectToAction("Index", "Home");
                }

                //SessionPersister.Username = avm.Account.Username;
                //return View("Success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Index", "ServiceInquiryController");
                throw;
            }

        }

        public ActionResult CaptchaImage()
        {
            try
            {
                var bitmap = new Bitmap(50, 30, PixelFormat.Format24bppRgb);
                var graphic = Graphics.FromImage(bitmap);

                var random = new Random();
                var captchaNum = random.Next(1234, 9999);
                TempData["captcha"] = captchaNum;
                Session["captcha"] = captchaNum;
                graphic.FillRectangle(new SolidBrush(System.Drawing.Color.Black), 0, 0, 50, 30f);
                graphic.DrawString(captchaNum.ToString(), new Font("Tahoma", 10, FontStyle.Bold),
                                   new SolidBrush(System.Drawing.Color.White), 4, 8);

                var memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Png);

                return File(memoryStream.ToArray(), "image/png");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "CaptchaImage", "ServiceInquiryController");
                throw;
            }
           

        }

        private void fillConfigSetting()
        {
            // Models.iooc_dbEntities db = new Models.iooc_dbEntities();
            var res = db.UserSettings.FirstOrDefault(q => q.Key == "TimeoutLogin");
            if (res != null)
            {
                SettingApp._TimeoutLogin = int.Parse(res.value.ToString());
            }
            else
            {
                SettingApp._TimeoutLogin = 5;
            }
        }
        public ActionResult logOut()
        {
            if (Session["login"] != null)
            {
                Session["Username"] = null;
                Session["FullName"] = null;
                Session["UserId"] = null;
                //Delete cookie
                if (Request.Cookies["OnlineShopAdminAuthCookie"] != null)
                {
                    HttpCookie currentUserCookie = Request.Cookies["OnlineShopAdminAuthCookie"];
                    Response.Cookies.Remove("OnlineShopAdminAuthCookie");
                    currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                    currentUserCookie.Value = null;
                    Response.SetCookie(currentUserCookie);
                }
                int id = Convert.ToInt16(Session["login"]);
                db.Users.Find(id).token = string.Empty;
                db.SaveChanges();
                Session.RemoveAll();
            }
            return RedirectToAction("index");
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