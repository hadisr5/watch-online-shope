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
    
    public partial class Wallet
    {
        public int id { get; set; }
        public Nullable<long> amount { get; set; }
        public string payWay { get; set; }
        public Nullable<bool> payByAdmin { get; set; }
        public Nullable<System.DateTime> creationDate { get; set; }
        public Nullable<int> userId { get; set; }
        public string desc { get; set; }
        public Nullable<bool> Success { get; set; }
        public string Token { get; set; }
    
        public virtual User User { get; set; }
    }
}