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
    
    public partial class GoalsView
    {
        public int GoalId { get; set; }
        public int ManagerId { get; set; }
        public string Manager { get; set; }
        public int ResourceId { get; set; }
        public string Employee { get; set; }
        public string Goal { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public int Verified { get; set; }
        public int MustCheck { get; set; }
        public int Progress { get; set; }
        public Nullable<int> TDU { get; set; }
        public string Objective { get; set; }
        public string TrainingCategory { get; set; }
        public string ManagerLocation { get; set; }
        public string EmployeeLocation { get; set; }
    }
}
