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
    
    public partial class CommnetsLike
    {
        public int id { get; set; }
        public Nullable<int> CommentId { get; set; }
        public Nullable<int> userId { get; set; }
        public Nullable<bool> Like { get; set; }
        public Nullable<bool> disLike { get; set; }
    
        public virtual Comment Comment { get; set; }
        public virtual User User { get; set; }
    }
}
