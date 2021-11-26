using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Class;
using Newtonsoft.Json;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت نظرات وبلاگ")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class BlogCommentsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        [Access("لیست نظرات وبلاگ")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Access("لیست نظرات وبلاگ")]
        public JsonResult List()
        {

            #region bands
            var fn = new functions();
            #endregion
            try
            {
                var Result = fn.CreateTable("Blog_Comments", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Blog_Comments>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "List", "BlogCommentsController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                throw;
            }
}
        [Access("مشاهده جزئیات نظر در وبلاگ")]

        public ActionResult detail(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [Access("حذف نظر در وبلاگ")]
        [HttpPost]
        public ActionResult remove(int? id)
        {
            try
            {
                db.Blog_Comments.Remove(db.Blog_Comments.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "remove", "BlogCommentsController");
                throw;
            }
           
        }
        [HttpPost]
        [Access("تائید نظر در وبلاگ")]
        [ValidateAntiForgeryToken]
        public JsonResult submitComment(int? id)
        {
            try
            {
                var comment = db.Blog_Comments.Find(id);
                comment.Submitted = true;
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "submitComment", "BlogCommentsController");
                throw;
            }
          
        }
        [HttpPost]
        [Access("غیرفعال سازی نظر در وبلاگ")]
        [ValidateAntiForgeryToken]
        public JsonResult deactiveComment(int? id)
        {
            try
            {
                var comment = db.Blog_Comments.Find(id);
                comment.Submitted = false;
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "deactiveComment", "BlogCommentsController");
                throw;
            }
           
        }

    }
}