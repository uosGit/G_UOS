using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UOS.Models.UserClasses;

namespace UOS.Models.Registration
{
    public class MetricDetails
    {
        public int Id { get; set; }
        [Display(Name = "Degree")]
        [Required]
        public string degree { get; set; }


        [Display(Name = "Subjects")]
        [Required]
        public string subjects { get; set; }


        [Display(Name = "Board")]
        [Required]
        public string board { get; set; }
        public IEnumerable<InterBoards> ddl_obj_intr_boad { get; set; }


        [Display(Name = "Passing Year")]
        [Required]
        public string p_year { get; set; }
        public IEnumerable<YearPopulate> ddl_obj_years { get; set; }


        [Display(Name = "Exam Type")]
        [Required]
        public string exam_type { get; set; }


        [Display(Name = "Roll No")]
        [Required]
        public string roll_no { get; set; }


        [Display(Name = "Obtained Marks")]
        [Required]
        public int o_marks { get; set; }


        [Display(Name = "Marks")]
        [Required]
        public int t_marks { get; set; }


        [Display(Name = "Division")]
        [Required]
        public string division { get; set; }
        public double Perstage { get; set; }


        [Display(Name = "Degree Scan")]
        public HttpPostedFileBase ImageFile { get; set; }
        public byte[] matric_scan { get; set; }
        public string FileName { get; set; }

    }
}