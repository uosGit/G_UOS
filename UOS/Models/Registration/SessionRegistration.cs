using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS.Models.Registration
{
    public class SessionRegistration
    {
        public int AppId { get; set; }
        public int discipline { get; set; }
        public int GrantedSeats { get; set; }
        public string category { get; set; }
        public int StudentId { get; set; }

    }
}