using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Models;
using Newtonsoft.Json;
using OnlineShop.Class;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت سئو")]
    public class SeoController : Controller
    {
        // GET: Admin/Seo
        [Access("لیست آیتم های سئو")]
        public ActionResult Index()
        {
            return View();
        }
        [Access("لیست آیتم های سئو")]
        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            #endregion
            try
            {
                var Result = fn.CreateTable("SEO", null, Request);
                var jobject = JsonConvert.DeserializeObject<List<SEO>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
            }
        }


    }
}