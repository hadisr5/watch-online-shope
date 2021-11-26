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
    public class sellerProductsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/sellerProducts
        public ActionResult Index()
        {
            if (Session["login"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }


      [ValidateAntiForgeryToken]
        public ActionResult submit (int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var product = db.Products.Find(id);
                    product.isValid = true;
                    product.submitDate = DateTime.Now;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "submit", "sellerProductsController");
                    throw;
                }
               

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

            return Json("success");
        }

        [ValidateAntiForgeryToken]
        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var product = db.Products.Find(id);

                    product.isValid = false;

                    db.SaveChanges();
                    return View("/areas/Admin/Views/SellerTickets/newTicket.cshtml", new { txt = "محصول شناسه " + product.id + " تائید نشد", sellerId = product.sellerId });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "sellerProductsController");
                    throw;
                }
               

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

            return Json("success");
        }
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
    }
}