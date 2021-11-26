using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShop.Areas.Admin.Security;
using OnlineShop.Class;
using OnlineShop.Models;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class FaqController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("لیست سوالات متداول")]
        // GET: Admin/Faq
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
        [Access("لیست سوالات متداول")]

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
                    var Result = fn.CreateTable("Faq", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Faq>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "List", "FaqController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;

                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }

        [Access("ایجاد سوالات متداول")]

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
        [Access("حذف سوالات متداول")]

        public ActionResult removefaq(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Faqs.Remove(db.Faqs.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "removefaq", "FaqController");
                    throw;
                }
                
            }
            return Json("success");
        }
        [Access("ویرایش سوالات متداول")]

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
        [Access("ویرایش سوالات متداول")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_Faq(Faq faq)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Faqs.AddOrUpdate(faq);
                    db.SaveChanges();

                    return Redirect("/Admin/AdminDashboard/Index#/Admin/faq");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Edit_Faq", "FaqController");
                    throw;
                }
                
            }
            else
            {
                return View();
            }
        }
        [Access("ایجاد سوالات متداول")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_faq(Faq faq)
        {

            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Faqs.Add(faq);
                    db.SaveChanges();

                    return Redirect("/Admin/AdminDashboard/Index#/Admin/faq");
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Create_faq", "FaqController");
                    throw;
                }
               
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [Access("لیست سوالات متداول")]
        [HttpGet]
        public ActionResult FaqCategories()
        {
            return View();
        }

        [HttpPost]
        public JsonResult FaqCategoryList()
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
                    var Result = fn.CreateTable("FaqCategory", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<FaqCategory>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "FaqCategoryList", "FaqController");
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }
        }


        [Access("ایجاد سوالات متداول")]

        [HttpGet]
        public ActionResult CreateFaqCategory()
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
        [Access("ایجاد سوالات متداول")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFaqCategory(FaqCategory fc)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.FaqCategories.Add(fc);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "CreateFaqCategory", "FaqController");
                    throw;
                }
                
            }

            return Redirect("/Admin/AdminDashboard/Index#/Admin/faq/faqcategories");
        }
        [HttpPost]
        [Access("حذف سوالات متداول")]

        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.FaqCategories.Remove(db.FaqCategories.Find(id));
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "remove", "FaqController");
                    throw;
                }
                
            }
            return Json("success");

        }
        [Access("ویرایش سوالات متداول")]

        public ActionResult EditFaqCategory(int? id)
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
        [Access("ویرایش سوالات متداول")]
        [ValidateAntiForgeryToken]
        public ActionResult EditFaqCategory(FaqCategory fc, int id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.FaqCategories.AddOrUpdate(fc);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "EditFaqCategory", "FaqController");
                    throw;
                }
               
            }
            return Redirect("/Admin/AdminDashboard/Index#/Admin/faq/faqcategories");
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
                    Class.Utility.CreateLog(ex, "upload", "FaqController");
                    throw;
                }
                

            }
            return Json("done");
        }
    }
}