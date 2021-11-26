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
    [DisplayName("مدیریت منو ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class MenusController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        // GET: Admin/Menus
        [Access("لیست منو ها")]
        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["login"]!=null)
            {
            return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home" , new { area = "" });
            }
        }
        [Access("لیست منو ها")]
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
                var Result = fn.CreateTable("Menus", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Menu>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "MenusController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;

                throw;
            }
        }
        [Access("ایجاد منو ها")]
        [HttpGet]
        public ActionResult Create()
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [HttpPost]
        [Access("ایجاد منو ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Menu(Menu menu)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Menus.Add(menu);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Create_Menu", "MenusController");
                    throw;
                }
               
            }

            return Json(new { success = "منو " + menu.title + " اضافه شد" });
        }
        [HttpPost]
        [Access("حذف منو ها")]
        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Menus.Remove(db.Menus.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "MenusController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [Access("ویرایش منو ها")]
        public ActionResult Edit(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                ViewBag.id = id;
                return View();
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

        }
        [Access("ویرایش منو ها")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(Menu menu, int id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Menus.AddOrUpdate(menu);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Edit_Post", "MenusController");
                    throw;
                }
               
            }
            return Json("success");
        }
    }
}