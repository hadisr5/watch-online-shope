using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web.Caching;

namespace OnlineShop.Security
{
    public class PreventSpamAttribute : ActionFilterAttribute
    {
        public int DelayRequest { get; set; }
        public string ErrorMessage { get; set; }

        public PreventSpamAttribute(int DelayRequest, string ErrorMessage)
        {
            this.DelayRequest = DelayRequest;
            this.ErrorMessage = ErrorMessage;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //Store our HttpContext (for easier reference and code brevity)
            var request = filterContext.HttpContext.Request;
            //Store our HttpContext.Cache (for easier reference and code brevity)
            var cache = filterContext.HttpContext.Cache;

            //Grab the IP Address from the originating Request (very simple implementation for example purposes)
            var originationInfo = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress;

            //Append the User Agent
            originationInfo += request.UserAgent;

            //Now we just need the target URL Information
            var targetInfo = request.RawUrl + request.QueryString;

            //Generate a hash for your strings (this appends each of the bytes of the value into a single hashed string
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(originationInfo + targetInfo)).Select(s => s.ToString("x2")));

            //Checks if the hashed value is contained in the Cache (indicating a repeat request)
            if (cache[hashValue] != null)
            {
                //ErrorMessage for raise in error page
                //Adds the Error Message to the Model and RedirectModelState
                //filterContext.Controller.ViewData..AddModelError("ExcessiveRequests", ErrorMessage);
                HttpContext.Current.Response.Redirect("/Error/BadRequestError");
               //اینجا باید errormessage بکنید
            }
            else
            {
                //Adds an empty object to the cache using the hashValue to a key (This sets the expiration that will determine
                //if the Request is valid or not
                cache.Add(hashValue, targetInfo, null, DateTime.Now.AddSeconds(DelayRequest), Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            }
            base.OnActionExecuting(filterContext);
        }
    }
}