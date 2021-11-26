using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using Newtonsoft.Json;
using OnlineShop.Class;
using System.Data;
using System.Data.OleDb;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;
using System.Configuration;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت یرند ساعت")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class WatchBrandController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("لیست  یرند ساعت")]

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
        [Access("لیست  یرند ساعت")]

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
                    var Result = fn.CreateTable("watchBrands", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<watchBrand>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "WatchBrandController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }



        [Access("ایجاد  یرند ساعت")]

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
        [Access("ایجاد  یرند ساعت")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_WatchBrands(watchBrand WatchBrands)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.watchBrands.Add(WatchBrands);
                    db.SaveChanges();
                    return Json(new { success = "اسلاید " + WatchBrands.Title + " اضافه شد" });
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Create_WatchBrands", "WatchBrandController");
                    throw;
                }

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }


        }
        [HttpPost]
        [Access("حذف  یرند ساعت")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.watchBrands.Remove(db.watchBrands.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "WatchBrandController");
                    throw;
                }

            }
            return Json("success");

        }
        [Access("ویرایش  یرند ساعت")]

        public ActionResult Edit(int? id)
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
        [HttpPost]
        [Access("ویرایش  یرند ساعت")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(watchBrand WatchBrands)
        {
          
            ch.checkLogin();
           
            if (Session["login"] != null)
            {
                try
                {
                    db.watchBrands.AddOrUpdate(WatchBrands);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_Post", "WatchBrandController");
                    throw;
                }
            }
            return Json("success");
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
                    Class.Utility.CreateLog(ex, "upload", "WatchBrandController");
                    throw;
                }

            }
            return Json("done");
        }
    }
}