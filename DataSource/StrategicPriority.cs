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
    
    public partial class StrategicPriority
    {
        public int StrategicPriorityId { get; set; }
        public string Action { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public string DefinitionOfDone { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Result { get; set; }
        public string Comment { get; set; }
        public int QuarterlyPrioritiesId { get; set; }
        public int AnnualPrioritiesID { get; set; }
        public int RankID { get; set; }
    
        public virtual AnnualPriorities AnnualPriorities { get; set; }
        public virtual QuarterlyPriorities QuarterlyPriorities { get; set; }
        public virtual RankCatalog RankCatalog { get; set; }
    }
}
