using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.Registration
{
    public class ApplicationForm
    {


        public StudentBasicDetails StudentBasicDetails { get; set; }
        public GuardianDetails GuardianDetails { get; set; }
        public MetricDetails MetricDetails { get; set; }
        public IntermediateDetails IntermediateDetails { get; set; }
        public BachelorDetails BachelorDetails { get; set; }
        public AttachDocument AttachDocument { get; set; }
        public AffiliatedCollegeDetails AffiliatedCollegeDetails { get; set; }

       
    }
}