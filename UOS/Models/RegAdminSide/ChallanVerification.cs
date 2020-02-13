using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.RegAdminSide
{
    public class ChallanVerification
    {
        [Display(Name = "College Name")]
        public string clg_name { get; set; }
        [Display(Name = "Program Name")]
        public string program { get; set; }
        [Display(Name = "Fee Slip_ID")]
        public string fee_slip { get; set; }
        [Display(Name = "Account_No")]
        public string account_no { get; set; }
        [Display(Name = "Amount")]
        public int amount { get; set; }
        [Display(Name = "Bank Name")]
        public string bnk_name{ get; set; }
        [Display(Name = "Branch Code")]
        public string b_code{ get; set; }
        [Display(Name = "Deposite Date")]
        public string deposite_date { get; set; }
        public byte[] imageByte { get; set; }
        public string ImageSource
        {
          get
            {
                if (imageByte != null)
                {
                    string mimeType = ("image / jpg"); /* Get mime type somehow (e.g. "image/png") */;
                    string base64 = Convert.ToBase64String(imageByte);
                    return string.Format("data:{0};base64,{1}", mimeType, base64);
                }
                else
                {
                    return "";
                }
            }
       }

    }
}