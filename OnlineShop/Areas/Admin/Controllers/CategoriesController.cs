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
    [DisplayName("مدیریت دسته بندی ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class CategoriesController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [HttpPost]

        public JsonResult addProperty(CatProperty pr)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.CatProperties.Add(pr);
                    db.SaveChanges();
                    return Json(new { status = 200 });
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "addProperty", "CategoriesController");
                    throw;
                }

            }
            return Json("Success");
        }

        [HttpPost]

        public JsonResult removeProperty(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    db.CatProperties.Remove(db.CatProperties.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "removeProperty", "CategoriesController");
                    throw;
                }

            }
            return Json(string.Empty);
        }
        [HttpGet]
        public ActionResult properyList(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                ViewBag.id = id;
                return View("/Areas/Admin/Views/Categories/properyList.cshtml");

            }
            return Json(string.Empty);
        }
        public ActionResult properties()
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
        // GET: Admin/Categories
        [Access("لیست دسته بندی ها")]

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
        [Access("لیست دسته بندی ها")]

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
                    var Result = fn.CreateTable("Categories", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Category>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "CategoriesController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }

        [Access("ایجاد دسته بندی ها")]

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
        [Access("حذف دسته بندی ها")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Categories.Remove(db.Categories.Find(id));
                    var properties = db.CatProperties.Where(r => r.catId == id).ToList();
                    if (properties.Count != 0)
                    {
                        foreach (var item in properties)
                        {
                            db.CatProperties.Remove(item);

                        }
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "CategoriesController");
                    throw;
                }
               
            }
            return Json("success");
        }
        [Access("ویرایش دسته بندی ها")]

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
        [Access("ویرایش دسته بندی ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Category(Category cat, List<string> prop)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    if (cat.parent == 0)
                        cat.parent = null;
                    db.Categories.AddOrUpdate(cat);
                    db.SaveChanges();
                    //foreach (string name in Request.Form.AllKeys)
                    //{
                    //    try
                    //    {
                    //        int id = Convert.ToInt16(name);
                    //        if (id != 0)
                    //        {
                    //            var property = db.CatProperties.Find(id);
                    //            if (property!=null)
                    //            {
                    //                property.title = Request.Form[name];


                    //            }
                    //            db.SaveChanges();
                    //        }
                    //    }
                    //    catch (Exception)
                    //    {

                    //    }
                    //}
                    //if (prop.Count!=0)
                    //{
                    //    foreach (var item in prop)
                    //    {
                    //        if (item!= null && item.Trim() !="")
                    //        {
                    //            var newProperty = new CatProperty();
                    //            newProperty.catId = cat.id;
                    //            newProperty.title = item;
                    //            db.CatProperties.Add(newProperty);
                    //            db.SaveChanges();
                    //        }

                    //    }

                    //}
                    return Redirect("/Admin/AdminDashboard/Index#/Admin/Categories/index");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_Category", "CategoriesController");
                    throw;
                }
               
            }
            else
            {
                return Redirect("/Admin/login");
            }
        }
        [Access("ایجاد دسته بندی ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_category(Category cat, List<string> prop)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    var propertyKeys = new List<string>();
                    var keyNumbers = new List<int>();

                    var allKeys = Request.Params.AllKeys.Where(r => r.Contains("prop"));
                    foreach (var item in allKeys)
                    {
                        if (item.Contains("prop"))
                        {
                            propertyKeys.Add(item);
                            keyNumbers.Add(Convert.ToInt32(item.Replace("prop", "")));
                        }
                    }
                    if (keyNumbers.Count != 0)
                    {
                        foreach (var item in keyNumbers)
                        {
                            var newProp = new CatProperty();

                        }
                    }

                    var omid = Request.Params["prop" + 0];

                    if (cat.parent == 0)
                        cat.parent = null;
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    //if (prop.Count != 0 && prop != null)
                    //{
                    //    foreach (var item in prop)
                    //    {
                    //        if (item.Trim() != "")
                    //        {
                    //            var newProp = new CatProperty();
                    //            newProp.catId = cat.id;
                    //            newProp.title = item;
                    //            db.CatProperties.Add(newProp);
                    //        }
                    //    }
                    //}
                    db.SaveChanges();
                    return Redirect("/Admin/AdminDashboard/Index#/Admin/Categories/index");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Create_category", "CategoriesController");
                    throw;
                }
               
            }
            else
            {
                return Redirect("/Admin/login");
            }
        }
        [Access("حذف دسته بندی ها")]

        public JsonResult RemoveCat(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.CatProperties.Remove(db.CatProperties.Find(id));
                    db.SaveChanges();
                    return Json("done");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "RemoveCat", "CategoriesController");
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
                    Class.Utility.CreateLog(ex, "upload", "CategoriesController");
                    throw;
                }
                

            }
            return Json("done");
        }



    }
}