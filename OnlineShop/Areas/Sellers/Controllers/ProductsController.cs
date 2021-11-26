using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using Newtonsoft.Json;
using System.Configuration;

namespace OnlineShop.Areas.Sellers.Controllers
{
    public class ProductsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginSeller ch = new checkLoginSeller();
        // GET: Admin/Products
        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["seller"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        public ActionResult Create()
        {
            ch.checkLogin();

            if (Session["seller"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        public ActionResult Edit(int? id)
        {
            ch.checkLogin();

            if (Session["seller"] != null)
            {
                ViewBag.id = id;
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }

        }
        [HttpPost]
        public JsonResult checkCatProperties(int? id)
        {
            ch.checkLogin();
            try
            {
                var properties = db.CatProperties.Where(r => r.catId == id).ToList();
                if (properties.Count != 0)
                {
                    return Json(properties);

                }
                else
                {
                    return Json(string.Empty);

                }
            }
            catch (Exception ex)
            {
                OnlineShop.Class.Utility.CreateLog(ex, "checkCatProperties", "ProductsController");
                throw;
            }
           
        }
        [HttpPost]
        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["seller"] != null)
            {
                try
                {
                    int userid = Convert.ToInt16(Session["seller"]);

                    var sellerPro = db.SellersProducts.Where(r => r.sellerId == userid && r.productId == id).FirstOrDefault();
                    if (sellerPro != null)
                    {
                        db.SellersProducts.Remove(sellerPro);
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "remove", "ProductsController");
                    throw;
                }
               
            }
            return Json("success");

        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_product(Product pro, List<string> image,string Tags, List<int?> cat, string desc, int? InventoryCount)
        {
            ch.checkLogin();
            int sellerId = Convert.ToInt16(Session["seller"]);
            if (pro.Inventory == null && InventoryCount != 0 && InventoryCount != null)
            {
                pro.Inventory = InventoryCount;
            }
            if (Session["seller"] != null)
            {
                try
                {
                    pro.description = desc;
                db.Products.AddOrUpdate(pro);
                var savedspro = db.SellersProducts.FirstOrDefault(p => p.productId == pro.id && p.sellerId == sellerId);
                if(savedspro!=null)
                {
                    savedspro.isValid = pro.isValid;
                    savedspro.price = pro.price;
                    savedspro.isActive = pro.Active;
                    db.SellersProducts.AddOrUpdate(savedspro);
                }
                
                    db.ProductGalleries.RemoveRange(db.ProductGalleries.Where(r => r.product_id == pro.id).ToList());
                

                    db.CatProes.RemoveRange(db.CatProes.Where(r => r.product_id == pro.id).ToList());

                    db.ProductPropertiesValues.RemoveRange(db.ProductPropertiesValues.Where(r => r.proId == pro.id).ToList());

                db.SaveChanges();

                if (image != null && image.Count != 0)
                {
                    foreach (var item in image)
                    {
                        if (item.Trim() != "")
                        {
                            var gl = new ProductGallery();
                            gl.product_id = pro.id;
                            gl.big = item;
                            db.ProductGalleries.Add(gl);
                        }

                    }
                }

                if (cat != null && cat.Count != 0)
                {
                    foreach (var item in cat)
                    {
                        if (item != 0)
                        {
                            var catpro = new CatPro();
                            catpro.categoryId = item;
                            catpro.product_id = pro.id;
                            db.CatProes.Add(catpro);
                            db.SaveChanges();
                        }

                    }
                }
                db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Edit_product", "ProductsController");
                    throw;
                }
                if (Tags != null && Tags != "")
                {
                    try
                    {
                        var tags = JsonConvert.DeserializeObject<List<TagString>>(Tags);
                        var taglist = new List<Tag>();
                        db.Database.BeginTransaction();
                        tags.ForEach(t =>
                        {
                            var savedtag = db.Tags.Where(p => p.title == t.value).FirstOrDefault();
                            if (savedtag == null)
                            {
                                var newtag = new Tag() { title = t.value };
                                db.Tags.Add(newtag);
                                taglist.Add(newtag);
                            }
                            else
                            {
                                taglist.Add(savedtag);
                            }

                        });
                        db.SaveChanges();
                        var producttags = db.ProductTags.Where(p => p.ProductId == pro.id).ToList();
                        db.ProductTags.RemoveRange(producttags);
                        db.SaveChanges();
                        taglist.ForEach(st =>
                        {
                            var ptag = new ProductTag() { TagId = st.id, ProductId = pro.id };
                            db.ProductTags.Add(ptag);
                        });

                        db.SaveChanges();
                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "Edit_product", "ProductsController");
                        db.Database.CurrentTransaction.Rollback();
                        throw;
                    }
                
                }

                foreach (string name in Request.Form.AllKeys)
                {
                    try
                    {
                        int id = Convert.ToInt16(name);
                        if (id != 0)
                        {
                            var properties = new ProductPropertiesValue();
                            properties.value = Request.Form[name];
                            properties.proId = pro.id;
                            properties.propertiesid = id;
                            db.ProductPropertiesValues.Add(properties);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "Edit_product", "ProductsController");
                        throw;
                    }
                }
                return Redirect("/sellers/sellerDashboard/Index#/sellers/products/Index");
            }
            else
            {
                return Json(string.Empty);

            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Product(Product pro, List<string> image, List<int?> cat,string Tags, string desc, int? InventoryCount, int? brandId)
        {
            pro.creationDate = DateTime.Now;
            double price =Convert.ToInt32( pro.price);
            pro.price = null; 
            int sellerId = Convert.ToInt16(Session["seller"]);
            pro.sellerId = sellerId; 
            ch.checkLogin();

            if (pro.Inventory == null && InventoryCount != 0 && InventoryCount != null)
            {
                pro.Inventory = InventoryCount;
            }
            if (Session["seller"] != null)
            {
                try
                {
                    pro.description = desc;
                    db.Products.Add(pro);
                    db.SellersProducts.Add(new SellersProduct
                    {
                        creationDate = DateTime.Now,
                        isValid = pro.isValid,
                        productId = pro.id,
                        sellerId = sellerId,
                        price = pro.price,
                        isActive = pro.Active,
                        submitDate = DateTime.Now
                    });
                    db.SaveChanges();
                    if (image != null && image.Count != 0)
                    {
                        foreach (var item in image)
                        {
                            if (item.Trim() != "")
                            {
                                var gl = new ProductGallery();
                                gl.product_id = pro.id;
                                gl.big = item;
                                db.ProductGalleries.Add(gl);
                            }

                        }
                    }

                    if (cat != null && cat.Count != 0)
                    {
                        foreach (var item in cat)
                        {
                            if (item != 0)
                            {
                                var catpro = new CatPro();
                                catpro.categoryId = item;
                                catpro.product_id = pro.id;
                                db.CatProes.Add(catpro);
                                db.SaveChanges();
                            }

                        }
                    }

                    if (price != 0)
                    {
                        var sellerPrice = new SellersProduct();
                        sellerPrice.price = Convert.ToInt32(price);
                        sellerPrice.sellerId = sellerId;
                        sellerPrice.isValid = null;
                        sellerPrice.productId = pro.id;

                        sellerPrice.creationDate = DateTime.Now;
                        db.SellersProducts.Add(sellerPrice);
                        db.SaveChanges();

                    }



                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "Create_Product", "ProductsController");
                    throw;
                }
            

                if (Tags != null && Tags != "")
                {
                    try
                    {
                        var tags = JsonConvert.DeserializeObject<List<TagString>>(Tags);
                        var taglist = new List<Tag>();
                        db.Database.BeginTransaction();
                        tags.ForEach(t =>
                        {
                            var savedtag = db.Tags.Where(p => p.title == t.value).FirstOrDefault();
                            if (savedtag == null)
                            {
                                var newtag = new Tag() { title = t.value };
                                db.Tags.Add(newtag);
                                taglist.Add(newtag);
                            }
                            else
                            {
                                taglist.Add(savedtag);
                            }

                        });
                        db.SaveChanges();
                        taglist.ForEach(st =>
                        {
                            var ptag = new ProductTag() { TagId = st.id, ProductId = pro.id };
                            db.ProductTags.Add(ptag);
                        });

                        db.SaveChanges();
                        db.Database.CurrentTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "Create_Product", "ProductsController");
                        db.Database.CurrentTransaction.Rollback();
                        throw;
                    }
                }

                foreach (string name in Request.Form.AllKeys)
                {
                    try
                    {
                        int id = Convert.ToInt16(name);
                        if (id != 0)
                        {
                            var properties = new ProductPropertiesValue();
                            properties.value = Request.Form[name];
                            properties.proId = pro.id;
                            properties.propertiesid = id;
                            db.ProductPropertiesValues.Add(properties);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        OnlineShop.Class.Utility.CreateLog(ex, "Create_Product", "ProductsController");
                        throw;
                    }
                }
                return Redirect("/sellers/sellerDashboard/Index#/sellers/products/Index");
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        public JsonResult upload()
        {
            ch.checkLogin();

            if (Session["seller"] != null)
            {
                try
                {
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
                        var appsettings = ConfigurationManager.AppSettings;
                        var baseaddress = appsettings["uploadpath"].ToString();
                        file.SaveAs(baseaddress + "uploads/" + dateString + ".jpg");
                        return Json("/uploads/" + dateString + ".jpg");

                    }
                    return Json(fileUrl);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "upload", "ProductsController");
                    throw;
                }
               

            }
            return Json("done");
        }
    }
}