using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class UserStatus
    {
        public string InstituteName { get; set; }
        public string PrincipalName { get; set; }
        public string PostalAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Nature { get; set; }
        public string Status { get; set; }
        public string Status1 { get; set; }
        public int ReturnStatusIdentity { get; set; }
        public string StatusValueFOrLink { get; set; }


    }
}