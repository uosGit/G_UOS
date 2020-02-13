using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class labortary
    {
        public int Id { get; set; }

        [Display(Name = "Subject/Lab")]
        public string sbjct_labname { get; set; }
        public IEnumerable<affi_ins_pop_all_subject_lab_Result> LabSubject { get; set; }

        [Display(Name = "Item Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string item_name { get; set; }

        [Display(Name = "Quantity")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Quantity Must Be In Digits")]
        public int quantity { get; set; }

        [Display(Name = "Remarks")]
        
        public string remarks { get; set; }

    }
}