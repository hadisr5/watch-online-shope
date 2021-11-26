using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class PromotionsController : Controller
    {
        // GET: Promotions
        public ActionResult Index(int id , string title)
        {
            ViewBag.id = id; 
            return View();
        }
    }
}