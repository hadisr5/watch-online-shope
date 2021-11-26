using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
namespace OnlineShop.Controllers
{
    public class ShippingController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();

        // GET: Shipping
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult choose_address(int id)
        {
            if (Session["userLogin"]!=null)
            {
                try
                {
                    int userid = Convert.ToInt32(Session["userLogin"]);
                    var basket = Session["basket"] as Basket;
                    var address = db.UsersAdresses.Where(r => r.userId == userid && r.id == id).FirstOrDefault();
                    if (address != null)
                    {
                        basket.addressId = id;
                        Session["basket"] = basket;
                        return RedirectToAction("index");
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "choose_address", "ShippingController");
                    throw;
                }
               
            }

            return Json(string.Empty);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult saveShippingData(string Peroid ,int? shipping_method)
        {
            if (Session["userLogin"]!=null)
            {
                try
                {
                    var setting = db.Settings.FirstOrDefault();
                    if (Session["basket"] != null)
                    {
                        int maxOrderPerDay = Convert.ToInt32(setting.maxOrdersPerDay);
                        var basket = Session["basket"] as Basket;
                        if (basket != null)
                        {
                            if (shipping_method == 2)
                            {
                                DateTime dt = DateTime.Now;
                                int part = 0;

                                dt = Convert.ToDateTime(Peroid.Substring(0, Peroid.IndexOf("_")));
                                part = Convert.ToInt32(Peroid.Replace(dt + "_", ""));

                                var newSendPeriod = new SendPeriod();
                                newSendPeriod.CreationDate = DateTime.Now;
                                newSendPeriod.Date = dt.Date;
                                newSendPeriod.Peroid = part;
                                newSendPeriod.UserId = Convert.ToInt32(Session["userLogin"]);

                                if (db.SendPeriods.Where(r => r.Date == dt.Date && r.Peroid == newSendPeriod.Peroid).ToList().Count <= maxOrderPerDay)
                                {
                                    var shipping = new Shipping();
                                    shipping.ShippingTypeString = "پیک موتوری";
                                    if (basketPrice() > setting.shippingFree)
                                    {
                                        shipping.ShippingPrice = 0;
                                    }
                                    else
                                    {
                                        shipping.ShippingPrice = setting.delivery_Price;
                                    }
                                    shipping.ShippingPrice = setting.delivery_Price;
                                    shipping.ShippingType = 2;
                                    shipping.CreationDate = DateTime.Now;
                                    shipping.DeliveryDate = newSendPeriod.Date;
                                    shipping.DeliveryPeriod = part;

                                    Session["Shipping"] = shipping;
                                    Session["SendPeriod"] = newSendPeriod;

                                    return Json(new { status = 200 });
                                }
                                else
                                {
                                    return Json(new { error = "تعداد سفارش های روز انتخاب شده تکمیل است" });
                                }

                            }
                            else
                            {
                                var shipping = new Shipping();

                                shipping.ShippingTypeString = "پُست";
                                shipping.ShippingPrice = setting.shippingPrice;
                                shipping.ShippingType = 1;
                                shipping.CreationDate = DateTime.Now;

                                Session["Shipping"] = shipping;

                                return Json(new { status = 200 });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "saveShippingData", "ShippingController");
                    throw;
                }
                
            }
            return Json(string.Empty);
        }



        public Int64 basketPrice()
        {
            try
            {
                Int64 sum = 0;
                int userid = Convert.ToInt32(Session["userLogin"]);
                var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                var prime = false;
                if (user.Special == true)
                {
                    prime = true;
                }
                var orders = Session["orders"] as List<Order>;
                if (orders == null || orders.Count == 0)
                {
                    return 0;
                }
                foreach (var item in orders)
                {
                    var pro = db.Products.Where(r => r.id == item.productId).FirstOrDefault();
                    var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == item.productId && r.expire > DateTime.Now).FirstOrDefault();
                    if (true)
                    {
                        var sellerPro = db.SellersProducts.Where(r => r.sellerId == item.SellerId && r.productId == item.productId).FirstOrDefault();
                        if (sellerPro == null || item.SellerId == null)
                        {
                            if (off != null)
                            {
                                if (prime)
                                {
                                    sum += Convert.ToInt64((pro.priceForSpecialUsers - (pro.priceForSpecialUsers * off.percentage) / 100) * item.quantity);

                                }
                                else
                                {
                                    sum += Convert.ToInt64((pro.price - (pro.price * off.percentage) / 100) * item.quantity);

                                }
                            }
                            else
                            {
                                if (prime)
                                {
                                    sum += Convert.ToInt64((pro.priceForSpecialUsers) * item.quantity);

                                }
                                else
                                {
                                    sum += Convert.ToInt64((pro.price) * item.quantity);

                                }
                            }
                        }
                        else
                        {
                            sum += Convert.ToInt64((sellerPro.price) * item.quantity);
                        }
                    }
                }

                return sum;
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "basketPrice", "ShippingController");
                throw;
            }
            
        }


    }
}