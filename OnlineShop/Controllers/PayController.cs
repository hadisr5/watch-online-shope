using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Runtime.InteropServices.ComTypes;
using OnlineShop.Areas.Admin.Functions;

namespace OnlineShop.Controllers
{
    public class PayController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        OnlineShopEntities dc = new OnlineShopEntities();

        public ActionResult Zarinpal()
        {
            return View();
        }

        public ActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public JsonResult PaymentSubmition(string Gateway)
        {
            if (Session["userLogin"] != null && Session["Basket"] != null && Session["Shipping"] != null)
            {
                try
                {
                    if (Gateway == "DoorToDoor")
                    {
                        int userid = Convert.ToInt32(Session["userLogin"]);
                        var basket = (Session["basket"]) as Basket;
                        var orders = Session["Orders"] as List<Order>;
                        var Shipping = Session["Shipping"] as Shipping;
                        var discount = new DiscountCode();
                        Int64 discountAmont = 0;
                        if (Session["Discount"] != null)
                        {
                            discount = Session["Discount"] as DiscountCode;
                            if (discount.Amont != null && discount.Amont != 0)
                            {
                                discountAmont = Convert.ToInt64(discount.Amont);
                            }
                            else
                            {
                                if (discount.Percentage != null && discount.Percentage != 0)
                                {
                                    int discountPercentage = 0;

                                    discountPercentage = Convert.ToInt32(discount.Percentage);
                                    var disPercentage = basketPrice() * discountPercentage / 100;
                                    if (discount.maxPercentage != null && discount.maxPercentage != 0)
                                    {
                                        if (disPercentage > discount.maxPercentage)
                                        {
                                            discountAmont = Convert.ToInt64(discount.maxPercentage);
                                        }
                                        else
                                        {
                                            discountAmont = disPercentage;
                                        }
                                    }
                                    else
                                    {
                                        discountAmont = disPercentage;
                                    }
                                }
                                if (discount != null && discount.id != 0)
                                {
                                    basket.DiscountCode = discount.id;
                                    basket.DiscountAmount = discountAmont;
                                }
                            }

                        }
                        if (basket.addressId == null)
                        {
                            var lastAddress = db.UsersAdresses.Where(r => r.userId == userid).OrderByDescending(r => r.id).ToList().FirstOrDefault();
                            basket.addressId = lastAddress.id;
                        }
                        basket.status = "در صف بررسی";
                        basket.payed = false;
                        basket.payMethod = "پرداخت درب منزل";
                        basket.creationDate = DateTime.Now;
                        basket.userId = userid;
                        basket.Price = basketPrice();
                        if (Shipping.id == 0 || Shipping.id == null)
                        {
                            db.Shippings.Add(Shipping);
                            db.SaveChanges();
                        }
                        //basket.Shipping = null;
                        basket.ShippingId = null;
                        basket.wizardStatus = "در صف بررسی";
                        basket.ShippingId = Shipping.id;
                        if (basket.id == 0)
                        {
                            db.Baskets.Add(basket);
                        }
                        db.SaveChanges();
                        if (Session["SendPeriod"] != null)
                        {
                            var send = Session["SendPeriod"] as SendPeriod;
                            if (send.id == null || send.id == 0)
                            {
                                send.isPost = false;
                                send.isDelivery = true;
                                send.BasketId = basket.id;
                                db.SendPeriods.Add(send);
                            }
                        }
                        db.SaveChanges();
                        if (Session["Discount"] != null)
                        {
                            var discount_for_save = db.DiscountCodes.Where(r => r.id == discount.id).FirstOrDefault();
                            discount_for_save.isUsed = true;
                            discount_for_save.useDate = DateTime.Now;
                            discount_for_save.basketId = basket.id;

                        }
                        Shipping.BasketId = basket.id;
                        basket.Orders = null;
                        db.SaveChanges();
                        if (orders.Count != 0)
                        {
                            foreach (var item in orders)
                            {
                                item.basketId = basket.id;
                                dc.Orders.Add(item);
                                dc.SaveChanges();
                            }
                        }
                        var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                        var setting = db.Settings.FirstOrDefault();
                        SMS sms = new SMS();
                        sms.Send(user.mobile, basket.id.ToString(), Convert.ToInt32(setting.smsToken_submittedOrder));
                        Session.Remove("basket");
                        Session.Remove("Orders");
                        Session.Remove("Shipping");
                        Session.Remove("Discount");
                        return Json(new { url = "/pay/DoorToDoorSuccess/" + basket.id });
                    }
                    else
                    {
                        if (Gateway == "Zarinpal")
                        {
                            return Json(new { url = "/zarinpalPay/pay/" });

                        }
                        else
                        {
                            if (Gateway == "WalletPay")
                            {
                                int userid = Convert.ToInt32(Session["userLogin"]);
                                var basket = (Session["basket"]) as Basket;
                                var orders = Session["Orders"] as List<Order>;
                                var Shipping = Session["Shipping"] as Shipping;
                                var discount = new DiscountCode();
                                Int64 discountAmont = 0;
                                if (Session["Discount"] != null)
                                {
                                    discount = Session["Discount"] as DiscountCode;
                                    if (discount.Amont != null && discount.Amont != 0)
                                    {
                                        discountAmont = Convert.ToInt64(discount.Amont);
                                    }
                                    else
                                    {
                                        if (discount.Percentage != null && discount.Percentage != 0)
                                        {
                                            int discountPercentage = 0;

                                            discountPercentage = Convert.ToInt32(discount.Percentage);
                                            var disPercentage = basketPrice() * discountPercentage / 100;
                                            if (discount.maxPercentage != null && discount.maxPercentage != 0)
                                            {
                                                if (disPercentage > discount.maxPercentage)
                                                {
                                                    discountAmont = Convert.ToInt64(discount.maxPercentage);
                                                }
                                                else
                                                {
                                                    discountAmont = disPercentage;
                                                }
                                            }
                                            else
                                            {
                                                discountAmont = disPercentage;
                                            }
                                        }
                                        if (discount != null && discount.id != 0)
                                        {
                                            basket.DiscountCode = discount.id;
                                            basket.DiscountAmount = discountAmont;
                                        }
                                    }
                                }
                                if (basket.addressId == null)
                                {
                                    var lastAddress = db.UsersAdresses.Where(r => r.userId == userid).OrderByDescending(r => r.id).ToList().FirstOrDefault();
                                    basket.addressId = lastAddress.id;
                                }
                                basket.wizardStatus = "در صف بررسی";
                                basket.status = "پرداخت با کیف پول";
                                basket.payed = false;
                                basket.payMethod = "پرداخت با کیف پول";
                                basket.creationDate = DateTime.Now;
                                basket.userId = userid;
                                basket.Price = basketPrice();
                                if (Shipping.id == 0 || Shipping.id == null)
                                {
                                    db.Shippings.Add(Shipping);
                                    db.SaveChanges();
                                }
                                //basket.Shipping = null;
                                basket.ShippingId = null;
                                basket.wizardStatus = "در صف بررسی";
                                basket.ShippingId = Shipping.id;
                                if (basket.id == 0)
                                {
                                    db.Baskets.Add(basket);
                                }
                                db.SaveChanges();
                                if (Session["SendPeriod"] != null)
                                {
                                    var send = Session["SendPeriod"] as SendPeriod;
                                    if (send.id == null || send.id == 0)
                                    {
                                        send.isPost = false;
                                        send.isDelivery = true;
                                        send.BasketId = basket.id;
                                        db.SendPeriods.Add(send);
                                    }
                                }
                                db.SaveChanges();
                                if (Session["Discount"] != null)
                                {
                                    var discount_for_save = db.DiscountCodes.Where(r => r.id == discount.id).FirstOrDefault();
                                    discount_for_save.isUsed = true;
                                    discount_for_save.useDate = DateTime.Now;
                                    discount_for_save.basketId = basket.id;
                                }
                                Shipping.BasketId = basket.id;
                                basket.Orders = null;
                                if (WalletMoney() >= basketPrice())
                                {
                                    var WalletPay = new WalletPay();
                                    WalletPay.amount = basketPrice() + Shipping.ShippingPrice - discountAmont;
                                    WalletPay.basketId = basket.id;
                                    WalletPay.userid = userid;
                                    WalletPay.creationDate = DateTime.Now;
                                    db.WalletPays.Add(WalletPay);
                                    basket.payed = true;
                                }
                                db.SaveChanges();
                                if (orders.Count != 0)
                                {
                                    foreach (var item in orders)
                                    {
                                        item.basketId = basket.id;
                                        dc.Orders.Add(item);
                                        dc.SaveChanges();
                                    }
                                }
                                var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                                var setting = db.Settings.FirstOrDefault();
                                SMS sms = new SMS();
                                sms.Send(user.mobile, basket.id.ToString(), Convert.ToInt32(setting.smsToken_submittedOrder));
                                Session.Remove("basket");
                                Session.Remove("Orders");
                                Session.Remove("Shipping");
                                Session.Remove("Discount");
                                return Json(new { url = "/pay/DoorToDoorSuccess/" + basket.id });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "PaymentSubmition", "PayController");
                    throw;
                }
               

            }
            return Json(string.Empty);
        }

        public ActionResult DoorToDoorSuccess(int id)
        {
            if (Session["userLogin"] != null)
            {
                ViewBag.id = id;
                return View();
            }
            return Json(string.Empty);
        }

        public ActionResult updatePayment()
        {
            if (Session["userlogin"] != null && Session["basket"] != null)
            {
                return View();
            }
            else
            {
                return Json(string.Empty);

            }
        }


        [HttpPost]
        public JsonResult discount(string code, string type)
        {
            try
            {

            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "discount", "PayController");
                throw;
            }
            if (type == "delete")
            {
                Session.Remove("Discount");
                return Json(new { status = 201, error = "کد تخفیف حذف شد" });

            }
            else
            {
                if (type == "create")
                {
                    var discount = db.DiscountCodes.Where(r => r.code == code).FirstOrDefault();
                    bool DiscountPercentage = false;
                    if (discount == null)
                    {
                        return Json(new { status = 201, error = "کد تخفیف وارد شده اشتباه است" });
                    }
                    else
                    {
                        if (discount.isUsed == true)
                        {
                            return Json(new { status = 201, error = "کد تخفیف وارد شده پیش تر استفاده شده است" });
                        }
                        else
                        {
                            var shippingPrice = 0;
                            if (Session["Shipping"] != null)
                            {
                                var shipping = Session["Shipping"] as Shipping;
                                shippingPrice = Convert.ToInt32(shipping.ShippingPrice);
                            }
                            Session["Discount"] = discount;
                            var basket = Session["Basket"] as Basket;

                            var pay = basketPrice() - discount.Amont + shippingPrice;
                            Int64 discountAmont = 0;
                            if (discount.Amont != null && discount.Amont != 0)
                            {
                                discountAmont = Convert.ToInt64(discount.Amont);
                                basket.DiscountAmount = discountAmont;
                            }
                            else
                            {
                                if (discount.Percentage != null && discount.Percentage != 0)
                                {
                                    DiscountPercentage = true;
                                    int discountPercentage = 0;
                                    pay = basketPrice() - ((basketPrice() * discount.Percentage) / 100) + shippingPrice;

                                    discountPercentage = Convert.ToInt32(discount.Percentage);
                                    var test1 = basketPrice(); 
                                    var test2 = shippingPrice; 
                                    var disPercentage = basketPrice() * discountPercentage / 100;
                                    if (discount.maxPercentage != null && discount.maxPercentage != 0)
                                    {
                                        if (disPercentage > discount.maxPercentage)
                                        {
                                            discountAmont = Convert.ToInt64(discount.maxPercentage);
                                            pay = basketPrice() - (discount.maxPercentage) + shippingPrice;

                                            basket.DiscountAmount = discountAmont;
                                        }
                                        else
                                        {
                                            discountAmont = disPercentage;
                                            basket.DiscountAmount = discountAmont;
                                        }
                                    }
                                    else
                                    {
                                        discountAmont = disPercentage;
                                        basket.DiscountAmount = discountAmont;

                                    }
                                }
                            }
                            Session["Basket"] = basket;

                            return Json(new { status = 400, amount = discountAmont, total = basketPrice() + shippingPrice, pay = pay, discountpercentage = DiscountPercentage });
                        }
                    }
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
                OnlineShop.Class.Utility.CreateLog(ex, "basketPrice", "PayController");
                throw;
            }
           
        }

        private Int64 WalletMoney()
        {
            try
            {
                long Total = 0;
                int userid = Convert.ToInt32(Session["userLogin"]);
                var allPays = db.Wallets.Where(r => r.userId == userid && (r.payByAdmin == true || r.Success == true)).ToList();

                Total += Convert.ToInt64(allPays.Sum(r => r.amount));

                var allMinus = db.WalletPays.Where(r => r.userid == userid).ToList();
                if (allMinus.Count != 0)
                {
                    Total -= Convert.ToInt64(allMinus.Sum(r => r.amount));
                }
                return Total;
            } 
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "WalletMoney", "PayController");
                throw;
            }
           
        }

    }
}