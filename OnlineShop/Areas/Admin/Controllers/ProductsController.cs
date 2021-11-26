using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Models;
using System.Data.Entity.Migrations;
using Newtonsoft.Json;
using OnlineShop.Class;
using System.Data;
using System.Data.OleDb;

using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;
using System.Configuration;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت محصولات")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class ProductsController : Controller
    {
        OnlineShopEntities db = new OnlineShopEntities();
        checkLoginAdmin ch = new checkLoginAdmin();
        // GET: Admin/Products
        [Access("لیست محصولات")]
        public ActionResult Index()
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [HttpPost]
        [Access("لیست محصولات")]
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
                    var Result = fn.CreateTable("Products", null, Request);
                    var jobject = JsonConvert.DeserializeObject<List<Product>>(Result.content);
                    return Json(new { iTotalRecords = Result.total, iTotalDisplayRecords = Result.total, sEcho = 0, sColumns = "", aaData = jobject }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    OnlineShop.Class.Utility.CreateLog(ex, "List", "ProductsController");
                    return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, sEcho = 0, sColumns = string.Empty, aaData = string.Empty }, JsonRequestBehavior.AllowGet); ;
                    throw;
                }

            }
            else
            {
                return Json(string.Empty);
            }

        }
        [Access("ایجاد محصولات")]
        public ActionResult Create()
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        [Access("ویرایش محصولات")]
        public ActionResult Edit(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
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
                db.Configuration.ProxyCreationEnabled = false;
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
        [Access("حذف محصولات")]
        public ActionResult remove(int? id)
        {
            ch.checkLogin();

            if (Session["login"] != null)
            {
                try
                {
                    db.Products.Remove(db.Products.Find(id));
                    db.SaveChanges();
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
        [Access("ویرایش محصولات")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_product(List<string> colorSize, List<int> ProductSizes, Product pro, string Tags, List<string> image, List<int?> cat, string desc, int? InventoryCount, int? RattingGroup)
        {
            ch.checkLogin();


            if (pro.Inventory == null && InventoryCount != 0 && InventoryCount != null)
            {
                pro.Inventory = InventoryCount;
            }
            if (Session["login"] != null)
            {
                //db.SizeProducts.RemoveRange(db.SizeProducts.Where(r => r.productId == pro.id).ToList());
                //db.SaveChanges();
                //if (ProductSizes.Count != 0)
                //{
                //    foreach (var item in ProductSizes)
                //    {
                //        var newItem = new SizeProduct();
                //        newItem.productId = pro.id;
                //        newItem.SizeId = item;
                //        db.SizeProducts.Add(newItem);
                //    }
                //    db.SaveChanges();
                //}
                //if (db.ColorSizes.Where(r => r.ProductId == pro.id).ToList().Count != 0)
                //{
                //    db.ColorSizes.RemoveRange(db.ColorSizes.Where(r => r.ProductId == pro.id).ToList());
                //    db.SaveChanges();
                //}
                //if (colorSize != null && colorSize.Count != 0)
                //{
                //    foreach (var item in colorSize)
                //    {
                //        var sizeColor = new ColorSize();
                //        sizeColor.ProductId = pro.id;
                //        sizeColor.SizeId = Convert.ToInt32(item.Substring(0, item.IndexOf("_")));
                //        int t1 = item.IndexOf("_") + 1;
                //        int t2 = item.Length;
                //        int t3 = t2 - t1;
                //        sizeColor.colorId = Convert.ToInt32(item.Substring(t1, t3));
                //        if (Request.Params["colorInventory_" + item] != null)
                //        {
                //            try
                //            {
                //                var test = Request.Params["colorInventory_" + item];
                //                sizeColor.Inventory = Convert.ToInt32(Request.Params["colorInventory_" + item]);
                //            }
                //            catch (Exception)
                //            {

                //            }

                //        }
                //        db.ColorSizes.Add(sizeColor);
                //        db.SaveChanges();
                //    }
                //}

                try
                {



                    db.ProductRatings.Remove(db.ProductRatings.Where(r => r.productId == pro.id).FirstOrDefault());

                    var ratingGroupProduct = new ProductRating();
                    ratingGroupProduct.productId = pro.id;
                    ratingGroupProduct.groupId = RattingGroup;

                    db.ProductRatings.Add(ratingGroupProduct);
                    db.SaveChanges();

                    pro.description = desc;
                    db.Products.AddOrUpdate(pro);

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
                        db.Database.CurrentTransaction.Rollback();
                        OnlineShop.Class.Utility.CreateLog(ex, "Edit_product", "ProductsController");
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
                return Redirect("/Admin/AdminDashboard/Index#/Admin/products/Index");
            }
            else
            {
                return Json(string.Empty);

            }
        }
        [HttpPost]
        [ValidateInput(false)]
        [Access("ایجاد محصولات")]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Product(List<string> colorSize, List<int> ProductSizes, Product pro, string Tags, List<string> image, List<int?> cat, string desc, int? InventoryCount, int? brandId, int? RattingGroup)
        {
            ch.checkLogin();

            if (pro.Inventory == null && InventoryCount != 0 && InventoryCount != null)
            {
                pro.Inventory = InventoryCount;
            }
            if (Session["login"] != null)
            {
                try
                {


                    pro.description = desc;
                    db.Products.Add(pro);
                    db.SaveChanges();

                    //if (colorSize!=null && colorSize.Count != 0)
                    //{
                    //    foreach (var item in colorSize)
                    //    {
                    //        var sizeColor = new ColorSize();
                    //        sizeColor.ProductId = pro.id;
                    //        sizeColor.SizeId = Convert.ToInt32(item.Substring(0, item.IndexOf("_")));
                    //        int t1 = item.IndexOf("_") + 1;
                    //        int t2 = item.Length;
                    //        int t3 = t2 - t1;
                    //        sizeColor.colorId = Convert.ToInt32(item.Substring(t1, t3));
                    //        if (Request.Params["colorInventory_" + item] != null)
                    //        {
                    //            try
                    //            {
                    //                var test = Request.Params["colorInventory_" + item];
                    //                sizeColor.Inventory = Convert.ToInt32(Request.Params["colorInventory_" + item]);
                    //            }
                    //            catch (Exception)
                    //            {

                    //            }

                    //        }
                    //        db.ColorSizes.Add(sizeColor);
                    //        db.SaveChanges();
                    //    }
                    //}


                    //if (ProductSizes!=null && ProductSizes.Count != 0)
                    //{
                    //    foreach (var item in ProductSizes)
                    //    {
                    //        var newItem = new SizeProduct();
                    //        newItem.SizeId = item;
                    //        newItem.productId = pro.id;
                    //        db.SizeProducts.Add(newItem);
                    //    }
                    //    db.SaveChanges();
                    //}
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
                        db.Database.CurrentTransaction.Rollback();
                        OnlineShop.Class.Utility.CreateLog(ex, "Create_Product", "ProductsController");
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
                return Redirect("/Admin/AdminDashboard/Index#/Admin/products/Index");
            }
            else
            {
                return RedirectToAction("NotFound", "Home", new { area = "" });
            }
        }
        public JsonResult upload()
        {
            ch.checkLogin();

            if (Session["login"] != null)
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
                    OnlineShop.Class.Utility.CreateLog(ex, "Create_Product", "ProductsController");
                    throw;

                }
               

            }
            return Json("done");
        }
    }


}