using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models; 

namespace OnlineShop.Areas.Sellers.Controllers
{
   
    public class PasswordController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginSeller ch = new checkLoginSeller();

        // GET: Sellers/Password
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult change(string password , string password2 , string prePassword) {
            ch.checkLogin();
            if (Session["seller"] != null)
            {
                try
                {
                    int sellerId = Convert.ToInt16(Session["seller"]);
                    var seller = db.Sellers.Where(r => r.id == sellerId).FirstOrDefault();
                    if (prePassword == seller.password)
                    {
                        if (password == password2)
                        {
                            if (password2.Length > 5)
                            {
                                seller.password = password;
                                db.SaveChanges();
                                return Json("success");
                            }
                            else
                            {
                                return Json(new { title = "error", desc = "پسورد جدید باید بیشتر از 5 کاراکتر باشد" });

                            }
                        }
                        else
                        {
                            return Json(new { title = "error", desc = "پسورد جدید با تکرار پسورد همخوانی ندارد" });

                        }
                    }
                    else
                    {
                        return Json(new { title = "error", desc = "پسورد قبلی صحیح نمی باشد" });

                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "change", "PasswordController");
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