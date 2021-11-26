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
    public class walletController : Controller
    {
        checkLoginAdmin ch = new checkLoginAdmin();
        OnlineShopEntities db = new OnlineShopEntities();

        [ValidateAntiForgeryToken]
        // GET: Admin/wallet
        public ActionResult pay(int? id)
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
        [ValidateAntiForgeryToken]
        public ActionResult pay(int? id , long amount , string desc)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    var wallet = new Wallet();
                    wallet.amount = amount;
                    wallet.userId = id;
                    wallet.desc = desc;
                    wallet.payByAdmin = true;
                    wallet.creationDate = DateTime.Now;
                    db.Wallets.Add(wallet);
                    db.SaveChanges();

                    return Json("success");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "pay", "walletController");
                    throw;
                }              

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

        }
        public ActionResult Payed(int? id) {
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
        public ActionResult payList(int? id)
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