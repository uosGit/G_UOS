using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UOS.Models.UserClasses;

namespace UOS.Models.Registration
{
    public class IntermediateDetails
    {
        public int Id { get; set; }

        [Display(Name = "Degree")]
        [Required(ErrorMessage = "Please select a Program")]
        public int i_degree { get; set; }
        public IEnumerable<Intermediate_Programs> Inter_DD { get; set; }


        [Display(Name = "Board")]
        [Required]
        public string i_board { get; set; }
        public IEnumerable<InterBoards> ddl_obj_intr_boad { get; set; }


        [Display(Name = "1st Subject(Major)")]
        [Required]
        public string i_subjects1 { get; set; }
        public IEnumerable<Affi_ins_all_Subject_lab_desc> i_subjects1_DD { get; set; }


        [Display(Name = "Obtained Marks")]
        [Required]
        public int i_1st_o_marks { get; set; }


        [Display(Name = "Total Marks")]
        [Required]
        public int i_1st_t_marks { get; set; }


        [Display(Name = "2nd Subject(Major)")]
        [Required]
        public string i_subjects2 { get; set; }
        public IEnumerable<Affi_ins_all_Subject_lab_desc> i_subjects2_DD { get; set; }


        [Display(Name = "Obtained Marks")]
        [Required]
        public int i_2nd_o_marks { get; set; }


        [Display(Name = "Total Marks")]
        [Required]
        public int i_2nd_t_marks { get; set; }


        [Display(Name = "3rd Subject(Major)")]
        [Required]
        public string i_subjects3 { get; set; }
        public IEnumerable<Affi_ins_all_Subject_lab_desc> i_subjects3_DD { get; set; }


        [Display(Name = "Obtained Marks")]
        [Required]
        public int i_3rd_o_marks { get; set; }


        [Display(Name = "Total Marks")]
        [Required]
        public int i_3rd_t_marks { get; set; }


        [Display(Name = "Exam Type")]
        [Required]
        public string i_exam_type { get; set; }


        [Display(Name = "Passing Year")]
        [Required]
        public string i_p_year { get; set; }
        public IEnumerable<YearPopulate> ddl_obj_years { get; set; }


        [Display(Name = "Roll No")]
        [Required]
        public string i_roll_no { get; set; }


        [Display(Name = "Obtained Marks")]
        [Required]
        public int i_o_marks { get; set; }


        [Display(Name = "Marks")]
        [Required]
        public int i_t_marks { get; set; }

        [Required]
        public float Perstage { get; set; }

        [Display(Name = "Division")]
        [Required]
        public string i_division { get; set; }


        [Display(Name = "Degree Scan")]
        [Required]
        public HttpPostedFileBase ImageFile { get; set; }
        public byte[] i_inter_scan { get; set; }
        public string FileName { get; set; }


        [Required]
        public int requeredDegEduction { get; set; }
    }
}