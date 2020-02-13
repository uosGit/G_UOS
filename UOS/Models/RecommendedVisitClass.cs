using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS.Models
{
    public class RecommendedVisitClass
    {
        public  int InstituteId { get; set; }
        public List<int> AssignedMemberId = new List<int>();
        public  int ApplicaitonId { get; set; }
        public  int YearId { get; set; }
        public  bool IsCheck { get; set; }
        public  DateTime VisitDate { get; set; }
    }
}