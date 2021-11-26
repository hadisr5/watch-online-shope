using Newtonsoft.Json;
using OnlineShop.Class;
using OnlineShop.Models;
using OnlineShop.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{

    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class AllowedIPAddressesController : Controller
    {
        private OnlineShopEntities db = new OnlineShopEntities();


        //[Access("لیست دسته بندی اخبار")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        //[Access("لیست دسته بندی اخبار")]
        public JsonResult List()
        {
            try
            {
                var Result = new functions().CreateTable("AllowedIpAddresses", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<AllowedIpAddress>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "List", "AllowedIPAddressesController");
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                throw;
            }
        }

        [HttpGet]
        //[Access("ایجاد دسته بندی اخبار")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //[Access("ایجاد دسته بندی اخبار")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_IpAddress(AddOrEditModel model)
        {
            try
            {
                using (var db = new OnlineShopEntities())
                {
                    var dbModel = db.AllowedIpAddresses.FirstOrDefault(f => f.IpAddress == model.IpAddress);
                    if (dbModel == null)
                    {
                        dbModel = new AllowedIpAddress { IpAddress = model.IpAddress, IsActive = model.IsActive };
                        db.AllowedIpAddresses.Add(dbModel);
                    }


                    db.SaveChanges();
                }

                return Redirect("/Admin/Dashboard/Index#/Admin/allowedipaddresses/index");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "Create_IpAddress", "AllowedIPAddressesController");
                throw;
            }
        }

        [HttpPost]
        //[Access("حذف دسته بندی اخبار")]
        public ActionResult Remove(int? id)
        {
            try
            {
                using (var db = new OnlineShopEntities())
                {
                    db.AllowedIpAddresses.Remove(db.AllowedIpAddresses.Find(id));
                    db.SaveChanges();
                }

                return Json("success");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "Remove", "AllowedIPAddressesController");
                throw;
            }
          
        }

        //[Access("ویرایش دسته بندی اخبار")]
        public ActionResult Edit(int? id)
        {
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        //[Access("ویرایش دسته بندی اخبار")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_IpAddress(AddOrEditModel model)
        {
            try
            {
                using (var db = new OnlineShopEntities())
                {
                    var dbModel = db.AllowedIpAddresses.FirstOrDefault(f => f.id == model.Id);
                    if (dbModel != null)
                    {
                        dbModel.IpAddress = model.IpAddress;
                        dbModel.IsActive = model.IsActive;
                        db.Entry(dbModel).State = System.Data.Entity.EntityState.Modified; ;
                    }

                    db.SaveChanges();
                }

                return Redirect("/Admin/Dashboard/Index#/Admin/allowedipaddresses/index");
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "Edit_IpAddress", "AllowedIPAddressesController");
                throw;
            }

        }

        private bool IsLogedIn()
        {
            return Session["login"] != null;
        }
    }

    public class AddOrEditModel
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public bool IsActive { get; set; }


    }
}