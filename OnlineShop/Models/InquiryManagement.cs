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
    
    public partial class InquiryManagement
    {
        public long InquiryId { get; set; }
        public Nullable<long> AdminId { get; set; }
        public Nullable<long> ModelId { get; set; }
        public Nullable<long> ServiceId { get; set; }
        public Nullable<long> Mobile { get; set; }
        public string Description { get; set; }
    }
}