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
    
    public partial class Seller_cats
    {
        public int id { get; set; }
        public Nullable<int> sellerId { get; set; }
        public Nullable<int> catId { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual Seller Seller { get; set; }
    }
}