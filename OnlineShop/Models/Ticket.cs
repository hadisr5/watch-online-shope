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
    
    public partial class Ticket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ticket()
        {
            this.TicketChats = new HashSet<TicketChat>();
        }
    
        public int id { get; set; }
        public string title { get; set; }
        public Nullable<int> userid { get; set; }
        public Nullable<System.DateTime> creationDate { get; set; }
        public string @for { get; set; }
        public string status { get; set; }
        public string voice { get; set; }
        public Nullable<int> sellerid { get; set; }
    
        public virtual Seller Seller { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TicketChat> TicketChats { get; set; }
        public virtual User User { get; set; }
    }
}
