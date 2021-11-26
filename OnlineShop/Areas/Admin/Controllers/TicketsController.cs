using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using SmsIrRestful;
using System.Configuration;
using OnlineShop.Areas.Admin.Functions;
using OnlineShop.Class;
using Newtonsoft.Json;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت تیکت ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class TicketsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("حذف تیکت ها")]
        [HttpPost]
        public ActionResult remove(int? id)
        {
            try
            {
                db.Tickets.Remove(db.Tickets.Find(id));
                db.TicketChats.RemoveRange(db.TicketChats.Where(r => r.ticketId == id).ToList());
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "remove", "TicketsController");
                throw;
            }
           

        }
        [Access("لیست تیکت ها")]
        [HttpPost]
        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            //var bans = new List<string>();
            //bans.Add("smsToken");
            #endregion
            try
            {
                var Result = fn.CreateTable("Tickets", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Ticket>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "TicketsController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
            }
        }
        [Access("لیست تیکت ها")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Access("پاسخ دهی به تیکت ها")]
        [ValidateAntiForgeryToken]
        public JsonResult reply(TicketChat cht)
        {
            try
            {
                var ticket = db.Tickets.Where(r => r.id == cht.ticketId).FirstOrDefault();
                if (ticket != null)
                {
                    var user = db.Users.Where(r => r.id == ticket.userid).FirstOrDefault();
                    if (user != null)
                    {
                        ticket.status = "پاسخ داده شده";
                        cht.creationDate = DateTime.Now;
                        cht.isUser = false;
                        db.TicketChats.Add(cht);
                        db.SaveChanges();
                        #region Send SMS to user
                        SMS sms = new SMS();
                        var setting = db.Settings.FirstOrDefault();

                        sms.Send(user.mobile, ticket.id.ToString(), Convert.ToInt32(setting.smsToken_TicketAnswer));
                        #endregion
                    }

                }

                return Json("done");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "reply", "TicketsController");
                throw;
            }
            
        }
        [Access("ارسال voice در جواب تیکت ها")]
        public JsonResult voice(int? id) {
            var chat = new TicketChat();
            string fileUrl = "";
            try
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    string dateString = DateTime.Now.ToString();
                    while (dateString.Contains(":") || dateString.Contains("/"))
                    {
                        dateString = dateString.Replace(":", "");
                        dateString = dateString.Replace("/", "");

                    }
                    fileUrl = dateString + ".wav";
                    db.SaveChanges();
                    System.IO.Stream fileContent = file.InputStream;
                    //To save file, use SaveAs method
                    //upload.url = fileUrl;
                    //db.Uploads.Add(upload);
                    chat.voice = fileUrl;
                    var ticket = db.Tickets.Find(id);
                    var user = db.Users.Where(r => r.id == ticket.userid).FirstOrDefault();
                    ticket.status = "پاسخ داده شده";
                    chat.creationDate = DateTime.Now;
                    chat.isUser = false;
                    chat.ticketId = id;
                    db.TicketChats.Add(chat);
                    db.SaveChanges();

                    #region Send SMS to user
                    SMS sms = new SMS();
                    var setting = db.Settings.FirstOrDefault();

                    sms.Send(user.mobile, ticket.id.ToString(), Convert.ToInt32(setting.smsToken_TicketAnswer));
                    #endregion



                    db.SaveChanges();
                    file.SaveAs(Server.MapPath("~/Voice/") + dateString + ".wav"); //File will be saved in application root
                    return Json("/Voice/" + dateString + ".wav");

                }


                return Json(fileUrl);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "voice", "TicketsController");
                throw;
            }           

        }
        [Access("مشاهده لیست پیام های تیکت")]
        public ActionResult Chat(int? id)
        {
            ViewBag.id = id;
            return View();
        }

    }
}