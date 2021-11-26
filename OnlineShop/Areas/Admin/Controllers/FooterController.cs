using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;


using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت فوتر سایت")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class FooterController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        // GET: Admin/Footer
        [Access("ویرایش فوتر سایت")]

        public ActionResult Edit()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    var ci = db.Footers.FirstOrDefault();
                    if (ci != null)
                    {
                        ViewBag.id = ci.id;
                        return View();
                    }
                    else
                    {
                        var newCi = new Footer();
                        db.Footers.Add(newCi);
                        db.SaveChanges();
                        ViewBag.id = newCi.id;
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit", "FooterController");
                    throw;
                }
                

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

        }

        [HttpPost]
        [Access("ویرایش فوتر سایت")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(Footer ci)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Footers.AddOrUpdate(ci);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_Post", "FooterController");
                    throw;
                }
               
            }
            return Json("success");
        }

    }
}