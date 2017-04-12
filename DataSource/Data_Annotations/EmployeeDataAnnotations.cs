namespace DataSource
{
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;

	//partial class
	[MetadataType(typeof(EmployeeDataAnnotations))]
	public partial class Employee
	{
	}

	//class used for attributes
	public class EmployeeDataAnnotations
	{
		[Required]
		[DisplayName("Current Position")]
		[MaxLength(1000)]
		public string CurrentPosition { get; set; }

		[Required]
		[DisplayName("Aspiring Position")]
		[MaxLength(1000)]
		public string AspiringPosition { get; set; }
	}
}
