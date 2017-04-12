namespace DataSource
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    //partial class
    [MetadataType(typeof(TDURewardDataAnnotations))]
    public partial class TDUReward : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Paid && string.IsNullOrEmpty(DatePaid.ToString()))
                yield return new ValidationResult("You must select the date the reward was paid.", new[] { "DatePaid" });
            else if (!Paid && !string.IsNullOrEmpty(DatePaid.ToString()))
                yield return new ValidationResult("Cannot set date without confirmed payment!", new[] { "Paid" });

        }
    }

    //class used for attributes
    public class TDURewardDataAnnotations
    {
        
        
        [DisplayName("Starting Quarter")]
        public int StartingQuarter { get; set; }
        [DisplayName("Ending Quarter")]
        public int EndingQuarter { get; set; }
        [DisplayName("Starting Year")]
        public int StartingYear { get; set; }
        [DisplayName("Ending Year")]
        public int EndingYear { get; set; }
        [DisplayName("Total TDU Reward")]
        public int TotalTDUReward { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [DisplayName("Date Redeemed")]
        public Nullable<System.DateTime> DateRedeemed { get; set; }
        public bool Paid { get; set; }
        //[Required(ErrorMessage="You must select the date the reward was paid.")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Date Paid")]
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DatePaid { get; set; }
        [DisplayName("Valid for Quarters")]
        public int ValidForQuarters { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Date to Lose Validity")]
        [DataType(DataType.Date)]
        public System.DateTime DatetoLoseValidity { get; set; }

        
        
    }
}