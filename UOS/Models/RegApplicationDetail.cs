using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UOS.Models
{
    public class RegApplicationDetail
    {
        public int St_ID { get; set; }
        public int St_App_ID { get; set; }
        public string Reg_Num { get; set; }
        public string Roll_No { get; set; }
        public string College_Name { get; set; }
        public string District { get; set; }
        public string Program { get; set; }
        public string Sesion { get; set; }

        public string student_Name { get; set; }
        public string student_Cnic { get; set; }
        public string Father_Name { get; set; }
        public string Father_Cnic { get; set; }
        public string DOB { get; set; }
        public string Religion { get; set; }
        public string Gender { get; set; }
        public string ContactNo { get; set; }
        public string EmailID { get; set; }
        public string Address { get; set; }

        public string M_DegreeName { get; set; }
        public string M_PassYear { get; set; }
        public string M_Total { get; set; }
        public string M_Obtain { get; set; }
        public string M_Perstage { get; set; }

        public string I_DegreeName { get; set; }
        public string I_PassYear { get; set; }
        public string I_Subject1 { get; set; }
        public string I_Subject1_Marks { get; set; }
        public string I_Subject2 { get; set; }
        public string I_Subject2_Marks { get; set; }
        public string I_Subject3 { get; set; }
        public string I_Subject3_Marks { get; set; }
        public string I_Total { get; set; }
        public string I_Obtain { get; set; }
        public string I_Perstage { get; set; }

        public string B_Degree { get; set; }
        public string B_DegreeType { get; set; }
        public string B_DegreeName { get; set; }
        public string B_PassYear { get; set; }
        //public string B_Subject1 { get; set; }
        //public string B_Subject1_Marks { get; set; }
        //public string B_Subject2 { get; set; }
        //public string B_Subject2_Marks { get; set; }
        //public string B_Subject3 { get; set; }
        //public string B_Subject3_Marks { get; set; }
        public string B_Total { get; set; }
        public string B_Obtain { get; set; }
        public float B_Perstage { get; set; }

        public string AppStatus { get; set; }
        public string Elegibelty { get; set; }

        public byte[] imageByte { get; set; }

        public string ImageSource
        {
            get
            {
                if (imageByte != null)
                {
                    string mimeType = ("image / jpg"); /* Get mime type somehow (e.g. "image/png") */;
                    string base64 = Convert.ToBase64String(imageByte);
                    return string.Format("data:{0};base64,{1}", mimeType, base64);
                }
                else
                {
                    return "";
                }
            }
        }
    }
}