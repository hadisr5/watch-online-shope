using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.SessionState;
using OnlineShop.Areas.Admin.Functions;
using OnlineShop.Models;  

namespace OnlineShop.Controllers
{
    public class onlineChatController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        public JsonResult messagesList(int id)
        {
            if (Session["onlineChat_token"]!=null)
            {
                string token = Session["onlineChat_token"].ToString();
                var message = db.onlineChatMessages.Where(r => r.id == id).FirstOrDefault();
                var onlineChat = db.OnlineChats.Where(r => r.token == token && r.id == message.onlineChatId).FirstOrDefault();
                if (onlineChat!=null)
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
                            cht.CreationDate =Convert.ToDateTime(item.CreationDate).ToString("HH:mm:ss");
                            cht.Content = item.Content;
                            cht.read = item.read;
                            cht.messageId = item.messageId;

                            if (message.UserId == null)
                            {
                                if (cht.isAdmin == true)
                                {
                                    cht.senderprofile = "/Template/images/user.svg";
                                    cht.username = "کارشناس";
                                }
                                else
                                {
                                    cht.username = "کاربر";
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
        public JsonResult getVerificationCode(string mobile)
        {
            if (mobile.Length == 11 && mobile.Substring(0,2) == "09")
            {
                var setting = db.Settings.FirstOrDefault();

                string ip = Request.UserHostAddress;
                var blocked = db.OnlineChats.Where(r => r.ipAddress == ip && r.attemps > 3 && r.releaseTime > DateTime.Now).FirstOrDefault();
                if (blocked != null)
                {
                    return Json(string.Empty);
                }
                if (Session["userLogin"] != null || Session["onlineChat"] != null)
                {
                    return Json(string.Empty);
                }
                if (mobile == null || mobile.Length != 11 || mobile.Substring(0, 2) != "09")
                {
                    return Json(string.Empty);
                }

                var onlineChat = new OnlineChat();
                onlineChat.ipAddress = ip;
                onlineChat.mobile = mobile;
                onlineChat.CreationDate = DateTime.Now.AddMinutes(2);
                onlineChat.code = GenerateRandomNo().ToString();
                onlineChat.token = RandomString(50);
                Session["onlineChat_token"] = onlineChat.token;
                db.OnlineChats.Add(onlineChat);

                SMS sms = new SMS();
                sms.Send(mobile, onlineChat.code, Convert.ToInt32(setting.smsToken_onlineChat));

                db.SaveChanges();
                return Json("done");

            }
            else
            {
                return Json("Invalid phone number");
            }


        }
        public JsonResult verify(string code)
        {
            if (Session["onlineChat_token"] != null)
            {
                string token = Session["onlineChat_token"].ToString();
                var newDate = DateTime.Now.AddMinutes(-2); 
                var onlineChat = db.OnlineChats.Where(r => r.token == token && r.CreationDate > newDate).FirstOrDefault();
                if (onlineChat!=null)
                {
                    var Message = new onlineChatMessage();
                    Message.onlineChatId = onlineChat.id;
                    Message.CreationDate = DateTime.Now;
                    db.onlineChatMessages.Add(Message);
                    db.SaveChanges();
                    return Json(new { status= "ok" , messageId= Message.id });
                }
            }
            return Json(string.Empty);
        }
        [HttpPost]
        public JsonResult getChatToken(int messageId)
        {
            if (Session["onlineChat_token"] != null )
            {
                string token = Session["onlineChat_token"].ToString();
                var message = db.onlineChatMessages.Where(r => r.id == messageId && r.OnlineChat.token == token).FirstOrDefault();
                if (message!=null)
                {
                    var lastOnlineAdmin = db.AdminUsers.OrderByDescending(r => r.lastOnline).FirstOrDefault();
                    if (lastOnlineAdmin.lastOnline > DateTime.Now.AddMinutes(-3))
                    {
                        message.adminId = lastOnlineAdmin.id;
                        var chat = new onlineChat_chat();
                        chat.isAdmin = true;
                        chat.Content = lastOnlineAdmin.introMessage;
                        chat.CreationDate = DateTime.Now;
                        chat.isCustomer = false;
                        chat.read = false;
                        chat.messageId = message.id;
                        db.onlineChat_chat.Add(chat);
                        db.SaveChanges();
                        return Json(new { msgId = message.id , intro = lastOnlineAdmin.introMessage });
                    }
                    else
                    {
                        message.adminId = 0;
                        var chat = new onlineChat_chat();
                        var setting = db.onlineChatSettings.FirstOrDefault();

                        chat.isAdmin = true;
                        chat.Content = setting.introMessage;
                        chat.CreationDate = DateTime.Now;
                        chat.isCustomer = false;
                        chat.read = false;
                        chat.messageId = message.id;
                        db.onlineChat_chat.Add(chat);
                        db.SaveChanges();
                        return Json(new { msgId = message.id, intro = setting.introMessage });
                    }
                }
            }
            return Json(string.Empty); 
        }
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz$#@!%^&*()";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public ActionResult Index()
        {

            return View();
        }
        public JsonResult chat(int id , string text , int reply)
        {
            if (Session["onlineChat_token"]!=null)
            {
                string token = Session["onlineChat_token"].ToString();
                var onlineChat = db.OnlineChats.Where(r => r.token == token).FirstOrDefault();
                if (onlineChat!=null)
                {
                    var message = db.onlineChatMessages.Find(id);
                    if (message!=null)
                    {
                        if (message.onlineChatId == onlineChat.id)
                        {
                            var chat = new onlineChat_chat();
                            if (reply!=0)
                            {
                                chat.replyTo = reply; 
                            }
                            chat.Content = text;
                            chat.CreationDate = DateTime.Now;
                            chat.isAdmin = false;
                            chat.isCustomer = true;
                            chat.messageId = message.id;
                            db.onlineChat_chat.Add(chat);
                            db.SaveChanges();
                            return Json(new {status=200 , dt=DateTime.Now , chatId = chat.id  });
                        }
                    }
                }

            }
            return Json(string.Empty);
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
    }
}