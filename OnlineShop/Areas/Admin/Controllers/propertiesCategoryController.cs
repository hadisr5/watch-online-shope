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
    [DisplayName("مدیریت دسته بندی ویژگی ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class propertiesCategoryController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/propertiesCategory
        [Access("لیست دسته بندی ویژگی ها")]
        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [HttpPost]
        [Access("لیست دسته بندی ویژگی ها")]
        public JsonResult List()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                #region bands
                var fn = new functions();
                #endregion
                try
                {
                    var Result = fn.CreateTable("propertiesCategory", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<propertiesCategory>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "List", "propertiesCategoryController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }
            }
            else
            {
                return Json(string.Empty);
            }

        }
        [Access("ایجاد بندی ویژگی ها")]
        public ActionResult Create()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [Access("ایجاد بندی ویژگی ها")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Menu(propertiesCategory pc)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.propertiesCategories.Add(pc);
                    db.SaveChanges();
                    return Redirect("/Admin/AdminDashboard/Index#/Admin/propertiesCategory/index");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Create_Menu", "propertiesCategoryController");
                    throw;
                }
               
            }
            else
            {
                return Redirect("/Admin/login");
            }

        }
        [HttpPost]
        [Access("حذف بندی ویژگی ها")]
        public JsonResult remove(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.propertiesCategories.Remove(db.propertiesCategories.Find(id));
                    db.SaveChanges();
                    return Json("Success");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "propertiesCategoryController");
                    throw;
                }
              
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [Access("ویرایش بندی ویژگی ها")]
        public ActionResult Edit(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                ViewBag.id = id;
                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [HttpPost]
        [Access("ویرایش بندی ویژگی ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(propertiesCategory pc)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.propertiesCategories.AddOrUpdate(pc);
                    db.SaveChanges();
                    return Redirect("/Admin/AdminDashboard/Index#/Admin/propertiesCategory/index");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Edit", "propertiesCategoryController");
                    throw;
                }
              
            }
            else
            {
                return Redirect("/Admin/login");
            }
        }
    }
}