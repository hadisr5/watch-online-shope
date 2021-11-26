using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Sellers.Controllers
{
    public class StatisticsController : Controller
    {
  
        [ValidateAntiForgeryToken]
        public ActionResult Index(string start , string end)
        {
            if (!string.IsNullOrWhiteSpace(start))
            {
                if (true)
                {
                    string dt = start;
                    int year = Convert.ToInt16(dt.Substring(0, 4));
                    int month = Convert.ToInt16(dt.Substring(5, 2));
                    int day = Convert.ToInt16(dt.Substring(8, 2));
                    PersianCalendar pc = new PersianCalendar();
                    DateTime gt = new DateTime(year, month, day, pc);
                    ViewBag.start = gt;
                }
                if (true)
                {
                    string dt = end;
                    int year = Convert.ToInt16(dt.Substring(0, 4));
                    int month = Convert.ToInt16(dt.Substring(5, 2));
                    int day = Convert.ToInt16(dt.Substring(8, 2));
                    PersianCalendar pc = new PersianCalendar();
                    DateTime gt = new DateTime(year, month, day, pc);
                    ViewBag.end = end;
                }
              
            }

            return View(); 
        }

    }
}