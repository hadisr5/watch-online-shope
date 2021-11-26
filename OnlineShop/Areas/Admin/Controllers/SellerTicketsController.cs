using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Areas.Admin.Functions;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class SellerTicketsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        public ActionResult newTicket() {
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
        public ActionResult remove(int? id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.Tickets.Remove(db.Tickets.Find(id));
                    db.TicketChats.RemoveRange(db.TicketChats.Where(r => r.ticketId == id).ToList());
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "SellerTicketsController");
                    throw;
                }
               
            }
            return Json("success");

        }
        // GET: Admin/Tickets
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
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewTicket(string sellerId ,string Desc , string title, string @for)
        {

            if (Session["login"] != null)
            {
                if (sellerId!=null)
                {
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        try
                        {
                            var newTicket = new Ticket();
                            newTicket.sellerid = Convert.ToInt16(sellerId);
                            newTicket.title = title;
                            newTicket.creationDate = DateTime.Now;
                            newTicket.@for = @for;
                            db.Tickets.Add(newTicket);
                            db.SaveChanges();
                            var chat = new TicketChat();
                            chat.creationDate = DateTime.Now;
                            chat.ticketId = newTicket.id;
                            chat.text = Desc;
                            chat.isSeller = false;
                            db.TicketChats.Add(chat);
                            db.SaveChanges();
                            return Json("success");
                        }
                        catch (Exception ex)
                        {
                            OnlineShop.Class.Utility.CreateLog(ex, "CreateNewTicket", "SellerTicketsController");
                            throw;
                        }
                        

                    }
                    else
                    {
                        return Json(new { title = "error", desc = "عنوان تیکت حتما باید وارد شود" });
                    }

                }
                else
                {
                    return Json(new { title = "error ", desc = "نام فروشنده حتما باید وارد شود" });
                }


                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult reply(TicketChat cht)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var ticket = db.Tickets.Where(r => r.id == cht.ticketId).FirstOrDefault();
                    if (ticket != null)
                    {
                        var seller = db.Sellers.Where(r => r.id == ticket.sellerid).FirstOrDefault();
                        if (seller != null)
                        {
                            ticket.status = "پاسخ داده شده";
                            cht.creationDate = DateTime.Now;
                            cht.isSeller = false;
                            db.TicketChats.Add(cht);
                            db.SaveChanges();
                            #region Send SMS to user
                            if (seller.phone != null)
                            {
                                SMS sms = new SMS();
                                var setting = db.Settings.FirstOrDefault();

                                sms.Send(seller.phone, ticket.id.ToString(), Convert.ToInt32(setting.smsToken_TicketAnswer));
                            }

                            #endregion
                        }

                    }

                    return Json("done");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "reply", "SellerTicketsController");
                    throw;
                }
               
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public JsonResult voice(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var chat = new TicketChat();
                    string fileUrl = "";
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
                        var seller = db.Sellers.Where(r => r.id == ticket.sellerid).FirstOrDefault();
                        ticket.status = "پاسخ داده شده";
                        chat.creationDate = DateTime.Now;
                        chat.isSeller = false;
                        chat.ticketId = id;
                        db.TicketChats.Add(chat);
                        db.SaveChanges();

                        #region Send SMS to user
                        SMS sms = new SMS();
                        var setting = db.Settings.FirstOrDefault();

                        sms.Send(seller.phone, ticket.id.ToString(), Convert.ToInt32(setting.smsToken_TicketAnswer));
                        #endregion



                        db.SaveChanges();
                        file.SaveAs(Server.MapPath("~/Voice/") + dateString + ".wav"); //File will be saved in application root
                        return Json("/Voice/" + dateString + ".wav");

                    }


                    return Json(fileUrl);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "voice", "SellerTicketsController");
                    throw;
                }
               

            }
            return Json("done");

        }
        public ActionResult Chat(int? id)
        {
            ch.checkLogin();

            ViewBag.id = id;
            if (Session["login"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
    }
}