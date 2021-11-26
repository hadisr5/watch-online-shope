using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Areas.Admin.Functions;
using Newtonsoft.Json;
using OnlineShop.Class;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class SellersController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [HttpPost]
        public JsonResult List()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                #region bands
                var fn = new functions();
                //var bans = new List<string>();
                //bans.Add("smsToken");
                #endregion
                try
                {
                    var Result = fn.CreateTable("Sellers", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Seller>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "List", "SellersController");
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }
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
        public ActionResult changeCats(int? id , List<int> MainCat)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    if (db.Seller_cats.Where(r => r.sellerId == id).ToList().Count != 0)
                    {
                        db.Seller_cats.RemoveRange(db.Seller_cats.Where(r => r.sellerId == id).ToList());
                    }
                    if (MainCat.Count != 0)
                    {
                        foreach (var item in MainCat)
                        {
                            var newMain = new Seller_cats();
                            newMain.catId = item;
                            newMain.sellerId = id;
                            db.Seller_cats.Add(newMain);
                        }
                        db.SaveChanges();
                    }
                    return Json("success");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "changeCats", "SellersController");
                    throw;
                }
              
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

        }
        public ActionResult details(int? id, int? sendVerificationCode)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                if (sendVerificationCode != null)
                {
                    try
                    {
                        var seller = db.Sellers.Find(id);
                        int rnd = GenerateRandomNo();

                        #region Send SMS to user
                        SMS sms = new SMS();
                        var setting = db.Settings.FirstOrDefault();

                        if (!sms.Send(seller.phone, rnd.ToString(), Convert.ToInt32(setting.smsToken_validateUser)))
                        {
                            ViewBag.sms = "مشکل در ارسال کد";
                        }
                        else
                        {
                            ViewBag.sms = rnd;
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "details", "SellersController");
                        throw;
                    }
                   
                }
                ViewBag.id = id;
                return View();
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        public int GenerateRandomNo()
        {
            ch.checkLogin();

            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        [HttpPost]
        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Sellers.Remove(db.Sellers.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "SellersController");
                    throw;
                }
                
            }
            return Json("success");

        }

        [ValidateAntiForgeryToken]
        public JsonResult changeStatus(bool? status, int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var seller = db.Sellers.Find(id);
                    seller.isValid = status;
                    db.SaveChanges();
                    return Json("done");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "changeStatus", "SellersController");
                    throw;
                }
                

            }
            else
            {
                return Json(string.Empty);
            }
        }

        public PartialViewResult Export()
        {
            return PartialView();
        }

    }
}