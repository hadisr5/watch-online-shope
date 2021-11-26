using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Security
{
    public static class SessionPersister
    {
        static string usernameSessionvar = "username";
        public static string Username
        {
            get
            {
                if (HttpContext.Current == null)
                    return string.Empty;
                    var sessionVar = HttpContext.Current.Session[usernameSessionvar];
                    if (sessionVar != null)
                        return sessionVar as string;
                    return null;
            }
            set
            {
                HttpContext.Current.Session[usernameSessionvar] = value;
            }
        }
        public static string FullName { get; set; }
        public static Guid UserId { get; set; }
        public static string[] Roles { get; internal set; }
        public static bool IsInRole(string requestRole) {
            if (HttpContext.Current.Session["Roles"] == null) return false;
            string[] UserRoles = ((IEnumerable)HttpContext.Current.Session["Roles"]).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();

          
           
            if (UserRoles == null) return false;
            foreach (var item in UserRoles)
            {
                if (item == requestRole) return true;
            }
            return false;
        }
    }
    //public class RoleVM {
    //    public string[] roles { get; set; }
    //}
}