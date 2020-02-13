using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UOS.Models.UserClasses
{
    public class last
    {

        [Display(Name = "Select Board")]
        public int board { get; set; }
        [Display(Name = "Select Discipline")]
        public string discipline { get; set; }
        [Display(Name = "No of Students")]
        public int no_students { get; set; }
        [Display(Name = "Board NOC")]
        public string b_NOC { get; set; }

        public byte[] Image { get; set; }
        public string FileName { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

        public IEnumerable<InterBoards> obj_intr_Board { get; set; }
        public IEnumerable<InterCourse> obj_intr_Course { get; set; }

    }
}
