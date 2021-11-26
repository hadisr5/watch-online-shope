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
    [DisplayName("مدیریت پیام های تماس باما")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class ContactController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        [Access("لیست پیام های تماس باما")]

        // GET: Admin/Contact
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

        [HttpPost]
        [Access("لیست پیام های تماس باما")]
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
                    var Result = fn.CreateTable("Contact", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Contact>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "ContactController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); 
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }
        [Access("حذف پیام های تماس باما")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Contacts.Remove(db.Contacts.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "ContactController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [Access("جزئیات پیام های تماس باما")]

        public ActionResult Detail(int? id)
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
    }
}