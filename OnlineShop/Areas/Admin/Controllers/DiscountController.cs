using Newtonsoft.Json;
using OnlineShop.Class;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت تخفیف ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class DiscountController : Controller
    {
        // GET: Admin/Discount
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        [HttpPost]
        [Access("لیست تخفیف ها")]

        public JsonResult List()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                #region bands
                var fn = new functions();
                //var bans = new List<string>();
                //bans.Add("smsToken");
                #endregion
                try
                {
                    var Result = fn.CreateTable("Discount", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Discount>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "DiscountController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }
        [Access("لیست تخفیف ها")]

        // GET: Admin/Discounts
        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [HttpGet]
        [Access("ایجاد تخفیف")]

        public ActionResult Create()
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [HttpPost]
        [Access("ایجاد تخفیف")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Post(Discount discount , string exp)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                if (exp == null || string.IsNullOrEmpty(exp))
                {
                }
                else
                {
                    try
                    {
                        string dt = exp;
                        int year = Convert.ToInt16(dt.Substring(0, 4));
                        int month = Convert.ToInt16(dt.Substring(5, 2));
                        int day = Convert.ToInt16(dt.Substring(8, 2));
                        PersianCalendar pc = new PersianCalendar();
                        DateTime gt = new DateTime(year, month, day, pc);
                        discount.expire = gt;
                    }
                    catch (Exception ex)
                    {
                        Class.Utility.CreateLog(ex, "Create_Post", "DiscountController");
                        throw;
                    }
                }
                try
                {
                    db.Discounts.Add(discount);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Create_Post", "DiscountController");
                    throw;
                }
             
            }

            return Json(new { success = "منو " + discount.title + " اضافه شد" });
        }
        [HttpPost]
        [Access("حذف تخفیف")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Discounts.Remove(db.Discounts.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "DiscountController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [Access("ویرایش تخفیف")]

        public ActionResult Edit(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                ViewBag.id = id;
                return View();
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

        }
        [HttpPost]
        [Access("ویرایش تخفیف")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(Discount discount, int id , string exp)
        {
            ch.checkLogin();

            if (exp == null || string.IsNullOrEmpty(exp))
            {
            }
            else
            {
                try
                {
                    string dt = exp;
                    int year = Convert.ToInt16(dt.Substring(0, 4));
                    int month = Convert.ToInt16(dt.Substring(5, 2));
                    int day = Convert.ToInt16(dt.Substring(8, 2));
                    PersianCalendar pc = new PersianCalendar();
                    DateTime gt = new DateTime(year, month, day, pc);
                    discount.expire = gt;
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_Post", "DiscountController");
                    throw;
                }
            }
            if (Session["login"] != null)
            {
                try
                {
                    db.Discounts.AddOrUpdate(discount);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_Post", "DiscountController");
                    throw;
                }
               
            }
            return Json("success");
        }
    }
}