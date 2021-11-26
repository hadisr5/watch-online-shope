using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Design;
using OnlineShop.Models;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("جستجو پنل")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class Quick_searchController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("اجازه جستجو در پنل")]
        public ActionResult Index(string query)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                ViewBag.query = query; 
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
    }
}