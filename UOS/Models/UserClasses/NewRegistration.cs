using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class NewRegistration
    {
        [Display(Name = "Institute Name")]
        public string institute_Name { get; set; }
        [Display(Name = "Institute Code")]
        public string institute_code { get; set; }
        [Display(Name = "Principal Name")]
        public string principal_Name { get; set; }
        [Display(Name = "Institute Landline")]
        public string ins_landline { get; set; }
        [Display(Name = "Institute Fax No.")]
        public string ins_Fax_No { get; set; }
        [Display(Name = "Email")]
        public string email { get; set; }
        [Display(Name = "Principal Contact")]
        public string principal_contact { get; set; }

        [Display(Name = "Country")]
        public string country { get; set; }
        [Display(Name = "Province")]
        public string province { get; set; }
        [Display(Name = "District")]
        public string district { get; set; }
        [Display(Name = "Tehsil")]
        public string tehsil { get; set; }
        [Display(Name = "Postal Address")]
        public string postal_address { get; set; }

        public bool is_gov { get; set; }
        [Display(Name = "User Name")]
        public string username { get; set; }
        [Display(Name = "Password")]
        public string password { get; set; }
      
    }
}