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
    
    public partial class ProductPropertiesValue
    {
        public int id { get; set; }
        public string value { get; set; }
        public Nullable<int> proId { get; set; }
        public Nullable<int> propertiesid { get; set; }
    
        public virtual CatProperty CatProperty { get; set; }
        public virtual Product Product { get; set; }
    }
}
