using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;

namespace UOS.Models.Validation
{
    public class CCMCell : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var CM = (Affi_Com_Members)validationContext.ObjectInstance;
            if (CM.Cell != null)
            {
                //Card Numbers
                if (CM.Cell.Length == 11)
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