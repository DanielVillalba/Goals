namespace DataSource
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;

	//partial class
    [MetadataType(typeof(GoalTrackingDataAnnotations))]
	public partial class GoalTracking
	{
	}

	//class used for attributes
	public class GoalTrackingDataAnnotations
	{
		[Required]
		[MaxLength(1000)]
		public string Goal { get; set; }

		[DisplayName("Status")]
		public int Progress { get; set; }

        [DisplayName("Verified By Manager")]
        public bool VerifiedByManager { get; set; }

		[DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
		[DisplayName("Last Updated")]
		public Nullable<System.DateTime> LastUpdate { get; set; }

        [Required]
		[DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}", ApplyFormatInEditMode = true)]
		[DisplayName("Finish By")]
		public Nullable<System.DateTime> FinishDate { get; set; }

        [DisplayName("Objective")]
        public int ObjectiveId { get; set; }

        [DisplayName("Training Category")]
        public string TrainingCategory{ get; set; }

        public int TDU { get; set; }
	}
}
