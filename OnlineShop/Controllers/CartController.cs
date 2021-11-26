using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Controllers
{
    public class CartController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCard(int? productId, int? quantity, int? seller, int? colorId, int? sizeId)
        {
            if (true)
            {
                try
                {
                    int userid = Convert.ToInt32(Session["userLogin"]);
                    var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                    var prime = false;
                    if (user != null && user.Special == true)
                    {
                        prime = true;
                    }
                    long singleProductPrice = 0;
                    var product = db.Products.Where(r => r.id == productId).FirstOrDefault();
                    var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == product.id && r.expire > DateTime.Now).FirstOrDefault();

                    if (seller == null)
                    {
                        if (off != null)
                        {
                            if (prime)
                            {
                                singleProductPrice += Convert.ToInt64((product.price - (product.price * off.percentage) / 100));
                            }
                            else
                            {
                                singleProductPrice += Convert.ToInt64((product.priceForSpecialUsers - (product.priceForSpecialUsers * off.percentage) / 100));
                            }

                        }
                        else
                        {
                            if (prime)
                            {
                                singleProductPrice = Convert.ToInt64(product.priceForSpecialUsers);
                            }
                            else
                            {
                                singleProductPrice = Convert.ToInt64(product.price);
                            }

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
                    var order = new Order();
                    order.SellerId = seller;
                    order.userId = userid;

                    order.colorId = colorId;
                    order.sizeId = sizeId;

                    if (sizeId != null && sizeId != 0)
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
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "AddToCard", "CartController");
                    throw;
                }
                
            }

            return RedirectToAction("index");
        }
        [HttpPost]
        public ActionResult Products()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AddProduct(int id)
        {
            if (Session["orders"] != null)
            {
                try
                {
                    int userid = Convert.ToInt32(Session["userLogin"]);
                    var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                    var prime = false;
                    if (user != null && user.Special == true)
                    {
                        prime = true;
                    }
                    Int64 sum = 0;
                    Int64 total = 0;
                    int quantity = 0;
                    var ordersList = Session["orders"] as List<Order>;
                    if (ordersList.Where(r => r.productId == id).FirstOrDefault() != null)
                    {
                        var preOrder = ordersList.Where(r => r.productId == id).FirstOrDefault();
                        preOrder.quantity++;
                        quantity = Convert.ToInt16(preOrder.quantity);

                        if (true)
                        {
                            var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == preOrder.productId && r.expire > DateTime.Now).FirstOrDefault();
                            var sellerPro = db.SellersProducts.Where(r => r.sellerId == preOrder.SellerId && r.productId == preOrder.productId).FirstOrDefault();
                            var pro = db.Products.Where(r => r.id == preOrder.productId).FirstOrDefault();
                            if (sellerPro == null)
                            {
                                if (off != null)
                                {
                                    if (prime)
                                    {
                                        sum += Convert.ToInt64((pro.priceForSpecialUsers - (pro.priceForSpecialUsers * off.percentage) / 100) * preOrder.quantity);

                                    }
                                    else
                                    {
                                        sum += Convert.ToInt64((pro.price - (pro.price * off.percentage) / 100) * preOrder.quantity);

                                    }
                                }
                                else
                                {
                                    if (prime)
                                    {
                                        sum += Convert.ToInt64((pro.priceForSpecialUsers) * preOrder.quantity);

                                    }
                                    else
                                    {
                                        sum += Convert.ToInt64((pro.price) * preOrder.quantity);

                                    }
                                }
                            }
                            else
                            {
                                sum += Convert.ToInt64((sellerPro.price) * preOrder.quantity);
                            }
                        }


                    }
                    if (ordersList != null && ordersList.Count != 0)
                    {
                        total = CalculateTotalBasketPrice(ordersList);
                    }

                    Session["orders"] = ordersList;
                    return Json(new { sum = sum, total = total, quantity = quantity });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "AddProduct", "CartController");
                    throw;
                }
               
            }
            return Json(string.Empty);
        }
        [HttpPost]
        public JsonResult ReduceProduct(int id)
        {
            if (Session["orders"] != null)
            {
                try
                {
                    int userid = Convert.ToInt32(Session["userLogin"]);
                    var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                    var prime = false;
                    if (user != null && user.Special == true)
                    {
                        prime = true;
                    }
                    Int64 sum = 0;
                    Int64 total = 0;
                    int quantity = 0;
                    var ordersList = Session["orders"] as List<Order>;
                    if (ordersList.Where(r => r.productId == id).FirstOrDefault() != null)
                    {
                        var preOrder = ordersList.Where(r => r.productId == id).FirstOrDefault();
                        if (preOrder.quantity != 1)
                        {
                            preOrder.quantity--;
                        }
                        quantity = Convert.ToInt16(preOrder.quantity);
                        if (true)
                        {
                            var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == preOrder.productId && r.expire > DateTime.Now).FirstOrDefault();
                            var sellerPro = db.SellersProducts.Where(r => r.sellerId == preOrder.SellerId && r.productId == preOrder.productId).FirstOrDefault();
                            var pro = db.Products.Where(r => r.id == preOrder.productId).FirstOrDefault();
                            if (sellerPro == null)
                            {
                                if (off != null)
                                {
                                    if (prime)
                                    {
                                        sum += Convert.ToInt64((pro.priceForSpecialUsers - (pro.priceForSpecialUsers * off.percentage) / 100) * preOrder.quantity);
                                    }
                                    else
                                    {
                                        sum += Convert.ToInt64((pro.price - (pro.price * off.percentage) / 100) * preOrder.quantity);
                                    }
                                }
                                else
                                {
                                    if (prime)
                                    {
                                        sum += Convert.ToInt64((pro.priceForSpecialUsers) * preOrder.quantity);
                                    }
                                    else
                                    {
                                        sum += Convert.ToInt64((pro.price) * preOrder.quantity);

                                    }
                                }
                            }
                            else
                            {
                                sum += Convert.ToInt64((sellerPro.price) * preOrder.quantity);
                            }
                        }
                    }
                    if (ordersList != null && ordersList.Count != 0)
                    {
                        total = CalculateTotalBasketPrice(ordersList);
                    }
                    Session["orders"] = ordersList;
                    return Json(new { sum = sum, total = total, quantity = quantity });
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "ReduceProduct", "CartController");
                    throw;
                }
                
            }
            return Json(string.Empty);
        }
        [HttpPost]
        public JsonResult RemoveProduct(int id)
        {
            if (Session["orders"] != null)
            {
                try
                {
                    var ordersList = Session["orders"] as List<Order>;
                    if (ordersList.Where(r => r.productId == id).FirstOrDefault() != null)
                    {
                        var preOrder = ordersList.Where(r => r.productId == id).FirstOrDefault();
                        ordersList.Remove(preOrder);
                    }
                    Session["orders"] = ordersList;
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "RemoveProduct", "CartController");
                    throw;
                }
               
            }
            return Json(string.Empty);
        }
        [HttpPost]
        public ActionResult miniCart() {
            return View();
        }      
        public Int64 CalculateTotalBasketPrice(List<Order> ordersList)
        {
            try
            {
                int userid = Convert.ToInt32(Session["userLogin"]);
                var user = db.Users.Where(r => r.id == userid).FirstOrDefault();
                var prime = false;
                if (user != null && user.Special == true)
                {
                    prime = true;
                }
                Int64 sum = 0;
                foreach (var item in ordersList)
                {
                    var off = db.Discounts.Where(r => r.percentage != null && r.status == 1 && r.productId == item.productId && r.expire > DateTime.Now).FirstOrDefault();
                    var sellerPro = db.SellersProducts.Where(r => r.sellerId == item.SellerId && r.productId == item.productId).FirstOrDefault();
                    var pro = db.Products.Where(r => r.id == item.productId).FirstOrDefault();
                    if (sellerPro == null)
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
                return sum;
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "CalculateTotalBasketPrice", "CartController");
                throw;
            }           

        }


    }
}