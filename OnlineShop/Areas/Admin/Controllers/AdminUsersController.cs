using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Class;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using OnlineShop.Areas.Admin.Security;
using System.Data.Entity.Migrations;
using OnlineShop.Security;
using System.Text;
using OnlineShop.Security.ViewModels;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت ادمین ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class AdminUsersController : Controller
    {
        checkLoginAdmin ch = new checkLoginAdmin();
        OnlineShopEntities db = new OnlineShopEntities();
        [Access("لیست ادمین ها")]
        public ActionResult Index()
        {
            return View();
        }
        [Access("ایجاد ادمین جدید")]

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Access("ایجاد ادمین جدید")]
        [ValidateAntiForgeryToken]
        public JsonResult Create(string username, string password)
        {
            try
            {
                db.Database.BeginTransaction();
                var adminroles = db.Roles.Where(r => r.RoleName == "Admin" || r.RoleName == "SuperAdmin");
                var user = new User
                {
                    Username = username,
                    password = Class.Utility.EncryptPassword(password),
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    IsResetPassword = true,

                };

                db.Users.Add(user);
                db.SaveChanges();

                foreach (var role in adminroles)
                {
                    var userrole = new UserRole
                    {
                        RoleId = role.RoleId,
                        UserId = user.id
                    };
                    db.UserRoles.Add(userrole);
                }

                db.SaveChanges();
                db.Database.CurrentTransaction.Commit();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                db.Database.CurrentTransaction.Rollback();
                Class.Utility.CreateLog(ex, "Create", "AdminUsersController");
                throw;
            }


        }

        [HttpPost]
        [Access("حذف ادمین")]
        public JsonResult Remove(int id)
        {
            var selecteduser = db.Users.Find(id);
            db.Users.Remove(selecteduser);
            db.SaveChanges();
            return Json(string.Empty);
        }


        [HttpPost]
        [Access("فعال یا غیر فعال نمودن ادمین")]

        public JsonResult status(int id, Boolean Active)
        {
            if (Session["login"] != null)
            {
                var admin = db.Users.Find(id);
                admin.IsActive = Active;
                db.SaveChanges();
                return Json(string.Empty);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [Access("ویرایش ادمین")]
        public ActionResult Edit(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [Access("ویرایش ادمین")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, string username, string password)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                user.Username = username;
                user.password = Class.Utility.EncryptPassword(password);
            }
            db.Users.AddOrUpdate(user);
            db.SaveChanges();
            return Json("success");
        }

        [HttpPost]
        [Access("لیست ادمین ها")]

        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            //var bans = new List<string>();
            //bans.Add("smsToken");
            #endregion
            try
            {
                //var Result = fn.CreateTable("AdminUsers", null, Request);
                //var jobject = JsonConvert.DeserializeObject<List<AdminUser>>(Result.content);
                db.Configuration.ProxyCreationEnabled = false;
                var jobject = db.Users.Where(u => u.UserRoles.Any(ur => db.Roles.Select(r => r.RoleId).Contains(ur.RoleId))).ToList();
                return Json(new { iTotalRecords = jobject.Count, iTotalDisplayRecords = jobject.Count, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
            }
        }

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

        //[HttpGet]
        //[PreventSpam(DelayRequest: 1, ErrorMessage: "You can send ChangePassword request every 20 seconds.")]
        //public ActionResult ChangePassword()

        //    try
        //    {
        //        List<string> lstMessage = new List<string>();
        //        CustomAuthorizeAttribute customAuthorizeAttribute = new CustomAuthorizeAttribute();
        //        ValidationEntity validationEntity = new ValidationEntity();
        //        Models.User oldUser = null;
        //        if (validationEntity.checkPasswordMustChanges())
        //        {
        //            lstMessage.Add("به علت اتمام زمان تغییر پسورد شما باید ابتدا پسورد خود را تغییر دهید");
        //            var PasswordMustChanges = db.UserSettings.FirstOrDefault(q => q.Key == "PasswordMustChanges");
        //            if (Session["UserId"] != null)
        //            {
        //                var UserId = int.Parse(Session["UserId"].ToString());
        //                oldUser = db.Users.SingleOrDefault(q => q.id == UserId);
        //                //TimeSpan spanPasswordMustChanges = DateTime.Now.Subtract(oldUser.DatePassRefresh ?? DateTime.Now);
        //                lstMessage.Add("تاریخ تغییر پسورد : " + oldUser.DatePassRefresh);
        //                //int Days = Convert.ToInt32(Math.Round(spanPasswordMustChanges.TotalDays, 0));
        //                lstMessage.Add("مدت زمان لازم برای تغییر : " + PasswordMustChanges.value);
        //            }

        //            ViewBag.message = lstMessage;
        //        }
        //        else
        //        {
        //            var UserId = int.Parse(Session["UserId"].ToString());
        //            oldUser = db.Users.SingleOrDefault(q => q.id == UserId);
        //        }
        //        return View("ChangePassword", oldUser);
        //    }
        //    catch (Exception ex)
        //    {

        //        OnlineShop.Class.Utility.CreateLog(ex, "Login", "AdminUsersController");
        //        throw;
        //    }

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(AccountViewModel avm)
        {
            try
            {
                List<string> lstString = new List<string>();

                var OldUser = db.Users.SingleOrDefault(q => q.Username == avm.Username);
                OldUser.IsResetPassword = false;
                OldUser.DatePassRefresh = DateTime.Now;
                OldUser.password = OnlineShop.Security.RsaEncryptDecrypt.RSACls.RSAEncryption(avm.NewPassword, Properties.Settings.Default.PublicKey);
                // add for check duplicate pass
                Models.LogPassword logPassword = new Models.LogPassword()
                {
                    Pass = OnlineShop.Security.RsaEncryptDecrypt.RSACls.RSAEncryption(avm.NewPassword, Properties.Settings.Default.PublicKey),
                    UserId = OldUser.id,

                };
                db.LogPasswords.Add(logPassword);

                db.SaveChanges();
                lstString.Add("با موفقیت پسورد شما تغییر کرد");
                ViewBag.Error = lstString;
                return Redirect("/admin/admindashboard/index");
                //return View("ChangePassword", OldUser);

            }
            catch (Exception ex)
            {

                OnlineShop.Class.Utility.CreateLog(ex, "ChangePassword", "AdminUsersController");
                throw;
            }


        }

        [HttpPost]
        public JsonResult CheckPassword(string oldpass, string newpass,string username,string captcha)
        {
            try
            {
                var user = db.Users.Where(u => u.Username == username).FirstOrDefault();
                string lstString = "";

                var Captcha = Session["captcha"].ToString();
                if (Captcha != captcha)
                {
                    List<string> lst = new List<string>();
                    lst.Add("پسورد امنیتی درست وارد نشده است");
                    ViewBag.Error = lst;

                    db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                    {
                        AuthenticationLogType = (byte)AuthenticationLogType.WrongCaptcha,
                        CreateDate = DateTime.Now,
                        Hostname = OnlineShop.Security.Utility.GetHostName(),
                        IP = OnlineShop.Security.Utility.GetLocalIPAddress(),
                        UserId = user.id,
                        UserName = username,
                        Description = "پسورد امنیتی درست وارد نشده است"
                    });
                    db.SaveChanges();
                    lstString = "پسورد امنیتی درست وارد نشده است";
                    return Json(new { Result = false, Message = lstString });

                }

                var OldUser = db.Users.SingleOrDefault(q => q.Username == username);
                //----------------------------
                var precount = db.UserSettings.Where(p => p.Key == "PreviousPasswordCount").FirstOrDefault().value;

                var oldLogPasswords = db.LogPasswords.Where(q => q.UserId == OldUser.id).OrderByDescending(q=>q.LogPasswordId).Take(Convert.ToInt32(precount)).ToList();
                foreach (var item in oldLogPasswords)
                {
                    var pass = OnlineShop.Security.RsaEncryptDecrypt.RSACls.RSADecryption(item.Pass, Properties.Settings.Default.PrivateKey);
                    if (pass == newpass)
                    {
                        db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                        {
                            AuthenticationLogType = (byte)AuthenticationLogType.WrongPassword,
                            CreateDate = DateTime.Now,
                            Hostname = OnlineShop.Security.Utility.GetHostName(),
                            IP = OnlineShop.Security.Utility.GetLocalIPAddress(),
                            UserId = user.id,
                            UserName = username,
                            Description = "این رمز عبور قبلا استفاده شده است"
                        });
                        db.SaveChanges();
                        lstString = "این رمز عبور قبلا استفاده شده است";
                        return Json(new { Result = false, Message = lstString });
                    }
                }

                if (OnlineShop.Security.RsaEncryptDecrypt.RSACls.RSADecryption(OldUser.password, Properties.Settings.Default.PrivateKey) != oldpass)
                {
                    db.LogUserAuthenticationDatas.Add(new LogUserAuthenticationData()
                    {
                        AuthenticationLogType = (byte)AuthenticationLogType.WrongPassword,
                        CreateDate = DateTime.Now,
                        Hostname = OnlineShop.Security.Utility.GetHostName(),
                        IP = OnlineShop.Security.Utility.GetLocalIPAddress(),
                        UserId = user.id,
                        UserName = username,
                        Description = "نام کاربری یا پسورد اشتباه است"
                    });
                    lstString = "نام کاربری یا پسورد اشتباه است";
                    return Json(new { Result = false, Message = lstString });

                }
                else
                {
                    ValidationEntity validationEntity = new ValidationEntity();
                    var res = validationEntity.checkPassword(newpass);
                    if (res.Count > 0)
                    {
                        ViewBag.Error = lstString;
                        return Json(new { Result = false, Message = string.Join(",", res.ToArray()) });
                    }
                }


                return Json(new { Result = true, Message = "رمز عبور با موفقیت تغییر کرد" });
            }

            catch (Exception ex)
            {

                OnlineShop.Class.Utility.CreateLog(ex, "CheckPassword", "AdminUsersController");
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
                OnlineShop.Class.Utility.CreateLog(ex, "CaptchaImage", "LoginController");
                throw;
            }


        }
    }
}