using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class NonTeachingStaff
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string name { get; set; }

        [Display(Name = "Degree")]
        public string degreename { get; set; }
        public IEnumerable<BI_POP_All_Degree_Result> Degree { get; set; }


        [Display(Name = "Subject")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string subject { get; set; }

        [Display(Name = "University")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string uni { get; set; }

        [Display(Name = "Passing Year")]
        public string pass_yearname { get; set; }
        public IEnumerable<YearPopulate> Year { get; set; }

        [Display(Name = "Designation")]
        public string designationname { get; set; }
        public IEnumerable<BI_POP_Designation_Teaching_Staff_Result> Designation { get; set; }

        //public DateTime appoint_date { get; set; }

        [Display(Name = "Appoint Date")]
        public string appoint_date { get; set; }

        [Display(Name = "Nature")]
        public string naturename { get; set; }
        public IEnumerable<BI_pop_staff_appoint_nature_Result> Nature { get; set; }

        [Display(Name = "Salary")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Salary Must Be In Digits")]
        public int salary { get; set; }

        [Display(Name = "Experience")]
        public string experiencename { get; set; }
        public IEnumerable<BI_POP_Duration_Experience_Teaching_Staff_Result> Experience { get; set; }

        [Display(Name = "Remarks")]
        [Required]
        public string remarks { get; set; }

    }
}