using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class sellerCommentsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

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
        public ActionResult detail(int? id)
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
        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.SellerComments.Remove(db.SellerComments.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "sellerCommentsController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult submitComment(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var comment = db.SellerComments.Find(id);
                    comment.show = true;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "submitComment", "sellerCommentsController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult deactiveComment(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var comment = db.SellerComments.Find(id);
                    comment.show = false;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "deactiveComment", "sellerCommentsController");
                    throw;
                }
               
            }
            return Json("success");
        }
    }
}