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
    
    public partial class Area
    {
        public Area()
        {
            this.Position = new List<Position>();
            this.SkillCompassGlossary = new List<SkillCompassGlossary>();
            this.Employee = new List<Employee>();
            this.Suggestions = new List<Suggestions>();
        }
    
        public int AreaId { get; set; }
        public string Name { get; set; }
        public string ImageCareerPath { get; set; }
        public string ImageSkillCompass { get; set; }
    
        public virtual IList<Position> Position { get; set; }
        public virtual IList<SkillCompassGlossary> SkillCompassGlossary { get; set; }
        public virtual IList<Employee> Employee { get; set; }
        public virtual IList<Suggestions> Suggestions { get; set; }
    }
}