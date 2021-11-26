using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using OnlineShop.Class;
using Newtonsoft.Json;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;
using System.Configuration;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت دسته بندی های اصلی")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class MainCatsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("لیست دسته بندی های اصلی")]

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
        [Access("ایجاد دسته بندی های اصلی")]

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

        [Access("لیست دسته بندی های اصلی")]

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
                    var Result = fn.CreateTable("MainCats", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<MainCat>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "List", "MainCatsController");
                    throw;
                }
            }
            else
            {
                return Json(string.Empty);
            }

        }

        [Access("ایجاد دسته بندی های اصلی")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_MainCat(MainCat MainCat)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.MainCats.Add(MainCat);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Create_MainCat", "MainCatsController");
                    throw;
                }
              
            }

            return Redirect("/Admin/AdminDashboard/Index#/Admin/MainCats/index");
        }
        [HttpPost]
        [Access("حذف دسته بندی های اصلی")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.MainCats.Remove(db.MainCats.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "MainCatsController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [Access("ویرایش دسته بندی های اصلی")]

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

        [Access("ویرایش دسته بندی های اصلی")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(MainCat mc, int id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.MainCats.AddOrUpdate(mc);
                    db.SaveChanges();
                    return Redirect("/Admin/AdminDashboard/Index#/Admin/MainCats/index");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Edit_Post", "MainCatsController");
                    throw;
                }
               
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
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
                    OnlineShop.Class.Utility.CreateLog(ex, "upload", "MainCatsController");
                    throw;
                }
                

            }
            return Json("done");
        }
    }
}