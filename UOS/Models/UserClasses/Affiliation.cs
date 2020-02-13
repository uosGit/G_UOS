using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class Affiliation
    {
        public int Id { get; set; }

        [Display(Name = "Category")]
        public string category { get; set; }

        [Display(Name = "Select Discipline")]
        public int discipline { get; set; }

        [Display(Name = "For the Year")]
        public string year { get; set; }

        [Display(Name = "No. of Seats")]
        public int no_seats { get; set; }


        public IEnumerable<DDL_Category> objCat { get; set; }


        public IEnumerable<DDL_prgram_desc> objprogram { get; set; }
    }
}