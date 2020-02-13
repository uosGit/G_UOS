using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class OtherDoc
    {
        public int Id { get; set; }

        [Display(Name = "Select Document Category")]
        public int doc_cat_Id { get; set; }
        public IEnumerable<BI_affi_pop_ins_related_doc_cat_Result> DocumentDD { get; set; }

        [Display(Name = "Page No")]
        [Required]
        public int pg_no { get; set; }

        [Display(Name = "Attach")]
        public string attach { get; set; }

        public byte[] Image { get; set; }
        public string FileName { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

    }
}