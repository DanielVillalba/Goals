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
    
    public partial class Position
    {
        public Position()
        {
            this.Employee = new List<Employee>();
            this.Employee1 = new List<Employee>();
            this.Position1 = new List<Position>();
        }
    
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public Nullable<int> NextPosition { get; set; }
        public int AreaId { get; set; }
    
        public virtual Area Area { get; set; }
        public virtual IList<Employee> Employee { get; set; }
        public virtual IList<Employee> Employee1 { get; set; }
        public virtual IList<Position> Position1 { get; set; }
        public virtual Position Position2 { get; set; }
    }
}
