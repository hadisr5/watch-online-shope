using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Sellers.Controllers
{

    public class OrdersController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginSeller ch = new checkLoginSeller();
        // GET: Sellers/Orders
        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["seller"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        public ActionResult details(int? id)
        {
            ch.checkLogin();
            if (Session["seller"] != null)
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
        [ValidateAntiForgeryToken]
        public ActionResult supply(int? id , bool? status)
        {
            ch.checkLogin();
            if (Session["seller"] != null)
            {
                try
                {
                    int sellerId = Convert.ToInt16(Session["seller"]);
                    var order = db.Orders.Where(r => r.id == id && r.SellerId == sellerId).FirstOrDefault();
                    if (order != null)
                    {

                        order.supply = status;
                        db.SaveChanges();
                    }
                    return Json("success");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "supply", "OrdersController");
                    throw;
                }
                
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
 
    }
}