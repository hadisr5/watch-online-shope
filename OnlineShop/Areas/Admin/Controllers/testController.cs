using Newtonsoft.Json;
using OnlineShop.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class testController : Controller
    {
        // GET: Admin/test
        public ActionResult Index()
        {


            return View();
        }
   
        public JsonResult List()
        {
            #region bands
            var fn = new functions();
            var bans = new List<string>();
            bans.Add("smsToken");
            #endregion
            try
            {
                var Result = fn.CreateTable("Users", bans, Request);
                var jobject = JsonConvert.DeserializeObject<List<User>>(Result.content);
                return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
            }
        }

        public class qwd
        {
            public string key { get; set; }
            public string val { get; set; }
        }
    }
}