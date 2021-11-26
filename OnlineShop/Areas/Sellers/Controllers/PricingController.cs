using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models; 

namespace OnlineShop.Areas.Sellers.Controllers
{
    public class PricingController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginSeller ch = new checkLoginSeller();
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
        public ActionResult edit(int? id)
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
        public ActionResult edit(int? price , int? Delivery , int id , string Warranty , bool? isActive)
        {
            ch.checkLogin();
            if (Session["seller"] != null)
            {
                if (price == null || Delivery == null )
                {
                    return Json(new { title = "error", desc = "قیمت محصول و تاریخ تحویل نمی تواند خالی باشد" });

                }
                try
                {
                    int sellerId = Convert.ToInt16(Session["seller"]);

                    var sellerPrice = new SellersProduct();
                    if (db.SellersProducts.Where(r => r.sellerId == sellerId && r.productId == id).FirstOrDefault() != null)
                    {
                        sellerPrice = db.SellersProducts.Where(r => r.sellerId == sellerId && r.productId == id).FirstOrDefault();
                    }
                    sellerPrice.productId = id;
                    if (isActive != null && isActive == true)
                    {
                        sellerPrice.isActive = true;
                    }
                    else
                    {
                        sellerPrice.isActive = false;
                    }
                    sellerPrice.sellerId = sellerId;
                    sellerPrice.price = Convert.ToInt32(price);
                    sellerPrice.Delivery = Convert.ToInt32(Delivery);
                    sellerPrice.Warranty = Warranty;
                    if (db.SellersProducts.Where(r => r.sellerId == sellerId && r.productId == id).FirstOrDefault() == null)
                    {
                        db.SellersProducts.Add(sellerPrice);

                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "edit", "PricingController");
                    throw;
                }


                return Json("success");
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
    }
}