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
    
    public partial class Model
    {
        public long ModelId { get; set; }
        public long BrandId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> AdminId { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
    
        public virtual watchBrand watchBrand { get; set; }
        public virtual ServicePrice ServicePrice { get; set; }
    }
}