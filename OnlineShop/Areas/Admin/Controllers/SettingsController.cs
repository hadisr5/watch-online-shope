using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("تنظیمات")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class SettingsController : Controller
    {
        checkLoginAdmin ch = new checkLoginAdmin();
        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Admin/Settings
        [Access("مشاهده تنظیمات سایت")]

        public ActionResult Index()
        {
            try
            {
                var setting = db.Settings.FirstOrDefault();
                if (setting == null)
                {
                    setting = new Setting();
                    db.Settings.Add(setting);
                    db.SaveChanges();
                }
                ViewBag.id = setting.id;
                return View();
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Index", "SettingsController");
                throw;
            }
           
        }
        [HttpPost]
        [Access("ویرایش تنظیمات سایت")]
        [ValidateAntiForgeryToken]
        public ActionResult indexPost(Setting settings,string CountUnsuccessfulattempt,string MinuteLockUser,string Forcedpasswordchange,
                                          string ForcedpasswordchangeDay,string TimeoutLogin,string ResetPassword,string PasswordMustChanges,
                                          string PreviousPasswordCount)
        {
            try
            {
                db.Settings.AddOrUpdate(settings);
                var CountUnsuccessfulattempt1 = db.UserSettings.Where(p => p.Key == nameof(CountUnsuccessfulattempt)).FirstOrDefault();
                if(CountUnsuccessfulattempt1!=null)
                {
                    CountUnsuccessfulattempt1.value = CountUnsuccessfulattempt;
                }
                db.UserSettings.AddOrUpdate(CountUnsuccessfulattempt1);

                var MinuteLockUser1 = db.UserSettings.Where(p => p.Key == nameof(MinuteLockUser)).FirstOrDefault();
                if (MinuteLockUser1 != null)
                {
                    MinuteLockUser1.value = MinuteLockUser;
                }
                db.UserSettings.AddOrUpdate(MinuteLockUser1);

                var Forcedpasswordchange1 = db.UserSettings.Where(p => p.Key == nameof(Forcedpasswordchange)).FirstOrDefault();
                if (Forcedpasswordchange1 != null)
                {
                    Forcedpasswordchange1.value = Forcedpasswordchange;
                }
                db.UserSettings.AddOrUpdate(Forcedpasswordchange1);

                var ForcedpasswordchangeDay1 = db.UserSettings.Where(p => p.Key == nameof(ForcedpasswordchangeDay)).FirstOrDefault();
                if (ForcedpasswordchangeDay1 != null)
                {
                    ForcedpasswordchangeDay1.value = ForcedpasswordchangeDay;
                }
                db.UserSettings.AddOrUpdate(ForcedpasswordchangeDay1);

                var TimeoutLogin1 = db.UserSettings.Where(p => p.Key == nameof(TimeoutLogin)).FirstOrDefault();
                if (TimeoutLogin1 != null)
                {
                    TimeoutLogin1.value = TimeoutLogin;
                }
                db.UserSettings.AddOrUpdate(TimeoutLogin1);

                var ResetPassword1 = db.UserSettings.Where(p => p.Key == nameof(ResetPassword)).FirstOrDefault();
                if (ResetPassword1 != null)
                {
                    ResetPassword1.value = ResetPassword;
                }
                db.UserSettings.AddOrUpdate(ResetPassword1);

                var PasswordMustChanges1 = db.UserSettings.Where(p => p.Key == nameof(PasswordMustChanges)).FirstOrDefault();
                if (PasswordMustChanges1 != null)
                {
                    PasswordMustChanges1.value = PasswordMustChanges;
                }

                var PreviousPasswordCount1 = db.UserSettings.Where(p => p.Key == nameof(PreviousPasswordCount)).FirstOrDefault();
                if (PreviousPasswordCount1 != null)
                {
                    PreviousPasswordCount1.value = PreviousPasswordCount;
                }
                db.UserSettings.AddOrUpdate(PreviousPasswordCount1);

                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "indexPost", "SettingsController");
                throw;
            }
           
        }
    }
}