using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UOS.Models.TwoFactorAuthentication;
namespace UOS.Models.TwoFactorAuthentication
{
    public class LoginVerification
    {
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string Code { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<Providers> ProviderDD { get; set; }
    }
}