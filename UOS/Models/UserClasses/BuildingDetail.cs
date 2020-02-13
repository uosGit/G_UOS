using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class BuildingDetail
    {
        public int Id { get; set; }

        [Display(Name = "Building Name")]
        [Required(ErrorMessage="*Required")]
        public string building_name { get; set; }

        [Display(Name = "Covered Area")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Area Must Be Numeric")]
        public string c_area { get; set; }

        [Display(Name = "Uncovered")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Area Must Be Numeric")]
        public string uc_area { get; set; }

        public string FileName { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public byte[] map { get; set; }


    }
}