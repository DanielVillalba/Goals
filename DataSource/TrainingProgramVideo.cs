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
    
    public partial class TrainingProgramVideo
    {
        public int IdTrainingProgramVideo { get; set; }
        public int IdTrainingProgram { get; set; }
        public Nullable<int> IdLocation { get; set; }
        public string LinkVideo { get; set; }
    
        public virtual Location Location { get; set; }
        public virtual TrainingProgram TrainingProgram { get; set; }
    }
}
