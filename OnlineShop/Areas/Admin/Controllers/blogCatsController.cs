using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Class;
using Newtonsoft.Json;
using OnlineShop.Security;
using System.Configuration;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("دسته بندی وبلاگ")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class blogCatsController : Controller
    {


        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();

        // GET: Admin/Menus
        [Access("حذف دسته بندی وبلاگ")]

        public ActionResult remove(int? id)
        {
            try
            {
                db.Blog_Cats.Remove(db.Blog_Cats.Find(id));
                db.SaveChanges();
                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "remove", "blogCatsController");
                throw;
            }

        }
        [Access("لیست دسته بندی وبلاگ")]

        public ActionResult Index()
        {
            return View();
        }
        [Access("لیست دسته بندی وبلاگ")]
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
                var Result = fn.CreateTable("Blog_Cats", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<Blog_Cats>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "List", "blogCatsController");
                return Json(new
                {
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    sEcho = 0,
                    sColumns = string.Empty,
                    aaData = string.Empty
                }, JsonRequestBehavior.AllowGet); ;

                throw;
            }
        }
        [Access("ایجاد دسته بندی وبلاگ")]

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [ValidateInput(false)]
        [Access("ایجاد دسته بندی وبلاگ")]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(Blog_Cats post)
        {
            try
            {
                db.Blog_Cats.Add(post);
                db.SaveChanges();
                return Json("done");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "CreatePost", "blogCatsController");
                throw;
            }
          
        }
        [Access("ویرایش دسته بندی وبلاگ")]

        public ActionResult Edit(int? id)
        {
            ViewBag.id = id;
            return View();
        }
        [Access("ایجاد دسته بندی وبلاگ")]

        public JsonResult upload()
        {
            try
            {
                var appsettings = ConfigurationManager.AppSettings;
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
                    var baseaddress = appsettings["uploadpath"].ToString();
                    file.SaveAs(baseaddress + "uploads/" + dateString + ".jpg");
                    return Json("/uploads/" + dateString + ".jpg");

                }
                return Json(fileUrl);
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "upload", "blogCatsController");
                throw;
            }
           
        }

        [ValidateInput(false)]
        [Access("ویرایش دسته بندی وبلاگ")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(Blog_Cats post)
        {
            try
            {
                db.Blog_Cats.AddOrUpdate(post);
                db.SaveChanges();
                return Json("done");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "EditPost", "blogCatsController");
                throw;
            }
           
        }
    }
}