using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaMvc.HtmlHelpers;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class BlogController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Blog
        public ActionResult Category(int? id)
        {
            ViewBag.id = id; 
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details(int? id , bool? successComment)
        {
            ViewBag.success = successComment;
            ViewBag.id = id;
            return View();
        }
        public ActionResult comment(int id , string Comment)
        {
            if (Session["userLogin"]!=null)
            {
                if (this.IsCaptchaValid("Captcha is not valid") == false)
                {
                    return Json("<p class=\"error\">کد امنیتی وارد شده اشتباه است .</p>");
                }
                try
                {
                    int userId = Convert.ToInt32(Session["userLogin"]);
                    var comment = new Blog_Comments();
                    comment.creationDate = DateTime.Now;
                    comment.PostId = id;
                    comment.comment = Comment;
                    comment.userid = userId;
                    comment.Submitted = false;
                    db.Blog_Comments.Add(comment);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "comment", "BlogController");
                    throw;
                }
                
            }

            return RedirectToAction("details", new { id = id , successComment=true });
        }
    }
}