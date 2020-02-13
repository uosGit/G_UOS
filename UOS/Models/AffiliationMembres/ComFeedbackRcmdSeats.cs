using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace UOS.Models.AffiliationMembres
{
    public class ComFeedbackRcmdSeats
    {
        [Display(Name="Select Institute")]
        public int InstituteId { get; set; }
        [Display(Name = "Select Course")]
        public int courseid { get; set; }
        public IEnumerable<DDLFeedbackRecommandedSeats> instNames { get; set; }
        public IEnumerable<DDLFeedbackRcmdSeatsCourse> CourseName { get; set; }
        [Display(Name = "Demanded Seats")]
        public int seatDemanded { get; set; }
        [Display(Name = "Recommand Seats")]
        public int seatRecommanded { get; set; }
    }
}