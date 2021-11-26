using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using CaptchaMvc.HtmlHelpers;
using System.Text.RegularExpressions;
using SmsIrRestful;
using System.Globalization;
using System.Data.Entity;
using System.Xml.Linq;
using System.Configuration;
using OnlineShop.Areas.Admin.Functions;
using System.Net;
using System.Web.DynamicData;
using OnlineShop.Class;


namespace OnlineShop.Controllers
{
    public class HomeController : Controller
    {
        private static Random random = new Random();
        OnlineShopEntities db = new OnlineShopEntities();
        #region Class
        public class SearchResult
        {
            public string query { get; set; }
            public List<SearchCats> categorys { get; set; }
            public List<keywords> keywords { get; set; }
        }
        public class SearchCats
        {
            public int id { get; set; }
            public string title { get; set; }
        }
        public class keywords
        {
            public int id { get; set; }
            public string title { get; set; }
        }
        #endregion


        [Route(".well-known/acme-challenge/5WeQNP33GgyEDk4oIbFQ3EPwVQSpLwB2_HOuRV1rOZk")]
        public ActionResult SSLVerify()
        {
            byte[] temp = System.Text.Encoding.UTF8.GetBytes("5WeQNP33GgyEDk4oIbFQ3EPwVQSpLwB2_HOuRV1rOZk.FCitmLyOq1FZJDa7QTlszh2AQZGrrlm-0qFGOEIBd9E");

            return File(temp, "unknown/unknown");
        }
        [Route(".well-known/acme-challenge/9SUSLnmufb_dNl_bcE5aOAbbKzedV6Jk5blkfWTOisI")]
        public ActionResult SSLVerify2()
        {

            byte[] temp = System.Text.Encoding.UTF8.GetBytes("9SUSLnmufb_dNl_bcE5aOAbbKzedV6Jk5blkfWTOisI.FCitmLyOq1FZJDa7QTlszh2AQZGrrlm-0qFGOEIBd9E");

            return File(temp, "unknown/unknown");
        }
        [HttpPost]
        public JsonResult getProductColors(int? id ,int? sizeId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var ColorSizes = db.ColorSizes.Where(r => r.ProductId == id && r.SizeId == sizeId && r.Inventory != 0).ToList();
                if (ColorSizes.Count != 0)
                {
                    var colors = new List<Color>();
                    foreach (var item in ColorSizes)
                    {
                        var cl = db.Colors.Where(r => r.id == item.colorId).FirstOrDefault();
                        if (cl != null)
                        {
                            //cl.ColorSizes = null; 

                            colors.Add(cl);
                        }
                    }
                    if (colors.Count != 0)
                    {
                        colors = colors.Distinct().ToList();
                        return Json(colors, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(string.Empty);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "getProductColors", "HomeController");
                throw;
            }
           
        }

        #region سوالات متداول
        
        public ActionResult Faq(int? id)
        {
            if (id == null)
            {
                try
                {
                    var firstcat = db.FaqCategories.OrderBy(f => f.priority).FirstOrDefault();
                    if (firstcat != null)
                        id = firstcat.id;
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Faq", "HomeController");
                    throw;
                }
                
            }
            ViewBag.Message = "صفحه پرسش های متداول";

            return View(id);
        }
        #endregion

        public ActionResult Page(int id) {
            ViewBag.id = id; 
            return View();
        }


        [HttpPost]
        public JsonResult subscribes(string emailsubscribe)
        {
            try
            {
                var subscribe = db.Subscribes.Where(s => s.email == emailsubscribe).FirstOrDefault();
                if (subscribe != null)
                {
                    return Json(new { Result = false, Message = "این ایمیل قبلا ثبت شده است." });
                }

                db.Subscribes.Add(new Subscribe()
                {
                    CreationDate = DateTime.Now,
                    email = emailsubscribe
                });
                db.SaveChanges();
                return Json(new { Result = true, Message = "ایمیل شما ثبت شد." });
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "subscribes", "HomeController");
                throw;
            }
            
        }


        [HttpPost]
        public JsonResult Ajax_Search(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    var result = new SearchResult();
                    var cats = new List<SearchCats>();
                    var keywords = new List<keywords>();
                    query = query.Trim();

                    var categories = db.Categories.Where(r => r.title.Contains(query)).ToList();
                    if (categories.Count != 0)
                    {
                        foreach (var item in categories)
                        {
                            var newCat = new SearchCats();
                            newCat.id = item.id;
                            newCat.title = item.title;
                            cats.Add(newCat);
                        }
                    }
                    var products = db.Products.Where(r => r.title.Contains(query) || r.subTitle.Contains(query) || r.ProductTags.Select(p => p.Tag).Any(t => t.title.Contains(query))).ToList();
                    if (products.Count != 0)
                    {
                        foreach (var item in products)
                        {
                            var newkey = new keywords();
                            newkey.id = item.id;
                            newkey.title = item.title;
                            keywords.Add(newkey);

                        }
                    }
                    return Json(new { query = query, categorys = cats, keywords = keywords, status = 200 }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Ajax_Search", "HomeController");
                    throw;
                }
                
            }
            else
            {
                return Json(new { query = query, status = 200 }, JsonRequestBehavior.AllowGet);
            }
        }


