using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
    [DisplayName("مدیریت گروه های امتیاز دهی")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class RattingGroupsController : Controller
    {
        checkLoginAdmin ch = new checkLoginAdmin();
        OnlineShopEntities db = new OnlineShopEntities();

        [Access("لیست گروه های امتیاز دهی")]
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
        [Access("لیست گروه های امتیاز دهی")]
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
                    var Result = fn.CreateTable("RatingGroup", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<RatingGroup>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }

                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "List", "RattingGroupsController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }
        [Access("ایجاد گروه های امتیاز دهی")]
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
        [Access("ایجاد گروه های امتیاز دهی")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RatingGroup obj)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.RatingGroups.Add(obj);
                    db.SaveChanges();
                    return Redirect("/Admin/AdminDashboard/Index#/Admin/ratinggroups/index");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Create", "RattingGroupsController");
                    throw;
                }
               
            }
            else
            {
                return Redirect("/Admin/login");
            }
            

        }
        [Access("ویرایش گروه های امتیاز دهی")]
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
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [HttpPost]
        [Access("ویرایش گروه های امتیاز دهی")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RatingGroup obj)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.RatingGroups.AddOrUpdate(obj);
                    db.SaveChanges();

                    return Redirect("/Admin/AdminDashboard/Index#/Admin/rattinggroups/index");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Edit", "RattingGroupsController");
                    throw;
                }
               
            }
            else
            {
                return Redirect("/Admin/login");
            }
        }
        [HttpPost]
        public JsonResult addRatting(string title , int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    var ratting = new Rating();
                    ratting.groupName = id;
                    ratting.title = title;
                    db.Ratings.Add(ratting);
                    db.SaveChanges();

                    return Json("");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "addRatting", "RattingGroupsController");
                    throw;
                }
               
            }
            else
            {
                return Json("");
            }

        }
        [HttpPost]
        public JsonResult removeRatting(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.Ratings.Remove(db.Ratings.Find(id));
                    db.SaveChanges();
                    return Json("");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "removeRatting", "RattingGroupsController");
                    throw;
                }
               
            }
            else
            {
                return Json("");
            }
        }
        [HttpPost]
        public JsonResult remove(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.RatingGroups.Remove(db.RatingGroups.Find(id));
                    db.SaveChanges();
                    return Json("");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "RattingGroupsController");
                    throw;
                }
                
            }
            else
            {
                return Json("");
            }
        }
    }
}