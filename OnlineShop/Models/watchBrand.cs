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
    
    public partial class watchBrand
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public watchBrand()
        {
            this.Models = new HashSet<Model>();
        }
    
        public long BrandId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> AdminId { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Model> Models { get; set; }
    }
}
