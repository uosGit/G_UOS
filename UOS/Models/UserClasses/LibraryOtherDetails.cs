using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UOS.Models.UserClasses
{
    public class LibraryOtherDetails
    {
        public int Id { get; set; }
        [Display(Name = "Select Item")]
        public string item { get; set; }
        public IEnumerable<BI_pop_All_library_Other_item_Result> ItemDD { get; set; }

        [Display(Name = "Quantity")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Quantity Must Be In Digits")]
        public int quantity { get; set; }


        [Display(Name = "Remarks")]
        public string remarks { get; set; }
    }
}