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
    
    public partial class TrainingProgramCategory
    {
        public TrainingProgramCategory()
        {
            this.TrainingProgram = new List<TrainingProgram>();
            this.GeneralTrainingProgram = new List<GeneralTrainingProgram>();
        }
    
        public int IdTrainingProgramCategory { get; set; }
        public string Name { get; set; }
    
        public virtual IList<TrainingProgram> TrainingProgram { get; set; }
        public virtual IList<GeneralTrainingProgram> GeneralTrainingProgram { get; set; }
    }
}
