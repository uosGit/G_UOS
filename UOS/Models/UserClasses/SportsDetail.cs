using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class SportsDetail
    {
        public int SportsId { get; set; }

        [Display(Name = "Sports Name")]
        public string sports_name { get; set; }
        public IEnumerable<BI_POP_ALL_SPORTS_Result> Sports { get; set; }

        [Display(Name = "Item Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string item_name { get; set; }

        [Display(Name = "Quantity")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number Must Be In Digits")]
        public int quantity { get; set; }

        [Display(Name = "Remarks")]
        public String remarks { get; set; }
    }
}