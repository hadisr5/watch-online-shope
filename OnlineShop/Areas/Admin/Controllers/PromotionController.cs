using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using System.Globalization;
using Newtonsoft.Json;
using OnlineShop.Class;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;
using System.Configuration;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت پروموشن ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class PromotionController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/Promotion
        [Access("لیست پروموشن ها")]

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
        [Access("ایجاد پروموشن ها")]

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
        [Access("ایجاد پروموشن ها")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult create(Promotion pro)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.Promotions.Add(pro);
                    db.SaveChanges();
                    return Json(string.Empty);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "create", "PromotionController");
                    throw;
                }
              
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [Access("ویرایش پروموشن ها")]

        public ActionResult Edit(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                ViewBag.id = id; 
                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [HttpPost]
        [Access("ویرایش پروموشن ها")]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(Promotion Pr)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.Promotions.AddOrUpdate(Pr);
                    db.SaveChanges();
                    return Json(string.Empty);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Edit", "PromotionController");
                    throw;
                }
               
            }
            else
            {
                return Json(string.Empty);
            }
        }

        [HttpPost]
        [Access("حذف پروموشن ها")]

        public JsonResult Remove(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.Promotions.Remove(db.Promotions.Find(id));
                    db.SaveChanges();
                    return Json(string.Empty);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Remove", "PromotionController");
                    throw;
                }
               
            }
            else
            {
                return Json(string.Empty);
            }
        }

        [HttpPost]
        [Access("لیست پروموشن ها")]

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
                    var Result = fn.CreateTable("Promotion", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Promotion>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "List", "PromotionController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }



        public JsonResult upload()
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    string fileUrl = "";
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                                    //Use the following properties to get file's name, size and MIMEType
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;
                        string dateString = DateTime.Now.ToString();
                        while (dateString.Contains(":") || dateString.Contains("/"))
                        {
                            dateString = dateString.Replace(":", "");
                            dateString = dateString.Replace("/", "");

                        }
                        fileUrl = dateString + ".jpg";
                        db.SaveChanges();
                        System.IO.Stream fileContent = file.InputStream;
                        //To save file, use SaveAs method
                        var appsettings = ConfigurationManager.AppSettings;
                        var baseaddress = appsettings["uploadpath"].ToString();
                        file.SaveAs(baseaddress + "uploads/" + dateString + ".jpg");
                        return Json("/uploads/" + dateString + ".jpg");

                    }
                    return Json(fileUrl);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "upload", "PromotionController");
                    throw;
                }
                

            }
            return Json("done");
        }

    }
}