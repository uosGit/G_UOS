using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS.Models
{
    public class Register
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Email { get; set; }
        public int UserType { get; set; }
        public int Institute_ID { get; set; }
    }
}