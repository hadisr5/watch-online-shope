using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OnlineShop.Models;

namespace OnlineShop.Security
{

    public class LogAttribute : ActionFilterAttribute
    {
        OnlineShopEntities db = new OnlineShopEntities();
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Log("OnActionExecuted", filterContext);
        }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    Log("OnActionExecuting", filterContext.RouteData);
        //}

        //public override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    Log("OnResultExecuted", filterContext.RouteData);
        //}

        //public override void OnResultExecuting(ResultExecutingContext filterContext)
        //{
        //    Log("OnResultExecuting ", filterContext.RouteData);
        //}

        private void Log(string methodName, ActionExecutedContext filterContext)
        {
            if (filterContext.RouteData.Values["action"].ToString() == "RemindSessionTime")
            {
                return;
            }
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();
            var areaName = filterContext.RouteData.DataTokens["area"]!=null? filterContext.RouteData.DataTokens["area"].ToString():"";
            var message = String.Format("{0}- area:{3} controller:{1} action:{2}", methodName, controllerName, actionName,areaName);
            if (HttpContext.Current.Session != null)
            { 
            
            

            if (HttpContext.Current.Session["Username"] != null)
            {
                var username = HttpContext.Current.Session["Username"].ToString();
                    //var Id = System.Guid.NewGuid();
                    //string host = Request.Url.Host;
                    Models.LogUserData logUserData = new Models.LogUserData()
                {
                    ControllerName = controllerName,
                    ActionName = actionName,
                    UserName = username,
                    AreaName= areaName,
                    CreateDate = DateTime.Now,
                   // UserId = int.Parse(System.Web.HttpContext.Current.Session["UserId"].ToString()),
                    IP=Utility.GetLocalIPAddress(),
                    Hostname=Utility.GetHostName()
                };
                    //int userid;
                    //if(int.TryParse(System.Web.HttpContext.Current.Session["UserId"].ToString(),out userid) ==true)
                    //{
                    //    logUserData.UserId = userid;
                    //}
                db.LogUserDatas.Add(logUserData);
                db.SaveChanges();
            }
            }
            //return Task.Run(() =>
            //{

            //          db.SaveChanges();

            //}
            //);



            //return Task.Run(() =>
            //{
            //   // db.SaveChanges();
            //   // db.SaveChanges();
            //}
            //   );
            // Debug.WriteLine(message);
        }
    }
}