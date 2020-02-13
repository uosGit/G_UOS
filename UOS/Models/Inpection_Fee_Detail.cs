using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace UOS.Models
{
    public class Inpection_Fee_Detail
    {
        
            // Display Attribute will appear in the Html.LabelFor
            [Display(Name = "Inpection Fee Detail")]
        public int InpectionId { get; set; }
            public IEnumerable<DDLloadUserVlue> ddInpection { get; set; }
        
    }
}