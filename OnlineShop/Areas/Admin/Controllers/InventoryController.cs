using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Class;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت انبارداری")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class InventoryController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/Inventory
        [Access("لیست موجودی انبار")]

        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {

                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }

        [Access("تغییر موجودی انبار")]

        public JsonResult isAvailable(int id, bool check)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    var product = db.Products.Where(r => r.id == id).FirstOrDefault();
                    product.isAvailable = check;
                    db.SaveChanges();
                    return Json(string.Empty);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "isAvailable", "InventoryController");
                    throw;
                }
              
            }
            else
            {
                return Json(string.Empty);
            }

        }

        [Access("تغییر موجودی انبار")]

        public JsonResult update(int id, int inventory)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    var product = db.Products.Where(r => r.id == id).FirstOrDefault();
                    product.Inventory = inventory;
                    db.SaveChanges();
                    return Json(string.Empty);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "update", "InventoryController");
                    throw;
                }
               
            }
            else
            {
                return Json(string.Empty);
            }

        }
        [Access("فعال/غیرفعال سازی محصول")]

        public JsonResult Active(int id, bool check, bool uncheck)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {

                    if (check)
                    {
                        var product = db.Products.Where(r => r.id == id).FirstOrDefault();
                        product.Active = true;
                        db.SaveChanges();
                        return Json(string.Empty);
                    }
                    if (uncheck)
                    {
                        var product = db.Products.Where(r => r.id == id).FirstOrDefault();
                        product.Active = false;
                        db.SaveChanges();
                        return Json(string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Active", "InventoryController");
                    throw;
                }
              
                return Json(string.Empty);
            }
            else
            {
                return Json(string.Empty);
            }

        }

    }
}