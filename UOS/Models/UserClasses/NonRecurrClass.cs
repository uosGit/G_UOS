using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class NonRecurrClass
    {
        public int Id { get; set; }


        public bool IsReceipt { get; set; }
        [Display(Name = "Select Receipt")]
        public string s_receiptname { get; set; }
        public IEnumerable<Affi_ins_Non_Recuring_All_Recipts> ReceiptDD { get; set; }

        

        public bool IsExpenditure { get; set; }
        public IEnumerable<Affi_ins_Non_Recuring_all_expences> ExpenditureDD { get; set; }
        [Display(Name = "Select Expenditure")]
        public string s_expenditurename { get; set; }

        [Display(Name = "Amount")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Amount Must Be In Digits")]
        public int amount { get; set; }

        

        

    }
}