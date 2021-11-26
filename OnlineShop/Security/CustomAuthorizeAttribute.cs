using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OnlineShop.Models;


namespace OnlineShop.Security
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        OnlineShopEntities db = new OnlineShopEntities();

        private static bool SkipAuthorization(AuthorizationContext filterContext)
        {
            Contract.Assert(filterContext != null);

            return filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any()
                   || filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();
        }

       

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //var controllerName = (string)routingValues["controller"];

            //var actionName = (string)routingValues["action"];

      


            Controller controller = filterContext.Controller as Controller;

            //را داشته باشد چک دسترسی ها انجام نمیشود AllowAnonymousAttribute اگر اکشن یا کنترلر دسترسی 
            if (SkipAuthorization(filterContext)) return;



            //----------------------------خواندن کوکی
            


            HttpCookie cookie = filterContext.HttpContext.Request.Cookies.Get("OnlineShopAdminAuthCookie");
            if (cookie != null)
            {

                string actionName = filterContext.ActionDescriptor.ActionName;
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

                bool isIpAccessToRoute = CheckAccessIpToRoute(actionName,controllerName);

                

                var cookieValue = Security.RsaEncryptDecrypt.RSACls.RSADecryption(cookie.Value, Properties.Settings.Default.PrivateKey);
                string[] tickets = cookieValue.Split(new[] { ">>>" }, StringSplitOptions.None);
                HttpContext.Current.Session["Username"] = tickets[0];
                HttpContext.Current.Session["FullName"] = tickets[1];
                HttpContext.Current.Session["UserId"] = int.Parse(tickets[2]);

                var userid = int.Parse(tickets[2]);
                try
                {
                    db = new OnlineShopEntities();
                   //db.Entry(Models.User).Reload();
                }
                catch (Exception ex)
                {

                    throw;
                }
              
                //using () { 
                
                //}
                    var oldUser = db.Users.SingleOrDefault(q => q.id == userid);
                if (oldUser.Hostname != Utility.GetHostName())
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                          (new { area = "admin", controller = "login", action = "Logout" }));
                    return;
                }
                DateTime oDate = DateTime.Parse(tickets[3].ToString());


                TimeSpan span = DateTime.Now.Subtract(oDate);
                int second = (Security.SettingApp._TimeoutLogin * 60) - Convert.ToInt32(Math.Round(span.TotalSeconds, 0));

                if (second <= 0)
                {
                    //controller.HttpContext.Response.Redirect("./Login");

                    HttpCookie currentUserCookie = HttpContext.Current.Request.Cookies["OnlineShopAdminAuthCookie"];
                    HttpContext.Current.Response.Cookies.Remove("OnlineShopAdminAuthCookie");
                    currentUserCookie.Expires = DateTime.Now.AddDays(-10);
                    currentUserCookie.Value = null;
                    HttpContext.Current.Response.SetCookie(currentUserCookie);

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                        (new { area = "admin", controller = "login", action = "index" }));
                    return;
                }


                var RolesName = db.UserRoles.Where(q => q.UserId == userid).Select(q => q.Role.RoleName).ToList();
                HttpContext.Current.Session["Roles"] = RolesName.ToArray();
            }
            else
             {
                controller.HttpContext.Response.Redirect("~/admin/login/Logout");
               
              
                // controller.HttpContext.Response.RedirectToRoute("http://localhost:50114/Account/Login");
                //
                // controller.HttpContext.Response.Redirect("/account/Login");
                //  controller.HttpContext.Response.Redirect("http://localhost:50114/Account/Login");

                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                //      (new { area = "", controller = "Account", action = "Logout" }));


                return;
            }
            
            //start--------------------------زمن لاگین تمام شده است-------------------------------
           
            if (HttpContext.Current.Session["DateCreated"] != null)
            {
               
                TimeSpan span = DateTime.Now.Subtract((DateTime)HttpContext.Current.Session["DateCreated"]);
                int second = (Security.SettingApp._TimeoutLogin * 60) - Convert.ToInt32(Math.Round(span.TotalSeconds, 0));

                if (second <= 0)
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                        (new { area = "admin", controller = "login", action = "index" }));
                }
            }
            //اگر خط زیر کامنت نباشد با هر درخواسات زمان لاگین بودن کاربر رفرش میشود
            HttpContext.Current.Session["DateCreated"] = DateTime.Now;

            //end-----------------------------end
            //در صورتی که کاربر ناشناخته باشد به صفحه لاگین هداین میشود در غیر اینصورت دسترسی کاربر برای مورد درخواستی چک میشود
            if (HttpContext.Current.Session["Username"]==null)
            {
                //filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                //    (new { area = "", controller = "Account", action = "Login" }));
                //return;

                controller.HttpContext.Response.Redirect("./admin/Logins");
                return;
            }
            else
            {

                //AccountModel am = new AccountModel();
                string Username = HttpContext.Current.Session["Username"].ToString();


               List<string> RolesString=new List<string>() ;
                var RolesAssign = (from U in db.Users
                           join UR in db.UserRoles
        on U.id equals UR.UserId
                           join R in db.Roles
                           on UR.RoleId equals R.RoleId
                           where (U.Username == Username)
                           select (R)
                         ).ToList();


                foreach (var item in RolesAssign)
                {
                    RolesString.Add(item.RoleName);
                }
                var oldUser= db.Users.SingleOrDefault(q => q.Username == Username);
                Account account = new Account() {
                    Username = Username,
                    FullName = oldUser.firstName+" "+oldUser.lastName,
                    UserId=oldUser.id,
                    Roles = RolesString.ToArray()
                };


                CustomPrincipal customPrincipal = new CustomPrincipal(account);

                //آیا کاربر دسترسی برای این کار را دارد
                if (!customPrincipal.IsInRole(Roles))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary(new { area = "", controller = "AccessDenied", action = "Index" })
                        );
                    return;
                }

            }
        }

        private bool CheckAccessIpToRoute(string actionName,string controllerName)
        {
            bool isAccess = false;
            var ipAddress= Utility.GetLocalIPAddress();
            var GetIP = Utility.GetIP();
            var GetIP4Address = Utility.GetIP4Address();
            return isAccess;
        }
    }
}