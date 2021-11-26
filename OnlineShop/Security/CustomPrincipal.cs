using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace OnlineShop.Security
{
    public class CustomPrincipal : IPrincipal
    {
        private Account Account;
        public string[] Roles { get; set; }
        public CustomPrincipal(Account account)
        {
            this.Account = account;
            this.Identity = new GenericIdentity(account.Username);
            
        }
        public IIdentity Identity { set; get; }
          

        public bool IsInRole(string role)
        {
            var roles = role.Split(new char[] { ',' });
            return roles.Any(r => this.Account.Roles.Contains(r));
        }
    }
}