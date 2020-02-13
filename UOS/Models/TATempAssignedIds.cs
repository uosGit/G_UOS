using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS.Models
{
    public class TATempAssignedIds
    {
        public List<int> InstituteId { get; set; }
        public List<int> YearId { get; set; }
        public List<int> ApplicatoinId { get; set; }
        public DateTime VisitDate { get; set; }
    }
}