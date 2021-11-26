using Newtonsoft.Json;
using OnlineShop.Class;
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
using OnlineShop.Security;
using System.Configuration;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("پست های وبلاگ")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class blogPostController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [HttpPost]
        [Access("لیست پست های وبلاگ")]

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
                    var Result = fn.CreateTable("Blog_post", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Blog_post>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "blogPostController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;

                    throw;
                }
            }
            else
            {
                return Json(string.Empty);
            }

        }
        // GET: Admin/Menus
        [Access("حذف پست های وبلاگ")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();
            try
            {
                if (Session["login"] != null)
                {
                    db.Blog_post.Remove(db.Blog_post.Find(id));
                    db.SaveChanges();
                }
                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "remove", "blogPostController");
                throw;
            }
            
        }
        [Access("لیست پست های وبلاگ")]

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
        [Access("ایجاد پست های وبلاگ")]

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
        [ValidateInput(false)]
        [Access("ایجاد پست های وبلاگ")]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(Blog_post post)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    post.CreationDate = DateTime.Now;
                    db.Blog_post.Add(post);
                    db.SaveChanges();
                    return Json("done");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "CreatePost", "blogPostController");
                    throw;
                }
             
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [Access("ویرایش پست های وبلاگ")]

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
                    Class.Utility.CreateLog(ex, "upload", "blogPostController");
                    throw;
                }
               

            }
            return Json("done");
        }


        [Access("ویرایش پست های وبلاگ")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditPost(Blog_post post)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Blog_post.AddOrUpdate(post);
                    db.SaveChanges();
                    return Json("done");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "EditPost", "blogPostController");
                    throw;
                }
               

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }

    }
}