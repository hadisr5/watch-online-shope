using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;


namespace OnlineShop.Controllers
{
    public class ProductController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Comment(int id, string title, string body)
        {
            if (Session["userLogin"] != null)
            {
                try
                {
                    int cmid = 0;
                    var product = db.Products.Find(id);
                    if (product != null)
                    {
                        var comment = new Comment();
                        comment.creationDate = DateTime.Now;
                        comment.title = title;
                        comment.productId = id;
                        comment.text = body;
                        comment.userid = Convert.ToInt32(Session["userLogin"]);
                        db.Comments.Add(comment);
                        db.SaveChanges();
                        cmid = comment.id;

                        var adv = Request.Params["comment[advantages][]"];
                        var disAdv = Request.Params["comment[disadvantages][]"];
                        if (adv != null || !string.IsNullOrEmpty(adv))
                        {
                            var stringList = new List<string>();
                            stringList = adv.Split(',').ToList();
                            if (stringList.Count != 0)
                            {
                                foreach (var item in stringList)
                                {
                                    var avg = new CommentAdvantage();
                                    avg.advantage = true;
                                    avg.disAdvantage = false;
                                    avg.commentId = comment.id;
                                    avg.productId = id;
                                    avg.text = item;
                                    db.CommentAdvantages.Add(avg);
                                }
                                db.SaveChanges();
                            }
                        }
                        if (disAdv != null || !string.IsNullOrEmpty(disAdv))
                        {
                            var stringList = new List<string>();
                            stringList = disAdv.Split(',').ToList();
                            if (stringList.Count != 0)
                            {
                                foreach (var item in stringList)
                                {
                                    var avg = new CommentAdvantage();
                                    avg.advantage = false;
                                    avg.disAdvantage = true;
                                    avg.commentId = comment.id;
                                    avg.productId = id;
                                    avg.text = item;
                                    db.CommentAdvantages.Add(avg);
                                }
                                db.SaveChanges();
                            }
                        }

                    }
                    return Json(cmid);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Comment", "ProductController");
                    throw;
                }
               
            }
            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult AddQuestion(int id, string question)
        {
            if (Session["userLogin"] != null)
            {
                try
                {
                    int cmid = 0;
                    var product = db.Products.Find(id);
                    if (product != null)
                    {
                        var q = new ProductQuestion();
                        q.CreationDate = DateTime.Now;
                        q.Question = question;
                        q.ProductId = id;
                        q.IsValidate = false;
                        q.UserId = Convert.ToInt32(Session["userLogin"]);
                        db.ProductQuestions.Add(q);
                        db.SaveChanges();

                    }
                    return Json(new { Success = true, Message = "پرسش شما ثبت شد. بعد از تایید نمایش داده خواهد شد." });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "AddQuestion", "ProductController");
                    throw;
                }
                
            }
            return Json(new { Success = false, Message = "ابتدا وارد شوید" });
        }
        [HttpPost]
        public JsonResult AddAnswer(int questionid, string answer)
        {
            if (Session["userLogin"] != null)
            {
                try
                {
                    int cmid = 0;
                    var question = db.ProductQuestions.Find(questionid);
                    if (question != null)
                    {
                        var ans = new ProductQAnswer();
                        ans.CreationDate = DateTime.Now;
                        ans.QuestionId = questionid;
                        ans.Answer = answer;
                        ans.IsValidate = false;
                        ans.UserId = Convert.ToInt32(Session["userLogin"]);
                        db.ProductQAnswers.Add(ans);
                        db.SaveChanges();

                    }
                    return Json(new { Success = true, Message = "پاسخ شما ثبت شد. بعد از تایید نمایش داده خواهد شد." });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "AddAnswer", "ProductController");
                    throw;
                }
               
            }
            return Json(new { Success = false, Message = "ابتدا وارد شوید" });
        }

        [HttpPost]
        public JsonResult ajaxLike(string type, int comment_id)
        {
            if (Session["userLogin"] != null)
            {
                try
                {
                    int userId = Convert.ToInt32(Session["userLogin"]);
                    var comment = db.Comments.Where(r => r.id == comment_id).FirstOrDefault();
                    var previusLikes = db.CommnetsLikes.Where(r => r.CommentId == comment_id && r.userId == userId).ToList();
                    if (previusLikes.Count != 0)
                    {
                        db.CommnetsLikes.RemoveRange(previusLikes);
                        db.SaveChanges();
                    }
                    if (comment != null)
                    {
                        var cmLike = new CommnetsLike();
                        cmLike.CommentId = comment_id;
                        cmLike.userId = userId;
                        if (type == "like")
                        {
                            cmLike.Like = true;
                        }
                        else
                        {
                            if (type == "dislike")
                            {
                                cmLike.disLike = true;
                            }
                        }
                        db.CommnetsLikes.Add(cmLike);
                        db.SaveChanges();
                        var likes = db.CommnetsLikes.Where(r => r.CommentId == comment_id && r.Like == true).ToList();
                        var disLike = db.CommnetsLikes.Where(r => r.CommentId == comment_id && r.disLike == true).ToList();
                        return Json(new { status = 200, like = likes.Count, dislike = disLike.Count });
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "ajaxLike", "ProductController");
                    throw;
                }
                
            }
            else
            {
                return Json(string.Empty);
            }
            return Json(string.Empty);

        }


        [HttpPost]
        public JsonResult Ratting(int id)
        {
            if (Session["userLogin"] != null)
            {
                try
                {
                    int userid = Convert.ToInt32(Session["userLogin"]);
                    var comment = db.Comments.Find(id);

                    var Keys = new List<string>();
                    foreach (var item in Request.Params.AllKeys)
                    {
                        if (item.Contains("rating_"))
                        {
                            Keys.Add(item);
                        }
                    }
                    if (Keys.Count != 0)
                    {
                        foreach (var item in Keys)
                        {
                            var rate = new RattingComment();
                            rate.productId = Convert.ToInt32(comment.productId);
                            rate.rating = Convert.ToInt32(Request.Params[item]);
                            rate.commentId = comment.id;
                            rate.userId = userid;
                            rate.rattingId = Convert.ToInt32(item.Replace("rating_", ""));
                            db.RattingComments.Add(rate);
                        }
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Ratting", "ProductController");
                    throw;
                }
                
            }
            return Json(string.Empty);
        }
    }
}