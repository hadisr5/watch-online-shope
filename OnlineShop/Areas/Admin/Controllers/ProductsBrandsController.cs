using Newtonsoft.Json;
using OnlineShop.Class;
using OnlineShop.Models;
using OnlineShop.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class ProductsBrandsController : Controller
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
                    var Result = fn.CreateTable("ProductsBrands", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<ProductsBrand>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "List", "ProductsBrandsController");
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
        public ActionResult Create_Brand(ProductsBrand brand)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.ProductsBrands.Add(brand);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Create_Brand", "ProductsBrandsController");
                    throw;
                }
               
            }

            return Json(new { success = "برند " + brand.title + " اضافه شد" });
        }
        [HttpPost]
        public ActionResult remove(int id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.ProductsBrands.Remove(db.ProductsBrands.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "ProductsBrandsController");
                    throw;
                }
               
            }
            return Json("success");

        }

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
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Post(ProductsBrand brand, int id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.ProductsBrands.AddOrUpdate(brand);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Edit_Post", "ProductsBrandsController");
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
                    OnlineShop.Class.Utility.CreateLog(ex, "upload", "ProductsBrandsController");
                    throw;
                }
               

            }
            return Json("done");
        }
    }
}