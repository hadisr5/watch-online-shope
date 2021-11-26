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
    [DisplayName("مدیریت نظرات کاربران")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class CommentsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        [Access("لیست نظرات کاربران")]

        public ActionResult Index()
        {
                return View();
        }
        [HttpPost]
        [Access("لیست نظرات کاربران")]

        public JsonResult List()
        {
         
                #region bands
                var fn = new functions();
                //var bans = new List<string>();
                //bans.Add("smsToken");
                #endregion
                try
                {
                    var Result = fn.CreateTable("Comments", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Comment>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "CommentsController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                throw;
                }
}

        [Access("جزئیات نظرات کاربران")]

        public ActionResult detail(int? id)
        {
                ViewBag.id = id;
                return View();
        }
        [Access("حذف نظر کاربران")]
        [HttpPost]
        public ActionResult remove(int? id)
        {
            try
            {
                db.Comments.Remove(db.Comments.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "remove", "CommentsController");
                throw;
            }
           

        }
        [HttpPost]
        [Access("تائید نظر کاربران")]
        [ValidateAntiForgeryToken]
        public JsonResult submitComment(int? id)
        {
            try
            {
                var comment = db.Comments.Find(id);
                comment.show = true;
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "submitComment", "CommentsController");
                throw;
            }
            

        }
        [HttpPost]
        [Access("غیرفعال سازی نظر کاربران")]
        [ValidateAntiForgeryToken]
        public JsonResult deactiveComment(int? id)
        {
            try
            {
                var comment = db.Comments.Find(id);
                comment.show = false;
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "deactiveComment", "CommentsController");
                throw;
            }
           
        }

    }
}