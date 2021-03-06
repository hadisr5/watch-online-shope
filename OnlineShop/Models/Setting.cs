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
    
    public partial class Setting
    {
        public int id { get; set; }
        public Nullable<long> shippingFree { get; set; }
        public Nullable<long> homePaye { get; set; }
        public Nullable<long> shippingPrice { get; set; }
        public string WebsiteName { get; set; }
        public string WebsiteUrl { get; set; }
        public string WebsiteLogo { get; set; }
        public string WebsiteFullname { get; set; }
        public string TopAlertText { get; set; }
        public string topAlertLink { get; set; }
        public string topAlertColor { get; set; }
        public string WebsiteLogo2 { get; set; }
        public string WebsiteLogo3 { get; set; }
        public string SmsToken1 { get; set; }
        public string SmsToken2 { get; set; }
        public Nullable<long> smsToken_welcome { get; set; }
        public Nullable<long> smsToken_forgetPassword { get; set; }
        public Nullable<long> smsToken_submittedOrder { get; set; }
        public Nullable<long> smsToken_fastLogin { get; set; }
        public Nullable<long> smsToken_TicketAnswer { get; set; }
        public Nullable<long> smsToken_validateUser { get; set; }
        public Nullable<long> smsToken_submittedCartToCart { get; set; }
        public Nullable<long> smsToken_CanceledCartToCart { get; set; }
        public string zarinpal_merchant_id { get; set; }
        public Nullable<long> delivery_Price { get; set; }
        public Nullable<long> PostalPrice { get; set; }
        public Nullable<int> maxOrdersPerDay { get; set; }
        public Nullable<bool> ActiveOnlineChat { get; set; }
        public Nullable<long> smsToken_onlineChat { get; set; }
        public Nullable<long> smsToken_changeStatusOfOrder { get; set; }
        public Nullable<bool> InstantOffer { get; set; }
        public string FooterShortText { get; set; }
        public string FooterLongText { get; set; }
    }
}
