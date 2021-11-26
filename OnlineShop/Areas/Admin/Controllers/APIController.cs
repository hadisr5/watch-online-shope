using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using System.ComponentModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class APIController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        public ActionResult product(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var pro = db.Products.Find(id);
                return Json(pro, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public ActionResult Blog_post(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var pro = db.Blog_post.Find(id);
                return Json(pro, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }

        public JsonResult Blog_Cats(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var Obj = db.Blog_Cats.Find(id);
                return Json(Obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public JsonResult TotalDiscountCodes(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var count = db.DiscountCodes.Where(r => r.GroupId == id).ToList();
                return Json(count.Count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public JsonResult UsedDiscountCodes(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var count = db.DiscountCodes.Where(r => r.GroupId == id && r.isUsed == true).ToList();
                return Json(count.Count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public JsonResult NotUsedDiscountCodes(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var count = db.DiscountCodes.Where(r => r.GroupId == id && r.isUsed != true).ToList();
                return Json(count.Count, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public JsonResult user(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var Obj = db.Users.Find(id);
                return Json(Obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public JsonResult menu(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var Obj = db.Menus.Find(id);
                return Json(Obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public JsonResult seller(int id)
        {
            if (LoginCheck())
            {
                LoginCheck();
                db.Configuration.ProxyCreationEnabled = false;
                var Obj = db.Sellers.Find(id);
                return Json(Obj, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public Boolean LoginCheck()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public JsonResult newOrders()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                int count = db.Baskets.Where(r => r.wizardStatus == "در صف بررسی").ToList().Count();
                return Json(count);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult newOrdersList(int count)
        {
            var baskets = db.Baskets.OrderByDescending(r => r.id).Take(count).ToList();
            if (baskets.Count != 0)
            {
                var notifyList = new List<Notifications_UI>();
                foreach (var item in baskets)
                {
                    var notify = new Notifications_UI();
                    notify.title = "سفارش جدید";
                    notify.text = "سفارش شناسه " + item.id + " ثبت شد.";
                    notify.type = "success";
                    notify.time = Convert.ToDateTime(item.creationDate);
                    notify.id = item.id;
                    notify.link = "/Admin/AdminDashboard/Index#/admin/orders/details/" + item.id;
                    notifyList.Add(notify);
                }
                return Json(notifyList);
            }
            else
            {
                return Json(string.Empty);
            }
        }


        public JsonResult NewTicket_List(int count)
        {
            var tickets = db.Tickets.Where(r => r.status != "پاسخ داده شده").OrderByDescending(r => r.id).Take(count).ToList();
            if (tickets.Count != 0)
            {
                var notifyList = new List<Notifications_UI>();
                foreach (var item in tickets)
                {
                    var chat = db.TicketChats.Where(r => r.ticketId == item.id).OrderByDescending(r => r.id).FirstOrDefault();
                    string text = chat.text;
                    if (text.Length > 40)
                    {
                        text = text.Substring(0, 49) + " ...";
                    }

                    var notify = new Notifications_UI();
                    notify.title = "تیکت";
                    notify.text = text;
                    notify.type = "warning";
                    notify.time = Convert.ToDateTime(item.creationDate);
                    notify.id = item.id;
                    notify.link = "/Admin/AdminDashboard/Index#/admin/Tickets/Chat/" + item.id;
                    notifyList.Add(notify);
                }
                return Json(notifyList);
            }
            else
            {
                return Json(string.Empty);
            }
        }

        public JsonResult NewTickets()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                int count = db.Tickets.Where(r => r.status != "پاسخ داده شده").ToList().Count();
                return Json(count);
            }
            else
            {
                return Json(0);
            }
        }


        public JsonResult newComments()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                int count = db.Comments.Where(r => r.show != true).ToList().Count();
                return Json(count);
            }
            else
            {
                return Json(0);
            }
        }
        public JsonResult newComment_list(int count)
        {
            var comments = db.Comments.Where(r => r.show != true).OrderByDescending(r => r.id).Take(count).ToList();
            if (comments.Count != 0)
            {
                var notifyList = new List<Notifications_UI>();
                foreach (var item in comments)
                {
                    string text = item.text;
                    if (text.Length > 40)
                    {
                        text = text.Substring(0, 49) + " ...";
                    }

                    var notify = new Notifications_UI();
                    notify.title = "کامنت";
                    notify.text = text;
                    notify.type = "info";
                    notify.time = Convert.ToDateTime(item.creationDate);
                    notify.id = item.id;
                    notify.link = "/Admin/AdminDashboard/Index#/admin/Comments/detail/" + item.id;
                    notifyList.Add(notify);
                }
                return Json(notifyList);
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [HttpPost]
        public JsonResult editProperty(int id, bool MainProperty, bool showInFilter, int? priority, string title)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                var property = db.CatProperties.Find(id);
                property.title = title;
                property.showInFilter = showInFilter;
                property.MainProperty = MainProperty;
                property.priority = priority;
                db.SaveChanges();
                return Json("Success");
            }
            else
            {
                return Json(string.Empty);
            }


        }

        public class Notifications_UI
        {
            public int id { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public string text { get; set; }
            public string link { get; set; }

            public DateTime time { get; set; }
        }



    }
}