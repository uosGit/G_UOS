using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.Registration
{
    public class AffiliatedCollegeDetails
    {
        [Display(Name = "College Name")]
        [Required]
        public string clg_name { get; set; }


        [Display(Name = "ID")]
        public string clg_ID { get; set; }


        [Display(Name = "Code")]
        [Required]
        public string clg_code { get; set; }


        [Display(Name = "Roll No")]
        [Required]
        public string clg_roll_no { get; set; }


        [Display(Name = "Session")]
        [Required]
        public string session { get; set; }


        [Display(Name = "Program")]
        [Required]
        public string program { get; set; }


        [Display(Name = "Education")]
        [Required]
        public string education { get; set; }


        [Display(Name = "Admission Date")]
        [Required]
        public string admission_date { get; set; }

    }
}