using Newtonsoft.Json;
using OnlineShop.Areas.Admin.Security;
using OnlineShop.Class;
using OnlineShop.Models;
using OnlineShop.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت پرسش ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class ProductQuestionsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        [Access("لیست پرسش ها")]

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Access("لیست پرسش ها")]

        public JsonResult List()
        {

            #region bands
            var fn = new functions();
            //var bans = new List<string>();
            //bans.Add("smsToken");
            #endregion
            try
            {
                var Result = fn.CreateTable("ProductQuestions", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<ProductQuestion>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "ProductQuestionsController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                throw;
            }
        }

        [Access("جزئیات پرسش ها")]

        public ActionResult detail(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [Access("حذف پرسش ها")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult remove(int? id)
        {
            try
            {
                db.ProductQuestions.Remove(db.ProductQuestions.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "remove", "ProductQuestionsController");
                throw;
            }          

        }
        [HttpPost]
        [Access("تائید پرسش ها")]
        [ValidateAntiForgeryToken]
        public JsonResult submitQuestion(int? id)
        {
            try
            {
                var q = db.ProductQuestions.Find(id);
                q.IsValidate = true;
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "submitQuestion", "ProductQuestionsController");
                throw;
            }
           

        }
        [HttpPost]
        [Access("غیرفعال سازی پرسش ها")]
        [ValidateAntiForgeryToken]
        public JsonResult deactiveQuestion(int? id)
        {
            try
            {
                var q = db.ProductQuestions.Find(id);
                q.IsValidate = false;
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "deactiveQuestion", "ProductQuestionsController");
                throw;
            }
            
        }
        [Access("حذف پرسش ها")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult removeanswer(int? id)
        {
            try
            {
                db.ProductQAnswers.Remove(db.ProductQAnswers.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "removeanswer", "ProductQuestionsController");
                throw;
            }           

        }
        [HttpPost]
        [Access("تائید پرسش ها")]
        [ValidateAntiForgeryToken]
        public JsonResult submitanswer(int? id)
        {
            try
            {
                var q = db.ProductQAnswers.Find(id);
                q.IsValidate = true;
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "submitanswer", "ProductQuestionsController");
                throw;
            }
           

        }
        [HttpPost]
        [Access("غیرفعال سازی پرسش ها")]
        [ValidateAntiForgeryToken]
        public JsonResult deactiveanswer(int? id)
        {
            try
            {
                var q = db.ProductQAnswers.Find(id);
                q.IsValidate = false;
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "deactiveanswer", "ProductQuestionsController");
                throw;
            }
           
        }
    }
}
