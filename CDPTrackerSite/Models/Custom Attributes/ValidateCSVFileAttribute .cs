using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.ComponentModel.DataAnnotations;
namespace CDPTrackerSite.Models.Custom_Attributes
{
    public class ValidateCSVFileAttribute :  ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
             var file = value as HttpPostedFileBase;
            string[] validExtensions = { ".csv" };
            string ext = Path.GetExtension(file.FileName);
            if (file == null || file.ContentLength < 1)
            {
                
                return new ValidationResult("File must not be empty!");
            }

            else if (file.ContentLength > 1 * 1024 * 1024)
            {
                return new ValidationResult("File must be less that 10Mb.");
            }
            else if (!validExtensions.Contains(ext))
            {
                return new ValidationResult("Only .csv files are permitted");
            }
            else return ValidationResult.Success;
        }


    }
}