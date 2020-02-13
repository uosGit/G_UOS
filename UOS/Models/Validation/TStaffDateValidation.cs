using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UOS.Models.UserClasses;
using System.ComponentModel.DataAnnotations;

namespace UOS.Models.Validation
{
    public class TStaffDateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var CM = (TeachingStaff)validationContext.ObjectInstance;
            if (DateTime.Compare(Convert.ToDateTime( CM.appoint_date), DateTime.Today) <= 0)
            {
                return new ValidationResult("Enter The Appointment Date Later Than Today");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}