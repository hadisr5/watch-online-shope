using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models;

namespace OnlineShop.Areas.Sellers.Controllers
{
    public class checkLoginSeller
    {

        OnlineShopEntities db = new OnlineShopEntities();
        public void checkLogin()
        {

            if (System.Web.HttpContext.Current.Session["seller"] == null)
            {
                try
                {
                    HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["OnlineShopSellerAuthCookie"];
                    if (authCookie != null)
                    {
                        string rnd = authCookie.Value;
                        var seller = db.Sellers.Where(r => r.token == rnd).FirstOrDefault();
                        if (seller != null)
                        {
                            System.Web.HttpContext.Current.Session["seller"] = seller.id;
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "checkLogin", "checkLoginSeller");
                    throw;
                }
                
            }

        }

    }
}