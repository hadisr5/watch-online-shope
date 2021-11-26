using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using OnlineShop.Class;
using Newtonsoft.Json;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت صفحات")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class PagesController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("لیست صفحات")]
        // GET: Admin/Pages
        public ActionResult Index()
        {
            return View();
        }
        [Access("ایجاد صفحات")]

        public ActionResult Create()
        {
            return View();
        }

        [Access("لیست صفحات")]
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
                var Result = fn.CreateTable("Pages", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Page>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "PagesController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                throw;
            }
        }
        [Access("ایجاد صفحات")]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Menu(Page page)
        {
            page.creationDate = DateTime.Now;
            try
            {
                db.Pages.Add(page);
                db.SaveChanges();
                return Json(new { success = "صفحه " + page.title + " اضافه شد" });
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Create_Menu", "PagesController");
                throw;
            }
         
        }
        [Access("حذف صفحات")]

        public ActionResult remove(int? id)
        {
            try
            {
                db.Pages.Remove(db.Pages.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "remove", "PagesController");
                throw;
            }
         
        }
        [Access("ویرایش صفحات")]
        public ActionResult Edit(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [Access("ویرایش صفحات")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(Page page, int id )
        {
            try
            {
                db.Pages.AddOrUpdate(page);
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Edit_Post", "PagesController");
                throw;
            }
            
        }
    }
}