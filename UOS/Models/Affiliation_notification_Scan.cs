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
    
    public partial class Affiliation_notification_Scan
    {
        public int ID { get; set; }
        public int inst_id { get; set; }
        public int app_id { get; set; }
        public byte[] notification_scan { get; set; }
        public string imageName { get; set; }
    }
}
