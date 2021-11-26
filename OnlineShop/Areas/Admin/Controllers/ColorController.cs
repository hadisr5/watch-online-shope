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
    [DisplayName("مدیریت رنگ ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class ColorController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        [HttpPost]

        public JsonResult getColors()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return Json(db.Colors.ToList());
        }
        // GET: Admin/Menus
        [Access("لیست رنگ ها")]
        public ActionResult Index()
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
        [Access("لیست رنگ ها")]

        public JsonResult List()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                #region bands
                var fn = new functions();
                //var bans = new List<string>();
                //bans.Add("smsToken");
                #endregion
                try
                {
                    var Result = fn.CreateTable("Color", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Color>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "ColorController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }
        [HttpGet]
        [Access("ایجاد رنگ جدید")]

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
        [Access("ایجاد رنگ جدید")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_color(Color color)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Colors.Add(color);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Create_color", "ColorController");
                    throw;
                }
               
            }

            return Json(new { success = "رنگ " + color.title + " اضافه شد" });
        }
        [HttpPost]
        [Access("حذف رنگ")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Colors.Remove(db.Colors.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "ColorController");
                    throw;
                }
                
            }
            return Json("success");

        }
        [Access("ویرایش رنگ ها")]

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
        [HttpPost]
        [Access("ویرایش رنگ ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(Color color, int id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Colors.AddOrUpdate(color);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "ColorController");
                    throw;
                }
               
            }
            return Json("success");
        }
    }
}