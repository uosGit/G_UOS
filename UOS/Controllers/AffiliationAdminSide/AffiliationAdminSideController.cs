using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using UOS.Models;
using Microsoft.AspNet.Identity;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using UOS.App_Start;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using System.IO;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;

namespace UOS.Controllers.AffiliationAdminSide
{
    [Authorize(Roles="Admin")]
    public class AffiliationAdminSideController : Controller
    {

        //Initiating Database
        UOSEntities db;         
        int[] Serial_result = new int[16];
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
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

        public AffiliationAdminSideController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            db = new UOSEntities();
        }
        public AffiliationAdminSideController()
        {
            db = new UOSEntities();
            
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult TestPage()
        {
            return View();
        }

        public ActionResult Draft()
        {
            return View();
        }

        // GET: /ApplicationForm/
        public ActionResult ApplicationForm()
        {
            return View();
        }

        // Sending the object for datatables to ApplicaitonForm
        public JsonResult ApplicationFormDataTableViewer()
        {   
            var dbpendingApps = db.Affi_ins_Pop_pending_applications();
            return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
        }

        // view to the affiliation committee
        public ActionResult ApplicationFormViewApp(int Id)
        {
            try
            {
                var ViewApplication = db.TA_Affi_ins_Pop_pending_applications_By_Id(Id).ToList();
                if (!ViewApplication.Any())
                {
                    ViewBag.ApplicationIdNotFound = "This Application Does Not Exist AnyMore";
                    return RedirectToAction("ApplicationForm", "AffiliationAdminSide");
                }
                foreach (var data in ViewApplication.ToList())
                {

                    AffiliationAdminDetail.Inst_Id_for_Admin = Convert.ToString(data.Institute_ID);
                    AffiliationAdminDetail.Application_id = Convert.ToInt32(data.Application_ID);
                    AffiliationAdminDetail.Affiliation_Year = Convert.ToInt32(data.Year);
                    AffiliationAdminDetail.Id_year_for_affilation = Convert.ToInt32(data.Year_ID);

                    AffiliationAdminDetail.InstituteName = data.Institute_Name;
                    AffiliationAdminDetail.PrincipalName = data.Principal_Name;
                    AffiliationAdminDetail.PostelAddress = data.postal_Address;
                    AffiliationAdminDetail.Phonenumber = data.phone_No;
                    AffiliationAdminDetail.faxNumber = data.Fax;
                    AffiliationAdminDetail.reg = data.reg;
                }
                return RedirectToAction("/ApplicationAdminDetail");
            }
            catch (Exception e)
            {
                ViewBag.Exception = "There Is An Error Occured Please Contact Developers If Message Showed Again";
                return RedirectToAction("ApplicationForm", "AffiliationAdminSide");
            }
        }

        //View the Application Admin Detail Page of Check lists
        public ActionResult ApplicationAdminDetail()
        {
            return View();
        }

        //Calculating Marks on the basis of Checklist
        public ActionResult ChecklistCalculation(PrevisitProcess model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ApplicationAdminDetail", "AffiliationAdminSide");
            int marks = calculate_marks(model);
            int gainedMarks = (marks * 100) / 16;
            int id = 0;
            var result = db.BI_pop_insertion_ID_from_Affi_Admin_Appli_Form_Result();
            foreach (var res in result)
            {
                id = Convert.ToInt16(res.Value.ToString());
                break;
            };
            for (int i = 1; i <= 16; i++)
            {
                db.BI_Affi_Insert_Admin_Appli_Form_Result_dtail(Convert.ToInt16(AffiliationAdminDetail.Application_id), id, Convert.ToInt16(AffiliationAdminDetail.Inst_Id_for_Admin), i, "Get Value From Table", Serial_result[i - 1]);
            }
            db.BI_Insert_affi_Admin_Appli_Form_marks_and_remarks(Convert.ToInt16(AffiliationAdminDetail.Application_id), gainedMarks, model.remarksApp);
            if (model.isAcceptedApp)
            {
                db.BI_Affi_Insert_Admin_Appli_Form_Result(Convert.ToInt16(AffiliationAdminDetail.Inst_Id_for_Admin), Convert.ToInt16(AffiliationAdminDetail.Application_id), true, true);
                //show accepetion message
            }
            else
            {
                db.BI_Affi_Insert_Admin_Appli_Form_Result(Convert.ToInt16(AffiliationAdminDetail.Inst_Id_for_Admin), Convert.ToInt16(AffiliationAdminDetail.Application_id), false, false);

            }

            return RedirectToAction("/ApplicationForm");
        }

        //View the Affiliation Committee
        public ActionResult AffiliationCommittee()
        {
            var Asdesignation = db.Affi_com_Designation.ToList();
            var affiliationCom = new AffiliationCommitteeMember() { AsMember = Asdesignation };
            return View(affiliationCom);
        }

        //Sending the object for datatables to Affiliation Committee
        public JsonResult ACDataTableViewer()
        {
            var committeemembers = db.TA_affi_pop_Affiliation_com_member(true);
            return Json(committeemembers, JsonRequestBehavior.AllowGet);
        }

        // function for calculation of marks for application admin detail
        public int calculate_marks(PrevisitProcess model)
        {
            int count = 0;
            if (model.SubjectApplied == true)
            { Serial_result[0] = 1; count++; } 
            if (model.SubjectAlreadyAffiliated == true)
            { Serial_result[1] = 1; count++; }
            if (model.AffiliationWithBISE == true)
            { Serial_result[2] = 1; count++; }
            if (model.NOCFromGovt == true)
            { Serial_result[3] = 1; count++; }
            if (model.Prospectus == true)
            { Serial_result[4] = 1; count++; }
            if (model.Building == true)
            { Serial_result[5] = 1; count++; }
            if (model.StudentsHostel == true)
            { Serial_result[6] = 1; count++; }
            if (model.StaffResidence == true)
            { Serial_result[7] = 1; count++; }
            if (model.Finance == true)
            { Serial_result[8] = 1; count++; }
            if (model.SportFacility == true)
            { Serial_result[9] = 1; count++; }
            if (model.TeachingStaff == true)
            { Serial_result[10] = 1; count++; }
            if (model.NonTeaching == true)
            { Serial_result[11] = 1; count++; }
            if (model.Library == true)
            { Serial_result[12] = 1; count++; }
            if (model.Labortary == true)
            { Serial_result[13] = 1; count++; }
            if (model.OtherDetail == true)
            { Serial_result[14] = 1; count++; }
            if (model.DemandDraft == true)
            { Serial_result[15] = 1; count++; }
            return count;
        }

        //Add  & Edit COmmittee Member
        
        public async Task<ActionResult> CreateCommitteeMember(AffiliationCommitteeMember model)
        {
            if (!ModelState.IsValid)
            {
                var Asdesignation = db.Affi_com_Designation.ToList();
                model.AsMember = Asdesignation;
                TempData["ModelState"] = "Check The Entered Information";
                if (model.UserId == null)
                    return View("AffiliationCommittee", model);
                else if (model.UserId != null)
                    return View("EditCommitteeMember", model);
            }
            try
            {
                //var UserStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                //var UserManager = new UserManager<ApplicationUser>(UserStore);
                if (model.UserId == null)
                {
                    var User = new ApplicationUser()
                    {
                        UserName = model.UserName,
                        UserType = 11,
                        isDisable = false,
                        PhoneNumber=model.ACMember.Cell,
                        Email = model.Email
                    };
                    var result = await UserManager.CreateAsync(User, model.Password);
                    if (result.Succeeded)
                    {
                        await UserManager.AddToRoleAsync(User.Id, "Member");
                        //TempData["CreateCommitteeMember"] = model;
                        //TempData["ReturnUrl"] = "/AffiliationAdminSide/CreateCommitteeMemberAfterTwoFactorRegister";
                        //return RedirectToAction("VerifyCellNo", "Accounts", new { PhoneNumber = model.ACMember.Cell, UserId = User.Id });
                        var asdesignation = db.BI_affi_pop_affi_com_designation();
                        foreach (var member in asdesignation)
                        {
                            if (model.ACMember.com_designation_ID == member.ID)
                            {
                                model.ACMember.As_Com_Designation = member.Designation;
                            }
                        }
                        if (model.ACMember.ID == null)
                        {
                            ObjectParameter output = new ObjectParameter("UserId", typeof(Int32));
                            db.TA3_affi_Create_affiliation_com(model.ACMember.Name, model.ACMember.Designation, model.ACMember.As_Com_Designation, model.ACMember.Department, model.ACMember.CNIC, model.ACMember.Cell, model.ACMember.for_the_year, model.ACMember.com_designation_ID, output);
                            db.SaveChanges();
                            User.Institute_ID = Convert.ToInt32(output.Value);
                            var updateresult = await UserManager.UpdateAsync(User);
                            if (updateresult.Succeeded)
                            {
                                TempData["CreatedSuccessfully"] = "Committee Member Created Successfully";

                            }
                        }
                    }//result.succeeded if ended here
                    else
                    {
                        var errorstring = "";
                        foreach (var error in result.Errors)
                            errorstring = errorstring + error + " ";
                        TempData["UserCreateError"] = errorstring;
                        return RedirectToAction("AffiliationCommittee","AffiliationAdminSide");
                    }
                }//if ending

                else
                {
                    var dbuser = await UserManager.FindByIdAsync(model.UserId);
                    if (dbuser != null)
                    {
                        var newPasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                        dbuser.UserName = model.UserName;
                        dbuser.PasswordHash = newPasswordHash;
                        var result = UserManager.Update(dbuser);
                        if (result.Succeeded)
                        {
                            db.BI_affi_update_affiliation_com(model.ACMember.ID, model.ACMember.Name, model.ACMember.Designation, model.ACMember.As_Com_Designation, model.ACMember.Department, model.ACMember.CNIC, model.ACMember.Cell, model.ACMember.com_designation_ID);
                            db.SaveChanges();
                            TempData["UpdatedSuccessfully"] = "User Updated Successfully";
                            return RedirectToAction("/AffiliationCommittee");
                        }
                    }
                    else
                    {
                        TempData["UserNotExist"] = "User Cannot Be Updated";
                        return View("AffiliationCommittee", model);
                    }
                }
                return RedirectToAction("/AffiliationCommittee");
            }//try ending 
            catch (Exception e)
            {
                TempData["Exception"] = "There Is Error In Updating Or Creating Contact Developer If It Shows Again";
                return RedirectToAction("/AffiliationCommittee");
            }
        }

        //public async Task<ActionResult> CreateCommitteeMemberAfterTwoFactorRegister()
        //{
        //    AffiliationCommitteeMember member = (AffiliationCommitteeMember)TempData["CreateCommitteeMember"];
        //    if (member != null)
        //    {
        //        var asdesignation = db.BI_affi_pop_affi_com_designation();
        //        foreach (var obj in asdesignation)
        //        {
        //            if (member.ACMember.com_designation_ID == obj.ID)
        //            {
        //                member.ACMember.As_Com_Designation = obj.Designation;
        //            }
        //        }
        //        if (member.ACMember.ID == null)
        //        {
        //            ObjectParameter output = new ObjectParameter("UserId", typeof(Int32));
        //            db.TA3_affi_Create_affiliation_com(member.ACMember.Name, member.ACMember.Designation, member.ACMember.As_Com_Designation, member.ACMember.Department, member.ACMember.CNIC, member.ACMember.Cell, member.ACMember.for_the_year, member.ACMember.com_designation_ID, output);
        //            db.SaveChanges();
        //            var userid = TempData["RegisteredUserId"].ToString();
        //            var User = await UserManager.FindByIdAsync(userid);
        //            User.Institute_ID = Convert.ToInt32(output.Value);
        //            var updateresult = await UserManager.UpdateAsync(User);
        //            if (updateresult.Succeeded)
        //            {
        //                TempData["CreatedSuccessfully"] = "Committee Member Created Successfully";
        //            }
        //        } 
        //    }
        //    return RedirectToAction("AffiliationCommittee", "AffiliationAdminSide");
        //}

        // View Edit Committee Member

        public ActionResult EditCommitteeMember(string id)
        {
            string[] userId = id.Split(',');
            var UserStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var UserManager = new UserManager<ApplicationUser>(UserStore);
            var dbUser=UserManager.FindById(userId[1]);
            if (dbUser != null)
            {
                var member = db.TA_affi_pop_Affiliation_com_member_By_Id(Convert.ToInt32(userId[0]), false);
                AffiliationCommitteeMember ACM = new AffiliationCommitteeMember();
                ACM.UserName = dbUser.UserName;
                ACM.UserId = userId[1];
                foreach (var data in member)
                {
                    ACM.ACMember = new Affi_Com_Members();
                    ACM.ACMember.Name = data.Name;
                    ACM.ACMember.ID = data.ID;
                    ACM.ACMember.CNIC = data.CNIC;
                    ACM.ACMember.Designation = data.Designation;
                    ACM.ACMember.Department = data.Department;
                    ACM.ACMember.Cell = data.Cell;
                }
                var asdesignation = db.Affi_com_Designation.ToList();
                ACM.AsMember = asdesignation;
                return View(ACM);
            }
            else
                return RedirectToAction("/AffiliationCommittee");
            
        }

        //InActive AffiliationCommittee Member
        public ActionResult InactiveMember(string id)
        {
            string[] userId = id.Split(',');
            db.BI_affi_inactive_affiliation_com_member(Convert.ToInt32(userId[0]), true);
            ViewBag.SuceessMessage = "Member Deactivated Successfully";
            return RedirectToAction("/AffiliationCommittee");
        }

        //Active AffiliationCommittee Member
        public ActionResult ActiveMember(string id)
        {
            string[] userId = id.Split(',');
            db.BI_affi_inactive_affiliation_com_member(Convert.ToInt32(userId[0]), false);
            ViewBag.SuceessMessage = "Member Activated Successfully";
            return RedirectToAction("/AffiliationCommittee");
        }

        //View Recomended Visit Page
        public ActionResult RecommendedVisit()
        {
            //RecommendedVisitList.RCVList = null;
            var dbVisit = db.affi_ins_pop_pending_visit_applications();
            return View();
        }

        //Returning Object Of Pending Visit Applicaitions For DataTable
        public JsonResult RVDataTableViewer()
        {
            var dbVisit = db.affi_ins_pop_pending_visit_applications();
            return Json(dbVisit, JsonRequestBehavior.AllowGet);
        }

        //Reciving Applicaiton , Institute ,Year Ids And Visit Date From Recommended Visit
        public ActionResult RVGetSelClgIds(TATempAssignedIds ids)
        {

            if (DateTime.Compare(ids.VisitDate, DateTime.Now) > 0)
            {
                if (ids.ApplicatoinId == null || ids.InstituteId == null || ids.YearId == null)
                {
                    return Json(new { successs = true, responseText = "Please Select At Least One Application" }, JsonRequestBehavior.AllowGet);
                }
                //RecommendedVisitList rvlist = new RecommendedVisitList();
                //foreach (var list  in RecommendedVisitList.RCVList)
                //{
                //    for(var i=0;i<ids.InstituteId.Count;i++)
                //        if(list.InstituteId==ids.InstituteId[i])
                //            return Json(new {success=false,responseText="You Have Already Checked The Application"},JsonRequestBehavior.AllowGet);

                //}
                for (var i = 0; i < ids.InstituteId.Count; i++)
                {
                    RecommendedVisitClass rcv = new RecommendedVisitClass();
                    rcv.ApplicaitonId = Convert.ToInt32(ids.ApplicatoinId[i]);
                    rcv.InstituteId = Convert.ToInt32(ids.InstituteId[i]);
                    rcv.YearId = Convert.ToInt32(ids.YearId[i]);
                    rcv.IsCheck = true;
                    rcv.VisitDate = ids.VisitDate;
                    RecommendedVisitList.RCVList.Add(rcv);
                }
                return Json(new { success = true, responseText = "../AffiliationAdminSide/RecommendedCommittee" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, responseText = "Choose The Date After Today" }, JsonRequestBehavior.AllowGet);
            }
        }

        //View Of Selection of Recommended Committee Members For Assigning Visit
        public ActionResult RecommendedCommittee()
        {
            if (RecommendedVisitList.RCVList.Count <1)
            {
                return RedirectToAction("RecommendedVisit", "AffiliationAdminSide");
            }
            return View();
        }

        //Returning Object of AffiliationCommittee Members for DataTable
        public JsonResult RCommitteeDataTableViewer()
        {
            var dbMembers = db.TA_affi_pop_Affiliation_com_member(true);
            return Json(dbMembers,JsonRequestBehavior.AllowGet);
        }

        //Reciving Recommended Members Ids From Recommended Committee and Manage Visit In DB
        public ActionResult RCGetSelMembrIds(TATempAssignMIds ids)
        {
            var dbVisitId = db.BI_fatch_visit_max_id();
             var visitId=0;
             foreach (var id in dbVisitId)
                 visitId = id.Value;
            foreach (var list in RecommendedVisitList.RCVList)
            {
                if (list.IsCheck == true)
                {
                    for (var i = 0; i < ids.MemberIds.Count; i++)
                    {
                        db.BI_ManageVisit(visitId, list.InstituteId, list.ApplicaitonId, ids.MemberIds[i], list.VisitDate);
                    }
                }
            }
            db.SaveChanges();
            RecommendedVisitList.RCVList.Clear();
           // return RedirectToAction("AffiliationAdminSide","ApplicationForm");
            return Json(new { success = true, responseText = "Grant Successfully" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AffRptViewer(string Rept)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            if (Rept == "A")
            {
                var DBvlu = db.AJ_Aff_Ins_Apply_Rep_Declaration(Convert.ToInt32( AffiliationAdminDetail.Inst_Id_for_Admin));
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Ins_Apply_Rep_Declaration.rdlc"); ;
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Aff_Ins_Apply_Rep_Declaration", DBvlu));
            }
            else if (Rept == "F1")
            {
                var DBvlu = db.AJ_POP_Bulding_INFO(Convert.ToInt32(AffiliationAdminDetail.Inst_Id_for_Admin));
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_BuldingDetails.rdlc"); 
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_POP_Bulding_INFO", DBvlu));
            }
            else if (Rept == "F2")
            {
                var DBvlu = db.AJ_Rep_Non_Recuring_INFO(Convert.ToInt32(AffiliationAdminDetail.Inst_Id_for_Admin));
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_ins_Non_Recuring_INFO.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Rep_Non_Recuring_INFO", DBvlu));
            }
            else if (Rept == "F3")
            {
                var DBvlu = db.AJ_Rep_Recuring_INFO(Convert.ToInt32(AffiliationAdminDetail.Inst_Id_for_Admin));
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_ins_Recuring_INFO.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Rep_Recuring_INFO", DBvlu));
            }
            else if (Rept == "F4")
            {
                var DBvlu = db.AJ_Rep_SportFacilities_INFO(Convert.ToInt32(AffiliationAdminDetail.Inst_Id_for_Admin));
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_SportFculty.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Rep_SportFacilities_INFO", DBvlu));
            }
            else if (Rept == "F5")
            {
                var DBvlu = db.AJ_Rep_Employee_INFO(Convert.ToInt32(AffiliationAdminDetail.Inst_Id_for_Admin), 0);
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_Staff_Info.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Rep_Employee_INFO", DBvlu));
            }
            else if (Rept == "F6")
            {
                var DBvlu = db.AJ_Rep_Employee_INFO(Convert.ToInt32(AffiliationAdminDetail.Inst_Id_for_Admin), 1);
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_Staff_Info.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Rep_Employee_INFO", DBvlu));
            }
            else if (Rept == "F7")
            {
                var DBvlu1 = db.AJ_Rep_Library_INFO(Convert.ToInt32(AffiliationAdminDetail.Inst_Id_for_Admin));
                var DBvlu2 = db.AJ_Affi_ins_Rep_OtherFacilities_INFO(Convert.ToInt32(Models.obj.Institute_Id));
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_Library_INFO.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Rep_Library_INFO", DBvlu1));
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Affi_ins_Rep_OtherFacilities_INFO", DBvlu2));
            }
            else if (Rept == "F8")
            {
                var DBvlu = db.AJ_Rep_Laboratory_INFO(Convert.ToInt32(AffiliationAdminDetail.Inst_Id_for_Admin));
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_inst_AJ_Rep_Laboratory_INFO.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Rep_Laboratory_INFO", DBvlu));
            }
            else if (Rept == "BLC")
            {
                var DBvlu = db.Aff_Ins_Rpt_Balance_Rec();
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Ins_Rpt_Balance_Rec.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Aff_Ins_Rpt_Balance_Rec", DBvlu));
            }
            ViewBag.ReportViewer = reportViewer;
            return View();
        }


        //register new institute
        public ActionResult Register()
        {
            var distlist = db.AJ_Pop_districts_cmb_List(2);
            List<AJ_Pop_districts_cmb_List_Result> list = new List<AJ_Pop_districts_cmb_List_Result>();
            AJ_Pop_districts_cmb_List_Result res = new AJ_Pop_districts_cmb_List_Result();
            res.ID = 0;
            res.Dist_Name = "--Select Distt--";
            list.Add(res);
            foreach (var dist in distlist)
            {
                AJ_Pop_districts_cmb_List_Result resl = new AJ_Pop_districts_cmb_List_Result();
                resl.ID = dist.ID;
                resl.Dist_Name = dist.Dist_Name;
                list.Add(resl);
            }


            List<AJ_Pop_districts_Tehsils_cmb_List_Result> tehlist = new List<AJ_Pop_districts_Tehsils_cmb_List_Result>();
            AJ_Pop_districts_Tehsils_cmb_List_Result tehsil = new AJ_Pop_districts_Tehsils_cmb_List_Result();
            tehsil.ID = 0;
            tehsil.Tehsils = "--Select Tehsil--";

            tehlist.Add(tehsil);
            AffiInsRegisteration model = new AffiInsRegisteration();
            model.DistrictDD = list;
            model.TehsilDD = tehlist;
            return View(model);
        }

        public JsonResult selectteshil(int value)
        {
            var tehsillist = db.AJ_Pop_districts_Tehsils_cmb_List(2, value);
           
            return Json(tehsillist, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> Register(AffiInsRegisteration model)
        {
            ObjectParameter output = new ObjectParameter("UserId", typeof(Int32));

            var UserStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var UserManager = new UserManager<ApplicationUser>(UserStore);
            var dbuser = await UserManager.FindByNameAsync(model.UserName);
            var dbinst = db.Affil_Ins_Info.SingleOrDefault(m => m.Ins_Name == model.InstName);
            var dbinstCode = db.Affil_Ins_Info.SingleOrDefault(m => m.Inst_code == model.InstCode);

            if (dbinstCode != null)
            {
                TempData["Error"] = "InstituteCode Already Exists";
                return RedirectToAction("Register", "AffiliationAdminSide");
            }
            if (dbinst != null)
            {
                TempData["Error"] = "InstituteName Already Exists";
                return RedirectToAction("Register", "AffiliationAdminSide");
            }
            if (dbuser != null)
            {
                TempData["Error"] = "UserName Already Exists";
                return RedirectToAction("Register", "AffiliationAdminSide");
            }

            var User = new ApplicationUser()
            {
                UserName = model.UserName,
                Email = model.Email,
                UserType = 7,
                isDisable = false

            };
            IdentityResult result = await UserManager.CreateAsync(User, model.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(User.Id, "User");
                bool isgov = false;
                if (model.InstituteType == "Government")
                    isgov = true;
                db.AJ_Affi_SinUp_User_Create(model.InstName, Convert.ToInt32(model.InstCode), Convert.ToString(model.PostalAddress), Convert.ToString(model.PTCL), Convert.ToString(model.OfficMoble), Convert.ToString(model.PrincipalName), Convert.ToString(model.PrincipalContact), isgov, Convert.ToInt32(model.DistrictId), Convert.ToInt32(model.TehsilId), Convert.ToString(model.Email), output);
                int instid = Convert.ToInt32(output.Value);
                User.Institute_ID = instid;
                await UserManager.UpdateAsync(User);
                TempData["Success"] = "User Created Successfully";
            }
            else
            {
                var msg = "";
                foreach (var error in result.Errors)
                {
                    msg = msg + " " + error;
                }
                TempData["Error"] = msg;
                return RedirectToAction("Register", "AffiliationAdminSide");
            }


            return RedirectToAction("Register", "AffiliationAdminSide");
        }


      }