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
    
    public partial class Blog_post
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Blog_post()
        {
            this.Blog_TagsPosts = new HashSet<Blog_TagsPosts>();
        }
    
        public int id { get; set; }
        public string Cnt { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string img { get; set; }
        public Nullable<int> catId { get; set; }
        public string title { get; set; }
        public string shortDesc { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Blog_TagsPosts> Blog_TagsPosts { get; set; }
    }
}