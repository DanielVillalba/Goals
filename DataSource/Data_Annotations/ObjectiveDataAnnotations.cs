namespace DataSource
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    //partial class
    [MetadataType(typeof(ObjectiveDataAnnotations))]
    public partial class Objective
    {
    }

    //class used for attributes
    public class ObjectiveDataAnnotations
    {
        [Required]
        [DisplayName("Objective")]
        public string Objective1 { get; set; }
    }
}
