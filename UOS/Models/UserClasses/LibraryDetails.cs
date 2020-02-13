using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class LibraryDetails
    {
        public int Id { get; set; }

        [Display(Name = "Subject")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string subject { get; set; }


        [Display(Name = "No.Titles")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "No.Titles Must Be In Digits")]
        public int no_titles { get; set; }


        [Display(Name = "No.Books")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "No.Books Must Be In Digits")]
        public int no_books { get; set; }


        [Display(Name = "No.Rel.")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "No.Rel. Must Be In Digits")]
        public int no_relevent { get; set; }


        [Display(Name = "No.Ref.")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "No.Ref. Must Be In Digits")]
        public int no_reff{ get; set; }


        [Display(Name = "Other Rel.")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Other Rel. Must Be In Digits")]
        public int other_rel { get; set; }


       

        
    }
}