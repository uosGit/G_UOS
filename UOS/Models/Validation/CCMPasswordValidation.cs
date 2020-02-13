using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Web;
using UOS.Models;

namespace UOS.Models.Validation
{
    public class CCMPasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var CM = (AffiliationCommitteeMember)validationContext.ObjectInstance;
            if (CM.ConfirmPassword != null)
            {
                //Card Numbers
                if (CM.ConfirmPassword == CM.Password)
                    return ValidationResult.Success;
                else
                    return new ValidationResult("Password DoesNot Match");
            }
            else
            {
                return new ValidationResult("Password Must Be Entered");
            }
        }
    }
}