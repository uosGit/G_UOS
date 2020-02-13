using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UOS.Models.Validation;

namespace UOS.Models.UserClasses
{
    public class DemandDraft
    {
        public int Id { get; set; }

        [Display(Name = "Bank Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string bank_name { get; set; }

        [Display(Name = "Bank Address")]
        public string bank_address { get; set; }

        [Display(Name = "Branch Code")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Bank Code must be numeric")]
        public string branch_code { get; set; }

        [Display(Name = "Amount")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Amount must be numeric")]
        public int amount { get; set; }

        [Display(Name = "Deposite Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime deposite_date { get; set; }

        [Display(Name = "Scan of Draft")]
        public string FileName { get; set; }


        public HttpPostedFileBase ImageFile { get; set; }

        public byte[] Draft_Scan { get; set; }


    }
}