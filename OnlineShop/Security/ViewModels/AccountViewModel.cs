using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Security.ViewModels
{
    public class AccountViewModel

    {
        public int UserId { get; set; }
        public string Username { get; set; }
        /// <summary>
        /// ورود من را به خاطر بسپار
        /// </summary>
        public bool remmember { get; set; }
        public string Password { get; set; }
        public Security.Account Account { get; set; }
        public List<string> lstString { get; set; }
        public bool IsResetPassword { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatPassword { get; set; }
        public string captcha { get; set; }
    }
}