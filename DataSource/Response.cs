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
    
    public partial class Response
    {
        public Response()
        {
            this.QuestionResponse = new List<QuestionResponse>();
        }
    
        public int ResponseId { get; set; }
        public string Answer { get; set; }
    
        public virtual IList<QuestionResponse> QuestionResponse { get; set; }
    }
}
