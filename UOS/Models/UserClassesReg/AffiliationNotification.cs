using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClassesReg
{
    public class AffiliationNotification
    {

        public int Id { get; set; }

        public string FileName { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public byte[] notification_Scan { get; set; }

        [Display(Name = "Admission Year")]
        public string yearID { get; set; }
        public IEnumerable<Affi_ins_Year> yeardd { get; set;}


    }
}