using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OnlineShop.Models;
using OnlineShop.Security;
using OnlineShop.Security.ViewModels;

namespace OnlineShop.Controllers
{
  
    public class AccountController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        [PreventSpam(DelayRequest: 1, ErrorMessage: "You can send login request every 20 seconds.")]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [PreventSpam(DelayRequest: 1, ErrorMessage: "You can send login request every 20 seconds.")]
        public ActionResult Login(AccountViewModel avm)
        {
            try
            {
                var captcha = Session["captcha"].ToString();
                if (captcha != avm.captcha)
                {
                    List<string> lst = new List<string>();
                    lst.Add("پسورد امنیتی درست وارد نشده است");
                    ViewBag.Error = lst;
                    return View("Login");

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
                    return View("Login");
                }
                else
                {
                    /// ورود من را به خاطر بسپار

                    //if (avm.remmember == true)
                    //{

                    var oldUserSettings = db.UserSettings.SingleOrDefault(q => q.Key == "TimeoutLogin");
                    // string cookievalue;
                    if (Request.Cookies["ioocCookie"] != null)
                    {
                        List<string> lst = new List<string>();
                        lst.Add("امکان نشست همزمان برای یک کاربر وجود ندارد");
                        ViewBag.Error = lst;
                        return View("Login");

                        cookievalue = Request.Cookies["ioocCookie"].ToString();
                    }
                    else
                    {

                        var ticket = avm.Username + ">>>" + avm.Account.FullName + ">>>" + avm.UserId + ">>>" + DateTime.Now;

                        Response.Cookies["ioocCookie"].Value = Security.RsaEncryptDecrypt.RSACls.RSAEncryption(ticket, Properties.Settings.Default.PublicKey);



                    }
                    //}
                    //var encryptConnectionString= Security.RsaEncryptDecrypt.RSACls.RSAEncryption(Properties.Settings.Default.ConnectionString,Properties.Settings.Default.PublicKey);

                    // var encryptConnectionStrin = Security.RsaEncryptDecrypt.RSACls.RSADecryption(Properties.Settings.Default.ConnectionString, Properties.Settings.Default.PrivateKey);

                    var oldUser = db.Users.SingleOrDefault(q => q.UserId == avm.Account.UserId);
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
                        return Redirect("/Account/ChangePassword");

                    }

                    return Redirect("/Admin/Dashboard/index/#/Admin/Dashboard/dashboard");
                    // return RedirectToAction("index", "dashboard", new { area = "Admin" });

                    //return RedirectToAction("Index", "Home");
                }

                //SessionPersister.Username = avm.Account.Username;
                //return View("Success");
            }
            catch (Exception ex)
            {
                iooc.Class.Utility.CreateLog(ex, "Login", "AccountController");
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

        [HttpGet]
        [PreventSpam(DelayRequest: 1, ErrorMessage: "You can send Registration request every 20 seconds.")]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(iooc.Models.User user)
        {
            try
            {
                Security.ValidationEntity validationEntity = new ValidationEntity();
                var res = validationEntity.ValidationRegister(user);
                if (res.Count != 0)
                {
                    ViewBag.Error = res;
                    return View(user);
                }
                //var x = Security.RsaEncryptDecrypt.RSACls.RSAEncryption("Farhad", Properties.Settings.Default.PublicKey);
                //var res = Security.RsaEncryptDecrypt.RSACls.RSADecryption(x, Properties.Settings.Default.PrivateKey);


                // var uniqueGuID = System.Guid.NewGuid();
                iooc.Models.User newUser = new iooc.Models.User()
                {
                    // UserId = uniqueGuID,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    mobile = user.mobile,
                    CreateDate = DateTime.Now,
                    DatePassRefresh = DateTime.Now,
                    IsActive = false,
                    Email = user.Email,
                    Password = Security.RsaEncryptDecrypt.RSACls.RSAEncryption(user.Password, Properties.Settings.Default.PublicKey),
                    AccessFailedCount = 0,
                    Token = Security.Utility.CreateToke()



                };
                db.Users.Add(newUser);
                db.SaveChanges();

                // -----------------------------add for check duplicate pass
                Models.LogPassword logPassword = new Models.LogPassword()
                {
                    Pass = newUser.Password,
                    UserId = newUser.UserId,
                };
                db.LogPasswords.Add(logPassword);
                db.SaveChanges();
                //---------------------------------------------
                //ارسال ایمیل برای تایید ایمیل کاربری
                var body = "<p>لطفا ایمیل زیر را تایید کنید</p>" +
                "<p>" + Request.Url.Scheme + "://" + Request.Url.Authority + "/Account/ConfirmEmail?username=" + newUser.Username + "&token=" + newUser.Token + "</p>";
                MessageSender _messageSender = new MessageSender();
                _messageSender.SendEmailAsync(user.Email, "تایید ایمیل کاربری", body, true);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {

                iooc.Class.Utility.CreateLog(ex, "Login", "AccountController");
                throw;
            }

        }

        [HttpGet]
        [PreventSpam(DelayRequest: 1, ErrorMessage: "You can send ConfirmEmail request every 20 seconds.")]
        public ActionResult ConfirmEmail(string username, string token)
        {
            try
            {
                var oldUser = db.Users.SingleOrDefault(q => q.Username == username);
                if (oldUser != null)
                {
                    if (oldUser.Token == token)
                    {
                        oldUser.IsEmailConfirm = true;
                    }
                    db.SaveChanges();
                }
                return Redirect("/Admin/Dashboard/index/#/Admin/Dashboard/dashboard");
            }
            catch (Exception ex)
            {

                iooc.Class.Utility.CreateLog(ex, "ConfirmEmail", "AccountController");
                throw;
            }

        }
        public ActionResult Logout()
        {
            Session["Username"] = null;
            Session["FullName"] = null;
            Session["UserId"] = null;


            //Delete cookie
            if (Request.Cookies["ioocCookie"] != null)
            {
                HttpCookie currentUserCookie = Request.Cookies["ioocCookie"];
                Response.Cookies.Remove("ioocCookie");
                currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                currentUserCookie.Value = null;
                Response.SetCookie(currentUserCookie);
            }

            return RedirectToAction("Login");
        }
        [HttpGet]
        [PreventSpam(DelayRequest: 1, ErrorMessage: "You can send ChangePassword request every 20 seconds.")]
        public ActionResult ChangePassword()
        {
            try
            {
                List<string> lstMessage = new List<string>();
                CustomAuthorizeAttribute customAuthorizeAttribute = new CustomAuthorizeAttribute();
                ValidationEntity validationEntity = new ValidationEntity();
                Models.User oldUser = null;
                if (validationEntity.checkPasswordMustChanges())
                {
                    lstMessage.Add("به علت اتمام زمان تغییر پسورد شما باید ابتدا پسورد خود را تغییر دهید");
                    var PasswordMustChanges = db.UserSettings.FirstOrDefault(q => q.Key == "PasswordMustChanges");
                    if (Session["UserId"] != null)
                    {
                        var UserId = int.Parse(Session["UserId"].ToString());
                        oldUser = db.Users.SingleOrDefault(q => q.UserId == UserId);
                        //TimeSpan spanPasswordMustChanges = DateTime.Now.Subtract(oldUser.DatePassRefresh ?? DateTime.Now);
                        lstMessage.Add("تاریخ تغییر پسورد : " + oldUser.DatePassRefresh);
                        //int Days = Convert.ToInt32(Math.Round(spanPasswordMustChanges.TotalDays, 0));
                        lstMessage.Add("مدت زمان لازم برای تغییر : " + PasswordMustChanges.value);
                    }

                    ViewBag.message = lstMessage;
                }
                else
                {
                    var UserId = int.Parse(Session["UserId"].ToString());
                    oldUser = db.Users.SingleOrDefault(q => q.UserId == UserId);
                }
                return View(oldUser);
            }
            catch (Exception ex)
            {

                iooc.Class.Utility.CreateLog(ex, "Login");
                throw;
            }

        }
        [HttpPost]
        public ActionResult ChangePassword(AccountViewModel avm)
        {
            try
            {
                List<string> lstString = new List<string>();

                var OldUser = db.Users.SingleOrDefault(q => q.Username == avm.Username);
                //----------------------------
                var oldLogPasswords = db.LogPasswords.Where(q => q.UserId == OldUser.UserId).ToList();
                foreach (var item in oldLogPasswords)
                {
                    var pass = Security.RsaEncryptDecrypt.RSACls.RSADecryption(item.Pass, Properties.Settings.Default.PrivateKey);
                    if (pass == avm.NewPassword)
                    {
                        lstString.Add("این رمز عبور قبلا استفاده شده است");

                        ViewBag.Error = lstString;
                        return View("ChangePassword", OldUser);
                    }
                }
                if (avm.NewPassword != avm.RepeatPassword)
                {
                    lstString.Add("عدم نطابق پسورد جدید و تکرار آن");

                    ViewBag.Error = lstString;
                    return View("ChangePassword", OldUser);
                }
                if (Security.RsaEncryptDecrypt.RSACls.RSADecryption(OldUser.Password, Properties.Settings.Default.PrivateKey) != avm.OldPassword)
                {
                    lstString.Add("نام کاربری یا پسورد اشتباه است");

                    ViewBag.Error = lstString;
                    return View("ChangePassword", OldUser);

                }
                else
                {
                    ValidationEntity validationEntity = new ValidationEntity();
                    lstString = validationEntity.checkPassword(avm.NewPassword);
                    if (lstString.Count > 0)
                    {
                        ViewBag.Error = lstString;
                        return View("ChangePassword", OldUser);
                    }

                    OldUser.IsResetPassword = false;
                    OldUser.DatePassRefresh = DateTime.Now;
                    OldUser.Password = Security.RsaEncryptDecrypt.RSACls.RSAEncryption(avm.NewPassword, Properties.Settings.Default.PublicKey);
                    // add for check duplicate pass
                    Models.LogPassword logPassword = new Models.LogPassword()
                    {
                        Pass = Security.RsaEncryptDecrypt.RSACls.RSAEncryption(avm.NewPassword, Properties.Settings.Default.PublicKey),
                        UserId = OldUser.UserId,

                    };
                    db.LogPasswords.Add(logPassword);

                    db.SaveChanges();
                    lstString.Add("با موفقیت پسورد شما تغییر کرد");
                    ViewBag.Error = lstString;
                    return RedirectToAction("Login");
                    return View("ChangePassword", OldUser);
                }
            }
            catch (Exception ex)
            {

                iooc.Class.Utility.CreateLog(ex, "ChangePassword", "AccountController");
                throw;
            }



        }
        [HttpGet]
        [PreventSpam(DelayRequest: 1, ErrorMessage: "You can send Edit Account request every 20 seconds.")]
        public ActionResult EditAccount()
        {
            var userid = int.Parse(Session["UserId"].ToString());
            var oldUser = db.Users.SingleOrDefault(q => q.UserId == userid);
            if (User != null)
            {
                return View(oldUser);
            }
            return View();
        }
        public ActionResult UpdateAccount(iooc.Models.User user)

        {
            try
            {
                Security.ValidationEntity validationEntity = new ValidationEntity();


                var oldUser = db.Users.SingleOrDefault(q => q.Username == user.Username);
                oldUser.FirstName = user.FirstName;
                oldUser.LastName = user.LastName;
                oldUser.Email = user.Email;
                oldUser.mobile = user.mobile;
                if (oldUser.IsEmailConfirm == false || oldUser.IsEmailConfirm == null)
                    oldUser.Token = Security.Utility.CreateToke();

                db.SaveChanges();
                string host = Request.Url.Host;
                if (oldUser.IsEmailConfirm == false || oldUser.IsEmailConfirm == null)
                {
                    var body = "<p>لطفا ایمیل زیر را تایید کنید</p>" +
                    "<p>" + Request.Url.Scheme + "://" + Request.Url.Authority + "/Account/ConfirmEmail?username=" + user.Username + "&token=" + oldUser.Token + "</p>";

                    MessageSender _messageSender = new MessageSender();
                    _messageSender.SendEmailAsync(user.Email, "تایید ایمیل کاربری", body, true);
                }
                //return RedirectToAction("Index", "Home");
                return Redirect("/Admin/Dashboard/index/#/Admin/Dashboard/dashboard");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "UpdateAccount", "AccountController");
                throw;
            }

        }
        [HttpPost]
        public JsonResult RemindSessionTime()
        {
            int second = 0;
            if (Session["DateCreated"] != null)
            {
                TimeSpan span = DateTime.Now.Subtract((DateTime)Session["DateCreated"]);
                second = (Security.SettingApp._TimeoutLogin * 60) - Convert.ToInt32(Math.Round(span.TotalSeconds, 0));
            }
            return Json(second, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
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
                    return RedirectToAction("Login");
                }
                //olduser.Token = Security.Utility.CreateToke();
                //db.SaveChanges();
                //ارسال ایمیل برای تایید ایمیل کاربری
                var body = "<p>پسورد شما :</p>" +
                "<p>" + Security.RsaEncryptDecrypt.RSACls.RSADecryption(olduser.Password, Properties.Settings.Default.PrivateKey) + "</p>";

                MessageSender _messageSender = new MessageSender();
                _messageSender.SendEmailAsync(olduser.Email, "فراموشی رمز عبور", body, true);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "forgetpassword", "AccountController");
                throw;
            }

        }

        [HttpPost]
        public ActionResult CheckPassword(string pass)
        {
            try
            {
                Security.ValidationEntity validationEntity = new ValidationEntity();
                var res = validationEntity.checkPassword(pass);

                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                Class.Utility.CreateLog(ex, "CheckPassword", "AccountController");
                throw;
            }

        }
        public ActionResult CaptchaImage()
        {
            var bitmap = new Bitmap(50, 30, PixelFormat.Format24bppRgb);
            var graphic = Graphics.FromImage(bitmap);

            var random = new Random();
            var captchaNum = random.Next(1234, 9999);
            TempData["captcha"] = captchaNum;
            Session["captcha"] = captchaNum;
            graphic.FillRectangle(new SolidBrush(Color.Black), 0, 0, 50, 30f);
            graphic.DrawString(captchaNum.ToString(), new Font("Tahoma", 10, FontStyle.Bold),
                               new SolidBrush(Color.White), 4, 8);

            var memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);

            return File(memoryStream.ToArray(), "image/png");

        }
    }
}