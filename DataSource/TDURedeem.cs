//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataSource
{
    using System;
    using System.Collections.Generic;
    
    public partial class TDURedeem
    {
        public int TDUReedeemId { get; set; }
        public int resourceId { get; set; }
        public int QuarterYear { get; set; }
        public int Quarter { get; set; }
        public int TDU { get; set; }
        public System.DateTime DateReached { get; set; }
        public Nullable<bool> Redeemed { get; set; }
        public Nullable<bool> Paid { get; set; }
        public Nullable<System.DateTime> DateRedeemed { get; set; }
        public Nullable<System.DateTime> DatePaid { get; set; }
        public Nullable<int> TDUReward { get; set; }
    
        public virtual Resource Resource { get; set; }
        public virtual TDUReward TDUReward1 { get; set; }
    }
}
