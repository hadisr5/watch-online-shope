using Newtonsoft.Json;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OnlineShop.Areas.UserProfile.Controllers
{
    public class PayInWalletController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        //[HttpPost]
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Pay(int id)
        {
            if (Session["userLogin"] != null)
            {
                var wallet = db.Wallets.Find(id);
                var setting = db.Settings.FirstOrDefault();

                var payment = new Payment();
                payment.Walletid = id; 
                payment.isWallet = true;
                payment.CreationDate = DateTime.Now;
                payment.Amount = wallet.amount; 
                db.Payments.Add(payment);
                db.SaveChanges();


                var rq = new ZARINAPL_request();
                rq.merchant_id = setting.zarinpal_merchant_id;
                rq.amount =Convert.ToInt32( wallet.amount) * 10;
                rq.callback_url = setting.WebsiteUrl + "/userProfile/PayInWallet/verify/" + wallet.id;
                rq.description = "خرید از " + setting.WebsiteFullname;

                var metaDt = new ZARINAPL_Metadata();
                metaDt.email = "omidalaienovin@gmail.com";
                metaDt.mobile = "09332160127";
                rq.metadata = metaDt;
                rq.amount = Convert.ToInt32(wallet.amount) * 10;
                var postBackRequest = SendRequestData(rq);
                if (postBackRequest != null && postBackRequest.data.authority != null)
                {
                    wallet.Token = postBackRequest.data.authority;
                    //Basket.authority = postBackRequest.data.authority;
                    db.SaveChanges();
                    string url = "https://www.zarinpal.com/pg/StartPay/" + postBackRequest.data.authority;
                    Response.Redirect(url);
                }
            }
            return Json(string.Empty);

        }
        public ActionResult Verify(int id, string Status, string Authority, Boolean? Deposit)
        {
            var wallet = db.Wallets.Find(id);
            //Int32 sum = 0;
            if (wallet != null && wallet.Token == Authority)
            {
                //long refID = -1;
                //int verf = -21;
                if (Status == "OK")
                {
                    var setting = db.Settings.FirstOrDefault();
                    var zp_verRequest = new ZARINPAL_Verify();
                    zp_verRequest.amount = Convert.ToInt32(wallet.amount) * 10;
                    zp_verRequest.authority = Authority;
                    zp_verRequest.merchant_id = setting.zarinpal_merchant_id;

                    var result = SendVerify(zp_verRequest);
                    if (result != null)
                    {
                        if (result.data.code > 0)
                        {
                            var payment = db.Payments.Where(r => r.Walletid == id).FirstOrDefault();
                            payment.GateWayString = "زرین‌پال";
                            payment.UserId = wallet.userId;
                            payment.Status = 1;
                            payment.isWallet = true;
                            payment.StatusString = "پرداخت شده";
                            payment.Token = result.data.ref_id.ToString();
                            payment.GatewayId = 1;
                            payment.Amount = wallet.amount;
                            payment.CreationDate = DateTime.Now;
                            //payment.BasketId = basket.id;
                            db.Payments.Add(payment);

                            wallet.Success = true;
                            wallet.Token = result.data.ref_id.ToString();
                            //basket.payed = true;
                            db.SaveChanges();

                        }
                        else
                        {
                            var payment = db.Payments.Where(r => r.Walletid == id).FirstOrDefault();

                            payment.GateWayString = "زرین‌پال";
                            payment.UserId = wallet.userId;
                            payment.Status = -1;
                            payment.isWallet = true;
                            payment.StatusString = "پرداخت ناموفق";
                            payment.Token = result.data.ref_id.ToString();
                            payment.GatewayId = 1;
                            payment.Amount = wallet.amount;
                            payment.CreationDate = DateTime.Now;
                            //payment.BasketId = basket.id;
                            db.Payments.Add(payment);



                            wallet.Success = false;
                            wallet.Token = string.Empty;
                            //wal.payed = false;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var payment = db.Payments.Where(r => r.Walletid == id).FirstOrDefault();

                        payment.GateWayString = "زرین‌پال";
                        payment.UserId = wallet.userId;
                        payment.Status = -1;
                        payment.isWallet = true;
                        payment.StatusString = "پرداخت ناموفق";
                        payment.Token = result.data.ref_id.ToString();
                        payment.GatewayId = 1;
                        payment.Amount = wallet.amount;
                        payment.CreationDate = DateTime.Now;
                        //payment.BasketId = basket.id;
                        db.Payments.Add(payment);



                        wallet.Success = false;
                        wallet.Token = string.Empty;
                        //wal.payed = false;
                        db.SaveChanges();
                    }


                }
                else
                {
                    var payment = db.Payments.Where(r => r.Walletid == id).FirstOrDefault();

                    payment.GateWayString = "زرین‌پال";
                    payment.UserId = wallet.userId;
                    payment.Status = -1;
                    payment.isWallet = true;
                    payment.StatusString = "پرداخت ناموفق";
                    //payment.Token = result.data.ref_id.ToString();
                    payment.GatewayId = 1;
                    payment.Amount = wallet.amount;
                    payment.CreationDate = DateTime.Now;
                    //payment.BasketId = basket.id;
                    db.Payments.Add(payment);

                    wallet.Success = false;
                    wallet.Token = string.Empty;
                    //wal.payed = false;
                    db.SaveChanges();
                }
            }
            //Session.Remove("orders");
            //Session.Remove("basket");


            return RedirectToAction("SuccessWalletPay", "Profile", new { id = wallet.id });


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