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
    
    public partial class Payment
    {
        public int id { get; set; }
        public Nullable<int> BasketId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string GateWayString { get; set; }
        public Nullable<int> GatewayId { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<long> Amount { get; set; }
        public string Token { get; set; }
        public Nullable<int> Status { get; set; }
        public string StatusString { get; set; }
        public Nullable<bool> isWallet { get; set; }
        public Nullable<int> Walletid { get; set; }
    }
}
