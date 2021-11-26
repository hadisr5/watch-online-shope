using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using OnlineShop.Models;


namespace OnlineShop.Areas.Admin.Controllers
{
    public class ExcelTextController : Controller
    {

        // GET: Admin/ExcelText
        public ActionResult Index()
        {
            return View();

        }
  
    }
}