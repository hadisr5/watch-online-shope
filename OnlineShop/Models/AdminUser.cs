//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlineShop.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AdminUser
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public Nullable<System.DateTime> lastOnline { get; set; }
        public string profileImage { get; set; }
        public string introMessage { get; set; }
        public Nullable<bool> Active { get; set; }
    }
}
