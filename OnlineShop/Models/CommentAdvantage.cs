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
    
    public partial class CommentAdvantage
    {
        public int id { get; set; }
        public Nullable<int> productId { get; set; }
        public Nullable<int> commentId { get; set; }
        public Nullable<bool> advantage { get; set; }
        public Nullable<bool> disAdvantage { get; set; }
        public string text { get; set; }
    
        public virtual Comment Comment { get; set; }
        public virtual Product Product { get; set; }
    }
}
