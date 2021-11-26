using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using SmsIrRestful;
using System.Configuration;
using OnlineShop.Areas.Admin.Functions;
using OnlineShop.Class;
using Newtonsoft.Json;


using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت کاربران")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class UsersController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch= new checkLoginAdmin();


        [Access("لیست کاربران")]
        [HttpPost]
        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            //var bans = new List<string>();
            //bans.Add("smsToken");
            #endregion
            try
            {
                var test = Request.Params["order[0][dir]"]; 


                var Result = fn.CreateTable("Users", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<User>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "UsersController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
            }

        }

        [Access("تبدیل کاربران به ویژه")]
        [HttpPost]
        public JsonResult Prime(int id  , bool check)
        {
            try
            {
                var user = db.Users.Find(id);
                user.Special = check;
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Prime", "UsersController");
                throw;
            }
           
        }
        [Access("لیست کاربران")]
        public ActionResult Index()
        {
            return View();
        }
        [Access("مشاهده جزئیات کاربر")]
        public ActionResult details(int? id, int? sendVerificationCode)
        {
            if (sendVerificationCode != null)
            {
                try
                {
                    var user = db.Users.Find(id);
                    int rnd = GenerateRandomNo();

                    #region Send SMS to user
                    SMS sms = new SMS();
                    var setting = db.Settings.FirstOrDefault();

                    if (!sms.Send(user.mobile, rnd.ToString(), Convert.ToInt32(setting.smsToken_validateUser)))
                    {
                        ViewBag.sms = "مشکل در ارسال کد";
                    }
                    else
                    {
                        ViewBag.sms = rnd;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "details", "UsersController");
                    throw;
                }
                
            }
            ViewBag.id = id;
            return View();
        }
        public int GenerateRandomNo()
        {
            ch.checkLogin();

            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        [HttpPost]
        [Access("حذف کاربران")]
        public ActionResult remove(int? id)
        {
            try
            {
                db.Users.Remove(db.Users.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "remove", "UsersController");
                throw;
            }
           
        }

        public PartialViewResult Export()
        {
            return PartialView();
        }

    }
}