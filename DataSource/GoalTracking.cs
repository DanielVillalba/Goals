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
    
    public partial class GoalTracking
    {
        public int GoalId { get; set; }
        public int ResourceId { get; set; }
        public string Goal { get; set; }
        public int Progress { get; set; }
        public bool VerifiedByManager { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public Nullable<int> ObjectiveId { get; set; }
        public Nullable<int> TrainingCategoryId { get; set; }
        public Nullable<int> TDU { get; set; }
        public Nullable<int> SourceId { get; set; }
    
        public virtual Resource Resource { get; set; }
        public virtual ProgressEnum ProgressEnum { get; set; }
        public virtual Objective Objective { get; set; }
        public virtual TrainingCategory TrainingCategory { get; set; }
        public virtual Sources Sources { get; set; }
    }
}
