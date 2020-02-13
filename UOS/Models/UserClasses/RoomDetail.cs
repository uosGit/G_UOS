using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class RoomDetail
    {
        public int Id { get; set; }

        [Display(Name = "Select Building Name")]
        [Required]
        public int building_id { get; set; }

        public IEnumerable<BuildingDetail> BuildingDetail { get; set; }

        [Required]
        [Display(Name = "Select Room Type")]
        public string room_type { get; set; }

        [Display(Name = "Length")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number Must Be In Digits")]
        public int length { get; set; }

        [Display(Name = "Width")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number Must Be In Digits")]
        public int width { get; set; }

        [Display(Name = "Quantity")]
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact Number Must Be In Digits")]
        public int quantity { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }

        public string BuidingName { get; set; }

    }
}