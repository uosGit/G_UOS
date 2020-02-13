using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UOS.Models.UserClasses;

namespace UOS.Models.Registration
{
    public class BachelorDetails
    {
        public int Id { get; set; }

        [Display(Name = "Degree Type")]
        [Required]
        public string b_degree { get; set; }

        [Display(Name = "Degree Name")]
        [Required]
        public string b_dgreeName { get; set; }
        public IEnumerable<Uos_All_Programs> b_dgreeName_dd { get; set; }


        [Display(Name = "University")]
        [Required]
        public string uni_inst { get; set; }


        [Display(Name = "Passing Year")]
        [Required]
        public string b_p_year { get; set; }
        public IEnumerable<YearPopulate> ddl_obj_years { get; set; }


        [Display(Name = "Roll No")]
        [Required]
        public string b_roll_no { get; set; }


        [Display(Name = "Major Subject One")]
        [Required]
        public string m_sub_1 { get; set; }
        public IEnumerable<Affi_ins_all_Subject_lab_desc> i_subjects1_DD { get; set; }

        [Display(Name = "Obtained Marks")]
        [Required]
        public int o_m_subject_1 { get; set; }

        [Display(Name = "Marks")]
        [Required]
        public int t_m_subject_1 { get; set; }




        [Display(Name = "Major Subject Two")]
        [Required]
        public string m_sub_2 { get; set; }
        public IEnumerable<Affi_ins_all_Subject_lab_desc> i_subjects2_DD { get; set; }

        [Display(Name = "Obtained Marks")]
        [Required]
        public int o_m_subject_2 { get; set; }

        [Display(Name = "Marks")]
        [Required]
        public int t_m_subject_2 { get; set; }





        [Display(Name = "Major Subject Three")]
        [Required]
        public string m_sub_3 { get; set; }
        public IEnumerable<Affi_ins_all_Subject_lab_desc> i_subjects3_DD { get; set; }

        [Display(Name = "Obtained Marks")]
        [Required]
        public int o_m_subject_3 { get; set; }

        [Display(Name = "Marks")]
        [Required]
        public int t_m_subject_3 { get; set; }




        [Display(Name = "Obtained Marks")]
        [Required]
        public int b_o_marks { get; set; }

        [Display(Name = "Total Marks")]
        [Required]
        public int b_t_marks { get; set; }


        [Required]
        public float b_Perstage { get; set; }

        [Display(Name = "Division")]
        [Required]
        public string b_division { get; set; }


        [Display(Name = "Exam Type")]
        [Required]
        public string b_exam_type { get; set; }


        [Display(Name = "Nature of Exam")]
        [Required]
        public string b_nature_exam { get; set; }


        [Display(Name = "Degree Scan")]
        [Required]
        public HttpPostedFileBase ImageFile { get; set; }
        public byte[] b_bachlor_scan { get; set; }
        public string FileName { get; set; }

    }
}