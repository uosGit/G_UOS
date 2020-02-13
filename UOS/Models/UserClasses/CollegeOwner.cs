using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UOS.Models.Validation;

namespace UOS.Models.UserClasses
{
    public class CollegeOwner
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        [Display(Name = "Owner Name")]
        public string owner_name { get; set; }

        [Required]
        [RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC No Must Follow The XXXXX-XXXXXXX-X Format")]
        [Display(Name = "CNIC")]
        public string cnic{ get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number Must Be In Digits")]
        [COwnerCell]
        [Display(Name = "Contact")]
        public string contact { get; set; }


    }
}