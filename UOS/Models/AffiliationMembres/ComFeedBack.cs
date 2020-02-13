using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UOS.Models
{
    public class ComFeedBack
    {
        public string Performance { get; set; }
        [Display(Name = "Select Institute")]
        public int InstituteId { get; set; }
        [Display(Name = "Select")]
        public string CheckName { get; set; }
        public IEnumerable<DDLComMemberCheckPoint> CheckPoint { get; set; }
        public IEnumerable<InstituteForMembersAllApplications> InstitutesDDL { get; set; }
        public string Recommendation { get; set; }
    }
}