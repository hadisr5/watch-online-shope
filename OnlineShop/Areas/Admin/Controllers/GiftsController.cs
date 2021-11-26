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
    [DisplayName("میدریت هدیه ها")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class GiftsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("لیست هدیه ها")]
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
        [Access("لیست هدیه ها")]

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
                    var Result = fn.CreateTable("Gifts", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Gift>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "GiftsController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }


        [Access("ایجاد هدیه جدید")]
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
        [Access("ویرایش هدیه ها")]

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
        public JsonResult checkCatProperties(int? id)
        {
            ch.checkLogin();
            try
            {
                var properties = db.CatProperties.Where(r => r.catId == id).ToList();
                if (properties.Count != 0)
                {
                    return Json(properties);

                }
                else
                {
                    return Json(string.Empty);

                }
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "checkCatProperties", "GiftsController");
                throw;
            }
           
        }
        [HttpPost]
        [Access("حذف هدیه ها")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Gifts.Remove(db.Gifts.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "GiftsController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [HttpPost]
        [ValidateInput(false)]
        [Access("ویرایش هدیه ها")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_product(Gift pro, List<string> image, string desc)
        {
            ch.checkLogin();

     
            if (Session["login"] != null)
            {
                try
                {
                    //pro.description = desc;
                    db.Gifts.AddOrUpdate(pro);
                    try
                    {
                        db.ProductGalleries.RemoveRange(db.ProductGalleries.Where(r => r.giftId == pro.id).ToList());
                    }
                    catch (Exception)
                    {

                    }
                    db.SaveChanges();

                    if (image != null && image.Count != 0)
                    {
                        foreach (var item in image)
                        {
                            if (item.Trim() != "")
                            {
                                var gl = new ProductGallery();
                                gl.giftId = pro.id;
                                gl.big = item;
                                db.ProductGalleries.Add(gl);
                            }

                        }
                    }

                    db.SaveChanges();

                    return Redirect("/Admin/AdminDashboard/Index#/Admin/gifts/index");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_product", "GiftsController");
                    throw;
                }
                
            }
            else
            {
                return  Redirect("/Admin/login");

            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [Access("ایجاد هدیه جدید")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Product(Gift pro, List<string> image, string desc)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    pro.description = desc;
                    db.Gifts.Add(pro);
                    db.SaveChanges();
                    if (image != null && image.Count != 0)
                    {
                        foreach (var item in image)
                        {
                            if (item.Trim() != "")
                            {
                                var gl = new ProductGallery();
                                gl.giftId = pro.id;
                                gl.big = item;
                                db.ProductGalleries.Add(gl);
                            }

                        }
                    }
                    db.SaveChanges();

                    return Redirect("/Admin/AdminDashboard/Index#/Admin/gifts/index");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Create_Product", "GiftsController");
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
                    Class.Utility.CreateLog(ex, "upload", "GiftsController");
                    throw;
                }
               

            }
            return Json("done");
        }
    }
}