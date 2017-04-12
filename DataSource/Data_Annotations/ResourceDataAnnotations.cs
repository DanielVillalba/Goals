namespace DataSource
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;

	//partial class
	[MetadataType(typeof(ResourceDataAnnotations))]
	public partial class Resource
	{
	}

	//class used for attributes
	public class ResourceDataAnnotations
	{
		[Required]
		[MaxLength(250)]
		public string Name { get; set; }

		[MaxLength(100)]
		[DisplayName("E-mail")]
		public string Email { get; set; }

		[Required]
		[DisplayName("Role")]
		public int RolId { get; set; }

		[Required]
		[MaxLength(50)]
		[DisplayName("Domain name")]
		public string DomainName { get; set; }

		[DisplayName("Last login")]
		public System.DateTime LastLogin { get; set; }

		[DisplayName("Goal Trackings")]
		public virtual IList<GoalTracking> GoalTrackings { get; set; }
	}
}
