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
    [DisplayName("مدیریت مدل ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class ModelController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch =new  checkLoginAdmin();
        [Access("لیست مدل ها")]

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
        [Access("لیست مدل ها")]

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
                    var Result = fn.CreateTable("Model", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Model>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "ModelController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }
            
        }



        [Access("ایجاد مدل ها")]

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
        [Access("ایجاد مدل ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Models(Model Models)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Models.Add(Models);
                    db.SaveChanges();
                    return Json(new { success = "اسلاید " + Models + " اضافه شد" });
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Create_Models", "ModelController");
                    throw;
                }
               
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }


        }
        [HttpPost]
        [Access("حذف مدل ها")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Models.Remove(db.Models.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "ModelController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [Access("ویرایش مدل ها")]

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
        [Access("ویرایش مدل ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(Model Models, int id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Models.AddOrUpdate(Models);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_Post", "ModelController");
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
                    Class.Utility.CreateLog(ex, "upload", "ModelController");
                    throw;
                }     

            }
            return Json("done");
        }
    }
}