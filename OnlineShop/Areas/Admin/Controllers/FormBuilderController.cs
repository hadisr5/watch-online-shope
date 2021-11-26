using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnlineShop.Models;
using OnlineShop.Class;
using Microsoft.Ajax.Utilities;
using System.Data.Entity.Migrations;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class FormBuilderController : Controller
    {
        checkLoginAdmin ch =new checkLoginAdmin();
        OnlineShopEntities db = new OnlineShopEntities();


        // GET: Admin/FormBuilder
        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["login"]!=null)
            {
                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public ActionResult Create()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }
        public ActionResult Edit(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                ViewBag.id = id; 
                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }


        [HttpPost]
        public JsonResult edit(FormBuilder fm)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                db.FormBuilders.AddOrUpdate(fm);
                db.SaveChanges();
                return Json(string.Empty);
            }
            else
            {
                return Json(string.Empty);
            }
        }

        [HttpPost]
        public JsonResult remove(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                var fm = db.FormBuilders.Find(id);
                db.FormBuilders.Remove(fm);
                db.SaveChanges();
                return Json(string.Empty);
            }
            else
            {
                return Json(string.Empty);
            }
        }


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
                    var Result = fn.CreateTable("FormBuilder", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<FormBuilder>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }

        public ActionResult Build(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                ViewBag.id = id;
                return View();
            }
            return Json(string.Empty);
        }



        [HttpPost]
        public JsonResult Create_Form(FormBuilder form)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                db.FormBuilders.Add(form);
                db.SaveChanges();
            }
            return Json(string.Empty);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Design(int id)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                string val = Request.Form["inputs"];
                var model = JsonConvert.DeserializeObject<List<Form_Input>>(val);
                foreach (var item in model)
                {
                    item.FormBuilderId = id; 
                    db.Form_Input.Add(item);
                    db.SaveChanges();
                }
            }
            return Json(string.Empty);
        }


    }
}