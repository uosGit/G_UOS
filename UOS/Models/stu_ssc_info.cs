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
    
    public partial class stu_ssc_info
    {
        public int m_ID { get; set; }
        public int st_id { get; set; }
        public string m_degree_name { get; set; }
        public string m_subject { get; set; }
        public Nullable<int> m_board_id { get; set; }
        public Nullable<int> m_passing_year { get; set; }
        public string m_examination_type { get; set; }
        public string m_rollnumber { get; set; }
        public Nullable<int> m_total_marks { get; set; }
        public Nullable<int> m_obtained_marks { get; set; }
        public string m_division { get; set; }
        public Nullable<double> m_percentage { get; set; }
        public byte[] m_result_scan { get; set; }
        public string image_name { get; set; }
    }
}
