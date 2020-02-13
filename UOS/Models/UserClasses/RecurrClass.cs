using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class RecurrClass
    {
        public int Id { get; set; }

        [Display(Name = "Select Receipt")]
        public string s_receiptname { get; set; }
        public IEnumerable<Affi_ins_Recuring_All_recipts> ReceiptDD { get; set; }


        public IEnumerable<Affi_ins_Recuring_All_Expence> ExpenditureDD { get; set; }
        [Display(Name = "Select Expenditure")]
        public string s_expenditurename { get; set; }


        [Display(Name = "Amount")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Amount Must Be In Digits")]
        public int amount { get; set; }
    }
}