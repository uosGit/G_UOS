using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models
{
    public class GrantSeat
    {
        public int ApplicationId { get; set; }
        public int ProgramId { get; set; }
        public int RecommendedSeats { get; set; }
        public int DemandedSeats { get; set; }
        public string ProgramName { get; set; }
        public int GrantedSeats { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
        public byte[] VcScan{ get; set; }
        public string FileName { get; set; }
        [Display(Name = "Notification No")]
        [Required]
        public string NotifectionNo { get; set; }
        [Display(Name = "Notification Date")]
        [Required]
        public string NotifectionDate { get; set; }
    }
}