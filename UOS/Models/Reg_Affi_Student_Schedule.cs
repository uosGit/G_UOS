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
    
    public partial class Reg_Affi_Student_Schedule
    {
        public int ID { get; set; }
        public Nullable<int> Aff_year { get; set; }
        public Nullable<int> Program_Catg_ID { get; set; }
        public string Admin_Catg { get; set; }
        public Nullable<System.DateTime> Start_Admin { get; set; }
        public Nullable<System.DateTime> Close_Admin { get; set; }
        public Nullable<System.DateTime> Last_Fee_Date { get; set; }
        public Nullable<System.DateTime> Return_date { get; set; }
        public Nullable<System.DateTime> Return_Last_date { get; set; }
        public Nullable<int> Fine { get; set; }
        public string Program_catg { get; set; }
        public Nullable<System.DateTime> Save_Date { get; set; }
        public Nullable<System.DateTime> Modify_date { get; set; }
    }
}
