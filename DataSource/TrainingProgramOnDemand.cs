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
    
    public partial class TrainingProgramOnDemand
    {
        public TrainingProgramOnDemand()
        {
            this.TrainingProgramOnDemandDetails = new List<TrainingProgramOnDemandDetails>();
            this.TrainingProgramOnDemandVisits = new List<TrainingProgramOnDemandVisits>();
        }
    
        public int IdTrainingProgramOnDemand { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> FinishDate { get; set; }
        public Nullable<bool> Enable { get; set; }
    
        public virtual IList<TrainingProgramOnDemandDetails> TrainingProgramOnDemandDetails { get; set; }
        public virtual IList<TrainingProgramOnDemandVisits> TrainingProgramOnDemandVisits { get; set; }
    }
}
