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
    
    public partial class Affi_ins_related_documents
    {
        public int id_ato { get; set; }
        public int ID { get; set; }
        public int inst_id { get; set; }
        public Nullable<int> Doc_cat_id { get; set; }
        public byte[] Doc_Scan { get; set; }
        public Nullable<int> page_no { get; set; }
        public Nullable<System.DateTime> Insertion_date { get; set; }
        public Nullable<System.DateTime> lastmodified_date { get; set; }
        public Nullable<System.DateTime> remove_date { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> ForTheYear { get; set; }
    }
}
