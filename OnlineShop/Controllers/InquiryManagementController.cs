using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;

using System.Data.Entity.Migrations;
using OnlineShop.Class;
using Newtonsoft.Json;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;
using System.Configuration;

namespace OnlineShop.Areas.Admin.Controllers
{
    
    public class InquiryManagementController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        
        public ActionResult Index()
        {
            
            return View();

        }



        [HttpPost]
        public JsonResult List()
        {
            
            if (Session["login"] != null)
            {
                #region bands
                var fn = new functions();
                //var bans = new List<string>();
                //bans.Add("smsToken");
                #endregion
                try
                {
                    var Result = fn.CreateTable("InquiryManagement", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<InquiryManagement>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "InquiryManagementController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }
            
        }

        public ActionResult Add(int? id)
        {
            try
            {
                db.Tickets.Add(db.Tickets.Find(id));
                db.TicketChats.AddRange(db.TicketChats.Where(r => r.ticketId == id).ToList());
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Add", "InquiryManagementController");
                throw;
            }


        }
    }
}