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
    
    public partial class stu_documents
    {
        public int ID { get; set; }
        public int st_id { get; set; }
        public byte[] domicile { get; set; }
        public string img_domicile_name { get; set; }
        public byte[] cnic { get; set; }
        public string img_cnic_name { get; set; }
        public byte[] noc_serving { get; set; }
        public string img_noc_serving_name { get; set; }
        public byte[] noc_migration { get; set; }
        public string img_noc_migration_name { get; set; }
    }
}
