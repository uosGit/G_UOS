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
    
    public partial class AJ_Aff_Ins_Apply_Rep_Draft_Result
    {
        public int Dreft_ID { get; set; }
        public Nullable<int> App_ID { get; set; }
        public string Draft_amount { get; set; }
        public Nullable<System.DateTime> Deposit_Date { get; set; }
        public string Bank_Name { get; set; }
        public string Bank_Address { get; set; }
        public string Bank_branch_code { get; set; }
        public byte[] Draft_scan { get; set; }
        public int inst_id { get; set; }
        public string Ins_Name { get; set; }
    }
}