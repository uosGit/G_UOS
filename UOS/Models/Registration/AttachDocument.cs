using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UOS.Models.Registration
{
    public class AttachDocument
    {
        public int Id { get; set; }

        [Display(Name = "Domicile")]
        public HttpPostedFileBase DomicileImage { get; set; }
        public byte[] domicile { get; set; }
        public string domicileName { get; set; }



        [Display(Name = "Attested CNIC")]
        public HttpPostedFileBase CnicImage { get; set; }
        public byte[] attsted_cnic { get; set; }
        public string attsted_cnicName { get; set; }


        [Display(Name = "Last Degree Passed")]
        public HttpPostedFileBase LastDegreeImage { get; set; }
        public byte[] l_dgree { get; set; }
        public string l_dgreeName { get; set; }


        [Display(Name = "Serving NOC")]
        public HttpPostedFileBase ServingNocImage { get; set; }
        public byte[] noc_serving { get; set; }
        public string noc_servingName { get; set; }


        [Display(Name = "Migration NOC")]
        public HttpPostedFileBase MigrationNocImage { get; set; }
        public byte[] migration_noc { get; set; }
        public string migration_nocName { get; set; }



    }
}