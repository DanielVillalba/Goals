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
    
    public partial class RankCatalog
    {
        public RankCatalog()
        {
            this.KPI = new List<KPI>();
            this.StrategicPriority = new List<StrategicPriority>();
            this.Threshold = new List<Threshold>();
        }
    
        public int RankID { get; set; }
        public string Description { get; set; }
        public short Score { get; set; }
        public byte RankTypeID { get; set; }
        public int GroupId { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public byte LevelRankID { get; set; }
    
        public virtual Groups Groups { get; set; }
        public virtual RankTypeCatalog RankTypeCatalog { get; set; }
        public virtual LevelRank LevelRank { get; set; }
        public virtual IList<KPI> KPI { get; set; }
        public virtual IList<StrategicPriority> StrategicPriority { get; set; }
        public virtual IList<Threshold> Threshold { get; set; }
    }
}
