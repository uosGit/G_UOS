//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UOS.Models
{
    using System;
    
    public partial class Aj_GetProg_Eligibility_Check_Info_OStudID_Result
    {
        public int ID { get; set; }
        public string Program_Desc { get; set; }
        public string Programs { get; set; }
        public Nullable<int> Req_marks { get; set; }
        public string Subjects1 { get; set; }
        public Nullable<int> Req_subject1_Tmarks { get; set; }
        public Nullable<int> Req_subject1_Omarks { get; set; }
        public string Subjects2 { get; set; }
        public Nullable<int> Req_subject2_Tmarks { get; set; }
        public Nullable<int> Req_subject2_Omarks { get; set; }
        public string Subjects3 { get; set; }
        public Nullable<int> Req_subject3_Tmarks { get; set; }
        public Nullable<int> Req_subject3_Omarks { get; set; }
        public Nullable<bool> EqualEdu { get; set; }
    }
}