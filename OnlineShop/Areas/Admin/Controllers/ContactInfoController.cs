using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت اطلاعات تماس")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class ContactInfoController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/ContactInfo
        [Access("ویرایش اطلاعات تماس")]

        public ActionResult Edit()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    var ci = db.ContactInformations.FirstOrDefault();
                    if (ci != null)
                    {
                        ViewBag.id = ci.id;
                        return View();
                    }
                    else
                    {
                        var newCi = new ContactInformation();
                        db.ContactInformations.Add(newCi);
                        db.SaveChanges();
                        ViewBag.id = newCi.id;
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit", "ContactInfoController");
                    throw;
                }
               
              
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

        }
        [HttpPost]
        [Access("ویرایش اطلاعات تماس")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(ContactInformation ci, int id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.ContactInformations.AddOrUpdate(ci);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_Post", "ContactInfoController");
                    throw;
                }
               
            }
            return Json("success");
        }
    }
}