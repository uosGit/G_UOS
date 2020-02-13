using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UOS.App_Start;
using UOS.Models;
using UOS.Models.Registration;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using UOS.App_Start;
using System.IO;
using UOS.Models.UserClasses;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using UOS.Models;
using UOS.Models.RegAdminSide;
using Microsoft.Reporting.WebForms;






namespace UOS.Controllers.RegistrationAdminSide
{
     

    [Authorize(Roles = "RegistrationAdmin")]
    public class RegistrationAdminSideController : Controller
    {


        UOSEntities db;
        SessionObject obj;
        SessionRegistration regsession;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public RegistrationAdminSideController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            db = new UOSEntities();
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public SessionObject GetData()
        {
            var claims = UserManager.GetClaims(User.Identity.GetUserId());
            SessionObject obj = new SessionObject();
            foreach (var claim in claims)
            {
                if (claim.Type == "InstituteId")
                    obj.Institute_Id = Convert.ToString(claim.Value);
                if (claim.Type == "AffiliationYear")
                    obj.Affiliation_Year = Convert.ToInt32(claim.Value);
                if (claim.Type == "YearId")
                    obj.Id_year_for_affilation = Convert.ToInt32(claim.Value);
            }
            return obj;
        }

        public SessionRegistration GetRegSessions()
        {
            var claims = UserManager.GetClaims(User.Identity.GetUserId());
            SessionRegistration obj = new SessionRegistration();
            foreach (var claim in claims)
            {
                if(claim.Type == "ApplicationId")
                    obj.AppId = Convert.ToInt32(claim.Value);
                if(claim.Type == "GrantedSeats")
                    obj.GrantedSeats = Convert.ToInt32(claim.Value);
                if(claim.Type == "Discipline")
                    obj.discipline = Convert.ToInt32(claim.Value);
                if(claim.Type == "Category")
                    obj.category = claim.Value;
                if (claim.Type == "StudentId")
                    obj.StudentId = Convert.ToInt32(claim.Value);
            }
            return obj;
        }

        public RegistrationAdminSideController()
        {
            db = new UOSEntities();
        }


        //
        // GET: /RegistrationAdminSide/
        public ActionResult Index()
        {
            return View();
        }
        //For Registration Affiliated colleges Those Challan Not Uploaded
        public ActionResult RegAffColleges()
        {
            return View();
        }

        public ActionResult ChallanVerfication(string ReturnId)
        {
            int ReturnID = Convert.ToInt32(ReturnId);
            var dbvlu = db.Aj_Aff_Ins_Admin_Fee_slip(ReturnID);
            ChallanVerification obj = new ChallanVerification();
            int FeeAmu=0;
            foreach (var item in dbvlu)
            {
                Session["Ret_ID"] = item.rt_id;
                obj.clg_name = item.Ins_Name;
                obj.program = item.Program_Desc;
                obj.fee_slip = item.FeeSlipNo;
                obj.account_no = item.Account_No;
                FeeAmu +=Convert.ToInt32(item.FeeAmount);
                if (item.Branch_Name != "")
                {
                    obj.bnk_name = item.Branch_Name;
                    obj.b_code = item.Branch_Code;
                    obj.deposite_date = Convert.ToDateTime(item.Recive_Date).ToString("dd-MMM-yyyy");
                    obj.imageByte = item.Scan;
                }
                else
                {
                    obj.bnk_name = "";
                    obj.b_code ="";
                    obj.deposite_date = "";
                    obj.imageByte = null;
                }
            }
            obj.amount = FeeAmu;
            if (FeeAmu == 0)
            {
                return RedirectToAction("RegAffColleges", "RegistrationAdminSide");
            }
            else
            {
                return View(obj);
            }
        }
        
        //fee Verify Bution Action
        [HttpPost]
        public ActionResult ChallanVerfication(ChallanVerification model)
        {


            int retID = Convert.ToInt32(Session["Ret_ID"]);
            db.Aj_Reg_Admin_verify_Fee_slip(Convert.ToInt32( model.fee_slip), retID, model.amount,Convert.ToDateTime( model.deposite_date), true);
           // db.Aj_Reg_Admin__Add_Status_Eligiblty(St_App_ID, St_ID, Eligiblty, model.AppStatus);

            TempData["Success"] = "Inserted Successfully";

            //RegAffCollegesStdView

            return RedirectToAction("RegAffColleges", "RegistrationAdminSide");
        }