        #region دریافت آدرس کاربر
        public ActionResult Address()
        {
            if (Session["orders"] != null)
            {
                if (Session["userLogin"] != null)
                {

                }
                else
                {
                    return RedirectToAction("login", "main", new { Area = "UserLogin" });
                }
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        [HttpPost]
        public JsonResult comment(int id)
        {

            string text = string.Empty;

            var omid = Request.Params.AllKeys.ToList();
            if (omid.Count != 0)
            {
                foreach (var item in omid)
                {
                    text += item + " : " + Request.Params[item] + Environment.NewLine;

                }
            }

            return Json(string.Empty);
        }



        public ActionResult Comments(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult comments(string title, string body, int id)
        {
            return RedirectToAction("product", new { id = id });
        }
        [HttpPost]
        public ActionResult applyDiscount(string code)
        {
            if (Session["orders"] != null)
            {
                var disCode = db.DiscountCodes.Where(r => r.code.ToLower() == code.ToLower()).FirstOrDefault();
                if (disCode != null)
                {
                    if (disCode.isUsed != true)
                    {
                        Session["Discount"] = disCode.id;
                        ViewBag.discountSuccess = 1;
                        return View("/Views/Home/Card.cshtml");
                    }
                    else
                    {
                        ViewBag.discountError = 1;
                        return View("/Views/Home/Card.cshtml");
                    }
                }
                else
                {
                    ViewBag.discountNotFound = 1;
                    return View("/Views/Home/Card.cshtml");
                }

            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        [HttpPost]
        public PartialViewResult Filter(int? id, string sortBy, int? has_selling_stock, string q, string price_min, string price_max , string title)
        {
            #region Get brands requests
            var brand = new List<int>();
            var brandRequest = Request.Params.AllKeys.Where(r => r.Contains("brand[")).ToList();
            if (brandRequest.Count != 0)
            {
                foreach (var item in brandRequest)
                {
                    try
                    {
                        brand.Add(Convert.ToInt16(Request.Params[item]));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            #endregion
            #region Get brands requests
            var propertiesValue = new List<string>();
            var ColorValue = new List<int>();
            var propertiesValueRequest = Request.Params.AllKeys.Where(r => r.Contains("property[")).ToList();
            var ColorValueRequest = Request.Params.AllKeys.Where(r => r.Contains("color[")).ToList();
            
            if (propertiesValueRequest.Count != 0)
            {
                foreach (var item in propertiesValueRequest)
                {
                    propertiesValue.Add(Request.Params[item]);
                }
            }
            if (ColorValueRequest.Count != 0)
            {
                foreach (var item in ColorValueRequest)
                {
                    try
                    {
                        ColorValue.Add(Convert.ToInt32(Request.Params[item]));
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            #endregion

            var productList = new List<Product>();
            var catPro = db.CatProes.Where(r => r.categoryId == id).ToList();
            if (catPro.Count != 0)
            {
                foreach (var item in catPro)
                {
                    var pro = db.Products.Where(r => r.id == item.product_id).FirstOrDefault();
                    if (pro != null)
                    {
                        productList.Add(pro);
                    }
                }
            }
            if (productList.Count != 0)
            {
                productList = productList.Distinct().ToList();
            }
            if (brand != null && brand.Count != 0)
                productList = filterbrand(productList, brand);
            if (propertiesValue.Count != 0)
            {
                if (productList.Count != 0)
                {
                    var newProList = new List<Product>();
                    foreach (var item in productList)
                    {

                        Boolean flag = false;
                        foreach (var item2 in propertiesValue)
                        {
                            var propVal = db.ProductPropertiesValues.Where(r => r.proId == item.id && r.value == item2).FirstOrDefault();
                            if (propVal != null)
                            {
                                flag = true;
                            }
                            else
                            {
                                //break;
                            }
                        }
                        if (flag == true)
                        {
                            newProList.Add(item);
                        }
                        else
                        {
                            try
                            {
                                newProList.Remove(item);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    productList = newProList;
                }
            }
            var productsForColorFilter = new List<Product>();
            if (productList.Count!=0)
            {
                foreach (var item in productList)
                {
                    productsForColorFilter.Add(item);
                }
            }
            if (ColorValue.Count!=0)
            {
                if (productList.Count!=0)
                {
                    foreach (var item in productList)
                    {
                        if (db.ColorSizes.Where(r=>r.ProductId == item.id ).ToList().Count!=0)
                        {
                            Boolean flag = false;
                            foreach (var item2 in db.ColorSizes.Where(r => r.ProductId == item.id).ToList())
                            {
                                if (item2.colorId!=null)
                                {
                                    var val = ColorValue.IndexOf(Convert.ToInt32( item2.colorId)); 
                                    if (val!=-1)
                                    {
                                        flag = true; 
                                    }
                                }
                            }
                            if (!flag)
                            {
                                productsForColorFilter.Remove(item);

                            }
                        }
                        else
                        {
                            productsForColorFilter.Remove(item);
                            db.SaveChanges();
                        }
                    }
                   
                }

            }
            productList = productsForColorFilter;
            if (!string.IsNullOrEmpty(price_max) && !string.IsNullOrEmpty(price_min))
            {
                if (price_max.Contains("."))
                {
                    price_max = price_max.Replace(price_max.Substring(price_max.IndexOf("."), price_max.Length - price_max.IndexOf(".")), "");
                }
                if (price_min.Contains("."))
                {
                    price_min = price_min.Replace(price_min.Substring(price_min.IndexOf("."), price_min.Length - price_min.IndexOf(".")), "");
                }
                price_max = String.Format("{0:0.000}", price_max).Replace(".00", "");
                price_min = String.Format("{0:0.000}", price_min).Replace(".00", "");

                Int64 minPrice = 0, maxPrice = 0;
                while (price_max.Contains(","))
                {
                    price_max = price_max.Replace(",", "");
                }
                while (price_min.Contains(","))
                {
                    price_min = price_min.Replace(",", "");
                }
                minPrice = Convert.ToInt64(price_min);
                maxPrice = Convert.ToInt64(price_max);
                productList = productList.Where(r => r.price >= minPrice && r.price <= maxPrice).ToList();
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy)
                {
                    case "top-price":
                        {
                            productList = productList.OrderByDescending(r => r.price).OrderByDescending(r => r.isAvailable).ToList();
                            break;
                        }
                    case "new":
                        {
                            productList = productList.OrderByDescending(r => r.id).ToList();
                            break;
                        }
                    case "down-price":
                        {
                            productList = productList.OrderBy(r => r.price).OrderByDescending(r => r.isAvailable).ToList();
                            break;
                        }

                    default:
                        break;
                }

            }
            if (!string.IsNullOrEmpty(q))
            {
                productList = productList.Where(r => r.title.Contains(q)).ToList();
            }
            if (has_selling_stock != null && has_selling_stock == 1)
            {
                if (productList.Count != 0)
                {
                    productList = productList.Where(r => r.isAvailable == true).ToList();
                }
            }
            ViewBag.products = productList;
            return PartialView();
        }
        public List<Product> filterbrand(List<Product> products, List<int> brands)
        {
            var newList = new List<Product>();
            if (brands.Count != 0 && products.Count != 0)
            {
                foreach (var item in brands)
                {
                    if (products.Where(r => r.brandId == item).ToList().Count != 0)
                    {
                        newList.AddRange(products.Where(r => r.brandId == item).ToList());
                    }
                }
            }
            return newList;
        }


















        [HttpPost]
        public ActionResult Address(string address, string post, string period, string location, string ReciverPhone, string Reciver, int? City, int? State)
        {

            if (Session["orders"] != null)
            {
                if (!string.IsNullOrWhiteSpace(address) && !string.IsNullOrWhiteSpace(post))
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);

                    var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                    if (user != null)
                    {
                        if (db.UsersAdresses.Where(r => r.Address == address && r.postalCode == post && r.userId == userid).FirstOrDefault() == null)
                        {
                            var newAdress = new UsersAdress();
                            newAdress.Address = address;
                            newAdress.postalCode = post;
                            newAdress.State = State;
                            newAdress.City = City;
                            newAdress.Reciver = Reciver;
                            newAdress.ReciverPhone = ReciverPhone;
                            newAdress.Location = location;
                            newAdress.userId = userid;
                            db.UsersAdresses.Add(newAdress);
                            db.SaveChanges();
                        }
                    }
                    Session["address"] = address;
                    Session["post"] = post;
                    Session["period"] = period;
                    return RedirectToAction("payWay");
                }
                else
                {
                    ViewBag.error = 1;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        #endregion
        #region صفحه اصلی
        public ActionResult Index(int? newsletter)
        {
            try
            {
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["onlineShopCats"];
                if (authCookie != null)
                {
                    var numbers = authCookie.Value.Split(',').Select(Int32.Parse).ToList();
                    ViewBag.recent = numbers;
                }
                if (newsletter != null)
                {
                    ViewBag.newsletter = 1;
                }
                checkLogin();

                return View();
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "Index", "HomeController");
                throw;
            }
           
        }
        #endregion
        #region سبد خرید
        #region نمایش سبد خرید کاربر
        public ActionResult Basket()
        {
            checkLogin();

            if (Session["userLogin"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound");
            }

        }

        #endregion
        #region سبد خرید
        public ActionResult Cart()
        {
            return View();

        }
        #endregion
        public ActionResult cardToCard()
        {
            checkLogin();
            if (Session["userLogin"] != null && Session["orders"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }


        public ActionResult removeProductToWishList(int? id)
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                int userId = Convert.ToInt16(Session["userLogin"]);
                db.WishLists.Remove(db.WishLists.Where(r => r.productId == id && r.userid == userId).FirstOrDefault());
                db.SaveChanges();

                return RedirectToAction("product", new { id = id });
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        public ActionResult wishlist()
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        public ActionResult lastVisits()
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        [HttpPost]
        public ActionResult addProductToWishList(int? id)
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                int userId = Convert.ToInt16(Session["userLogin"]);
                if (db.Products.Where(r => r.id == id).FirstOrDefault() != null && db.WishLists.Where(r => r.productId == id && r.userid == userId).FirstOrDefault() == null)
                {
                    var wishList = new WishList();
                    wishList.userid = userId;
                    wishList.productId = id;
                    db.WishLists.Add(wishList);
                    db.SaveChanges();
                    return RedirectToAction("product", new { id = id });
                }
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }

        public ActionResult parentCategory(int id)
        {
            ViewBag.id = id;
            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCard(int? productId, int? quantity, int? seller , int? colorId , int? sizeId   )
        {
            checkLogin();
            if (true)
            {
                long singleProductPrice = 0;
                var product = db.Products.Where(r => r.id == productId).FirstOrDefault();
                var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == product.id && r.expire > DateTime.Now).FirstOrDefault();

                if (seller == null)
                {
                    if (off != null)
                    {
                        singleProductPrice += Convert.ToInt64((product.price - (product.price * off.percentage) / 100));

                    }
                    else
                    {
                        singleProductPrice = Convert.ToInt64(product.price);

                    }
                }
                else
                {
                    var sellerProduct = db.SellersProducts.Where(r => r.productId == productId && r.sellerId == seller).FirstOrDefault();
                    if (sellerProduct != null)
                    {
                        singleProductPrice = Convert.ToInt64(sellerProduct.price);

                    }
                }
                int userid = Convert.ToInt16(Session["userLogin"]);
                var order = new Order();
                order.SellerId = seller;
                order.userId = userid;

                order.colorId = colorId;
                order.sizeId = sizeId;

                if (sizeId!=null && sizeId!=0)
                {
                    var size = db.Sizes.Where(r => r.id == sizeId).FirstOrDefault();
                    order.colorSizeDescription = "سایز : " + size.title;
                    if (colorId != null && colorId != 0)
                    {
                        var color = db.Colors.Where(r => r.id == colorId).FirstOrDefault();
                        order.colorSizeDescription += " , رنگ " + color.title;
                    }
                }

                


                order.quantity = quantity;
                order.singleProductPrice = singleProductPrice;
                order.creationDate = DateTime.Now;
                order.productId = productId;
                var ordersList = new List<Order>();
                if (Session["orders"] != null)
                {
                    ordersList = Session["orders"] as List<Order>;
                    if (ordersList.Where(r => r.productId == productId).FirstOrDefault() != null)
                    {
                        var preOrder = ordersList.Where(r => r.productId == productId).FirstOrDefault();
                        ordersList.Remove(preOrder);
                    }
                }
                ordersList.Add(order);
                Session["orders"] = ordersList;
            }

            return RedirectToAction("card");
        }

        [HttpGet]
        public ActionResult Card()
        {
            checkLogin();
            if (false)
            {
                //return RedirectToAction("NotFound");
            }
            else
            {
                if (Session["orders"] == null)
                {
                    return RedirectToAction("EmptyCard");
                }
                else
                {
                    var orders = Session["orders"] as List<Order>;
                    if (orders.Count == 0)
                    {
                        return RedirectToAction("EmptyCard");
                    }
                }
                return View();
            }
        }
        [HttpGet]
        public ActionResult EmptyCard()
        {
            return View();
        }
        public ActionResult cardPlus(int? id)
        {
            //checkLogin();

            if (false)
            {
                return RedirectToAction("NotFound");

            }
            else
            {
                if (Session["orders"] != null)
                {
                    var ordersList = Session["orders"] as List<Order>;
                    if (ordersList.Where(r => r.productId == id).FirstOrDefault() != null)
                    {
                        var preOrder = ordersList.Where(r => r.productId == id).FirstOrDefault();
                        preOrder.quantity++;
                    }
                    Session["orders"] = ordersList;
                }
                return RedirectToAction("Card");
            }
        }
        public ActionResult cardMinus(int? id)
        {
            //checkLogin();

            if (false)
            {
                return RedirectToAction("NotFound");

            }
            else
            {
                if (Session["orders"] != null)
                {
                    var ordersList = Session["orders"] as List<Order>;
                    if (ordersList.Where(r => r.productId == id).FirstOrDefault() != null)
                    {
                        var preOrder = ordersList.Where(r => r.productId == id).FirstOrDefault();
                        if (preOrder.quantity != 1)
                        {
                            preOrder.quantity--;

                        }
                    }
                    Session["orders"] = ordersList;
                }
                return RedirectToAction("Card");
            }
        }
        public ActionResult removeCard(int? id)
        {
            checkLogin();

            if (Session["orders"] != null)
            {
                var ordersList = Session["orders"] as List<Order>;
                if (ordersList.Where(r => r.productId == id).FirstOrDefault() != null)
                {
                    var preOrder = ordersList.Where(r => r.productId == id).FirstOrDefault();
                    ordersList.Remove(preOrder);
                }
                Session["orders"] = ordersList;
            }
            return RedirectToAction("Card");
        }
        #endregion
        #region صورتحساب
        public ActionResult invoice()
        {
            checkLogin();

            if (Session["userLogin"] == null)
            {
                return RedirectToAction("NotFound");

            }
            else
            {
                return View();

            }
        }

        #endregion
        #region تیکت ها

        [HttpPost]
        public ActionResult ticketDetails(int? id, string text)
        {
            checkLogin();

            if (Session["userLogin"] != null)
            {
                try
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    var ticket = db.Tickets.Find(id);
                    if (ticket.userid == userid)
                    {
                        var newChat = new TicketChat();
                        newChat.creationDate = DateTime.Now;
                        newChat.isUser = true;
                        newChat.text = text;
                        newChat.ticketId = ticket.id;
                        ticket.status = "در انتظار پاسخ";
                        db.TicketChats.Add(newChat);
                        db.SaveChanges();
                        ViewBag.id = id;
                        return RedirectToAction("ticketDetails", new { id = ticket.id });

                    }
                    else
                    {
                        return RedirectToAction("NotFound");

                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "ticketDetails", "HomeController");
                    throw;
                }          

            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }

        public ActionResult ticketDetails(int? id)
        {
            checkLogin();

            if (Session["userLogin"] == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                try
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    if (db.Tickets.Where(r => r.userid == userid && r.id == id).FirstOrDefault() != null)
                    {
                        ViewBag.id = id;
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("NotFound");
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "ticketDetails", "HomeController");
                    throw;
                }
                

            }
        }

        public ActionResult newTicket(string error)
        {
            checkLogin();

            ViewBag.error = error;
            if (Session["userLogin"] == null)
            {
                return RedirectToAction("NotFound");

            }
            else
            {
                return View();

            }
        }
        [HttpPost]
        public ActionResult newTicket(string title, string @for, string text)
        {
            checkLogin();

            #region check captcha
            if (this.IsCaptchaValid("Captcha is not valid") == false)
            {
                ViewBag.error = "کد امنیتی اشتباه است";
                return View();

            }
            #endregion
            if (Session["userLogin"] == null)
            {
                return RedirectToAction("NotFound");
            }
            else
            {
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(text))
                {
                    return RedirectToAction("newTicket", new { error = "عنوان و متن پیام برای ارسال تیکت پشتیبانی الزامی است" });

                }
                else
                {
                    try
                    {
                        var userid = Convert.ToInt16(Session["userLogin"]);
                        var newTicket = new Ticket();
                        newTicket.creationDate = DateTime.Now;
                        newTicket.@for = @for;
                        newTicket.title = title;
                        newTicket.userid = userid;
                        db.Tickets.Add(newTicket);
                        db.SaveChanges();
                        var ticketChat = new TicketChat();
                        ticketChat.text = text;
                        ticketChat.ticketId = newTicket.id;
                        ticketChat.isUser = true;
                        ticketChat.creationDate = DateTime.Now;
                        db.TicketChats.Add(ticketChat);
                        db.SaveChanges();
                        return RedirectToAction("ticketDetails", new { id = newTicket.id });
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "newTicket", "HomeController");
                        throw;
                    }
                   
                }
            }
        }



        public ActionResult Ticket()
        {
            checkLogin();

            if (Session["userLogin"] == null)
            {
                return RedirectToAction("NotFound");

            }
            else
            {
                return View();

            }
        }
        #endregion
        #region کامنت محصولات
        [HttpPost]
        public ActionResult productComment(string name, string text, int? rate, int productId, string title)
        {
            checkLogin();

            #region check captcha
            if (this.IsCaptchaValid("Captcha is not valid") == false)
            {
                ViewBag.error = "کد امنیتی اشتباه است";
                return View();

            }
            #endregion
            if (Session["userLogin"] == null)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    try
                    {
                        var cm = new Comment();
                        cm.productId = productId;
                        if (!string.IsNullOrEmpty(name))
                        {
                            cm.guestName = name;
                        }
                        else
                        {
                            cm.guestName = "ناشناس";
                        }
                        cm.text = text;
                        cm.title = title;
                        cm.show = false;
                        cm.creationDate = DateTime.Now;
                        db.Comments.Add(cm);
                        db.SaveChanges();
                        return RedirectToAction("product", new { id = productId, successMessage = "نظر شما با موفقیت ثبت شد و پس از تائید ادمین سایت منتشر خواهد شد." });
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "productComment", "HomeController");
                        throw;
                    }
                    

                }
                return RedirectToAction("product", new { id = productId });
            }
            else
            {
                try
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    var newCm = new Comment();
                    newCm.productId = productId;
                    newCm.text = text;
                    newCm.userid = userid;
                    newCm.text = text;
                    newCm.show = false;
                    if (rate != null)
                    {
                        newCm.rate = rate;
                    }
                    newCm.creationDate = DateTime.Now;
                    db.Comments.Add(newCm);
                    db.SaveChanges();
                    return RedirectToAction("product", new { id = productId, successMessage = "نظر شما با موفقیت ثبت شد و پس از تائید ادمین سایت منتشر خواهد شد." });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "productComment", "HomeController");
                    throw;
                }
               
            }
        }

        [HttpPost]
        public ActionResult sellerComment(string name, string text, int? rate, int sellerId)
        {
            checkLogin();

            #region check captcha
            if (this.IsCaptchaValid("Captcha is not valid") == false)
            {
                ViewBag.error = "کد امنیتی اشتباه است";
                return View();

            }
            #endregion
            if (Session["userLogin"] == null)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    try
                    {
                        var cm = new SellerComment();
                        cm.sellerId = sellerId;
                        if (!string.IsNullOrEmpty(name))
                        {
                            cm.name = name;
                        }
                        else
                        {
                            cm.name = "ناشناس";
                        }
                        cm.text = text;
                        cm.show = false;
                        cm.creationDate = DateTime.Now;
                        db.SellerComments.Add(cm);
                        db.SaveChanges();
                        return RedirectToAction("sellers", new { id = sellerId, successMessage = "نظر شما با موفقیت ثبت شد و پس از تائید ادمین سایت منتشر خواهد شد." });
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "sellerComment", "HomeController");
                        throw;
                    }
                   

                }
                return RedirectToAction("sellers", new { id = sellerId });
            }
            else
            {
                try
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    var newCm = new SellerComment();
                    newCm.sellerId = sellerId;
                    newCm.userid = userid;
                    newCm.text = text;
                    newCm.show = false;
                    if (rate != null)
                    {
                        newCm.rate = rate;
                    }
                    newCm.creationDate = DateTime.Now;
                    db.SellerComments.Add(newCm);
                    db.SaveChanges();
                    return RedirectToAction("sellers", new { id = sellerId, successMessage = "نظر شما با موفقیت ثبت شد و پس از تائید ادمین سایت منتشر خواهد شد." });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "sellerComment", "HomeController");
                    throw;
                }
                
            }
        }

        #endregion
        #region لاگین 
        #region چک کردن کوکی برای لاگین شدن کاربر
        public void checkLogin()
        {
            if (Session["userLogin"] == null)
            {
                try
                {
                    HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["OnlineShopUserAuthCookie"];
                    if (authCookie != null)
                    {
                        string rnd = authCookie.Value;
                        var user = db.Users.Where(r => r.token == rnd).FirstOrDefault();
                        if (user != null)
                        {
                            Session["userLogin"] = user.id;
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "checkLogin", "HomeController");
                    throw;
                }
               
            }

        }

        #endregion






        #region صفحه ثبت نام

        #endregion
        #region دریافت شماره موبایل و پسورد برای ورود کاربر

        #endregion

        #region Generate random String 
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz$#@!%^&*()";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion
        #region  بازیابی رمزعبور


        #endregion


        #endregion
        #region بارگذاری عکس پروفایل

        [HttpPost]
        public JsonResult UploadprofileImage()
        {
            checkLogin();

            if (Session["userLogin"] != null)
            {
                try
                {
                    foreach (var item in Request.Form.AllKeys)
                    {
                        var test = item;
                    }
                    string fileUrl = "";
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                                    //Use the following properties to get file's name, size and MIMEType
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;
                        string dateString = DateTime.Now.ToString();
                        while (dateString.Contains(":") || dateString.Contains("/"))
                        {
                            dateString = dateString.Replace(":", "");
                            dateString = dateString.Replace("/", "");

                        }
                        fileUrl = dateString + ".jpg";
                        db.SaveChanges();
                        System.IO.Stream fileContent = file.InputStream;
                        //To save file, use SaveAs method
                        file.SaveAs(Server.MapPath("~/uploads/") + dateString + ".jpg"); //File will be saved in application root
                        return Json("/uploads/" + dateString + ".jpg");

                    }
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                    user.image = fileUrl;
                    db.SaveChanges();
                    return Json(fileUrl);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "UploadprofileImage", "HomeController");
                    throw;
                }
                

            }
            return Json("done");

        }
        #endregion

        #region محصولات

        public ActionResult Product(int id, string title, string successMessage)
        {
            checkLogin();
            ViewBag.successMessage = successMessage;

            ViewBag.id = id;

            return View();
        }
        #endregion
        #region 404
        public ActionResult NotFound()
        {
            checkLogin();
            return View();

        }
        #endregion
        #region سفارش
        #region جزئیات سفارش
        public ActionResult ordersDetail(int? id)
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                ViewBag.id = id;
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }

