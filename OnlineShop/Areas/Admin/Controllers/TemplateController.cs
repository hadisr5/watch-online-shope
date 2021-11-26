using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Class;
using Newtonsoft.Json;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت قالب وبسایت")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class TemplateController : Controller
    {

        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Admin/Template
        [Access("لیست قالب ها")]
        public ActionResult Index()
        {
            return View();
        }
        [Access("لیست قالب ها")]
        [HttpPost]
        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            #endregion
            try
            {
                var Result = fn.CreateTable("Template", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Template>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "TemplateController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
            }
        }

        [Access("ایجاد قالب جدید")]
        public ActionResult Create()
        {
            return View();
        }
        [Access("ایجاد قالب جدید")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Template temp)
        {
            try
            {
                db.Templates.Add(temp);
                db.SaveChanges();
                return Redirect("/Admin/AdminDashboard/Index#/Admin/Template/index");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Create", "TemplateController");
                throw;
            }
            
        }
        [Access("ویرایش قالب")]
        public ActionResult Edit(int id)
        {
            ViewBag.id = id; 
            return View();
        }

        [Access("ویرایش قالب")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Template temp)
        {
            try
            {
                db.Templates.AddOrUpdate(temp);
                db.SaveChanges();
                return Redirect("/Admin/AdminDashboard/Index#/Admin/Template/index");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Edit", "TemplateController");
                throw;
            }
           
        }

        [HttpPost]
        [Access("حذف قالب")]
        public JsonResult Remove(int id)
        {
            try
            {
                db.Templates.Remove(db.Templates.Find(id));
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Remove", "TemplateController");
                throw;
            }
           
        }


    }
}