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
    [DisplayName("مدیریت اسلایدر ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class SlidersController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/Sliders
        [Access("لیست اسلایدر ها")]
        public ActionResult Index()
        {
            return View();
        }
        [Access("لیست اسلایدر ها")]
        [HttpPost]
        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            //var bans = new List<string>();
            //bans.Add("smsToken");
            #endregion
            try
            {
                var Result = fn.CreateTable("Sliders", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Slider>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "List", "SlidersController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                throw;
            }
        }
        [Access("ایجاد اسلایدر ها")]
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Access("ایجاد اسلایدر ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Slider(Slider slide)
        {
            try
            {
                db.Sliders.Add(slide);
                db.SaveChanges();
                return Json(new { success = "اسلاید " + slide.title + " اضافه شد" });
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Create_Slider", "SlidersController");
                throw;
            }
           
        }
        [HttpPost]
        [Access("حذف اسلایدر ها")]
        public ActionResult remove(int? id)
        {
            try
            {
                db.Sliders.Remove(db.Sliders.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "remove", "SlidersController");
                throw;
            }
            
        }
        [Access("ویرایش اسلایدر ها")]
        public ActionResult Edit(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        [Access("ویرایش اسلایدر ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(Slider Menu, int id)
        {
            try
            {
                db.Sliders.AddOrUpdate(Menu);
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Edit_Post", "SlidersController");
                throw;
            }
           
        }
        public JsonResult upload()
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
                OnlineShop.Class.Utility.CreateLog(ex, "upload", "SlidersController");
                throw;
            }
            
        }

    }
}