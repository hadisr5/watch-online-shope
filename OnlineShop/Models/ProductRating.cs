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
    
    public partial class ProductRating
    {
        public int id { get; set; }
        public Nullable<int> productId { get; set; }
        public Nullable<int> groupId { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual RatingGroup RatingGroup { get; set; }
    }
}
