using Newtonsoft.Json;
using OnlineShop.Class;
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
    [DisplayName("مدیریت بنر ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class BannersController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/Banners
        [Access("لیست بنر ها")]

        public ActionResult Index()
        {
            return View();
        }
        [Access("لیست بنر ها")]

        [HttpPost]
        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            #endregion
            try
            {

                var Result = fn.CreateTable("Banners", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Banner>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "List", "BannersController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
            }

}
        [Access("ایجاد بنر جدید")]


        public ActionResult Create()
        {
            return View();
        }
        [Access("ایجاد بنر جدید")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Banner banner)
        {
            try
            {
                if (banner.active == true)
                {
                    if (db.Banners.Where(r => r.active == true).FirstOrDefault() != null)
                    {
                        db.Banners.Where(r => r.active == true).FirstOrDefault().active = false;
                    }
                }
                db.Banners.Add(banner);
                return View();

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "Create", "BannersController");
                throw;
            }
           
        }
        [Access("ویرایش بنر ")]

        public ActionResult edit(int id)
        {
            ViewBag.id = id;
            return View();
        }

        [Access("ویرایش بنر ")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult edit(Banner banner)
        {
            try
            {
                if (banner.active == true)
                {
                    if (db.Banners.Where(r => r.active == true).FirstOrDefault() != null)
                    {
                        db.Banners.Where(r => r.active == true).FirstOrDefault().active = false;
                    }
                }
                db.Banners.AddOrUpdate(banner);
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "edit", "BannersController");
                throw;
            }
          
        }
    }
}