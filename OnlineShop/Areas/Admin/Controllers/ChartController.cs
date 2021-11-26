using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Class;
using OnlineShop.Models;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت نمودار ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class ChartController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        [Access("لیست نمودار ها")]
        public ActionResult Index()
        {
            if (Session["login"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [Access("نمودار سفارش ها")]
        public JsonResult orders(DateTime begin, DateTime end)
        {
            if (Session["login"] != null)
            {
                try
                {
                    var baskets = db.Baskets.Where(r => r.creationDate >= begin && r.creationDate <= end).ToList();
                    double days = (end - begin).TotalDays;
                    var labels = new List<string>();
                    var data = new List<int>();

                    for (int i = 0; i < days; i++)
                    {
                        var dt = begin.AddDays(i);
                        var tommorow = dt.AddDays(1);
                        PersianCalendar pc = new PersianCalendar();
                        string label = pc.GetMonth(dt).ToString() + "/" + pc.GetDayOfMonth(dt);
                        int dataInDay = baskets.Where(r => r.creationDate >= dt && r.creationDate <= tommorow).ToList().Count;

                        labels.Add(label);
                        data.Add(dataInDay);
                    }


                    return Json(new { data = data, label = labels });
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "orders", "CategoriesController");
                    throw;
                }
               
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [Access("نمودار کاربر ها")]
        public JsonResult Users(DateTime begin, DateTime end)
        {
            if (Session["login"] != null)
            {
                try
                {
                    var Users = db.Users.Where(r => r.CreationDate >= begin && r.CreationDate <= end).ToList();
                    double days = (end - begin).TotalDays;
                    var labels = new List<string>();
                    var data = new List<int>();

                    for (int i = 0; i < days; i++)
                    {
                        var dt = begin.AddDays(i);
                        var tommorow = dt.AddDays(1);
                        PersianCalendar pc = new PersianCalendar();
                        string label = pc.GetMonth(dt).ToString() + "/" + pc.GetDayOfMonth(dt);
                        int dataInDay = Users.Where(r => r.CreationDate >= dt && r.CreationDate <= tommorow).ToList().Count;

                        labels.Add(label);
                        data.Add(dataInDay);
                    }


                    return Json(new { data = data, label = labels });
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Users", "CategoriesController");
                    throw;
                }
              
            }
            else
            {
                return Json(string.Empty);
            }
        }
    }
}