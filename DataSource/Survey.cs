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
    
    public partial class Survey
    {
        public Survey()
        {
            this.Question = new List<Question>();
            this.SurveyResource = new List<SurveyResource>();
        }
    
        public int SurveyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> SurveyType { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedTimeStamp { get; set; }
        public Nullable<int> Quarter { get; set; }
        public Nullable<int> Year { get; set; }
    
        public virtual IList<Question> Question { get; set; }
        public virtual IList<SurveyResource> SurveyResource { get; set; }
    }
}