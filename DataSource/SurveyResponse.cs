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
    
    public partial class SurveyResponse
    {
        public int SurveyResponseId { get; set; }
        public int QuestionId { get; set; }
        public int ResourceId { get; set; }
        public Nullable<int> ResponseId { get; set; }
        public string ResponseText { get; set; }
        public int SurveyResourceId { get; set; }
    
        public virtual Question Question { get; set; }
        public virtual Resource Resource { get; set; }
        public virtual SurveyResource SurveyResource { get; set; }
    }
}
