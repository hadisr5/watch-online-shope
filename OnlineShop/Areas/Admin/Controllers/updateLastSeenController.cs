using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class updateLastSeenController : Controller
    {
        checkLoginAdmin ch = new checkLoginAdmin();
        OnlineShopEntities db = new OnlineShopEntities();
        [HttpPost]
        public JsonResult Tick() {
            ch.checkLogin();
            if (Session["login"]!=null)
            {
                try
                {
                    //int id = Convert.ToInt32(Session["login"]);
                    //var admin = db.AdminUsers.Find(id);
                    //admin.lastOnline = DateTime.Now;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Tick", "updateLastSeenController");
                    throw;
                }
               
            }
            return Json(string.Empty);  
        }
    }
}