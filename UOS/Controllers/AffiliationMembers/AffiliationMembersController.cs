using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using UOS.Models;
using UOS.Models.AffiliationMembres;
using UOS.Models.UpdateEditbit.CommitteeMemSide;
using System.Web.Script.Serialization;
using UOS.App_Start;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Reporting.WebForms;

namespace UOS.Controllers.AffiliationMembers
{
    [Authorize(Roles="Member")]
    public class AffiliationMembersController : Controller
    {
        UOSEntities db;
        SessionObject obj;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public AffiliationMembersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            db = new UOSEntities();
        }

        public AffiliationMembersController()
        {
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

        public ActionResult Index()
        {
            return View();
        }

        //view of Com_Members/Pannel To download App
        public ActionResult Com_Members()
        {

            return View();
        }

        //Returning object for Com_members of DataTable
        public JsonResult CMDataTableViewer()
        {
            obj = GetData();
            var dbVisitApp = db.Affi_ins_Pop_Visit_applications(Convert.ToInt32(obj.Institute_Id));
            return Json(dbVisitApp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadForm(string appId)
        {
            obj = GetData();
           
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Rep_Affi_Ins_Visting_Member.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Aff_Ins_Vist_Rep_Commety", db.AJ_Aff_Ins_Vist_Rep_Commety(Convert.ToInt32(appId))));
            rpt.DataSources.Add(new ReportDataSource("AJ_Aff_Ins_Vist_Rep_MEmBEr_Commety", db.AJ_Aff_Ins_Vist_Rep_MEmBEr_Commety(Convert.ToInt32(obj.Institute_Id))));

            byte[] bytes = rpt.Render("PDF");
            Response.Buffer = true;
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename= Application" + "." + "PDF");
            Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
            Response.Flush(); // send it to the client to download  
            Response.End();
            //return View();
            //return Json("Secuss", JsonRequestBehavior.AllowGet);
            return View();
        }
        public ActionResult AllApplications()
        {
            var obj = GetData();
            var dbinstitutes = db.BI_Affi_ins_pop_visited_institute(Convert.ToInt32(obj.Institute_Id)).ToList();
            ComFeedBack comfb = new ComFeedBack();
            string[] instituteName = new string[4]{"Faculty","Labortries","Library","Other"};
            List<DDLComMemberCheckPoint> checklist = new List<DDLComMemberCheckPoint>();
            List<InstituteForMembersAllApplications> ifmlist = new List<InstituteForMembersAllApplications>();
            InstituteForMembersAllApplications dummy = new InstituteForMembersAllApplications();
            dummy.InstituteName = "Select Institute";
            ifmlist.Add(dummy);
            foreach (var ins in dbinstitutes)
            {
                InstituteForMembersAllApplications ifm = new InstituteForMembersAllApplications();
                ifm.InstituteId = Convert.ToInt32(ins.Institute_ID);
                ifm.InstituteName = ins.Institute_Name;
                ifmlist.Add(ifm);
            }
            for (var i = 0; i < instituteName.Length; i++)
            {
                DDLComMemberCheckPoint ddl = new DDLComMemberCheckPoint();
                ddl.CheckName = instituteName[i];
                checklist.Add(ddl);
            }
            comfb.InstitutesDDL = ifmlist;
            comfb.CheckPoint = checklist;
            return View(comfb);
        }
        [HttpPost]
        public ActionResult AllApplications(ComFeedBack model)
        {
            obj = GetData();
            int marks = 0;
            if (model.Performance == "Poor")
                marks = 0;
            else if (model.Performance == "Fair")
                marks = 25;
            else if (model.Performance == "Average")
                marks = 50;
            else if (model.Performance == "Good")
                marks = 75;
            else if (model.Performance == "Excellent")
                marks = 100;
            db.BI_insert_com_member_feedback(model.InstituteId, Convert.ToInt32(obj.Institute_Id), model.CheckName, marks, model.Recommendation, obj.Application_id, obj.Affiliation_Year);
            db.SaveChanges();
            ComMembers.Application_id = model.InstituteId;
            var dbinstitutes = db.BI_Affi_ins_pop_visited_institute(Convert.ToInt32(obj.Institute_Id)).ToList();
            ComFeedBack comfb = new ComFeedBack();
            string[] instituteName = new string[4] { "Faculty", "Labortries", "Library", "Other" };
            List<DDLComMemberCheckPoint> checklist = new List<DDLComMemberCheckPoint>();
            List<InstituteForMembersAllApplications> ifmlist = new List<InstituteForMembersAllApplications>();
            foreach (var ins in dbinstitutes)
            {
                InstituteForMembersAllApplications ifm = new InstituteForMembersAllApplications();
                ifm.InstituteId = Convert.ToInt32(ins.Institute_ID);
                ifm.InstituteName = ins.Institute_Name;
                ifmlist.Add(ifm);
            }
            for (var i = 0; i < instituteName.Length; i++)
            {
                DDLComMemberCheckPoint ddl = new DDLComMemberCheckPoint();
                ddl.CheckName = instituteName[i];
                checklist.Add(ddl);
            }
            comfb.InstitutesDDL = ifmlist;
            comfb.CheckPoint = checklist;
            return View(comfb);
        }

        public JsonResult AllADataTableViewer(int id)
        {
            var obj = GetData();
            var result2 = db.BI_Affi_pop_applications_id_wrt_inst_id(id);
            foreach (var v in result2)
            {
                ComMembers.Application_id = Convert.ToInt16(v.Value.ToString());
            }
            //var result = db.BI_affi_pop_com_mem_result(ComMembers.Application_id, userId, AffiliationComMemberDetail.Year_id);
            var result = db.BI_affi_pop_com_mem_result(ComMembers.Application_id, Convert.ToInt32(obj.Institute_Id), 1);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        // update bit for insertion
        public ActionResult InsertionBit(bool val)
        {
            InsertUpdatebit.IsInsertFeedBack = true;
            return View();
        }
        // view for insertion feed back of committee members
        public ActionResult FeedbackRcmdSeats()
        {
            obj = GetData();
            var VisitedInst = db.BI_Affi_ins_pop_visited_institute(Convert.ToInt32(obj.Institute_Id)).ToList();// give id from section 
            ComFeedbackRcmdSeats objfb = new ComFeedbackRcmdSeats();
            List<DDLFeedbackRecommandedSeats> objlist = new List<DDLFeedbackRecommandedSeats>();
            foreach (var item in VisitedInst)
            {
                DDLFeedbackRecommandedSeats objj= new DDLFeedbackRecommandedSeats();
                objj.instituteID = Convert.ToInt16(item.Institute_ID);
                objj.instituteName = item.Institute_Name;
                objlist.Add(objj);
            }
             //pop and fill course according to visited institute 
            //var courseswId = db.BI_pop_applyed_courses(Convert.ToInt16(VisitedInst[0].Institute_ID)).ToList();
            List<DDLFeedbackRcmdSeatsCourse> objlist1 = new List<DDLFeedbackRcmdSeatsCourse>();
            //foreach (var item in courseswId)
            //{
                DDLFeedbackRcmdSeatsCourse obj1 = new DDLFeedbackRcmdSeatsCourse();
                obj1.CourseId = 0;
                obj1.CourseDesc = "------Select Program------";
                objlist1.Add(obj1);
            //}
            objfb.CourseName = objlist1;
            objfb.instNames = objlist;
            return View(objfb);
        }
        // get cources with Respect to institute 
        public ActionResult GetCourses(string stateID)
        
        {
            List<DDLFeedbackRcmdSeatsCourse> objlist = new List<DDLFeedbackRcmdSeatsCourse>();
            int stateiD = Convert.ToInt32(stateID);
            var courseswId = db.BI_pop_applyed_courses(stateiD).ToList();
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(courseswId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // Get demanded seats accordin to the selected course
        public ActionResult GetDemandedSeats(string val)
        {
            string[] temp = val.Split(',');
            var demandedSeats = db.BI_pop_demandedseats(Convert.ToInt16(temp[0]), Convert.ToInt16(temp[1])).ToList();
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(demandedSeats);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult FeedbackRcmdSeats(ComFeedbackRcmdSeats model)
        {
            try
            {
                var obj = GetData();
                db.BI_affi_insert_Com_recmond_seats(Convert.ToUInt16(model.InstituteId), Convert.ToInt16(model.courseid), Convert.ToInt16(obj.Institute_Id), Convert.ToInt16(model.seatRecommanded));
                TempData["Message_string"] = "Add sucessfully";
                return RedirectToAction("FeedbackRcmdSeats", "AffiliationMembers");
            }
            catch (Exception exp)
            {
                TempData["Message_string"] = exp.Message;
                return RedirectToAction("FeedbackRcmdSeats", "AffiliationMembers");
            }
            
            
        }
        // populate datatable values for recommanded seats 
        public JsonResult Pop_recommed_seats_dtbl(int instituteID)
        {
            var obj = GetData();
            var dbVisitApp = db.BI_pop_recmonded_seats(instituteID, Convert.ToInt32(obj.Institute_Id));
            return Json(dbVisitApp, JsonRequestBehavior.AllowGet);
        }
	}
}