        // For Registration Main Campus
        public ActionResult RegMainCampus()
        {
            return View();
        }

        // For Registration Examination
        public ActionResult RegExam()
        {
            return View();
        }

        // Sending the object for datatables to ApplicaitonForm
        public JsonResult NewReturns()
        {
            var dbpendingApps = db.BI_reg_pop_New_Return_AC(2);
            return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
        }

        //For Registration Affiliated colleges Those Challan Not Uploaded
        public ActionResult RegAffCollegesuploadedChallans()
        {
            return View();
        }

        public ActionResult DownloadAnex(int RntID)
        {
            byte[] bytes = null;
            //if (returnId == null)
            //    returnId = "0";
            //int catg = Convert.ToInt32(returnId.Substring(0, 1));
            //int RntID = Convert.ToInt32(returnId.Substring(1, returnId.Length - 1));
            string DocumetNAme = "";
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            
                var DBvlu = db.AJ_Stud_Admin_Return_Reg_C(RntID);
                rpt.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Affi_reg_return_Anax_C.rdlc");
                rpt.DataSources.Add(new ReportDataSource("Rdlc_Affi_reg_return_Anax_C", DBvlu));
                DocumetNAme = "Annex-C" + "." + "PDF";
                rpt.GetDefaultPageSettings();
                bytes = rpt.Render("PDF");
           
           
            // bytes = rpt.Render("PDF");
            if (bytes.Length > 0)
            {
                Response.Buffer = true;
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename=" + DocumetNAme);
                Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
                Response.Flush(); // send it to the client to download  
                Response.End();
                //  return View();
            }
            return RedirectToAction("ReturnStatus", "Registration");
        }


        public ActionResult VerifiedEligible()
        {
            return View();
        }

