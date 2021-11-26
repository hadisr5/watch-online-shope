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
    
    public partial class CatProperty
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CatProperty()
        {
            this.ProductPropertiesValues = new HashSet<ProductPropertiesValue>();
        }
    
        public int id { get; set; }
        public string title { get; set; }
        public Nullable<int> catId { get; set; }
        public Nullable<int> priority { get; set; }
        public Nullable<bool> showInFilter { get; set; }
        public Nullable<bool> MainProperty { get; set; }
        public Nullable<int> category { get; set; }
    
        public virtual Category Category1 { get; set; }
        public virtual propertiesCategory propertiesCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductPropertiesValue> ProductPropertiesValues { get; set; }
    }
}
