using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Areas.UserProfile.Controllers
{
    public class ProfileController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        // GET: UserProfile/Profile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tickets()
        {
            if (Session["userLogin"] != null)
            {
                return View();
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }
        public ActionResult Create_Ticket()
        {
            if (Session["userLogin"] != null)
            {
                return View();
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_New_Ticket(string title, string @for, string Ticket_content)
        {
            if (Session["userLogin"] != null)
            {
                #region Validations
                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(@for) || string.IsNullOrWhiteSpace(Ticket_content))
                {
                    ViewBag.error = "لطفا همه فیلد ها را پر نمائید";
                    return View("/Areas/UserProfile/Views/Profile/Create_Ticket.cshtml");
                }



                #endregion

                int userid = Convert.ToInt32(Session["userLogin"]);
                var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                var ticket = new Ticket();
                ticket.title = title;
                ticket.@for = @for;
                ticket.userid = userid;
                ticket.status = "پاسخ کاربر";
                ticket.creationDate = DateTime.Now;
                db.Tickets.Add(ticket);
                db.SaveChanges();

                var ticket_chat = new TicketChat();
                ticket_chat.creationDate = DateTime.Now;
                ticket_chat.isUser = true;
                ticket_chat.ticketId = ticket.id;
                ticket_chat.text = Ticket_content;
                ticket_chat.isSeller = false;
                db.TicketChats.Add(ticket_chat);
                db.SaveChanges();


                return RedirectToAction("ticket_chat", "Profile", new { id = ticket.id });
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ticket_Message(int id, string text)
        {
            if (Session["userLogin"] != null)
            {
                int userid = Convert.ToInt32(Session["userLogin"]);
                var ticket = db.Tickets.Where(r => r.userid == userid && r.id == id).FirstOrDefault();
                if (ticket != null)
                {
                    ticket.status = "پاسخ کاربر";
                    var ticketChat = new TicketChat();
                    ticketChat.isUser = true;
                    ticketChat.creationDate = DateTime.Now;
                    ticketChat.text = text;
                    ticketChat.ticketId = id;
                    ticketChat.isSeller = false;
                    db.TicketChats.Add(ticketChat);
                    db.SaveChanges();
                }
                return RedirectToAction("ticket_chat", new { id = id });
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }









        public ActionResult ticket_chat(int id)
        {
            if (Session["userLogin"] != null)
            {
                int userid = Convert.ToInt32(Session["userLogin"]);
                var ticket = db.Tickets.Where(r => r.userid == userid && r.id == id).FirstOrDefault();

                if (ticket != null)
                {
                    ViewBag.id = id;
                    return View();
                }
                else
                {
                    return RedirectToAction("NotFound", "Home", new { area = "" });
                }

            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }

        public ActionResult ChangePassword()
        {
            if (Session["userLogin"] != null)
            {
                return View();
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changePassword(string password,bool UserHasPassword, List<string> newPassword)
        {
            string userpass=null;
            if (Session["userLogin"] != null)
            {
                int id = Convert.ToInt32(Session["userLogin"]);
                var user = db.Users.Find(id);
                if(UserHasPassword==true)
                //password = CreateMD5(password);
                userpass = Security.RsaEncryptDecrypt.RSACls.RSADecryption(user.password, Properties.Settings.Default.PrivateKey);

                if (( userpass != null && userpass == password) || UserHasPassword==false)
                {
                    if (newPassword.Count == 2)
                    {
                        string psw = newPassword.FirstOrDefault();
                        Boolean flag = false;
                        foreach (var item in newPassword)
                        {
                            if (item != psw)
                            {
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            if (newPassword.FirstOrDefault().Length > 5)
                            {
                                if (newPassword.FirstOrDefault().Trim() == newPassword.FirstOrDefault())
                                {
                                    string nPsw = newPassword.FirstOrDefault();
                                    nPsw = OnlineShop.Security.RsaEncryptDecrypt.RSACls.RSAEncryption(nPsw, Properties.Settings.Default.PublicKey); ;
                                    user.password = nPsw;
                                    db.SaveChanges();
                                    ViewBag.success = "پسورد شما با موفقیت تغییر یافت";
                                }
                                else
                                {
                                    ViewBag.error = "پسورد جدید نمی تواند شامل space باشد";
                                }
                            }
                            else
                            {
                                ViewBag.error = "رمزعبور جدید باید بیش از 5 کاراکتر باشد";
                            }

                        }
                        else
                        {
                            ViewBag.error = "رمزعبور با تکرار آن همخوانی ندارد";
                        }
                    }
                    else
                    {
                        ViewBag.error = "لطفا رمزعبور و تکرار رمزعبور جدید را وارد فرمائید";
                    }
                }
                else
                {
                    ViewBag.error = "رمزعبور فعلی شما اشتباه است";
                }

                return View();
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public ActionResult changeProfileInfo()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changeProfileInfo(string name, string family, string email, string codeMeli,
            string mobile, string card, bool RecieveNews = false)
        {
            #region Check Empty Inputs
            if (name == "")
            {
                name = null;
            }
            if (family == "")
            {
                family = null;
            }
            if (email == "")
            {
                email = null;
            }
            if (codeMeli == "")
            {
                codeMeli = null;
            }
            if (mobile == "")
            {
                mobile = null;
            }
            if (card == "")
            {
                card = null;
            }
            #endregion

            if (Session["userLogin"] != null)
            {
                int id = Convert.ToInt16(Session["userLogin"]);
                var user = db.Users.Find(id);
                user.firstName = name;
                user.lastName = family;
                user.card = card;
                user.mobile = mobile;
                user.email = email;
                user.codeMeli = codeMeli;
                user.RecieveNews = RecieveNews;
                db.SaveChanges();
                return RedirectToAction("index");
            }
            return RedirectToAction("NotFound", "Home", new { area = "" });
        }

        public ActionResult Orders()
        {
            return View();
        }

        public ActionResult OrderDetail(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult Wallet()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult payWallet(long amount)
        {
            if (Session["userLogin"] == null)
            {
                return RedirectToAction("index");
            }
            else
            {
                int userird = Convert.ToInt16(Session["userLogin"]);
                var newPay = new Wallet();
                newPay.amount = amount;
                newPay.creationDate = DateTime.Now;
                newPay.payByAdmin = false;
                newPay.userId = userird;
                newPay.payWay = "درگاه پرداخت";
                newPay.Success = false; 
                db.Wallets.Add(newPay);
                db.SaveChanges();
                return RedirectToAction("Pay" , "PayInWallet" , new { id = newPay.id });
            }
        }
        public ActionResult Favorites()
        {
            return View();
        }

        public ActionResult SuccessWalletPay(int id)
        {
            if (Session["userLogin"] != null)
            {
                ViewBag.id = id; 
                return View();
            }
            return RedirectToAction("NotFound");
        }

        public ActionResult removeProductToWishList(int? id)
        {
            if (Session["userLogin"] != null)
            {
                int userId = Convert.ToInt16(Session["userLogin"]);
                db.WishLists.Remove(db.WishLists.Where(r => r.productId == id && r.userid == userId).FirstOrDefault());
                db.SaveChanges();

                return RedirectToAction("Favorites");
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        public ActionResult Comments()
        {
            return View();
        }

        public ActionResult removeComment(int id)
        {
            if (Session["userLogin"] != null)
            {
                int userid = Convert.ToInt16(Session["userLogin"]);
                var comment = db.Comments.Where(r => r.id == id).FirstOrDefault();
                if (comment.userid == userid)
                {
                    db.Comments.Remove(comment);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Comments");
        }
        public ActionResult recentViews()
        {
            return View();
        }

    }
}