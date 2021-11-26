using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Controllers
{
    public class FormController : Controller
    {
        // GET: Form
        public ActionResult Index(int id ,string title)
        {
            ViewBag.id = id;
            return View();
        }
    }
}