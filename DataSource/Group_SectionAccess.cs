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
    
    public partial class Group_SectionAccess
    {
        public long id { get; set; }
        public int GroupId { get; set; }
        public string Section { get; set; }
        public bool Allow { get; set; }
    
        public virtual Groups Groups { get; set; }
    }
}
