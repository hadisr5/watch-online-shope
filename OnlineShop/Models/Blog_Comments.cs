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
    
    public partial class Blog_Comments
    {
        public int id { get; set; }
        public string comment { get; set; }
        public Nullable<int> userid { get; set; }
        public Nullable<System.DateTime> creationDate { get; set; }
        public Nullable<bool> Submitted { get; set; }
        public Nullable<int> PostId { get; set; }
    
        public virtual User User { get; set; }
    }
}
