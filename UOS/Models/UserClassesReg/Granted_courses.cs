using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UOS.Models.UserClasses;

namespace UOS.Models.UserClassesReg
{
    public class Granted_courses_cls
    {
        public int Id { get; set; }

        [Display(Name = "Category")]
        public string category { get; set; }

        [Display(Name = "Select Discipline")]
        public int discipline { get; set; }

        public int AppId { get; set; }

        public int GrantedSeats { get; set; }

        public IEnumerable<Uos_All_Program_Apply_Category> objCat { get; set; }


        public IEnumerable<DDL_prgram_desc> objprogram { get; set; }
    }
}