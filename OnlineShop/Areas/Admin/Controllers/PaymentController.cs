using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Class;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class PaymentController : Controller
    {
        checkLoginAdmin ch = new checkLoginAdmin();
        OnlineShopEntities db = new OnlineShopEntities();
        // GET: Admin/Payment
        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    var setting = db.Settings.FirstOrDefault();
                    if (setting == null)
                    {
                        setting = new Setting();
                        db.Settings.Add(setting);
                        db.SaveChanges();
                    }
                    ViewBag.id = setting.id;
                    return View();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Index", "PaymentController");
                    throw;
                }
              
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }

        [ValidateAntiForgeryToken]
        public JsonResult Update(string zarinpal_merchant_id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    var setting = db.Settings.FirstOrDefault();
                    setting.zarinpal_merchant_id = zarinpal_merchant_id;
                    db.SaveChanges();
                    return Json("Success");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Update", "PaymentController");
                    throw;
                }
                
            }
            else
            {
                return Json(string.Empty);
            }
        }
    }
}