        // Sending the object for datatables to ApplicaitonForm
        public JsonResult NewReturnsWithChallan(int RetStu)
        {
            var dbpendingApps = db.BI_reg_pop_New_Return_AC(RetStu);
            return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
        }
        //For Registration Affiliated colleges Student View
        public ActionResult RegAffCollegesStdView(string returnId)
        {
            if (returnId == null)
                returnId = "0";
            TempData["ReturnID"] = returnId.ToString();
            return View();
        }
        public JsonResult dtblReturnStudents(string returnid)
        {
            if (returnid == null)
                returnid = "0";
            var returnstatus = db.bi_reg_pop_stu_wrt_return(Convert.ToInt32(returnid));
            return Json(returnstatus, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApplicationDetail(string returnId)
        {
            int vlu = Convert.ToInt32(returnId);
            RegApplicationDetail obj = new RegApplicationDetail();
            if(vlu>0)
            {
                int stud_App_ID = 0;
            var dbvlu = db.Stud_Admin_Basic_info(vlu);
            foreach (var item in dbvlu)
            {
                obj.St_ID = item.st_id;
                obj.St_App_ID = item.StudAppID;
                Session["St_ID"] = item.st_id;
                Session["St_App_ID"] = item.StudAppID;
                obj.Reg_Num = item.reg_number;
                obj.Roll_No = item.roll_number;
                obj.College_Name = item.Ins_Name;
                obj.District = "";
                obj.Program = item.Program_Desc;
                obj.Sesion = item.Prog_session;
                stud_App_ID = item.StudAppID;

                obj.student_Name = item.st_name;
                obj.student_Cnic = item.st_cinc;
                obj.Father_Name = item.st_gr_name;
                obj.Father_Cnic = item.st_gr_cnic;
                obj.DOB = Convert.ToDateTime(item.st_dob).ToString("dd-MMMM-yyyy");
                obj.Religion = item.st_religion;
                obj.Gender = item.st_gender;
                obj.ContactNo = item.st_contact;
                obj.EmailID = item.st_email;
                obj.Address = item.st_address;

                obj.M_DegreeName = item.m_degree_name;
                obj.M_PassYear = item.m_passing_year.ToString();
                obj.M_Obtain = item.m_obtained_marks.ToString();
                obj.M_Total = item.m_total_marks.ToString();
                obj.M_Perstage = item.m_percentage.ToString();

                obj.I_DegreeName = item.Inter_Program;
                obj.I_PassYear = item.i_passing_year.ToString();
                obj.I_Subject1 = item.I_sub_name_1.ToString();
                obj.I_Subject1_Marks = item.i__major_subject1_numbers + "/" + item.i__major_subject1_t_numbers;
                obj.I_Subject2 = item.I_sub_name_2.ToString();
                obj.I_Subject2_Marks = item.i__major_subject2_numbers + "/" + item.i__major_subject2_t_numbers;
                obj.I_Subject3 = item.I_sub_name_3.ToString();
                obj.I_Subject3_Marks = item.i__major_subject3_numbers + "/" + item.i__major_subject3_t_numbers;
                obj.I_Obtain = item.i_obtained_marks.ToString();
                obj.I_Total = item.i_total_marks.ToString();
                obj.I_Perstage = item.i_percentage.ToString();

                obj.B_DegreeName = item.degree_name;
                obj.B_DegreeType = item.degree_type;
                obj.B_PassYear = item.passing_year.ToString();
                //obj.B_Subject1 = item.i__major_subject1.ToString();
                //obj.B_Subject1_Marks = item.i__major_subject1_t_numbers + "\\" + item.i__major_subject1_numbers;
                //obj.B_Subject2 = item.i__major_subject2.ToString();
                //obj.B_Subject2_Marks = item.i__major_subject2_t_numbers + "\\" + item.i__major_subject2_numbers;
                //obj.B_Subject3 = item.i__major_subject3.ToString();
                //obj.B_Subject3_Marks = item.i__major_subject3_t_numbers + "\\" + item.i__major_subject3_numbers;
                obj.B_Obtain = item.obtained_marks.ToString();
                obj.B_Total = item.total_marks.ToString();
                obj.I_Perstage = item.percentage.ToString();
                obj.imageByte = item.st_image;
                
            }
            string Eleg = null;
            var dbvluElg = db.Aj_GetProg_Eligibility_Check_Info_OStudID(stud_App_ID);
            foreach (var item in dbvluElg)
            {
                if (Convert.ToBoolean( item.EqualEdu))
                {
                    Eleg = item.Programs + " Or Equivalent Eualification";
                }
                else
                { Eleg = item.Programs; }
                Eleg+="( "+item.Req_marks.ToString()+" %)";


                  if (item.Subjects1 != null)
                    { var sub1 = "Subject-1: (" + item.Subjects1 + ")";
                        if(item.Req_subject1_Tmarks>0)
                        {  sub1 += "Subject Marks:" + item.Req_subject1_Tmarks ;}
                         if (item.Req_subject1_Omarks > 0)
                          { sub1 += "Required Percentage:" + item.Req_subject1_Omarks + " %"; }
                     

                     Eleg += "\n" + sub1;
                   }
                  if (item.Subjects2 != null)
                  {
                      var sub1 = "Subject-2: (" + item.Subjects2 + ")";
                      if (item.Req_subject2_Tmarks > 0)
                      { sub1 += "Subject Marks:" + item.Req_subject2_Tmarks; }
                      if (item.Req_subject2_Omarks > 0)
                      { sub1 += "Required Percentage:" + item.Req_subject2_Omarks + " %"; }


                      Eleg += "\n" + sub1;
                  }
                  if (item.Subjects3 != null)
                  {
                      var sub1 = "Subject-3: (" + item.Subjects3 + ")";
                      if (item.Req_subject3_Tmarks > 0)
                      { sub1 += "Subject Marks:" + item.Req_subject3_Tmarks; }
                      if (item.Req_subject3_Omarks > 0)
                      { sub1 += "Required Percentage:" + item.Req_subject3_Omarks + " %"; }


                      Eleg += "\n" + sub1;
                  }

            }
            obj.Elegibelty = Eleg;
            return View(obj);


            }
            else
                return RedirectToAction( "Index","RegistrationAdminSide");
        }

        [HttpPost]
        public ActionResult ApplicationDetail(RegApplicationDetail model)
        {
            int Eligiblty = 0;
            if (model.AppStatus == "Yes")
                Eligiblty=1;
            else if (model.AppStatus == "No")
                Eligiblty = 2;
            else if (model.AppStatus == "Pending")
                Eligiblty = 3;
            int  St_ID= Convert.ToInt32( Session["St_ID"]) ;
              int St_App_ID =Convert.ToInt32( Session["St_App_ID"]);
             // db.Aj_Reg_Admin__Add_Status_Eligiblty(model.St_App_ID, model.St_ID, Eligiblty, model.AppStatus);
              db.Aj_Reg_Admin__Add_Status_Eligiblty(St_App_ID, St_ID, Eligiblty, model.AppStatus);

            TempData["Success"] = "Inserted Successfully";
            return RedirectToAction("Index", "RegistrationAdminSide");
        }
	}
}