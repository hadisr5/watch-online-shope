using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using System.Data.Entity.Migrations;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت شبکه های اجتماعی")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class SocialMediaController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/Banners
        [Access("ویرایش شبکه های اجتماعی")]
        // GET: Admin/SocialMedia
        public ActionResult Index()
        {
            try
            {
                if (db.SocialMedias.FirstOrDefault() == null)
                {
                    var SocialMedia = new SocialMedia();
                    db.SocialMedias.Add(SocialMedia);
                    db.SaveChanges();
                }
                return View();
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Index", "SocialMediaController");
                throw;
            }
          
        }
        [Access("ویرایش شبکه های اجتماعی")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // GET: Admin/SocialMedia
        public ActionResult Index(SocialMedia SC)
        {
            try
            {
                db.SocialMedias.AddOrUpdate(SC);
                db.SaveChanges();
                return View();
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Index", "SocialMediaController");
                throw;
            }
            
        }
    }
}