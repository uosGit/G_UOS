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
    using System.Collections.Generic;
    
    public partial class Uos_All_Programs
    {
        public int ID { get; set; }
        public int Cat_Id { get; set; }
        public int Dpt_Id { get; set; }
        public string Program_Desc { get; set; }
        public double Program_Duration { get; set; }
        public Nullable<int> Semester { get; set; }
        public Nullable<int> Program_Apply_Category { get; set; }
        public bool Status { get; set; }
        public Nullable<int> program_duration_month { get; set; }
        public Nullable<int> Duration_Id { get; set; }
    }
}
