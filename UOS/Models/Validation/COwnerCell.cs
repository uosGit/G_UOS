using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UOS.Models.UserClasses;

namespace UOS.Models.Validation
{
    public class COwnerCell : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var CM = (CollegeOwner)validationContext.ObjectInstance;
            if (CM.contact != null)
            {
                //Card Numbers
                if (CM.contact.Length == 11)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Cell Number Contains 11 Digits");
            }
            else
            {
                return new ValidationResult("Cell Number Must Be Entered");
            }
        }
    }
}