using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShop.Class;
using OnlineShop.Models;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Areas.Admin.Functions;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سفارش ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class OrdersController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("لیست سفارش ها")]
        public ActionResult Index()
        {
            return View();
        }
        [Access("تغییر آدرس سفارش")]
        [HttpPost]
        public JsonResult changeAddress(UsersAdress ua)
        {
            try
            {
                db.UsersAdresses.AddOrUpdate(ua);
                db.SaveChanges();
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "changeAddress", "OrdersController");
                throw;
            }
            
        }
        [Access("تغییر وضعیت سفارش")]
        [ValidateAntiForgeryToken]
        public JsonResult changeStatus(string status , int? id) {

            try
            {
                var basket = db.Baskets.Find(id);
                var user = db.Users.Where(r => r.id == basket.userId).FirstOrDefault();
                var setting = db.Settings.FirstOrDefault();

                basket.wizardStatus = status;
                SMS sms = new SMS();
                sms.Send(user.mobile, "سفارش:" + basket.id + "-وضعیت:" + status, Convert.ToInt32(setting.smsToken_changeStatusOfOrder));
                db.SaveChanges();
                return Json("done");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "changeStatus", "OrdersController");
                throw;
            }
           
        }

        [Access("جزئیات سفارش")]

        public ActionResult Details(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [Access("حذف سفارش")]
        public ActionResult remove(int? id)
        {

            try
            {
                db.Baskets.Remove(db.Baskets.Find(id));
                db.Orders.RemoveRange(db.Orders.Where(r => r.basketId == id).ToList());
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "remove", "OrdersController");
                throw;
            }
            db.SaveChanges();
            return Json("success");

        }

        [HttpPost]
        [Access("لیست سفارش ها")]
        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            //var bans = new List<string>();
            //bans.Add("smsToken");
            #endregion
            try
            {
                var Result = fn.CreateTable("Baskets", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Basket>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "OrdersController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                throw;
            }

        }

        public PartialViewResult Export()
        {
            return PartialView();
        }

    }
}