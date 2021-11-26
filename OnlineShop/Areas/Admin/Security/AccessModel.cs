using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop.Areas.Admin.Security
{
    public class AccessModel
    {
        public class _Access
        {
            public string name { get; set; }
            public List<_Method> methods { get; set; }
            public string Controller { get; set; }
        }

        public class _Method
        {
            public string name { get; set; }

        }

    }
}