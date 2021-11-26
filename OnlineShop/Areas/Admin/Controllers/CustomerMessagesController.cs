using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using OnlineShop.Models;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("چت آنلاین با کاربران")]

    public class CustomerMessagesController : Controller
    {
        checkLoginAdmin ch = new checkLoginAdmin();
        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Admin/CustomerMessages
        [Access("چت آنلاین")]

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
        public JsonResult Contacts()
        {
            var contactList = new List<contactItem>();
            int adminId = Convert.ToInt32(Session["login"]);
            var contacts = db.onlineChatMessages.Where(r => r.adminId == adminId).OrderByDescending(r => r.CreationDate).ToList();
            if (contacts.Count != 0)
            {
                foreach (var item in contacts)
                {
                    var cn = new contactItem();
                    cn.id = item.id;
                    if (item.User != null)
                    {
                        cn.userId = item.User.id;
                        cn.isUser = true;
                    }
                    else
                    {
                        cn.isUser = false;
                    }
                    cn.mobile = item.OnlineChat.mobile;
                    if (item.onlineChat_chat.FirstOrDefault() != null)
                    {
                        cn.date = item.onlineChat_chat.OrderByDescending(r=>r.id).ToList().FirstOrDefault().CreationDate.ToString();
                    }
                    else
                    {
                        cn.date = item.CreationDate.ToString();
                    }
                    cn.unread = item.onlineChat_chat.Where(r => r.read != true && r.isAdmin!=true).ToList().Count; 
                    contactList.Add(cn);
                }

            }
            return Json(new { list = contactList.OrderByDescending(r=>r.date).ToList() });
        }

        public class contactItem
        {
            public int id { get; set; }
            public string mobile { get; set; }
            public Boolean isUser { get; set; }
            public int userId { get; set; }
            public int unread { get; set; }
            public string date { get; set; }
        }
        public class Cht
        {
            public int id { get; set; }
            public Boolean? isAdmin { get; set; }
            public Boolean? isCustomer { get; set; }
            public string CreationDate { get; set; }
            public string Content { get; set; }
            public Boolean? read { get; set; }

            public int? messageId { get; set; }

            public string username { get; set; }

            public string senderprofile { get; set; }
        }

        [HttpPost]
        [Access("چت آنلاین")]

        public JsonResult listOfMessages(int id)
        {
            ch.checkLogin();
            if (Session["login"]!=null)
            {
                int adminId = Convert.ToInt32(Session["login"]);
                var message = db.onlineChatMessages.Where(r => r.adminId == adminId && r.id == id).FirstOrDefault();
                if (message != null)
                {
                    var list = new List<Cht>();
                    var chats = db.onlineChat_chat.Where(r => r.messageId == message.id).ToList();
                    chats.Where(r => r.isAdmin != true).ToList().ForEach(r => r.read = true);
                    db.SaveChanges();
                    if (chats.Count != 0)
                    {
                        foreach (var item in chats)
                        {
                            var cht = new Cht();
                            cht.id = item.id;
                            cht.isAdmin = item.isAdmin;
                            cht.isCustomer = item.isCustomer;
                            cht.CreationDate = item.CreationDate.ToString() ;
                            cht.Content = item.Content;
                            cht.read = item.read;
                            cht.messageId = item.messageId;

                            if (message.UserId == null)
                            {
                                if (cht.isAdmin == true)
                                {
                                    cht.senderprofile = "/Template/images/user.svg";
                                    var admin = db.AdminUsers.Where(r => r.id == adminId).FirstOrDefault();
                                    if (admin.profileImage != null)
                                    {
                                        cht.senderprofile = admin.profileImage; 
                                    }

                                    cht.username = "ادمین";
                                }
                                else
                                {
                                    cht.username = "ناشناس";
                                }
                            }
                            else
                            {
                                cht.username = message.User.firstName + " " + message.User.lastName;
                            }
                            list.Add(cht);
                        }
                        list = list.OrderBy(r => r.CreationDate).ToList();
                    }
                    return Json(new { list = list });
                }
            }
            return Json(string.Empty);
        }


        [Access("چت آنلاین")]

        [HttpPost]
        public JsonResult text(int id  , string text)
        {
            ch.checkLogin();
            if (Session["login"]!=null)
            {
                int adminId = Convert.ToInt32(Session["login"]);
                var message = db.onlineChatMessages.Where(r => r.adminId == adminId && r.id == id).FirstOrDefault();
                if (message!=null)
                {
                    var chat = new onlineChat_chat();
                    chat.isAdmin = true;
                    chat.isCustomer = false;
                    chat.messageId = id;
                    chat.CreationDate = DateTime.Now;
                    chat.Content = text;
                    chat.read = false;
                    db.onlineChat_chat.Add(chat);
                    db.SaveChanges();
                }
            }
            return Json(string.Empty); 
        }
    }
}