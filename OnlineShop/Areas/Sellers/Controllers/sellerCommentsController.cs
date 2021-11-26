using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Areas.Sellers.Controllers
{
    public class sellerCommentsController : Controller
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
        public ActionResult detail(int? id)
        {
            ch.checkLogin();

            if (Session["seller"] != null)
            {
                int userird = Convert.ToInt16(Session["seller"]);
                if (db.SellerComments.Where(r => r.sellerId == userird && r.id == id).FirstOrDefault() != null)
                {
                    ViewBag.id = id;
                    return View();
                }
                else
                {
                    return RedirectToAction("NotFound", "Home", new { area = "" });

                }

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

        }


    }
}