        #endregion
        #region لیست سفارش ها
        public ActionResult orders()
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        public ActionResult GiftList(int? orderId)
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                if (orderId != null)
                {
                    ViewBag.orderId = orderId;
                }
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }

        #endregion
        #region آپلود اسکن فیش پرداخت


        #endregion
        #region انتخاب روش پرداخت
        public ActionResult payWay()
        {
            checkLogin();
            if (Session["userLogin"] != null && Session["orders"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        public JsonResult uploadScanedFish()
        {
            checkLogin();

            if (Session["userLogin"] != null)
            {
                try
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    var settings = db.Settings.FirstOrDefault();

                    var ordersList = Session["orders"] as List<Order>;
                    var basket = new Basket();

                    db.Baskets.Add(basket);
                    db.SaveChanges();
                    if (ordersList != null && ordersList.Count != 0)
                    {
                        basket.address = Session["address"].ToString();
                        basket.post = Session["post"].ToString();
                        basket.periodString = Session["period"].ToString();
                        basket.creationDate = DateTime.Now;
                        basket.userId = userid;
                        basket.payMethod = "کارت به کارت";

                        Int64 sum = 0;
                        foreach (var item in ordersList)
                        {

                            var product = db.Products.Where(r => r.id == item.productId).FirstOrDefault();
                            var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == item.productId && r.expire > DateTime.Now).FirstOrDefault();
                            if (item.SellerId == null)
                            {
                                if (off != null)
                                {
                                    sum += Convert.ToInt64((product.price - (product.price * off.percentage) / 100) * item.quantity);

                                }
                                else
                                {
                                    sum += Convert.ToInt64(product.price * item.quantity);

                                }
                            }
                            else
                            {
                                var sellerProduct = db.SellersProducts.Where(r => r.productId == product.id && r.sellerId == item.SellerId).FirstOrDefault();
                                if (sellerProduct != null)
                                {
                                    sum += Convert.ToInt64(sellerProduct.price * item.quantity);

                                }

                            }
                            item.userId = userid;
                            item.basketId = basket.id;
                            item.creationDate = DateTime.Now;
                            db.Orders.Add(item);
                            db.SaveChanges();
                            if (product.point != null)
                            {
                                for (int j = 0; j < item.quantity; j++)
                                {
                                    var proPoint = new UserPoint();
                                    proPoint.point = product.point;
                                    proPoint.userId = userid;
                                    proPoint.CreationDate = DateTime.Now;
                                    proPoint.orderId = item.id;
                                    proPoint.productId = product.id;
                                    db.UserPoints.Add(proPoint);
                                }

                            }

                        }
                        if (settings.shippingFree > sum)
                        {
                            basket.shippingPrice = settings.shippingPrice;
                        }
                        basket.Price = sum;
                        var discount = new DiscountCode();
                        int discountId = 0;
                        if (Session["Discount"] != null)
                        {
                            discountId = Convert.ToInt32(Session["Discount"]);
                            discount = db.DiscountCodes.Where(r => r.id == discountId).FirstOrDefault();

                        }
                        basket.status = "در انتظار تائید پرداخت";
                        db.SaveChanges();
                        if (discount != null)
                        {
                            if (discount.Amont != null)
                            {
                                basket.DiscountAmount = discount.Amont;

                            }
                            else
                            {
                                if (discount.Percentage != null)
                                {
                                    if (discount.maxPercentage < basket.Price)
                                    {
                                        basket.DiscountAmount = ((discount.maxPercentage * discount.Percentage) / 100);
                                    }
                                    else
                                    {
                                        basket.DiscountAmount = ((basket.Price * discount.Percentage) / 100);
                                    }
                                }
                            }
                            basket.DiscountCode = discountId;
                            discount.isUsed = true;
                            discount.useDate = DateTime.Now;
                            discount.basketId = basket.id;

                        }
                        db.SaveChanges();

                    }
                    Session["orders"] = null;

                    var scan = new Scan();
                    scan.CreationDate = DateTime.Now;
                    scan.basketId = basket.id;
                    scan.isConfirmed = false;
                    scan.statusString = "در انتظار تائید";
                    scan.status = 0;
                    string fileUrl = "";
                    int i = 0;
                    HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    string dateString = DateTime.Now.ToString();
                    while (dateString.Contains(":") || dateString.Contains("/"))
                    {
                        dateString = dateString.Replace(":", "");
                        dateString = dateString.Replace("/", "");
                    }
                    fileUrl = dateString + ".jpg";
                    db.SaveChanges();
                    System.IO.Stream fileContent = file.InputStream;
                    scan.url = fileUrl;
                    db.Scans.Add(scan);
                    db.SaveChanges();
                    file.SaveAs(Server.MapPath("~/uploads/") + dateString + ".jpg"); //File will be saved in application root
                    return Json(basket.id);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "uploadScanedFish", "HomeController");
                    throw;
                }
                
            }
            return Json("done");
        }
        #endregion

        #endregion
        #region ویرایش اطلاعات کاربری
        public ActionResult Profile()
        {
            checkLogin();

            if (Session["userLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }

        }

        #endregion
        #region دسته بندی محصولات


        public void UpdateCategoryCookies(int id)
        {
            try
            {
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["onlineShopCats"];
                if (authCookie != null)
                {
                    var numbers = authCookie.Value.Split(',').Select(Int32.Parse).ToList();
                    numbers.Add(id);
                    numbers = numbers.Distinct().ToList();
                    string useString = string.Empty;
                    int i = 1;
                    foreach (var item in numbers)
                    {
                        useString += item.ToString();
                        if (i != numbers.Count)
                        {
                            useString += ",";
                        }
                        i += 1;
                    }

                    HttpCookie newauthCookie = new HttpCookie("onlineShopCats", useString)
                    {
                        Expires = DateTime.Now.AddDays(10)
                    };
                    Response.Cookies.Add(newauthCookie);
                }
                else
                {
                    HttpCookie newauthCookie = new HttpCookie("onlineShopCats", id.ToString())
                    {
                        Expires = DateTime.Now.AddDays(10)
                    };
                    Response.Cookies.Add(newauthCookie);
                }
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "UpdateCategoryCookies", "HomeController");
                throw;
            }
           

        }

        public ActionResult Category(int id, string title)
        {
            UpdateCategoryCookies(id);
            ViewBag.id = id;
            return View();
        }
        public ActionResult MainCat(int? id, string title)
        {
            ViewBag.id = id;
            return View();
        }
        public JsonResult GetMainCats(int id)
        {
            try
            {
                var childs = db.Categories.Where(p => p.MainCat == id && p.parent == null).ToList();
                var childlist = childs.Select(c => new { Id = c.id, Title = c.title }).ToList();

                return Json(childlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "GetMainCats", "HomeController");
                throw;
            }
           
        }

        public JsonResult ListService(int id)
        {
            try
            {
                var childs = db.ServicePrices.Where(p => p.ModelId == id ).ToList();
                var childlist = childs.Select(c => new { Id = c.ServiceId, Title = c.Service.Title }).ToList();

                return Json(childlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "ListService", "HomeController");
                throw;
            }

        }

        public JsonResult ListModel(int id)
        {
            try
            {
                var childs = db.Models.Where(p => p.BrandId == id ).ToList();
                var childlist = childs.Select(c => new { Id = c.ModelId, Title = c.Title }).ToList();

                return Json(childlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "ListModel", "HomeController");
                throw;
            }

        }

        public JsonResult GetRootCats(int id)
        {
            try
            {
                var childs = db.Categories.Where(p => p.parent == id).ToList();
                var childlist = childs.Select(c => new { Id = c.id, Title = c.title }).ToList();

                return Json(childlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "GetRootCats", "HomeController");
                throw;
            }
            
        }
        #endregion
        #region جستجو
        public ActionResult Search(string search)
        {
            ViewBag.search = search;
            return View();
        }

        #endregion
        #region نقشه سایت
        public ActionResult SiteMap()
        {
            try
            {
                var setting = db.Settings.FirstOrDefault();
                string WebsiteUrl = setting.WebsiteUrl;

                var products = db.Products.Where(r => r.Active == true).ToList();
                var categories = db.Categories.ToList();
                var mainCats = db.MainCats.ToList();
                var posts = db.Blog_post.ToList();
                var postCats = db.Blog_Cats.ToList();
                var pages = db.Pages.ToList();
                XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
                const string url = "http://motano.com/freelancer/index/{0}";
                var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from i in products
                    select
                    new XElement(ns + "url",
                        new XElement(ns + "loc", string.Format(WebsiteUrl + "home/product/{0}", i.id + "/") + (i.title.Trim()).Replace(" ", "-")),
                        new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                        new XElement(ns + "changefreq", "always"),
                        new XElement(ns + "priority", "1")
                )
                ,

                    from i in categories
                    select
                    new XElement(ns + "url",

                        new XElement(ns + "loc", string.Format(WebsiteUrl + "home/category/{0}", i.id + "/") + (i.title.Trim()).Replace(" ", "-")),
                        new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                        new XElement(ns + "changefreq", "always"),
                        new XElement(ns + "priority", "1")
                )
                  ,

                   from i in pages
                   select
                   new XElement(ns + "url",

                       new XElement(ns + "loc", string.Format(WebsiteUrl + "home/page/{0}", i.id + "/") + (i.title.Trim()).Replace(" ", "-")),
                       new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
                       new XElement(ns + "changefreq", "always"),
                       new XElement(ns + "priority", "1")
               )

               //,

               //    from i in mainCats
               //    select
               //    new XElement(ns + "url",

               //        new XElement(ns + "loc", string.Format("https://OnlineShop.com/home/mainCat/{0}", i.id + "/") + (i.title).Replace(" ", "_")),
               //        new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
               //        new XElement(ns + "changefreq", "always"),
               //        new XElement(ns + "priority", "1")
               //)
               //   ,

               //    from i in posts
               //    select
               //    new XElement(ns + "url",

               //        new XElement(ns + "loc", string.Format("https://OnlineShop.com/blog/details/{0}", i.id + "/") + (i.title).Replace(" ", "_")),
               //        new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
               //        new XElement(ns + "changefreq", "always"),
               //        new XElement(ns + "priority", "1")
               //)
               //     ,

               //    from i in postCats
               //    select
               //    new XElement(ns + "url",

               //        new XElement(ns + "loc", string.Format("https://OnlineShop.com/blog/category/{0}", i.id + "/") + (i.title).Replace(" ", "_")),
               //        new XElement(ns + "lastmod", String.Format("{0:yyyy-MM-dd}", DateTime.Now)),
               //        new XElement(ns + "changefreq", "always"),
               //        new XElement(ns + "priority", "1")
               //)
               ));

                return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + sitemap.ToString(), "text/xml");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "SiteMap", "HomeController");
                throw;
            }
            
        }

        #endregion
        #region خبرنامه
        [HttpPost]
        public ActionResult newsLetter(string email)
        {
            try
            {
                if (IsValidEmail(email))
                {
                    if (db.newsLetters.Where(r => r.email.ToLower() == email.ToLower().Trim()).FirstOrDefault() == null)
                    {
                        var newsLetter = new newsLetter();
                        newsLetter.CreationDate = DateTime.Now;
                        newsLetter.email = email.Trim();
                        db.newsLetters.Add(newsLetter);
                        db.SaveChanges();
                        return RedirectToAction("index", new { newsletter = 1 });

                    }

                }
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "newsLetter", "HomeController");
                throw;
            }
            
        }
        #endregion
        #region تماس با ما
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpPost]
        public JsonResult Contact(Contact ct, string fullName, string name)
        {

            #region check captcha
            if (false)
            {
                return Json("<p class=\"error\">کد امنیتی وارد شده اشتباه است .</p>");
            }
            else
            {
                try
                {
                    if (name != null)
                    {
                        var namearr = name.Split(' ');
                        ct.firstName = namearr[0];
                        if (namearr.Length > 1)
                            ct.lastName = namearr[1];
                    }
                    else if (fullName != null)
                    {
                        var namearr = fullName.Split(' ');
                        ct.firstName = namearr[0];
                        if (namearr.Length > 1)
                            ct.lastName = namearr[1];
                    }
                    ct.date = DateTime.Now;
                    db.Contacts.Add(ct);
                    db.SaveChanges();
                    return Json("<p class=\"SuccessPost\"> پیام شما با موفقیت ثبت شد </p>");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Contact", "HomeController");
                    throw;
                }
              

            }
            #endregion
        }
        #endregion
        #region ثبت سفارش موفق
        public ActionResult SuccessOrder(int? id)
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                ViewBag.order = id;
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }

        #endregion

        #region FUNCTIONS
        #region اعتبارسنجی ایمیل
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #region تولید شماره رندم
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        #endregion
        #endregion
        public ActionResult Wallet()
        {
            return View();
        }
        public ActionResult payWallet()
        {
            return View();
        }

        public ActionResult payByWallet()
        {
            checkLogin();
            if (Session["userLogin"] != null && Session["orders"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }
        [HttpPost]
        public ActionResult payByWallet(string result)
        {
            checkLogin();

            if (Session["userLogin"] != null)
            {
                try
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    var ordersList = Session["orders"] as List<Order>;
                    var basket = new Basket();
                    db.Baskets.Add(basket);
                    db.SaveChanges();
                    var settings = db.Settings.FirstOrDefault();
                    if (ordersList != null && ordersList.Count != 0)
                    {



                        basket.address = Session["address"].ToString();
                        basket.post = Session["post"].ToString();
                        basket.periodString = Session["period"].ToString();
                        basket.creationDate = DateTime.Now;
                        basket.userId = userid;
                        basket.payMethod = "کیف پول";



                        Int64 sum = 0;
                        foreach (var item in ordersList)
                        {
                            var product = db.Products.Where(r => r.id == item.productId).FirstOrDefault();
                            var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == item.productId && r.expire > DateTime.Now).FirstOrDefault();
                            if (item.SellerId == null)
                            {
                                if (off != null)
                                {
                                    sum += Convert.ToInt64((product.price - (product.price * off.percentage) / 100) * item.quantity);

                                }
                                else
                                {
                                    sum += Convert.ToInt64(product.price * item.quantity);

                                }
                            }
                            else
                            {
                                var sellerProduct = db.SellersProducts.Where(r => r.productId == product.id && r.sellerId == item.SellerId).FirstOrDefault();
                                if (sellerProduct != null)
                                {
                                    sum += Convert.ToInt64(sellerProduct.price * item.quantity);

                                }

                            }

                            item.userId = userid;
                            item.basketId = basket.id;
                            item.creationDate = DateTime.Now;
                            db.Orders.Add(item);
                            db.SaveChanges();
                            if (product.point != null)
                            {
                                for (int j = 0; j < item.quantity; j++)
                                {
                                    var proPoint = new UserPoint();
                                    proPoint.point = product.point;
                                    proPoint.userId = userid;
                                    proPoint.CreationDate = DateTime.Now;
                                    proPoint.orderId = item.id;
                                    proPoint.productId = product.id;
                                    db.UserPoints.Add(proPoint);
                                }

                            }

                        }
                        if (settings.shippingFree > sum)
                        {
                            basket.shippingPrice = settings.shippingPrice;
                        }
                        basket.Price = sum;
                        var discount = new DiscountCode();
                        int discountId = 0;
                        if (Session["Discount"] != null)
                        {
                            discountId = Convert.ToInt32(Session["Discount"]);
                            discount = db.DiscountCodes.Where(r => r.id == discountId).FirstOrDefault();

                        }
                        var payed = new WalletPay();
                        payed.basketId = basket.id;
                        payed.creationDate = DateTime.Now;
                        payed.amount = basket.Price;
                        payed.userid = userid;
                        db.WalletPays.Add(payed);
                        basket.status = "پرداخت شده با کیف پول";
                        db.SaveChanges();
                        if (discount != null)
                        {
                            if (discount.Amont != null)
                            {
                                basket.DiscountAmount = discount.Amont;

                            }
                            else
                            {
                                if (discount.Percentage != null)
                                {
                                    if (discount.maxPercentage < basket.Price)
                                    {
                                        basket.DiscountAmount = ((discount.maxPercentage * discount.Percentage) / 100);
                                    }
                                    else
                                    {
                                        basket.DiscountAmount = ((basket.Price * discount.Percentage) / 100);
                                    }
                                }
                            }
                            basket.DiscountCode = discountId;
                            discount.isUsed = true;
                            discount.useDate = DateTime.Now;
                            discount.basketId = basket.id;

                        }
                        db.SaveChanges();
                    }
                    Session["orders"] = null;


                    db.SaveChanges();
                    return RedirectToAction("successorder", new { order = basket.id });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "payByWallet", "HomeController");
                    throw;
                }
                

            }
            return Json("done");

        }
        public ActionResult sellerDetails(int? id)
        {
            ViewBag.id = id;
            return View();
        }


        [HttpPost]
        public ActionResult payWallet(long amount)
        {
            if (Session["userLogin"] == null)
            {
                return RedirectToAction("index");
            }
            else
            {
                try
                {
                    int userird = Convert.ToInt16(Session["userLogin"]);
                    var newPay = new Wallet();
                    newPay.amount = amount;
                    newPay.creationDate = DateTime.Now;
                    newPay.payByAdmin = false;
                    newPay.userId = userird;
                    newPay.payWay = "درگاه پرداخت";
                    db.Wallets.Add(newPay);
                    db.SaveChanges();
                    return RedirectToAction("wallet");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "payWallet", "HomeController");
                    throw;
                }
               
            }
        }

        public ActionResult sellers(int? id, string title, string successMessage)
        {
            checkLogin();
            ViewBag.successMessage = successMessage;
            ViewBag.id = id;
            return View();
        }

        public ActionResult homePay()
        {

            return View();

        }
        [HttpPost]
        public ActionResult payHomePost()
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                try
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    var ordersList = Session["orders"] as List<Order>;
                    var basket = new Basket();
                    db.Baskets.Add(basket);
                    db.SaveChanges();
                    var settings = db.Settings.FirstOrDefault();
                    if (ordersList != null && ordersList.Count != 0)
                    {
                        basket.address = Session["address"].ToString();
                        basket.post = Session["post"].ToString();
                        basket.periodString = Session["period"].ToString();
                        basket.creationDate = DateTime.Now;
                        basket.userId = userid;
                        basket.payMethod = "پرداخت درب منزل";
                        Int64 sum = 0;
                        foreach (var item in ordersList)
                        {
                            var product = db.Products.Where(r => r.id == item.productId).FirstOrDefault();
                            var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == item.productId && r.expire > DateTime.Now).FirstOrDefault();
                            if (item.SellerId == null)
                            {
                                if (off != null)
                                {
                                    sum += Convert.ToInt64((product.price - (product.price * off.percentage) / 100) * item.quantity);

                                }
                                else
                                {
                                    sum += Convert.ToInt64(product.price * item.quantity);

                                }
                            }
                            else
                            {
                                var sellerProduct = db.SellersProducts.Where(r => r.productId == product.id && r.sellerId == item.SellerId).FirstOrDefault();
                                if (sellerProduct != null)
                                {
                                    sum += Convert.ToInt64(sellerProduct.price * item.quantity);

                                }

                            }
                            item.userId = userid;
                            item.basketId = basket.id;
                            item.creationDate = DateTime.Now;
                            db.Orders.Add(item);
                            db.SaveChanges();
                            if (product.point != null)
                            {
                                for (int j = 0; j < item.quantity; j++)
                                {
                                    var proPoint = new UserPoint();
                                    proPoint.point = product.point;
                                    proPoint.userId = userid;
                                    proPoint.CreationDate = DateTime.Now;
                                    proPoint.orderId = item.id;
                                    proPoint.productId = product.id;
                                    db.UserPoints.Add(proPoint);
                                }
                            }

                        }
                        if (settings.shippingFree > sum)
                        {
                            basket.shippingPrice = settings.shippingPrice;
                        }
                        basket.Price = sum;
                        var discount = new DiscountCode();
                        int discountId = 0;
                        if (Session["Discount"] != null)
                        {
                            discountId = Convert.ToInt32(Session["Discount"]);
                            discount = db.DiscountCodes.Where(r => r.id == discountId).FirstOrDefault();

                        }
                        basket.status = "پرداخت شده درب منزل";
                        db.SaveChanges();
                        if (discount != null)
                        {
                            if (discount.Amont != null)
                            {
                                basket.DiscountAmount = discount.Amont;

                            }
                            else
                            {
                                if (discount.Percentage != null)
                                {
                                    if (discount.maxPercentage < basket.Price)
                                    {
                                        basket.DiscountAmount = ((discount.maxPercentage * discount.Percentage) / 100);
                                    }
                                    else
                                    {
                                        basket.DiscountAmount = ((basket.Price * discount.Percentage) / 100);
                                    }
                                }
                            }

                            basket.DiscountCode = discountId;
                            discount.isUsed = true;
                            discount.useDate = DateTime.Now;
                            discount.basketId = basket.id;

                        }
                        db.SaveChanges();
                    }
                    Session["orders"] = null;
                    db.SaveChanges();
                    return RedirectToAction("successorder", new { order = basket.id });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "payHomePost", "HomeController");
                    throw;
                }
                
            }
            return Json("done");
        }

        public ActionResult ServiceList()
        {
            return View();
        }

        public ActionResult Service()
        {
            return View();
        }

        #region برند ها
        public ActionResult Brands(int? id, string title)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult brandList(int? id, string title)
        {

            return View();
        }
        #endregion

        #region مقایسه
        [HttpPost]
        public ActionResult comparison(int[] product, int? cat)
        {
            var prods = product.ToList();
            if (prods.Count > 4)
            {
                prods = prods.Take(4).ToList();
            }
            ViewBag.cat = cat;
            ViewBag.id = prods;
            return View();
        }
        #endregion
        #region ثبت نام فروشنده
        public ActionResult sellerRegister()
        {
            return View();
        }

        [HttpPost]
        public ActionResult reciveGift(int? id)
        {
            checkLogin();
            if (Session["userLogin"] != null && id != null)
            {
                try
                {
                    int userid = Convert.ToInt16(Session["userLogin"]);
                    var points = db.UserPoints.Where(r => r.userId == userid).ToList().Sum(r => r.point);
                    var gift = db.Gifts.Where(r => r.id == id).FirstOrDefault();
                    if (points >= gift.price)
                    {
                        var order = new GiftOrder();
                        order.userid = userid;
                        order.creationDate = DateTime.Now;
                        order.isDelivery = false;
                        order.isSent = false;
                        order.GiftId = id;
                        order.pointRecived = Convert.ToInt32(gift.price);
                        db.GiftOrders.Add(order);
                        db.SaveChanges();
                        return RedirectToAction("giftlist", new { orderId = order.id });

                    }
                    return RedirectToAction("NotFound");
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "reciveGift", "HomeController");
                    throw;
                }
               
            }
            else
            {
                return RedirectToAction("NotFound");
            }
        }


        public ActionResult recivedGift()
        {
            checkLogin();
            if (Session["userLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("NotFound");

            }
        }




        [HttpPost]
        public ActionResult sellerRegister(string name, string lastname, string codeMeli, string email, string password, string rePassword, string shomareKart, string shomareSheba, string phone, List<int> sellerCats)
        {
            var seller = new Seller();
            if (!(string.IsNullOrWhiteSpace(name)) && !(string.IsNullOrWhiteSpace(lastname)) && !(string.IsNullOrWhiteSpace(codeMeli)) && !(string.IsNullOrWhiteSpace(email)) && !(string.IsNullOrWhiteSpace(password)) && !(string.IsNullOrWhiteSpace(rePassword)) && !(string.IsNullOrWhiteSpace(shomareKart)) && !(string.IsNullOrWhiteSpace(shomareSheba)) && !(string.IsNullOrWhiteSpace(phone)))
            {
                if (password.Length >= 5)
                {
                    if (password == rePassword)
                    {
                        if (IsValidEmail(email))
                        {
                            if (codeMeli.Length == 10)
                            {
                                if (phone.Length == 11)
                                {
                                    if (db.Sellers.Where(r => r.email.ToLower() == email.ToLower()).FirstOrDefault() == null)
                                    {
                                        try
                                        {
                                            seller.name = name;
                                            seller.lastname = lastname;
                                            seller.isValid = false;
                                            seller.phone = phone;
                                            seller.shomareKart = shomareKart;
                                            seller.sheba = shomareSheba;
                                            seller.codeMeli = codeMeli;
                                            seller.creationDate = DateTime.Now;
                                            seller.password = password;
                                            seller.email = email;
                                            db.Sellers.Add(seller);
                                            db.SaveChanges();
                                            if (sellerCats.Count != 0)
                                            {
                                                foreach (var item in sellerCats)
                                                {
                                                    var newSellerCat = new Seller_cats();
                                                    newSellerCat.sellerId = seller.id;
                                                    newSellerCat.catId = item;
                                                    db.Seller_cats.Add(newSellerCat);
                                                }
                                                db.SaveChanges();
                                            }
                                            ViewBag.success = 1;
                                        }
                                        catch (Exception ex)
                                        {
                                            OnlineShop.Class.Utility.CreateLog(ex, "sellerRegister", "HomeController");
                                            throw;
                                        }
                                       
                                    }
                                    else
                                    {
                                        ViewBag.error = "ایمیل وارد شده قبلا در سیستم ثبت نام کرده است";

                                    }

                                }
                                else
                                {
                                    ViewBag.error = "شماره موبایل وارد شده اشتباه است";
                                }


                            }
                            else
                            {
                                ViewBag.error = "کد ملی وارد شده اشتباه است";
                            }

                        }
                        else
                        {
                            ViewBag.error = "ایمیل وارد شده اشتباه است";
                        }

                    }
                    else
                    {
                        ViewBag.error = "پسورد وارد شده با تکرار آن همخوانی ندارد";
                    }
                }
                else
                {
                    ViewBag.error = "پسورد وارد شده باید بیشتر از 5 کاراکتر باشد";

                }
            }
            else
            {
                ViewBag.error = "برای ثبت نام باید تمامی فیلد های مورد نظر را پر کنید";
            }


            return View();




        }

        #endregion

        public ActionResult Points()
        {
            return View();
        }
    }
}