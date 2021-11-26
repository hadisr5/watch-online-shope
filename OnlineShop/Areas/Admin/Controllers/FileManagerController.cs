using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineShop.Class;
using System.ComponentModel;
using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{
    [DisplayName("مدیریت فایل های پروژه")]
    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    public class FileManagerController : Controller
    {
        checkLoginAdmin ch = new checkLoginAdmin();
        [Access("فایل منیجر")]

        // GET: Admin/FileManager
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Access("اطلاعات فایل ها")]

        public ActionResult FileInfo(string fileName, Boolean isFolder)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                ViewBag.isFolder = isFolder;
                ViewBag.FileName = fileName;
                return View();
            }
            else
            {
                return Json(string.Empty);
            }

        }
        [Access("دانلود فایل ها")]

        public ActionResult download(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                try
                {
                    string filename2 = Path.GetFileName(filename);
                    //string filepath = AppDomain.CurrentDomain.BaseDirectory + filename;
                    byte[] filedata = System.IO.File.ReadAllBytes(filename);
                    string contentType = MimeMapping.GetMimeMapping(filename);

                    var cd = new System.Net.Mime.ContentDisposition
                    {
                        FileName = filename2,
                        Inline = true,
                    };

                    Response.AppendHeader("Content-Disposition", cd.ToString());

                    return File(filename, contentType);
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "download", "FileManagerController");
                    throw;
                }
                
            }
            else
            {
                return Json(string.Empty);
            }

        }

        [Access("حذف فایل ها")]

        [HttpPost]
        public ActionResult removeFiles(List<string> fileName)
        {
            if (fileName.Count != 0)
            {
                try
                {
                    foreach (var item in fileName)
                    {
                        if (System.IO.File.Exists(item))
                        {
                            System.IO.File.Delete(item);
                        }
                        else
                        {
                            try
                            {
                                Directory.Delete(item, true);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "removeFiles", "FileManagerController");
                    throw;
                }
               
            }
            return Json(string.Empty);
        }

        [HttpPost]
        [Access("ویرایش نام فایل ها")]
        [ValidateAntiForgeryToken]
        public JsonResult Rename(string path, string val, string folderName, string fileName, Boolean? isFolder, Boolean? isFile)
        {
            string fname = Path.GetFileName(path);
            if (isFolder != null && isFolder == true)
            {
                try
                {
                    bool exists = System.IO.Directory.Exists(path + val);
                    if (!exists)
                    {
                        Directory.Move(path + folderName, path + val);
                    }
                    else
                    {
                        return Json(new { title = "error", desc = "فولدر " + val + " پیش تر در این Directory وجود دارد ، لطفا نام دیگری انتخاب کنید." });

                    }

                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Rename", "FileManagerController");
                    throw;
                }
                
            }
            else
            {
                try
                {
                    if (!System.IO.File.Exists(path + val))
                    {
                        System.IO.File.Move(path + fileName, path + val);
                    }
                    else
                    {
                        return Json(new { title = "error", desc = "فایل " + val + " پیش تر در این Directory وجود دارد ، لطفا نام دیگری انتخاب کنید." });
                    }
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "Rename", "FileManagerController");
                    throw;
                }
                
            }
            //DirectoryInfo parentDir = Directory.GetParent(path);

            // The result is available here
            //var myParentDir = parentDir.Parent.FullName;
            //string newPath = myParentDir+"\\" + val;

            //Directory.Move(path, newPath);
            //System.IO.File.Move(path, newPath);

            return Json(string.Empty);
        }



        [HttpPost]
        [Access("حذف فولدر جدید")]

        public JsonResult CreateNewFolder(string path)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    int i = 1;
                    string pth = path + "New folder";
                    while (Directory.Exists(pth))
                    {
                        pth = path + "New folder(" + i + ")";
                        i++;
                    }
                    System.IO.Directory.CreateDirectory(pth);
                    return Json(new { newFolder = pth.Replace(path, "") });
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "CreateNewFolder", "FileManagerController");
                    throw;
                }
               
            }
            return Json(string.Empty);
        }







        [HttpPost]
        [Access("بارگذاری فایل جدید")]

        public ActionResult Upload(string path)
        {
            ViewBag.path = path;
            return View();
        }
        [Access("بارگذاری فایل جدید")]

        public JsonResult uploader(string pth)
        {
            ch.checkLogin();
            if (Session["login"] != null)
            {
                try
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                                                                    //Use the following properties to get file's name, size and MIMEType
                        int fileSize = file.ContentLength;
                        string fileName = file.FileName;
                        string mimeType = file.ContentType;
                        int j = 1;
                        string filename = file.FileName;
                        while (System.IO.File.Exists(pth + filename))
                        {
                            filename = file.FileName.Replace(".", "(" + j + ").");
                            j++;
                        }
                        System.IO.Stream fileContent = file.InputStream;
                        file.SaveAs(pth + filename); //File will be saved in application root
                        return Json(pth + filename);
                    }
                }
                catch (Exception ex)
                {
                    Class.Utility.CreateLog(ex, "uploader", "FileManagerController");
                    throw;
                }
                
            }
            return Json("done");
        }
    }
}