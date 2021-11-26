using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models; 

namespace OnlineShop.Controllers
{
    public class AddressController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Address
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            try
            {
                int userId = Convert.ToInt32(Session["userLogin"]);
                var address = db.UsersAdresses.Where(r => r.userId == userId && r.id == id).FirstOrDefault();
                if (address != null)
                {
                    ViewBag.id = id;
                    return View();
                }
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Edit", "AddressController");
                throw;
            }
           
        }

        public ActionResult add()
        {
            if (Session["userLogin"]!=null)
            {
                return View();
            }
            else
            {
                return Json(string.Empty);
            }
        }
        [HttpPost]
        public ActionResult store(UsersAdress us)
        {
            

            if (Session["userlogin"]!=null)
            {
                try
                {
                    if (us != null && !string.IsNullOrEmpty(us.Address) && !string.IsNullOrEmpty(us.Reciver) && !string.IsNullOrEmpty(us.ReciverPhone) && us.State != null && us.City != null && !string.IsNullOrEmpty(us.postalCode))
                    {
                        int userid = Convert.ToInt32(Session["userLogin"]);
                        us.userId = userid;
                        db.UsersAdresses.Add(us);
                        db.SaveChanges();
                        var basket = Session["basket"] as Basket;
                        basket.userId = us.id;
                        Session["basket"] = basket;
                        return RedirectToAction("index", "shipping");
                    }
                    else
                    {
                        return Json(new { status = 500, text = "لطفا اطلاعات آدرس خود را بصورت کامل وارد نمائید" });
                        //Response.StatusCode = 202;
                        //Response.Write("لطفا اطلاعات آدرس خود را بصورت کامل وارد نمائید");
                        //return null;
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "store", "AddressController");
                    throw;
                }
               
            }
            return Json(string.Empty);
        }

        public JsonResult cities(int id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            return Json(db.Cities.Where(r => r.StateId == id).ToList(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult remove(int id)
        {
            if (Session["userLogin"]!=null)
            {
                try
                {
                    var userid = Convert.ToInt32(Session["userLogin"]);
                    var address = db.UsersAdresses.Where(r => r.id == id && r.userId == userid).FirstOrDefault();
                    if (address != null)
                    {
                        db.UsersAdresses.Remove(address);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "AddressController");
                    throw;
                }
                
            }
            return Json(new { status = 200});
        }
        [HttpPost]
        public JsonResult update(int id , UsersAdress us)
        {
            if (Session["userLogin"]!=null)
            {
                try
                {
                    var userAddress = db.UsersAdresses.Where(r => r.id == id).FirstOrDefault();
                    int userid = Convert.ToInt32(Session["userLogin"]);
                    if (userAddress.userId == userid)
                    {
                        us.id = userAddress.id;
                        us.userId = userAddress.userId;
                        db.UsersAdresses.AddOrUpdate(us);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "update", "AddressController");
                    throw;
                }
               

            }
            return Json(string.Empty);

        }

    }
}