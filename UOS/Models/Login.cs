using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UOS.Models.Validation;

namespace UOS.Models
{
    public class Login
    {
        [Required(ErrorMessage="Email Is Required")]
        public string Email { get; set; }

        [Required(ErrorMessage="Enter Password")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}