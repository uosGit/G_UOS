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
    
    public partial class affi_ins_disaffiliation_scan
    {
        public int id { get; set; }
        public Nullable<int> inst_id { get; set; }
        public Nullable<int> application_id { get; set; }
        public Nullable<int> page_no { get; set; }
        public byte[] scan { get; set; }
    }
}