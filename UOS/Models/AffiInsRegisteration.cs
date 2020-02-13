using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UOS.Models;


namespace UOS.Models
{
    public class AffiInsRegisteration
    {
        public int Id { get; set; }

        public string InstName { get; set; }

        public int InstCode { get; set; }

        public string PrincipalName { get; set; }

        [Display(Name="LandLine")]
        public string PTCL { get; set; }

        public string FaxNo { get; set; }

        public string Email { get; set; }

        public string PrincipalContact { get; set; }

        public string PostalAddress { get; set; }

        public string OfficMoble { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string InstituteType { get; set; }

        public int DistrictId { get; set; }
        public IEnumerable<AJ_Pop_districts_cmb_List_Result> DistrictDD { get; set; }

        public int TehsilId { get; set; }
        public IEnumerable<AJ_Pop_districts_Tehsils_cmb_List_Result> TehsilDD { get; set; }
    }
}