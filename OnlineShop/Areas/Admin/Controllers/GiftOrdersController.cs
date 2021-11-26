using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShop.Class;
using OnlineShop.Models;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سفارش های هدیه")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class GiftOrdersController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        [HttpPost]
        [Access("لیست سفارش های هدیه")]

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
                    var Result = fn.CreateTable("GiftOrder", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<GiftOrder>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "GiftOrdersController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;

                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }
        [Access("لیست سفارش های هدیه")]

        // GET: Admin/GiftOrders
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
        [Access("تغییر وضعیت سفارش های هدیه")]
        [ValidateAntiForgeryToken]
        public JsonResult changeStatus(bool? status, int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var goftOrder = db.GiftOrders.Find(id);
                    goftOrder.isSent = status;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "changeStatus", "GiftOrdersController");
                    throw;
                }
               
                return Json("done");

            }
            else
            {
                return Json(string.Empty);
            }
        }
        [Access("تغییر جزئیات سفارش هدیه")]

        public ActionResult Details(int? id)
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
        [Access("حدف سفارش هدیه")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.GiftOrders.Remove(db.GiftOrders.Where(r => r.id == id).FirstOrDefault());
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "GiftOrdersController");
                    throw;
                }               

            }
            return Json("success");

        }

    }
}