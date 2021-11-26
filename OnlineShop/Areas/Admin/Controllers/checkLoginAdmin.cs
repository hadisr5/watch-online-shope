using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineShop.Models; 

namespace OnlineShop.Areas.Admin.Controllers
{
    public class checkLoginAdmin
    {

        OnlineShopEntities db = new OnlineShopEntities();
        public void checkLogin()
        {

            if (System.Web.HttpContext.Current.Session["login"] == null)
            {
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["OnlineShopAdminAuthCookie"];
                if (authCookie != null)
                {
                    string rnd = authCookie.Value;
                    var user = db.AdminUsers.Where(r => r.token == rnd).FirstOrDefault();
                    if (user != null)
                    {
                        System.Web.HttpContext.Current.Session["login"] = user.id;
                    }
                }
            }

        }

    }
}