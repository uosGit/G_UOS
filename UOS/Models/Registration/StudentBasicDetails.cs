using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using UOS.Models.Common;

namespace UOS.Models.Registration
{
    public class StudentBasicDetails
    {
        public int Id { get; set; }

        public int AppId { get; set; }
        public int GrantedSeats { get; set; }
        public int Discipline { get; set; }
        public string DisciplineName { get; set; }
        public string Category { get; set; }
        public string CategoryName { get; set; }

        [Display(Name = "Reg. No(if any)")]
        public string regNumber { get; set; }

        [Display(Name = "Student Name")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string stu_name { get; set; }

        [Display(Name = "S/O, D/O")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string stu_Father { get; set; }

        public string stu_img { get; set; }

        [Required]
        [Display(Name = "CNIC")]
        [RegularExpression("^[0-9]{5}-[0-9]{7}-[0-9]$", ErrorMessage = "CNIC No Must Follow The XXXXX-XXXXXXX-X Format")]
        public string cnic { get; set; }

        [Display(Name = "Cell No")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number Must Be In Digits")]

        public string cell_no { get; set; }

        [Display(Name = "Email")]
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string email { get; set; }

        [Display(Name = "Place of Birth")]
        [Required]
        public string pob { get; set; }

        [Display(Name = "Date of Birth")]
        [Required]
        public string dob { get; set; }

        [Display(Name = "Gender")]
        [Required]
        public string gender { get; set; }
        public IEnumerable<Gender> GenderDD { get; set; }

        [Display(Name = "Marital Status")]
        [Required]
        public string m_status { get; set; }
        public IEnumerable<MeritalStatus> m_statusDD { get; set; }

        [Display(Name = "Religion")]
        [Required]
        public string religion { get; set; }

        [Display(Name = "Nationality")]
        [Required]
        [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Use Alphabets only")]
        public string nationality { get; set; }

        [Display(Name = "Blood Group")]
        [Required]
        public string b_group { get; set; }

        //[Display(Name = "Residence")]
        //[Required]
        //public string residence { get; set; }

        [Display(Name = "Present Address")]
        [Required]
        public string address { get; set; }

        [Display(Name = "Country")]
        [Required]
        public int country { get; set; }
        public IEnumerable<BI_POP_ALL_COUNTRIES_Result> CountryDD { get; set; }

        [Display(Name = "Province")]
        [Required]
        public int province { get; set; }
        public IEnumerable<BI_POP_ALL_PROVIENCES_Result> ProvinceDD { get; set; }

        [Display(Name = "District")]
        [Required]
        public int district { get; set; }
        public IEnumerable<BI_POP_ALL_DISTT_WRT_PROVIENCE_Result> DistrictDD { get; set; }

        [Display(Name = "Tehsil")]
        [Required]
        public int tehsil { get; set; }
        public IEnumerable<BI_POP_ALL_TEHSIL_WRT_DIST_Result> TehsilDD { get; set; }

        [Display(Name = "Permanent Address")]
        [Required]
        public string per_address { get; set; }

        [Display(Name = "Country")]
        [Required]
        public int per_country { get; set; }
        public IEnumerable<BI_POP_ALL_COUNTRIES_Result> Per_countryDD { get; set; }

        [Display(Name = "Province")]
        [Required]
        public int per_province { get; set; }
        public IEnumerable<BI_POP_ALL_PROVIENCES_Result> Per_provinceDD { get; set; }

        [Display(Name = "District")]
        [Required]
        public int per_district { get; set; }
        public IEnumerable<BI_POP_ALL_DISTT_WRT_PROVIENCE_Result> Per_districtDD { get; set; }

        [Display(Name = "Tehsil")]
        [Required]
        public int per_tehsil { get; set; }
        public IEnumerable<BI_POP_ALL_TEHSIL_WRT_DIST_Result> Per_tehsilDD { get; set; }

        public HttpPostedFileBase imageUpload { get; set; }
        public byte[] Draft_Scan { get; set; }
        public string FileName { get; set; }


    }
}