using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Sellers.Controllers
{
    public class SellerDashboardController : Controller
    {
        checkLoginSeller ch = new checkLoginSeller();

        // GET: Admin/AdminDashboard
        public ActionResult Index()
        {
            ch.checkLogin();
            return View();
        }
    }
}