using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UOS.Models.Registration
{
    public class GuardianDetails
    {
        [Display(Name = "Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string g_name { get; set; }

        [Display(Name = "CNIC")]
        [Required]
        [RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC No Must Follow The XXXXX-XXXXXXX-X Format")]
        public string g_cnic { get; set; }

        [Display(Name = "Cell No")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number Must Be In Digits")]
        public string g_cell_no { get; set; }

        [Display(Name = "Relationship")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string relationship { get; set; }

        [Display(Name = "Education")]
        [Required]

        public string eductn { get; set; }

        [Display(Name = "Occupation")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string occpatn { get; set; }

        [Display(Name = "Sector")]
        public bool is_gov { get; set; }

        [Display(Name = "Annual Income")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Amount must be numeric")]
        public string income { get; set; }

    }
}