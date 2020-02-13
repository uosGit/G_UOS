using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UOS.Models.UserClasses;

namespace UOS.Models.Validation
{
    public class DateValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var CM = (DemandDraft)validationContext.ObjectInstance;
            if (DateTime.Compare(CM.deposite_date,DateTime.Today) <= 0)
            {
                    return new ValidationResult("Enter The Demand Draft Date Later Than Today");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}