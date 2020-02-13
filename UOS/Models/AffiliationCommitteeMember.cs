using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UOS.Models.Validation;

namespace UOS.Models
{
    public class AffiliationCommitteeMember
    {
        public Affi_Com_Members  ACMember { get; set; }
        public IEnumerable<Affi_com_Designation> AsMember { get; set; }
        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [CCMPasswordValidation]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}