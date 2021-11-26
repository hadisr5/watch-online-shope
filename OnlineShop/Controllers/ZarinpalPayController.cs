using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using OnlineShop.Models;


namespace OnlineShop.Controllers
{
    public class ZarinpalPayController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        //[HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Pay()
        {
            if (Session["userLogin"] != null)
            {
                try
                {
                    var setting = db.Settings.FirstOrDefault();
                    Int32 sum = 0;
                    int userId = Convert.ToInt32(Session["userLogin"]);
                    var Orders = (Session["orders"]) as List<Order>;
                    var Basket = Session["Basket"] as Basket;


                    Basket.creationDate = DateTime.Now;
                    Basket.userId = userId;
                    Basket.payMethod = "پرداخت آنلاین-درگاه زرین پال";

                    Basket.Price = Calculate() * 10;
                    Basket.payed = false;
                    Basket.status = "در انتظار پرداخت";

                    db.Baskets.Add(Basket);
                    db.SaveChanges();

                    var payment = new Payment();
                    payment.BasketId = Basket.id;
                    payment.isWallet = false;
                    payment.CreationDate = DateTime.Now;
                    payment.Amount = sum * 10;
                    db.Payments.Add(payment);
                    db.SaveChanges();



                    var rq = new ZARINAPL_request();
                    rq.merchant_id = setting.zarinpal_merchant_id;
                    rq.amount = sum * 10;
                    rq.callback_url = setting.WebsiteUrl + "ZarinpalPay/verify/" + Basket.id;
                    rq.description = "خرید از " + setting.WebsiteFullname;

                    var metaDt = new ZARINAPL_Metadata();
                    metaDt.email = "omidalaienovin@gmail.com";
                    metaDt.mobile = "09332160127";
                    rq.metadata = metaDt;
                    rq.amount = Calculate() * 10;
                    var postBackRequest = SendRequestData(rq);

                    if (postBackRequest != null && postBackRequest.data.authority != null)
                    {
                        Basket.authority = postBackRequest.data.authority;
                        db.SaveChanges();
                        string url = "https://www.zarinpal.com/pg/StartPay/" + postBackRequest.data.authority;
                        Response.Redirect(url);
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Pay", "ZarinpalPayController");
                    throw;
                }
               
            }
            return Json(string.Empty);

        }
        public ActionResult Verify(int id, string Status, string Authority, Boolean? Deposit)
        {
            if (Deposit == null)
            {
                try
                {
                    var basket = db.Baskets.Find(id);
                    //Int32 sum = 0;
                    if (basket != null && basket.authority == Authority)
                    {
                        //long refID = -1;
                        //int verf = -21;
                        if (Status == "OK")
                        {
                            var setting = db.Settings.FirstOrDefault();
                            var zp_verRequest = new ZARINPAL_Verify();
                            zp_verRequest.amount = Convert.ToInt32(basket.Price) * 10;
                            zp_verRequest.authority = Authority;
                            zp_verRequest.merchant_id = setting.zarinpal_merchant_id;

                            var result = SendVerify(zp_verRequest);
                            if (result != null)
                            {
                                if (result.data.code > 0)
                                {
                                    var payment = db.Payments.Where(r => r.BasketId == id).FirstOrDefault();
                                    payment.GateWayString = "زرین‌پال";
                                    payment.UserId = basket.userId;
                                    payment.Status = 1;
                                    payment.isWallet = false;
                                    payment.StatusString = "پرداخت شده";
                                    payment.Token = result.data.ref_id.ToString();
                                    payment.GatewayId = 1;
                                    payment.Amount = basket.Price;
                                    payment.CreationDate = DateTime.Now;
                                    payment.BasketId = basket.id;
                                    db.Payments.Add(payment);

                                    basket.status = "پرداخت موفق";
                                    basket.token = result.data.ref_id.ToString();
                                    basket.payed = true;
                                    db.SaveChanges();

                                }
                                else
                                {
                                    var payment = db.Payments.Where(r => r.BasketId == id).FirstOrDefault();

                                    payment.GateWayString = "زرین‌پال";
                                    payment.UserId = basket.userId;
                                    payment.Status = -1;
                                    payment.isWallet = false;
                                    payment.StatusString = "پرداخت ناموفق";
                                    payment.Token = result.data.ref_id.ToString();
                                    payment.GatewayId = 1;
                                    payment.Amount = basket.Price;
                                    payment.CreationDate = DateTime.Now;
                                    payment.BasketId = basket.id;
                                    db.Payments.Add(payment);



                                    basket.status = "پرداخت ناموفق";
                                    basket.token = string.Empty;
                                    basket.payed = false;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                var payment = db.Payments.Where(r => r.BasketId == id).FirstOrDefault();

                                payment.GateWayString = "زرین‌پال";
                                payment.UserId = basket.userId;
                                payment.Status = -1;
                                payment.isWallet = false;
                                payment.StatusString = "پرداخت ناموفق";
                                payment.Token = result.data.ref_id.ToString();
                                payment.GatewayId = 1;
                                payment.Amount = basket.Price;
                                payment.CreationDate = DateTime.Now;
                                payment.BasketId = basket.id;
                                db.Payments.Add(payment);


                                basket.status = "پرداخت ناموفق";
                                basket.token = string.Empty;
                                basket.payed = false;
                                db.SaveChanges();
                            }


                        }
                        else
                        {
                            var payment = db.Payments.Where(r => r.BasketId == id).FirstOrDefault();

                            payment.GateWayString = "زرین‌پال";
                            payment.UserId = basket.userId;
                            payment.Status = -1;
                            payment.isWallet = false;
                            payment.StatusString = "پرداخت ناموفق";
                            payment.GatewayId = 1;
                            payment.Amount = basket.Price;
                            payment.CreationDate = DateTime.Now;
                            payment.BasketId = basket.id;
                            db.Payments.Add(payment);



                            basket.status = "پرداخت ناموفق";
                            basket.token = string.Empty;
                            basket.payed = false;
                            db.SaveChanges();
                        }
                    }
                    Session.Remove("orders");
                    Session.Remove("basket");


                    return RedirectToAction("successOrder", "home", new { id = basket.id });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Verify", "ZarinpalPayController");
                    throw;
                }
              
            }
            else
            {
                try
                {
                    var basket = db.Baskets.Find(id);
                    Int32 sum = 0;
                    if (basket != null && basket.authority == Authority)
                    {
                        if (basket.Orders.Count != 0)
                        {
                            foreach (var item in basket.Orders)
                            {
                                item.basketId = basket.id;
                                var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == item.productId && r.expire > DateTime.Now).FirstOrDefault();
                                var pro = db.Products.Where(r => r.id == item.productId).FirstOrDefault();
                                var sellerPro = db.SellersProducts.Where(r => r.sellerId == item.SellerId && r.productId == item.productId).FirstOrDefault();
                                if (sellerPro == null)
                                {
                                    if (off != null)
                                    {
                                        sum += Convert.ToInt32((pro.price - (pro.price * off.percentage) / 100) * item.quantity);
                                    }
                                    else
                                    {
                                        sum += Convert.ToInt32((pro.price) * item.quantity);
                                    }
                                }
                                else
                                {
                                    sum += Convert.ToInt32((sellerPro.price) * item.quantity);
                                }
                            }
                            basket.Price = sum;
                            basket.payed = false;
                            basket.status = "در انتظار پرداخت";
                            db.SaveChanges();
                        }
                        var setting = db.Settings.FirstOrDefault();
                        if (Status == "OK")
                        {
                            var zp_verRequest = new ZARINPAL_Verify();

                            //int perc = Convert.ToInt32(setting.DepositPercentage);
                            //zp_verRequest.amount = (100 * sum % perc);
                            zp_verRequest.authority = Authority;
                            zp_verRequest.merchant_id = setting.zarinpal_merchant_id;

                            var result = SendVerify(zp_verRequest);
                            if (result != null)
                            {
                                if (result.data.code > 0)
                                {
                                    basket.status = "پرداخت موفق بیعانه";
                                    basket.token = result.data.ref_id.ToString();
                                    basket.payed = true;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    basket.status = "پرداخت ناموفق بیعانه";
                                    basket.token = string.Empty;
                                    basket.payed = false;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                basket.status = "پرداخت ناموفق بیعانه";
                                basket.token = string.Empty;
                                basket.payed = false;
                                db.SaveChanges();
                            }


                        }
                        else
                        {
                            basket.status = "پرداخت ناموفق";
                            basket.token = string.Empty;
                            basket.payed = false;
                            db.SaveChanges();
                        }
                    }
                    Session.Remove("orders");
                    Session.Remove("basket");


                    return RedirectToAction("successOrder", "home", new { id = basket.id });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Verify", "ZarinpalPayController");
                    throw;
                }
               
            }



        }
        public Int32 Calculate()
        {
            try
            {
                Int32 sum = 0;
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
                                    sum += Convert.ToInt32((pro.priceForSpecialUsers - (pro.priceForSpecialUsers * off.percentage) / 100) * item.quantity);

                                }
                                else
                                {
                                    sum += Convert.ToInt32((pro.price - (pro.price * off.percentage) / 100) * item.quantity);

                                }
                            }
                            else
                            {
                                if (prime)
                                {
                                    sum += Convert.ToInt32((pro.priceForSpecialUsers) * item.quantity);

                                }
                                else
                                {
                                    sum += Convert.ToInt32((pro.price) * item.quantity);

                                }
                            }
                        }
                        else
                        {
                            sum += Convert.ToInt32((sellerPro.price) * item.quantity);
                        }
                    }
                }
                var Shipping = Session["Shipping"] as Shipping;

                var discount = new DiscountCode();
                Int32 discountAmont = 0;
                if (Session["Discount"] != null)
                {
                    discount = Session["Discount"] as DiscountCode;
                    if (discount.Amont != null && discount.Amont != 0)
                    {
                        discountAmont = Convert.ToInt32(discount.Amont);
                    }
                    else
                    {
                        if (discount.Percentage != null && discount.Percentage != 0)
                        {
                            int discountPercentage = 0;

                            discountPercentage = Convert.ToInt32(discount.Percentage);
                            var disPercentage = sum * discountPercentage / 100;
                            if (discount.maxPercentage != null && discount.maxPercentage != 0)
                            {
                                if (disPercentage > discount.maxPercentage)
                                {
                                    discountAmont = Convert.ToInt32(discount.maxPercentage);
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

                    }

                }










                if (Shipping != null && Shipping.ShippingPrice != null)
                {
                    sum += Convert.ToInt32(Shipping.ShippingPrice);
                }
                sum -= discountAmont;

                return sum;
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Calculate", "ZarinpalPayController");
                throw;
            }
           
        }
        public ZARINAPL_Request_data SendRequestData(ZARINAPL_request RQ)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(RQ, Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });
                var baseAddress = "https://api.zarinpal.com/pg/v4/payment/request.json";
                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";

                string parsedContent = jsonString;
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<ZARINAPL_Request_data>(content);
            }
            catch (Exception er)
            {
                OnlineShop.Class.Utility.CreateLog(er, "SendRequestData", "ZarinpalPayController");
                var request = new ZARINAPL_Request_data();
                return request;
            }
        
        }
        public ZARINPAL_verify_data_container SendVerify(ZARINPAL_Verify verify)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(verify, Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                });
                var baseAddress = "https://api.zarinpal.com/pg/v4/payment/verify.json";
                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";
                string parsedContent = jsonString;
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] bytes = encoding.GetBytes(parsedContent);
                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();
                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                var content = sr.ReadToEnd();
                content = content.Replace("[]", "{}");
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var errors = new Errors();
                var data = new ZARINPAL_verify_data();
                return serializer.Deserialize<ZARINPAL_verify_data_container>(content);
            }
            catch (Exception er)
            {
                OnlineShop.Class.Utility.CreateLog(er, "SendVerify", "ZarinpalPayController");
                ZARINPAL_verify_data_container zp = new ZARINPAL_verify_data_container();
                return zp;
            }
          
        }
        public class Validation
        {
            public string merchant_id { get; set; }
            public string amount { get; set; }
            public string authority { get; set; }
        }
        public class Errors
        {
            public int code { get; set; }
            public string message { get; set; }
            public List<Validation> validations { get; set; }
        }
        public class ZARINPAL_verify_data_container
        {
            public ZARINPAL_verify_data data { get; set; }
            public Errors errors { get; set; }
        }
        public class ZARINPAL_Verify
        {
            public string merchant_id { get; set; }
            public Int32 amount { get; set; }
            public string authority { get; set; }

        }
        public class ZARINPAL_verify_data
        {
            public string card_pan { get; set; }
            public int code { get; set; }
            public string ref_id { get; set; }
            public string card_hash { get; set; }
            public string fee_type { get; set; }
            public int fee { get; set; }
        }
        public class ZARINAPL_Data
        {
            public int code { get; set; }
            public string message { get; set; }
            public string authority { get; set; }
            public string fee_type { get; set; }
            public int fee { get; set; }
        }
        public class ZARINAPL_Metadata
        {
            public string mobile { get; set; }
            public string email { get; set; }
        }
        public class ZARINAPL_request
        {
            public string merchant_id { get; set; }
            public int amount { get; set; }
            public string description { get; set; }
            public string callback_url { get; set; }
            public ZARINAPL_Metadata metadata { get; set; }
        }
        public class ZARINAPL_Validation
        {
            public string merchant_id { get; set; }
        }
        public class ZARINAPL_Errors
        {
            public int code { get; set; }
            public string message { get; set; }
            public List<ZARINAPL_Validation> validations { get; set; }
        }
        public class ZARINAPL_Request_data
        {
            public ZARINAPL_Data data { get; set; }
            public List<object> errors { get; set; }
        }
    }
}