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
    
    public partial class Discount
    {
        public int id { get; set; }
        public Nullable<int> productId { get; set; }
        public Nullable<System.DateTime> creationDay { get; set; }
        public Nullable<System.DateTime> expire { get; set; }
        public Nullable<long> amount { get; set; }
        public Nullable<int> percentage { get; set; }
        public string title { get; set; }
        public Nullable<int> status { get; set; }
        public Nullable<int> promotionId { get; set; }
        public Nullable<int> priority { get; set; }
    
        public virtual Product Product { get; set; }
    }
}
