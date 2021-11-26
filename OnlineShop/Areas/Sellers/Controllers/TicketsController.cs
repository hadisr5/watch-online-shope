using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Areas.Sellers.Controllers
{
    public class TicketsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginSeller ch = new checkLoginSeller();

        public ActionResult newTicket()
        {
            ch.checkLogin();
            if (Session["seller"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }

        // GET: Admin/Tickets
        public ActionResult Index()
        {
            ch.checkLogin();

            if (Session["seller"] != null)
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
        public ActionResult CreateNewTicket(string Desc, string title, string @for)
        {

            if (Session["seller"] != null)
            {

                if (!string.IsNullOrWhiteSpace(title))
                {
                    try
                    {
                        int sellerid = Convert.ToInt16(Session["seller"]);
                        var newTicket = new Ticket();
                        newTicket.sellerid = sellerid;
                        newTicket.title = title;
                        newTicket.creationDate = DateTime.Now;
                        newTicket.@for = @for;
                        db.Tickets.Add(newTicket);
                        db.SaveChanges();
                        var chat = new TicketChat();
                        chat.creationDate = DateTime.Now;
                        chat.ticketId = newTicket.id;
                        chat.text = Desc;
                        chat.isSeller = true;
                        db.TicketChats.Add(chat);
                        db.SaveChanges();
                        return Json("success");
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "CreateNewTicket", "TicketsController");
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
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }

        public ActionResult Chat(int? id)
        {
            ch.checkLogin();

            ViewBag.id = id;
            if (Session["seller"] != null)
            {
                try
                {
                    int sellerId = Convert.ToInt16(Session["seller"]);
                    if (db.Tickets.Where(r => r.id == id && r.sellerid == sellerId).FirstOrDefault() != null)
                    {
                        return View();

                    }
                    else
                    {
                        return RedirectToAction("NotFound", "Home", new { area = "" });

                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Chat", "TicketsController");
                    throw;
                }
                

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

            if (Session["seller"] != null)
            {
                try
                {
                    var ticket = db.Tickets.Where(r => r.id == cht.ticketId).FirstOrDefault();
                    if (ticket != null)
                    {
                        var seller = db.Sellers.Where(r => r.id == ticket.sellerid).FirstOrDefault();
                        if (seller != null)
                        {
                            int sellerId = Convert.ToInt16(Session["seller"]);

                            if (ticket.sellerid == sellerId)
                            {
                                ticket.status = "پاسخ فروشنده";
                                cht.creationDate = DateTime.Now;
                                cht.isSeller = true;
                                db.TicketChats.Add(cht);
                                db.SaveChanges();
                            }

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
            else
            {
                return Json(string.Empty);
            }
        }


        public JsonResult voice(int? id)
        {
            ch.checkLogin();

            if (Session["seller"] != null)
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
                        chat.isSeller = true;
                        chat.ticketId = id;
                        db.TicketChats.Add(chat);
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
            return Json("done");

        }

    }
}