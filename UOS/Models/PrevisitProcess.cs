using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UOS.Models
{
    public class PrevisitProcess
    {
        [Required]
        public bool SubjectApplied { get; set; }
        [Required]
        public bool SubjectAlreadyAffiliated { get; set; }
        public bool AffiliationWithBISE { get; set; }
        public bool NOCFromGovt { get; set; }
        public bool Prospectus { get; set; }
        public bool Building { get; set; }
        public bool StudentsHostel { get; set; }
        public bool StaffResidence { get; set; }
        public bool Finance { get; set; }
        public bool SportFacility { get; set; }
        public bool TeachingStaff { get; set; }
        public bool NonTeaching { get; set; }
        public bool Library { get; set; }
        public bool Labortary { get; set; }
        public bool OtherDetail { get; set; }
        public bool DemandDraft { get; set; }
        public bool isAcceptedApp { get; set; }
        public string remarksApp { get; set; }
    }
}