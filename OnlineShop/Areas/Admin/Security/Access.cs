using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Security
{
    public class Access : System.Web.Mvc.ActionFilterAttribute
    {
        OnlineShopEntities db = new OnlineShopEntities();
        string Permition;

        public Access(string per)
        {
            Permition = per;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["login"]!=null)
            {
                int id = Convert.ToInt16(filterContext.HttpContext.Session["login"]);
                var admin = db.Users.Find(id);

                if (admin == null)
                    filterContext.Result = new RedirectResult(string.Format("/admin/login", filterContext.HttpContext.Request.Url.AbsolutePath));
                else
                {
                    var Permissions = db.Permissions.Where(r => r.AdminId == id && r.Permition == Permition).FirstOrDefault();
                    if (Permissions == null)
                    {
                        filterContext.Result = new RedirectResult(string.Format("/admin/AccessDenied/index", filterContext.HttpContext.Request.Url.AbsolutePath));
                    }
                }
            }
            else
            {
                RenewAdminSession();
                if (filterContext.HttpContext.Session["login"] != null)
                {
                    int id = Convert.ToInt16(filterContext.HttpContext.Session["login"]);
                    var admin = db.Users.Find(id);

                    if (admin == null)
                        filterContext.Result = new RedirectResult(string.Format("/admin/login", filterContext.HttpContext.Request.Url.AbsolutePath));
                    else
                    {
                        var Permissions = db.Permissions.Where(r => r.AdminId == id && r.Permition == Permition).FirstOrDefault();
                        if (Permissions == null)
                        {
                            filterContext.Result = new RedirectResult(string.Format("/admin/AccessDenied/index", filterContext.HttpContext.Request.Url.AbsolutePath));
                        }
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult(string.Format("/admin/AccessDenied/index", filterContext.HttpContext.Request.Url.AbsolutePath));
                }
            }
        }
        public void RenewAdminSession()
        
        {
            if (System.Web.HttpContext.Current.Session["login"] == null)
            {
                HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies["OnlineShopAdminAuthCookie"];
                if (authCookie != null)
                {
                    string rnd = authCookie.Value;
                    var user = db.Users.Where(r => r.token == rnd).FirstOrDefault();
                    if (user != null)
                    {
                        System.Web.HttpContext.Current.Session["login"] = user.id;
                    }
                }
            }
        }
    }
}