using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models; 
namespace OnlineShop.Areas.Sellers.Controllers
{
    public class ProfileController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginSeller ch = new checkLoginSeller();
        // GET: Sellers/Profile
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult edit(string shomareKart ,  string name , string lastname , string sheba , string codeMeli , string email , string phone , string img)
        {
            ch.checkLogin();
            if (Session["seller"] != null)
            {
                try
                {
                    int id = Convert.ToInt16(Session["seller"]);
                    var seller = db.Sellers.Where(r => r.id == id).FirstOrDefault();
                    seller.name = name;
                    seller.img = img;
                    seller.lastname = lastname;
                    seller.sheba = sheba;
                    seller.codeMeli = codeMeli;
                    seller.email = email;
                    seller.phone = phone;
                    seller.shomareKart = shomareKart;
                    db.SaveChanges();
                    return Json("");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "edit", "ProfileController");
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