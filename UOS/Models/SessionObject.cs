using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS.Models
{
    public class SessionObject
    {
        public  string Institute_Id { set; get; }
        public  string Inst_Id_for_Admin { set; get; }
        public  int Id_year_for_affilation { set; get; }
        public  int Affiliation_Year { set; get; }
        public  int Application_id { set; get; }
    }
}