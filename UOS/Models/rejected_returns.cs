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
    
    public partial class rejected_returns
    {
        public int id { get; set; }
        public Nullable<int> inst_id { get; set; }
        public Nullable<int> app_id { get; set; }
        public Nullable<int> program_id { get; set; }
        public Nullable<int> st_id_rejected { get; set; }
        public int due_to_return_id { get; set; }
        public string remarks { get; set; }
    }
}
