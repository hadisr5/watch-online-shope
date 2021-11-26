using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using OnlineShop.Models;

using OnlineShop.Areas.Admin.Security;
using static OnlineShop.Areas.Admin.Security.AccessModel;
using OnlineShop.Security;

namespace OnlineShop.Areas.Admin.Controllers
{

    [CustomAuthorize(Roles = "Admin,SuperAdmin")]
    [Log]
    [DisplayName("مدیریت سطوح دسترسی")]
    public class AccessLevelController : Controller
    {

        OnlineShopEntities db = new OnlineShopEntities();
        [OnlineShop.Areas.Admin.Security.Access("نمایش سطوح دسترسی کاربر")]
        public ActionResult Index(int id)
        {
            try
            {
                ViewBag.id = id;
                Assembly asm = Assembly.GetAssembly(typeof(OnlineShop.MvcApplication));
                var controlleractionlist = asm.GetTypes()
                        .Where(type => typeof(System.Web.Mvc.Controller).IsAssignableFrom(type))
                        .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                        .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any() && m.DeclaringType.CustomAttributes.FirstOrDefault() != null)

                    .Select(
                x => new
                {
                    Controller = x.DeclaringType.Name,
                    Action = x.Name,
                    ReturnType = x.ReturnType.Name,
                    Attributes = String.Join(",", x.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))),
                    displayName = x.CustomAttributes,
                    controller = x.DeclaringType.CustomAttributes.FirstOrDefault().ConstructorArguments.FirstOrDefault().Value

                }
                    )
                    .OrderBy(x => x.Controller).ThenBy(x => x.Action).ToList();
                var accessList = new List<_Access>();
                foreach (var item in controlleractionlist)
                {
                    var singleAccess = new _Access();
                    singleAccess.name = item.controller?.ToString();
                    singleAccess.Controller = item.Controller;
                    accessList.Add(singleAccess);
                }
                accessList = accessList.DistinctBy(p => p.name).ToList();
                foreach (var item in accessList)
                {
                    var newlist = controlleractionlist.Where(r => r.controller?.ToString() == item.name).ToList();
                    if (newlist.Count != 0)
                    {
                        var MethodList = new List<_Method>();
                        foreach (var item2 in newlist)
                        {
                            if (item2.displayName.ToList().Count != 0)
                            {
                                var _m = new _Method();
                                if (item2.displayName.Where(r => r.AttributeType.Name == "Access").FirstOrDefault() != null)
                                {
                                    _m.name = item2.displayName.Where(r => r.AttributeType.Name == "Access").FirstOrDefault().ConstructorArguments.FirstOrDefault().Value.ToString();
                                    if (MethodList.Where(r => r.name == _m.name).FirstOrDefault() == null)
                                    {
                                        MethodList.Add(_m);
                                    }
                                }

                            }
                        }
                        item.methods = MethodList;
                    }
                }
                ViewBag.listOfPermissions = accessList.OrderBy(r => r.methods.Count).ToList();
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "Index", "AccessLevelController");
                throw;
            }
            return View();
        }


        [OnlineShop.Areas.Admin.Security.Access("تغییر سطوح دسترسی")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult changeAcessLevel(int id, List<string> Permission)
        {
            try
            {
                db.Permissions.RemoveRange(db.Permissions.Where(r => r.AdminId == id).ToList());
           
            if (Permission.Count != 0)
            {
                foreach (var item in Permission)
                {
                    var Splitor = item.Split('_');
                    var newPer = new Permission();
                    newPer.AdminId = id;
                    newPer.Controller = Splitor[1];
                    newPer.Permition = Splitor[0];
                    db.Permissions.Add(newPer);
                }
                db.SaveChanges();


            }
            }
            catch (Exception ex)
            {
                Class.Utility.CreateLog(ex, "changeAcessLevel", "AccessLevelController");
                throw;
            }
            return Json(string.Empty);
        }





    }
}