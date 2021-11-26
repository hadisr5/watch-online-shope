using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShop.Class;
using OnlineShop.Models;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سایز ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class SizeController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("لیست سایز ها")]
        public ActionResult Index()
        {
            return View();
        }
        [Access("لیست سایز ها")]
        [HttpPost]
        public JsonResult List()
        {
            #region bands
            var fn = new functions();

            #endregion
            try
            {
                var Result = fn.CreateTable("Sizes", null, Request);
                while (Result.content.Contains("\"Size\":"))
                {
                    Result.content = Result.content.Replace("\"Size\":", "\"Size1\":");
                }
                var jobject = JsonConvert.DeserializeObject<List<Size>>(Result.content);

                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "SizeController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                
            }

        }
        [HttpGet]
        [Access("ایجاد سایز ها")]

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Access("ایجاد سایز ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Size(Size size)
        {
            try
            {
                db.Sizes.Add(size);
                db.SaveChanges();

                return Json(new { success = "سایز " + size.title + " اضافه شد" });
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Create_Size", "SizeController");
                throw;
            }
           
        }
        [HttpPost]
        [Access("حذف سایز ها")]
        public ActionResult remove(int? id)
        {
            try
            {
                db.Sizes.Remove(db.Sizes.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "remove", "SizeController");
                throw;
            }
            
        }
        [Access("ویرایش سایز ها")]
        public ActionResult Edit(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [Access("ویرایش سایز ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(Size size, int id)
        {
            try
            {
                db.Sizes.AddOrUpdate(size);
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Edit_Post", "SizeController");
                throw;
            }
           
        }
    }
}