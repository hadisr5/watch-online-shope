using Newtonsoft.Json;
using OnlineShop.Class;
using OnlineShop.Models;
using OnlineShop.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class DiscountGroupController : Controller
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
                    var Result = fn.CreateTable("DiscountGroup", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<DiscountGroup>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "DiscountGroupController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
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
        [HttpGet]
        public ActionResult Create()
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
        public ActionResult Create(string Name, long? Amount, int count, string pre, int? Percentage, long? maxPercentage)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var group = new DiscountGroup();
                    group.Active = true;
                    group.CreationDate = DateTime.Now;
                    group.Name = Name;
                    if (Percentage == null)
                    {
                        group.Amount = Amount;
                    }
                    else
                    {
                        group.Percentage = Percentage;
                        group.maxPercentage = maxPercentage;
                    }
                    db.DiscountGroups.Add(group);
                    db.SaveChanges();
                    for (int i = 0; i < count; i++)
                    {
                        var code = new DiscountCode();
                        code.GroupId = group.id;
                        if (Percentage == null)
                        {
                            code.Amont = Amount;
                        }
                        else
                        {
                            code.Percentage = Percentage;
                            code.maxPercentage = maxPercentage;
                        }
                        code.creationDate = DateTime.Now;
                        code.isUsed = false;
                        string codeString = pre + RandomString(10);
                        while (db.DiscountCodes.Where(r => r.code == codeString).FirstOrDefault() != null)
                        {
                            codeString = pre + RandomString(10);
                        }
                        code.code = codeString;
                        db.DiscountCodes.Add(code);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Create", "DiscountGroupController");
                    throw;
                }
                

            }

            return Json(new { success = "تخفیف اضافه شد" });
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.DiscountGroups.Remove(db.DiscountGroups.Find(id));
                    var codes = db.DiscountCodes.Where(r => r.GroupId == id && r.isUsed == null).ToList();
                    if (codes.Count != 0)
                    {
                        db.DiscountCodes.RemoveRange(codes);
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "DiscountGroupController");
                    throw;
                }
               
            }
            return Json("success");

        }
        public ActionResult Details(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                ViewBag.id = id;
                return View();
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }


    }
}