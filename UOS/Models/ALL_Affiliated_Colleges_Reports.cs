using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS.Models
{
    public class ALL_Affiliated_Colleges_Reports
    {

        public int DistrictId { get; set; }
        public IEnumerable<AJ_Pop_districts_cmb_List_Result> DistrictDD { get; set; }

        public int TehsilId { get; set; }
        public IEnumerable<AJ_Pop_districts_Tehsils_cmb_List_Result> TehsilDD { get; set; }

        public int YearId { get; set; }
        public IEnumerable<AJ_Pop_year_Close_cmb_List_Result_Result> YearDD { get; set; }
    }
}