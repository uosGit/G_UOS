using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UOS.Models;
using UOS.Models.UserClassesReg;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using UOS.App_Start;
using System.IO;
using UOS.Models.UserClasses;
using System.Web.Script.Serialization;
using UOS.Models.Registration;
using UOS.Models.UserClassesReg;
using UOS.Models.Common;
using System.Threading.Tasks;
using Microsoft.Reporting.WebForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Script.Serialization;


namespace UOS.Controllers.Registration
{
    
    [Authorize(Roles = "User")]
    public class RegistrationController : Controller
    {
//UOSEntities db;
        SessionObject obj;
        SessionRegistration regsession;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public RegistrationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
          //  db = new UOSEntities();
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

        public RegistrationController()
        {
          //  db = new UOSEntities();
        }

        public ActionResult StudentBasicInfo()
        {
            regsession = GetRegSessions();
            string[] arraygender = new string[3] { "Male", "Female", "Other" };
            List<Gender> listgender = new List<Gender>();
            for (var i = 0; i < 3; i++)
            {
                Gender gender = new Gender() { Name = arraygender[i] };
                listgender.Add(gender);
            }


            string[] arraymeritalstatus = new string[3] { "Married", "Unmarried", "Divorced" };
            List<MeritalStatus> listmeritalstatus = new List<MeritalStatus>();
            for (var i = 0; i < 3; i++)
            {
                MeritalStatus meritalstatus = new MeritalStatus() { Name = arraymeritalstatus[i] };
                listmeritalstatus.Add(meritalstatus);
            }
            using (UOSEntities db = new UOSEntities())
            {
                var countries = db.BI_POP_ALL_COUNTRIES().ToList();

                //Province
                List<BI_POP_ALL_PROVIENCES_Result> listprovinces = new List<BI_POP_ALL_PROVIENCES_Result>();
                BI_POP_ALL_PROVIENCES_Result province = new BI_POP_ALL_PROVIENCES_Result() { Province_name = "--Select--" };
                listprovinces.Add(province);

                //District
                List<BI_POP_ALL_DISTT_WRT_PROVIENCE_Result> listdistricts = new List<BI_POP_ALL_DISTT_WRT_PROVIENCE_Result>();
                BI_POP_ALL_DISTT_WRT_PROVIENCE_Result district = new BI_POP_ALL_DISTT_WRT_PROVIENCE_Result() { Dist_Name = "--Select--" };
                listdistricts.Add(district);

                //Tehsil
                List<BI_POP_ALL_TEHSIL_WRT_DIST_Result> listtehsils = new List<BI_POP_ALL_TEHSIL_WRT_DIST_Result>();
                BI_POP_ALL_TEHSIL_WRT_DIST_Result tehsil = new BI_POP_ALL_TEHSIL_WRT_DIST_Result() { Tehsil = "--Select--" };
                listtehsils.Add(tehsil);

                regsession = GetRegSessions();
                obj = GetData();

                int YearID = Convert.ToInt32(Session["year_id"]);
                var vlu = db.TA_pop_granted_programs_wrt_Program(Convert.ToInt32(obj.Institute_Id), Convert.ToInt32(regsession.category), YearID, Convert.ToInt32(regsession.discipline)).ToList();
                string ProgramNAme = null;
                string ProgramCate = null;
                int Seat = 0;
                foreach (var item in vlu)
                {
                    ProgramNAme = item.Program_Desc;
                    ProgramCate = item.Cat;
                    Seat = Convert.ToInt32(item.Granted_Seats);
                }


                StudentBasicDetails model = new StudentBasicDetails()
                {

                    GenderDD = listgender,
                    m_statusDD = listmeritalstatus,
                    CountryDD = countries,
                    ProvinceDD = listprovinces,
                    DistrictDD = listdistricts,
                    TehsilDD = listtehsils,
                    Per_countryDD = countries,
                    Per_provinceDD = listprovinces,
                    Per_districtDD = listdistricts,
                    Per_tehsilDD = listtehsils,
                    AppId = regsession.AppId,
                    Category = regsession.category,
                    CategoryName = ProgramCate,
                    GrantedSeats = regsession.GrantedSeats,
                    Discipline = regsession.discipline,
                    DisciplineName = ProgramNAme

                };

                return View(model);
            }
        }

        //for incomplete students application
        public ActionResult ProcessPendingApp(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {

                var Std_App_dt = from dbo in db.stu_applied_reg_f_affi_clg where (dbo.id == id) select dbo;
                int stu_ID = 0;

                foreach (var item in Std_App_dt)
                {
                    stu_ID = Convert.ToInt32(item.st_id);
                }
                Session["Std_App_ID"] = id;
                var status = db.stu_get_info_status(stu_ID).ToList();
                //SessionRegistration obj = new SessionRegistration()
                //{
                //    AppId = model.AppId,
                //    GrantedSeats = model.GrantedSeats,
                //    discipline = model.discipline,
                //    category = model.category
                //};


                regsession = GetRegSessions();
                obj = GetData();

                foreach (var row in status)
                {
                    if (row.inst_id == Convert.ToInt32(obj.Institute_Id) && row.st_id == stu_ID)
                    {
                        regsession.StudentId = stu_ID;
                        var claims = UserManager.GetClaims(User.Identity.GetUserId());
                        foreach (var claim in claims)
                        {
                            if (claim.Type == "StudentId")
                            {
                                UserManager.RemoveClaim(User.Identity.GetUserId(), claim);
                            }
                        }
                        UserManager.AddClaim(User.Identity.GetUserId(), new Claim("StudentId", regsession.StudentId.ToString()));
                        if (row.is_gardian_info == false)
                        {
                            return RedirectToAction("StuGuardianDetails", "Registration");
                        }
                        else if (row.is_matric_info == false)
                        {
                            return RedirectToAction("StuMetricDetails", "Registration");
                        }

                        else if (row.is_inter_info == false)
                        {
                            return RedirectToAction("StuInterDetails", "Registration");
                        }
                        else if (row.is_bechelor_info == false)
                        {
                            return RedirectToAction("StuBachelorsDetails", "Registration");
                        }
                        else if (row.is_document_attached == false)
                        {
                            return RedirectToAction("StuAttachDocument", "Registration");
                        }
                        else if (row.is_complete == false)
                        {
                            return RedirectToAction("StuAffiliatedClg", "Registration");
                        }
                        else if (row.is_complete == true)
                        {
                            return RedirectToAction("stuApplicationFormComplete", "Registration");
                        }
                    }
                }
                return RedirectToAction("Granted_cources", "Registration");
            }
        }

        public StudentBasicDetails DDLReturner()
        {
            using (UOSEntities db = new UOSEntities())
            {
                string[] arraygender = new string[3] { "Male", "FeMale", "Other" };
                List<Gender> listgender = new List<Gender>();
                for (var i = 0; i < 3; i++)
                {
                    Gender gender = new Gender() { Name = arraygender[i] };
                    listgender.Add(gender);
                }
                string[] arraymeritalstatus = new string[3] { "Married", "UnMarried", "Divorced" };
                List<MeritalStatus> listmeritalstatus = new List<MeritalStatus>();
                for (var i = 0; i < 3; i++)
                {
                    MeritalStatus meritalstatus = new MeritalStatus() { Name = arraymeritalstatus[i] };
                    listmeritalstatus.Add(meritalstatus);
                }
                var countries = db.BI_POP_ALL_COUNTRIES().ToList();
                //Province
                List<BI_POP_ALL_PROVIENCES_Result> listprovinces = new List<BI_POP_ALL_PROVIENCES_Result>();
                BI_POP_ALL_PROVIENCES_Result province = new BI_POP_ALL_PROVIENCES_Result() { Province_name = "Select Province" };
                listprovinces.Add(province);

                //District
                List<BI_POP_ALL_DISTT_WRT_PROVIENCE_Result> listdistricts = new List<BI_POP_ALL_DISTT_WRT_PROVIENCE_Result>();
                BI_POP_ALL_DISTT_WRT_PROVIENCE_Result district = new BI_POP_ALL_DISTT_WRT_PROVIENCE_Result() { Dist_Name = "Select District" };
                listdistricts.Add(district);

                //Tehsil
                List<BI_POP_ALL_TEHSIL_WRT_DIST_Result> listtehsils = new List<BI_POP_ALL_TEHSIL_WRT_DIST_Result>();
                BI_POP_ALL_TEHSIL_WRT_DIST_Result tehsil = new BI_POP_ALL_TEHSIL_WRT_DIST_Result() { Tehsil = "Select Tehsil" };
                listtehsils.Add(tehsil);

                StudentBasicDetails model = new StudentBasicDetails()
                {
                    GenderDD = listgender,
                    m_statusDD = listmeritalstatus,
                    CountryDD = countries,
                    ProvinceDD = listprovinces,
                    DistrictDD = listdistricts,
                    TehsilDD = listtehsils,
                    Per_countryDD = countries,
                    Per_provinceDD = listprovinces,
                    Per_districtDD = listdistricts,
                    Per_tehsilDD = listtehsils
                };
                return model;
            }
        }

        [HttpPost]// work pending
        public ActionResult StudentBasicInfo(StudentBasicDetails model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                regsession = GetRegSessions();
                if (!ModelState.IsValid)
                {

                    var returner = DDLReturner();
                    model.GenderDD = returner.GenderDD;
                    model.m_statusDD = returner.m_statusDD;
                    model.CountryDD = returner.CountryDD;
                    model.ProvinceDD = returner.ProvinceDD;
                    model.DistrictDD = returner.DistrictDD;
                    model.TehsilDD = returner.TehsilDD;
                    model.Per_countryDD = returner.Per_countryDD;
                    model.Per_provinceDD = returner.Per_provinceDD;
                    model.Per_districtDD = returner.Per_districtDD;
                    model.Per_tehsilDD = returner.Per_tehsilDD;
                    return View(model);
                }

                if (model.imageUpload == null || model.imageUpload.ContentLength <= 0)
                {
                    ViewBag.DDImageNull = "Please Choose the Image Of Yourself";
                    var returner = DDLReturner();
                    model.GenderDD = returner.GenderDD;
                    model.m_statusDD = returner.m_statusDD;
                    model.CountryDD = returner.CountryDD;
                    model.ProvinceDD = returner.ProvinceDD;
                    model.DistrictDD = returner.DistrictDD;
                    model.TehsilDD = returner.TehsilDD;
                    model.Per_countryDD = returner.Per_countryDD;
                    model.Per_provinceDD = returner.Per_provinceDD;
                    model.Per_districtDD = returner.Per_districtDD;
                    model.Per_tehsilDD = returner.Per_tehsilDD;
                    return View(model);
                }
                var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
                var extension = Path.GetExtension(model.imageUpload.FileName);
                if (!allowedExtensions.Contains(extension.ToLower()))
                {
                    ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                    var returner = DDLReturner();
                    model.GenderDD = returner.GenderDD;
                    model.m_statusDD = returner.m_statusDD;
                    model.CountryDD = returner.CountryDD;
                    model.ProvinceDD = returner.ProvinceDD;
                    model.DistrictDD = returner.DistrictDD;
                    model.TehsilDD = returner.TehsilDD;
                    model.Per_countryDD = returner.Per_countryDD;
                    model.Per_provinceDD = returner.Per_provinceDD;
                    model.Per_districtDD = returner.Per_districtDD;
                    model.Per_tehsilDD = returner.Per_tehsilDD;
                    return View(model);
                }
                if (model.imageUpload.ContentLength > (1024 * 300))
                {
                    ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                    var returner = DDLReturner();
                    model.GenderDD = returner.GenderDD;
                    model.m_statusDD = returner.m_statusDD;
                    model.CountryDD = returner.CountryDD;
                    model.ProvinceDD = returner.ProvinceDD;
                    model.DistrictDD = returner.DistrictDD;
                    model.TehsilDD = returner.TehsilDD;
                    model.Per_countryDD = returner.Per_countryDD;
                    model.Per_provinceDD = returner.Per_provinceDD;
                    model.Per_districtDD = returner.Per_districtDD;
                    model.Per_tehsilDD = returner.Per_tehsilDD;
                    return View(model);
                }
                string FileName = Path.GetFileNameWithoutExtension(model.imageUpload.FileName);

                //Converting into Bytes
                try
                {
                    Stream fs = model.imageUpload.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    model.Draft_Scan = br.ReadBytes((Int32)fs.Length);
                    model.FileName = Path.GetFileName(model.imageUpload.FileName);
                    if (model.Id == 0)
                    {
                        var qry_resut = db.BI_REG_INSERT_STU_BASIC_INFO(Convert.ToInt32(Convert.ToInt32(regsession.discipline)), regsession.AppId, Convert.ToInt32(obj.Institute_Id), model.regNumber, model.stu_name, model.stu_Father, model.cnic, model.cell_no, model.email, model.pob, Convert.ToDateTime(Convert.ToDateTime(model.dob).ToString("yyyy-MM-dd")), model.gender, model.m_status, model.religion, model.nationality, model.b_group, model.address, Convert.ToInt32(model.country), Convert.ToInt32(model.province), Convert.ToInt32(model.district), Convert.ToInt32(model.tehsil), model.address, Convert.ToInt32(model.per_country), Convert.ToInt32(model.per_province), Convert.ToInt32(model.per_district), Convert.ToInt32(model.per_tehsil), model.Draft_Scan, model.FileName, regsession.category);
                        foreach (var item in qry_resut)
                        {
                            if (Convert.ToInt32(item.Value) > 0)
                            {
                                regsession.StudentId = Convert.ToInt32(item.Value);
                                UserManager.AddClaim(User.Identity.GetUserId(), new Claim("StudentId", regsession.StudentId.ToString()));
                                TempData["Success"] = "Basic Details Added Successfully";
                                return RedirectToAction("StuGuardianDetails", "Registration");
                            }
                        }
                        //process pending already registerd student..!!

                    }
                    else if (model.Id != 0)
                    {
                        //store procedure of update
                    }
                    TempData["Exception"] = "This CNIC is Already Registerd ...redirec to update and view page is pending...!!";
                    return RedirectToAction("StudentBasicInfo", "Registration");


                }
                catch (Exception e)
                {
                    TempData["Exception"] = "Error In Insertion Please Try Again  " + e.Message;
                    return RedirectToAction("StudentBasicInfo", "Registration");
                }
            }
        }

        public ActionResult StuGuardianDetails()
        {
            regsession = GetRegSessions();
            if (Convert.ToInt32(regsession.StudentId) == 0)
            {
                return RedirectToAction("Granted_cources", "Registration");
            }
            else
            {
                string s = CheckApplicationStatus(regsession.StudentId);
                if (s == "OK")
                    return RedirectToAction("stuApplicationFormComplete", "Registration");
                else if (s == "StuGuardianDetails")
                    return View();
                else
                    return RedirectToAction(s, "Registration");
            }
        }

        [HttpPost]
        public ActionResult StuGuardianDetails(GuardianDetails model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                try
                {
                    obj = GetData();
                    regsession = GetRegSessions();
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }
                    var dbValu = (from dbo in db.stu_applied_reg_f_affi_clg where (dbo.st_id == regsession.StudentId) select dbo).ToList();
                    foreach (var item in dbValu)
                    {
                        Session["Std_App_ID"] = item.id;
                    }
                    string isgovt = "Govt";
                    if (model.is_gov != true) { isgovt = "Private"; }
                    var query_result = db.I_REG_INSERT_STU_GARDIAN_INFO(Convert.ToInt32(obj.Institute_Id), Convert.ToInt32(regsession.StudentId), model.g_name, model.g_cnic, model.g_cell_no, model.relationship, model.eductn, model.occpatn, isgovt, Convert.ToInt32(model.income));
                    foreach (var item in query_result)
                    {
                        if (item.Value == 0)
                        {
                            TempData["Exception"] = "Basic Info Not Found , Enter The Basic Info First....!";
                            return RedirectToAction("StudentBasicInfo", "Registration");
                        }
                        else if (item.Value == 1)
                        {
                            TempData["Success"] = "Guardian Details Added Successfully..!";
                            return RedirectToAction("StuMetricDetails", "Registration");
                        }
                    }
                    TempData["Exception"] = "Error In Insertion Gardian Deatil Please try Again.....!";
                    return RedirectToAction("StuGuardianDetails", "Registration");
                }
                catch (Exception exp)
                {
                    TempData["Exception"] = "Error In Insertion Gardian Deatil Please try Again.....!";
                    return RedirectToAction("StuGuardianDetails", "Registration");
                }
            }
        }

        public ActionResult StuMetricDetails()
        {
            using (UOSEntities db = new UOSEntities())
            {
                regsession = GetRegSessions();
                if (Convert.ToInt32(regsession.StudentId) == 0)
                {
                    return RedirectToAction("Granted_cources", "Registration");
                }
                else
                {
                    string s = CheckApplicationStatus(regsession.StudentId);
                    if (s == "OK")
                        return RedirectToAction("stuApplicationFormComplete", "Registration");
                    else if (s == "StuMetricDetails")
                    {
                        // popuating all boards
                        MetricDetails objmodl = new MetricDetails();
                        List<InterBoards> objList = new List<InterBoards>();
                        List<YearPopulate> objList_year = new List<YearPopulate>();
                        var rasult = db.pop_inter_boards().ToList();
                        foreach (var item in rasult)
                        {
                            InterBoards objb = new InterBoards();
                            objb.B_ID = Convert.ToInt32(item.ID);
                            objb.Board_Name = item.Board_Name;
                            objList.Add(objb);
                        }

                        // populating all years 
                        for (int i = 2; i <= 10; i++)
                        {
                            YearPopulate objb1 = new YearPopulate();
                            objb1.Year = DateTime.Now.AddYears(-i).Year.ToString();
                            objb1.Id = DateTime.Now.AddYears(-i).Year;
                            objList_year.Add(objb1);

                        }
                        objmodl.ddl_obj_years = objList_year;
                        objmodl.ddl_obj_intr_boad = objList;
                        return View(objmodl);
                    }
                    else
                        return RedirectToAction(s, "Registration");
                }
            }
        }

        public MetricDetails DDFillerForMetric()
        {
            using (UOSEntities db = new UOSEntities())
            {
                MetricDetails objmodl = new MetricDetails();
                List<InterBoards> objList = new List<InterBoards>();
                List<YearPopulate> objList_year = new List<YearPopulate>();
                var rasult = db.pop_inter_boards().ToList();
                foreach (var item in rasult)
                {
                    InterBoards objb = new InterBoards();
                    objb.B_ID = Convert.ToInt32(item.ID);
                    objb.Board_Name = item.Board_Name;
                    objList.Add(objb);
                }

                // populating all years 
                for (int i = 2; i <= 10; i++)
                {
                    YearPopulate objb1 = new YearPopulate();
                    objb1.Year = DateTime.Now.AddYears(-i).Year.ToString();
                    objb1.Id = DateTime.Now.AddYears(-i).Year;
                    objList_year.Add(objb1);

                }
                objmodl.ddl_obj_years = objList_year;
                objmodl.ddl_obj_intr_boad = objList;
                return objmodl;
            }
        }

        [HttpPost]
        public ActionResult StuMetricDetails(MetricDetails model)
        {
            if (!ModelState.IsValid)
            {
                MetricDetails mod = new MetricDetails();
                mod = DDFillerForMetric();
                return View(mod);
            }
            if (model.ImageFile == null || model.ImageFile.ContentLength <= 0)
            {
                ViewBag.DDImageNull = "Please Choose the Image Of Metric Result Card";
                MetricDetails mod = new MetricDetails();
                mod = DDFillerForMetric();
                return View(mod);
            }
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                MetricDetails mod = new MetricDetails();
                mod = DDFillerForMetric();
                return View(mod);
            }
            if (model.ImageFile.ContentLength > (1024 * 300))
            {
                ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                MetricDetails mod = new MetricDetails();
                mod = DDFillerForMetric();
                return View(mod);
            }
            string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);

            //Converting into Bytes
            try
            {
                using (UOSEntities db = new UOSEntities())
                {
                    Stream fs = model.ImageFile.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    model.matric_scan = br.ReadBytes((Int32)fs.Length);
                    model.FileName = Path.GetFileName(model.ImageFile.FileName);

                    if (model.Id == 0)
                    {
                        obj = GetData();
                        regsession = GetRegSessions();
                        var result = db.BI_REG_INSERT_STU_SSC_INFO(regsession.StudentId, model.degree, model.subjects, Convert.ToInt32(model.board), Convert.ToInt32(model.p_year), model.exam_type, model.roll_no, model.t_marks, model.o_marks, model.division, model.matric_scan, model.FileName).ToList();
                        foreach (var item in result)
                        {
                            if (item.Value == 0)
                            {
                                TempData["Exception"] = "Basic Info Or Other Required Data Not Found , Enter The Basic Info First....!";
                                return RedirectToAction("StudentBasicInfo", "Registration");
                            }
                            else if (item.Value == 1)
                            {
                                TempData["Success"] = "SSC Details Added Successfully..!";
                                return RedirectToAction("StuInterDetails", "Registration");
                            }

                        }
                        TempData["Success"] = "Metric Details Inserted Successfully";
                    }
                    else if (model.Id != 0)
                    {
                        //store procedure of update
                    }

                    return RedirectToAction("StuInterDetails", "Registration");
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion Please Try Again";
                return RedirectToAction("StuMetricDetails", "Registration");
            }

        }

        public ActionResult StuInterDetails()
        {
            regsession = GetRegSessions();
            if (Convert.ToInt32(regsession.StudentId) == 0)
            {
                return RedirectToAction("Granted_cources", "Registration");
            }
            else
            {
                string s = CheckApplicationStatus(regsession.StudentId);
                if (s == "OK")
                    return RedirectToAction("stuApplicationFormComplete", "Registration");
                else if (s == "StuInterDetails")
                {
                    using (UOSEntities db = new UOSEntities())
                    {
                        // popuating all boards
                        IntermediateDetails objmodl = new IntermediateDetails();
                        List<InterBoards> objList = new List<InterBoards>();
                        List<YearPopulate> objList_year = new List<YearPopulate>();
                        var rasult = db.pop_inter_boards().ToList();
                        foreach (var item in rasult)
                        {
                            InterBoards objb = new InterBoards();
                            objb.B_ID = Convert.ToInt32(item.ID);
                            objb.Board_Name = item.Board_Name;
                            objList.Add(objb);
                        }
                        //Pop Inter degery
                        var DegreeName = db.Intermediate_Programs.Where(x => x.Level_ID == 2).ToList();
                        objmodl.Inter_DD = DegreeName.Select(x => new Intermediate_Programs { ID = x.ID, Inter_Program = x.Inter_Program }).ToList();

                        //Pop Subject1
                        var subject1 = db.Affi_ins_all_Subject_lab_desc.ToList();
                        objmodl.i_subjects1_DD = subject1.Select(x => new Affi_ins_all_Subject_lab_desc { Id = x.Id, Lab_Desc = x.Lab_Desc }).ToList();

                        //Pop Subject2
                        var subject2 = db.Affi_ins_all_Subject_lab_desc.ToList();
                        objmodl.i_subjects2_DD = subject2.Select(x => new Affi_ins_all_Subject_lab_desc { Id = x.Id, Lab_Desc = x.Lab_Desc }).ToList();

                        //Pop Subject3
                        var subject3 = db.Affi_ins_all_Subject_lab_desc.ToList();
                        objmodl.i_subjects3_DD = subject3.Select(x => new Affi_ins_all_Subject_lab_desc { Id = x.Id, Lab_Desc = x.Lab_Desc }).ToList();

                        // populating all years 
                        for (int i = 0; i <= 10; i++)
                        {
                            YearPopulate objb1 = new YearPopulate();
                            objb1.Year = DateTime.Now.AddYears(-i).Year.ToString();
                            objb1.Id = DateTime.Now.AddYears(-i).Year;
                            objList_year.Add(objb1);

                        }
                        objmodl.ddl_obj_years = objList_year;
                        objmodl.ddl_obj_intr_boad = objList;
                        return View(objmodl);
                    }
                        }
                else
                    return RedirectToAction(s, "Registration");
            }
        }

        public IntermediateDetails DDLFILLERINTERMEDIATE()
        {
            using (UOSEntities db = new UOSEntities())
            {
                // popuating all boards
                IntermediateDetails objmodl = new IntermediateDetails();
                List<InterBoards> objList = new List<InterBoards>();
                List<YearPopulate> objList_year = new List<YearPopulate>();
                var rasult = db.pop_inter_boards().ToList();
                foreach (var item in rasult)
                {
                    InterBoards objb = new InterBoards();
                    objb.B_ID = Convert.ToInt32(item.ID);
                    objb.Board_Name = item.Board_Name;
                    objList.Add(objb);
                }

                // populating all years 
                for (int i = 2; i <= 10; i++)
                {
                    YearPopulate objb1 = new YearPopulate();
                    objb1.Year = DateTime.Now.AddYears(-i).Year.ToString();
                    objb1.Id = DateTime.Now.AddYears(-i).Year;
                    objList_year.Add(objb1);

                }
                objmodl.ddl_obj_years = objList_year;
                objmodl.ddl_obj_intr_boad = objList;
                return objmodl;
            }
        }

        [HttpPost] 
        public ActionResult StuInterDetails(IntermediateDetails model)
        {
            if (!ModelState.IsValid )
            {
                IntermediateDetails mod = new IntermediateDetails();
                mod = DDLFILLERINTERMEDIATE();
                //return View(mod);
                return RedirectToAction("StuInterDetails", "Registration");
            }

            else  if (model.ImageFile == null || model.ImageFile.ContentLength <= 0)
            {
                ViewBag.DDImageNull = "Please Choose the Image Of InterMediate Result Card";
                IntermediateDetails mod = new IntermediateDetails();
                mod = DDLFILLERINTERMEDIATE();
                // return View(mod);
                return RedirectToAction("StuInterDetails", "Registration");
            }
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                IntermediateDetails mod = new IntermediateDetails();
                mod = DDLFILLERINTERMEDIATE();
                // return View(mod);
                return RedirectToAction("StuInterDetails", "Registration");
            }
            if (model.ImageFile.ContentLength > (1024 * 300))
            {
                ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                IntermediateDetails mod = new IntermediateDetails();
                mod = DDLFILLERINTERMEDIATE();
                // return View(mod);
                return RedirectToAction("StuInterDetails", "Registration");
            }
            string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);

            //Converting into Bytes
            try
            {
                using (UOSEntities db = new UOSEntities())
                {
                    Stream fs = model.ImageFile.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    model.i_inter_scan = br.ReadBytes((Int32)fs.Length);
                    model.FileName = Path.GetFileName(model.ImageFile.FileName);
                    regsession = GetRegSessions();

                    int ProgID = regsession.discipline;
                    int Int_App_ID = regsession.AppId;

                    //bool ElegBlStudent = false;
                    //string LastDEgree = "";
                    //string Program_catgery="";
                    //int Program_type = 0;
                    //int Program_Catgery = 0;
                    //double DegMarksReq = 0;
                    //string ReqSub1Stg = null;
                    //string ReqSub2Stg = null;
                    //string ReqSub3Stg = null;
                    //double ReqSub1TMArks = 0;
                    //double ReqSub2TMArks = 0;
                    //double ReqSub3TMArks = 0;
                    //double ReqSub1OMarks = 0;
                    //double ReqSub2OMarks = 0;
                    //double ReqSub3OMarks = 0;
                    //var StdAppData = from dbo in db.stu_applied_reg_f_affi_clg where (dbo.id == Convert.ToInt32(Session["Std_App_ID"])) select dbo;

                    //foreach (var item in StdAppData)
                    //{
                    //        Program_catgery=item.Prog_Category;
                    //}

                    //var ElegibltyCartear = from dbo in db.Eligibility_Check join Dbo1 in db.Uos_All_Program_Apply_Category on dbo.Prog_catg equals Dbo1.ID where (Dbo1.App_Program_Catg == Program_catgery && dbo.Program_ID == ProgID && dbo.year_ID == Convert.ToInt32(Session["Std_App_ID"])) select dbo;
                    //foreach (var item in ElegibltyCartear)
                    //{
                    //    Program_Catgery =Convert.ToInt32( item.Prog_catg);
                    //    LastDEgree = item.Req_program_string;
                    //    DegMarksReq = Convert.ToDouble(item.Req_marks);
                    //    ReqSub1Stg = item.Req_subject1_ID;
                    //    ReqSub2Stg = item.Req_subject2_ID;
                    //    ReqSub3Stg = item.Req_subject3_ID;
                    //    ReqSub1TMArks = Convert.ToDouble(item.Req_subject1_Tmarks);
                    //    ReqSub2TMArks = Convert.ToDouble(item.Req_subject2_Tmarks);
                    //    ReqSub3TMArks = Convert.ToDouble(item.Req_subject3_Tmarks);
                    //    ReqSub1OMarks = Convert.ToDouble(item.Req_subject1_Omarks);
                    //    ReqSub2OMarks = Convert.ToDouble(item.Req_subject2_Omarks);
                    //    ReqSub3OMarks = Convert.ToDouble(item.Req_subject3_Omarks);
                    //}

                    //    bool Requer_Progerm = false;
                    //    bool Dagree_Mark_requer = false;
                    //    bool Subject_1 = false;
                    //    bool Subject_2 = false;
                    //    bool Subject_3 = false;
                    //    bool Subject_1_Tmark = false;
                    //    bool Subject_2_Tmark = false;
                    //    bool Subject_3_Tmark = false;
                    //    bool Subject_1_Omark = false;
                    //    bool Subject_2_Omark = false;
                    //    bool Subject_3_Omark = false;

                    //if (Program_Catgery == 1)
                    //{
                    //    // Elegibelty Chak

                    //    // Degree Chak
                    //    string[] ReqDEgreeArray = LastDEgree.Split(',');
                    //    foreach (var item in ReqDEgreeArray)
                    //    {
                    //        if (item == model.i_degree)
                    //        {
                    //            Requer_Progerm = true;
                    //            break;
                    //        }
                    //    }

                    //    // Marks Chak
                    //    double TMrk = Convert.ToDouble(model.i_t_marks);
                    //    double OMrk = Convert.ToDouble(model.i_o_marks);
                    //    double Perstage = (OMrk * 100) / TMrk;

                    //    if (Perstage >= DegMarksReq)
                    //        {
                    //            Dagree_Mark_requer = true;

                    //        }

                    //    // Subject_1 Marks
                    //    double StudMarks_1_TM = 0;
                    //    double StudMarks_1_Perstage = 0;
                    //    if (ReqSub1Stg == null || ReqSub1Stg == "")
                    //    {
                    //        Subject_1 = true;
                    //    }
                    //    else
                    //    {
                    //        string[] ReqSub1Arry = ReqSub1Stg.Split(',');
                    //        foreach (var item in ReqSub1Arry)
                    //        {
                    //            if (item == model.i_subjects1)
                    //            {
                    //                Subject_1 = true;
                    //                StudMarks_1_TM = model.i_1st_t_marks;
                    //                StudMarks_1_Perstage = (model.i_1st_o_marks * 100) / model.i_1st_t_marks;
                    //                break;
                    //            }
                    //            else if (item == model.i_subjects2)
                    //            {
                    //                Subject_1 = true;
                    //                StudMarks_1_TM = model.i_2nd_t_marks;
                    //                StudMarks_1_Perstage = (model.i_2nd_o_marks * 100) / model.i_2nd_t_marks;
                    //                    break;
                    //            }
                    //            else if (item == model.i_subjects3)
                    //            {
                    //                Subject_1 = true;
                    //                StudMarks_1_TM = model.i_3rd_t_marks;
                    //                StudMarks_1_Perstage = (model.i_3rd_o_marks * 100) / model.i_3rd_t_marks;
                    //                break;
                    //            }
                    //        }

                    //        // Chark Subject Marks 
                    //        if (ReqSub1TMArks == null || ReqSub1TMArks == 0)
                    //        {
                    //            Subject_1_Tmark = true;
                    //        }
                    //        else if (StudMarks_1_TM >= ReqSub1TMArks && ReqSub1TMArks > 0)
                    //        {
                    //                Subject_1_Tmark = true;
                    //        }

                    //        // Chark Subject Obtain Subject Marks 
                    //        if (ReqSub1OMarks == null || ReqSub1OMarks == 0)
                    //        {
                    //            Subject_1_Omark = true;
                    //        }
                    //        else if (StudMarks_1_Perstage >= ReqSub1OMarks && ReqSub1OMarks > 0)
                    //        {
                    //            Subject_1_Omark = true;
                    //        }
                    //    }

                    //    // Subject_2 Marks
                    //    double StudMarks_2_TM = 0;
                    //    double StudMarks_2_Perstage = 0;
                    //    if (ReqSub2Stg == null || ReqSub2Stg == "")
                    //    {
                    //        Subject_2 = true;
                    //    }
                    //    else
                    //    {
                    //        string[] ReqSub2Arry = ReqSub2Stg.Split(',');
                    //        foreach (var item in ReqSub2Arry)
                    //        {
                    //            if (item == model.i_subjects1)
                    //            {
                    //                Subject_2 = true;
                    //                StudMarks_1_TM = model.i_1st_t_marks;
                    //                StudMarks_1_Perstage = (model.i_1st_o_marks * 100) / model.i_1st_t_marks;
                    //                break;
                    //            }
                    //            else if (item == model.i_subjects2)
                    //            {
                    //                Subject_2 = true;
                    //                StudMarks_2_TM = model.i_2nd_t_marks;
                    //                StudMarks_2_Perstage = (model.i_2nd_o_marks * 100) / model.i_2nd_t_marks;
                    //                break;
                    //            }
                    //            else if (item == model.i_subjects3)
                    //            {
                    //                Subject_2 = true;
                    //                StudMarks_2_TM = model.i_3rd_t_marks;
                    //                StudMarks_2_Perstage = (model.i_3rd_o_marks * 100) / model.i_3rd_t_marks;
                    //                break;
                    //            }
                    //        }

                    //        // Chark Subject Marks 
                    //        if (ReqSub2TMArks == null || ReqSub2TMArks == 0)
                    //        {
                    //            Subject_2_Tmark = true;
                    //        }
                    //        else if (StudMarks_2_TM >= ReqSub2TMArks && ReqSub2TMArks > 0)
                    //        {
                    //            Subject_2_Tmark = true;
                    //        }

                    //        // Chark Subject Obtain Subject Marks 
                    //        if (ReqSub2OMarks == null || ReqSub2OMarks == 0)
                    //        {
                    //            Subject_2_Omark = true;
                    //        }
                    //        else if (StudMarks_2_Perstage >= ReqSub2OMarks && ReqSub2OMarks > 0)
                    //        {
                    //            Subject_2_Omark = true;
                    //        }
                    //    }

                    //    // Subject_3Marks
                    //    double StudMarks_3_TM = 0;
                    //    double StudMarks_3_Perstage = 0;
                    //    if (ReqSub3Stg == null || ReqSub3Stg == "")
                    //    {
                    //        Subject_3 = true;
                    //    }
                    //    else
                    //    {
                    //        string[] ReqSub3Arry = ReqSub3Stg.Split(',');
                    //        foreach (var item in ReqSub3Arry)
                    //        {
                    //            if (item == model.i_subjects1)
                    //            {
                    //                Subject_3 = true;
                    //                StudMarks_3_TM = model.i_1st_t_marks;
                    //                StudMarks_3_Perstage = (model.i_1st_o_marks * 100) / model.i_1st_t_marks;
                    //                break;
                    //            }
                    //            else if (item == model.i_subjects2)
                    //            {
                    //                Subject_3 = true;
                    //                StudMarks_3_TM = model.i_2nd_t_marks;
                    //                StudMarks_3_Perstage = (model.i_2nd_o_marks * 100) / model.i_2nd_t_marks;
                    //                break;
                    //            }
                    //            else if (item == model.i_subjects3)
                    //            {
                    //                Subject_3 = true;
                    //                StudMarks_3_TM = model.i_3rd_t_marks;
                    //                StudMarks_3_Perstage = (model.i_3rd_o_marks * 100) / model.i_3rd_t_marks;
                    //                break;
                    //            }
                    //        }

                    //        // Chark Subject Marks 
                    //        if (ReqSub3TMArks == null || ReqSub3TMArks == 0)
                    //        {
                    //            Subject_3_Tmark = true;
                    //        }
                    //        else if (StudMarks_3_TM >= ReqSub3TMArks && ReqSub3TMArks > 0)
                    //        {
                    //            Subject_3_Tmark = true;
                    //        }

                    //        // Chark Subject Obtain Subject Marks 
                    //        if (ReqSub3OMarks == null || ReqSub3OMarks == 0)
                    //        {
                    //            Subject_3_Omark = true;
                    //        }
                    //        else if (StudMarks_3_Perstage >= ReqSub3OMarks && ReqSub3OMarks > 0)
                    //        {
                    //            Subject_3_Omark = true;
                    //        }
                    //    }
                    //}
                    //else {
                    //Requer_Progerm=true;
                    //    Dagree_Mark_requer=true;
                    //}

                    if (model.Id == 0)
                    {
                        //if (Requer_Progerm && Dagree_Mark_requer && Subject_1 && Subject_1_Tmark && Subject_1_Omark && Subject_2 && Subject_2_Tmark && Subject_2_Omark && Subject_3 && Subject_3_Tmark && Subject_3_Omark)
                        //{
                        obj = GetData();
                        var result = db.BI_REG_INSERT_STU_INTER_INFO(regsession.StudentId, model.i_degree.ToString(), model.i_subjects1, model.i_1st_t_marks, model.i_1st_o_marks, model.i_subjects2, model.i_2nd_t_marks, model.i_2nd_o_marks, model.i_subjects3, model.i_3rd_t_marks, model.i_3rd_o_marks, Convert.ToInt32(model.i_board), Convert.ToInt32(model.i_p_year), model.i_exam_type, model.i_roll_no, model.i_t_marks, model.i_o_marks, model.i_division, model.i_inter_scan, model.FileName).ToList();
                        foreach (var item in result)
                        {
                            if (item.Value == 0)
                            {
                                TempData["Exception"] = "Basic Info Or Other Required Data Not Found , Enter The Basic Info First....!";
                                return RedirectToAction("StudentBasicInfo", "Registration");
                            }
                            else if (item.Value == 1)
                            {
                                TempData["Success"] = "Inter Details Added Successfully..!";
                                return RedirectToAction("StuBachelorsDetails", "Registration");
                            }

                        }
                        TempData["Success"] = "Intermediate Details Inserted Successfully";
                    }
                    else
                    {
                        TempData["Exception"] = "Eligibility criteria is Not Full Fill";
                        return RedirectToAction("StuInterDetails", "Registration");
                    }
                    //}
                    //else if (model.Id != 0)
                    //{
                    //    //store procedure of update
                    //}

                    return RedirectToAction("StuBachelorsDetails", "Registration");
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion Please Try Again";
                return RedirectToAction("StuInterDetails", "Registration");
            }
        }


        // Get Program Elegibelty Catag.
        public JsonResult GetProgElegCatera()   //string DataString
        {
            using (UOSEntities db = new UOSEntities())
            {
                regsession = GetRegSessions();

                int ProgID = regsession.discipline;
                int Program_catgery = 0;
                int Stud_Admin_ID = Convert.ToInt32(Session["Std_App_ID"]);
                var StdAppData = from dbo in db.stu_applied_reg_f_affi_clg where (dbo.id == Stud_Admin_ID) select dbo;
                foreach (var item in StdAppData)
                {
                    Program_catgery = Convert.ToInt32(item.Prog_Category);
                }

                int YearID = Convert.ToInt32(Session["year_id"]);
                //  var ElegibltyCartear = from dbo in db.Eligibility_Check join Dbo1 in db.Uos_All_Program_Apply_Category on dbo.Program_Type equals Dbo1.ID where (Dbo1.App_Program_Catg == Program_catgery && dbo.Program_ID == ProgID && dbo.year_ID == YearID) select dbo;
                var ElegibltyCartear = (from dbo in db.Eligibility_Check where (dbo.Prog_catg == Program_catgery && dbo.Program_ID == ProgID && dbo.year_ID == YearID) select dbo).ToList();
                foreach (var item in ElegibltyCartear)
                {
                    var programValu = item.Req_program_string;
                }


                return Json(ElegibltyCartear, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetProgElegModelData(string id)   //string DataString
        {

            using (UOSEntities db = new UOSEntities())
            {
                var ElegibltyCartear = db.Aj_GetProg_Eligibility_Check_Info_OID(Convert.ToInt32(id));
                //foreach (var item in ElegibltyCartear)
                //{
                //    var programValu = item.Req_program_string;
                //}


                return Json(ElegibltyCartear, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult StuBachelorsDetails()
        {
            regsession = GetRegSessions();
            if (Convert.ToInt32(regsession.StudentId) == 0)
            {
                return RedirectToAction("Granted_cources", "Registration");
            }
            else
            {
                using (UOSEntities db = new UOSEntities())
                {
                    string s = CheckApplicationStatus(regsession.StudentId);
                    if (s == "OK")
                        return RedirectToAction("stuApplicationFormComplete", "Registration");
                    else if (s == "StuBachelorsDetails")
                    {
                        BachelorDetails objmodl = new BachelorDetails();
                        List<YearPopulate> objList_year = new List<YearPopulate>();

                        //Pop Degree
                        var DegreeName = db.Uos_All_Programs.Where(x => x.Cat_Id == 1).ToList();
                        objmodl.b_dgreeName_dd = DegreeName.Select(x => new Uos_All_Programs { ID = x.ID, Program_Desc = x.Program_Desc }).ToList();

                        //Pop Subject1
                        var subject1 = db.Affi_ins_all_Subject_lab_desc.ToList();
                        objmodl.i_subjects1_DD = subject1.Select(x => new Affi_ins_all_Subject_lab_desc { Id = x.Id, Lab_Desc = x.Lab_Desc }).ToList();

                        //Pop Subject2
                        var subject2 = db.Affi_ins_all_Subject_lab_desc.ToList();
                        objmodl.i_subjects2_DD = subject2.Select(x => new Affi_ins_all_Subject_lab_desc { Id = x.Id, Lab_Desc = x.Lab_Desc }).ToList();

                        //Pop Subject3
                        var subject3 = db.Affi_ins_all_Subject_lab_desc.ToList();
                        objmodl.i_subjects3_DD = subject3.Select(x => new Affi_ins_all_Subject_lab_desc { Id = x.Id, Lab_Desc = x.Lab_Desc }).ToList();


                        // populating all years 
                        for (int i = 0; i <= 10; i++)
                        {
                            YearPopulate objb1 = new YearPopulate();
                            objb1.Year = DateTime.Now.AddYears(-i).Year.ToString();
                            objb1.Id = DateTime.Now.AddYears(-i).Year;
                            objList_year.Add(objb1);

                        }
                        objmodl.ddl_obj_years = objList_year;
                        return View(objmodl);

                    }
                    else
                        return RedirectToAction(s, "Registration");
                }
            }
        }

        [HttpPost]
        public ActionResult StuBachelorsDetails(BachelorDetails model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("StuBachelorsDetails", "Registration");
            if (model.ImageFile == null || model.ImageFile.ContentLength <= 0)
            {
                ViewBag.DDImageNull = "Please Choose the Image Of Bachelor's Transcript";
                return View();
            }
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                return View();
            }
            if (model.ImageFile.ContentLength > (1024 * 300))
            {
                ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                return View();
            }
            string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);

            //Converting into Bytes
            try
            {
                Stream fs = model.ImageFile.InputStream;
                BinaryReader br = new BinaryReader(fs);
                model.b_bachlor_scan = br.ReadBytes((Int32)fs.Length);
                model.FileName = Path.GetFileName(model.ImageFile.FileName);

                if (model.Id == 0)
                {
                    using (UOSEntities db = new UOSEntities())
                    {
                        obj = GetData();
                        regsession = GetRegSessions();
                        var result = db.BI_REG_INSERT_STU_BACHELOR_INFO(regsession.StudentId, model.b_degree, model.b_dgreeName, model.uni_inst, Convert.ToInt32(model.b_p_year), model.b_roll_no, model.m_sub_1, model.o_m_subject_1, model.t_m_subject_1, model.m_sub_2, model.o_m_subject_2, model.t_m_subject_2, model.m_sub_3, model.o_m_subject_3, model.t_m_subject_3, model.b_o_marks, model.b_t_marks, model.b_division, model.b_nature_exam, model.b_exam_type, model.b_bachlor_scan, model.FileName).ToList();
                        foreach (var item in result)
                        {
                            if (item.Value == 0)
                            {
                                TempData["Exception"] = "Basic Info Or Other Required Data Not Found , Enter The Basic Info First....!";
                                return RedirectToAction("StudentBasicInfo", "Registration");
                            }
                            else if (item.Value == 1)
                            {
                                TempData["Success"] = "Bachelors Details Added Successfully..!";
                                return RedirectToAction("StuAttachDocument", "Registration");
                            }

                        }
                        TempData["Success"] = "Bachelors Details Inserted Successfully";
                    }
                }
                else if (model.Id != 0)
                {
                    //store procedure of update
                }

                return RedirectToAction("StuAttachDocument", "Registration");
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion Please Try Again";
                return RedirectToAction("StuBachelorsDetails", "Registration");
            }
        }

        public ActionResult StuAttachDocument()
        {
            regsession = GetRegSessions();
            if (Convert.ToInt32(regsession.StudentId) == 0)
            {
                return RedirectToAction("Granted_cources", "Registration");
            }
            else
            {
                string s = CheckApplicationStatus(regsession.StudentId);
                if (s == "OK")
                    return RedirectToAction("stuApplicationFormComplete", "Registration");
                else if (s == "StuAttachDocument")
                    return View();
                else
                    return RedirectToAction(s, "Registration");
            }
        }

        [HttpPost]
        public ActionResult StuAttachDocument(AttachDocument model)
        {
            try
            {
                if (model.CnicImage.ContentLength <= 0)    //model.DomicileImage.ContentLength <= 0 || model.LastDegreeImage.ContentLength <= 0 ||
            {
                ViewBag.DDImageNull = "Please Upload Needed Files";
                return View();
            }
            }
            catch (Exception e)
            {
                ViewBag.DDImageNull = "Please Upload Needed Files";
                return View();
            }
            
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
          //  var domicile = Path.GetExtension(model.DomicileImage.FileName);
            var attsted_cnic = Path.GetExtension(model.CnicImage.FileName);
         //   var l_dgree = Path.GetExtension(model.LastDegreeImage.FileName);

            if (model.DomicileImage != null)
            {
                var domicile = Path.GetExtension(model.DomicileImage.FileName);
                if (!allowedExtensions.Contains(domicile.ToLower()))
                {
                    ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                    return View();
                }
                else if (model.DomicileImage.ContentLength > (1024 * 300))
                {
                    ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                    return View();
                }
            }
            
            if (model.MigrationNocImage != null)
            {
                var migration_noc = Path.GetExtension(model.MigrationNocImage.FileName);
                if (!allowedExtensions.Contains(migration_noc.ToLower()))
                {
                    ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                    return View();
                }
                else if (model.MigrationNocImage.ContentLength > (1024 * 300))
                {
                    ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                    return View();
                }
            }
            if (model.ServingNocImage != null)
            {
                var noc_serving = Path.GetExtension(model.ServingNocImage.FileName);
                if (!allowedExtensions.Contains(noc_serving.ToLower()))
                {
                    ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                    return View();
                }
                else if(model.ServingNocImage.ContentLength > (1024 * 300))
                {
                    ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                    return View();
                }
            }


            if (!allowedExtensions.Contains(attsted_cnic.ToLower())) //||  !allowedExtensions.Contains(l_dgree.ToLower()) !allowedExtensions.Contains(domicile.ToLower()) ||
            {
                ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                return View();
            }
            if (model.CnicImage.ContentLength > (1024 * 300)) //|| model.LastDegreeImage.ContentLength > (1024 * 300)   model.DomicileImage.ContentLength > (1024 * 300) ||
            {
                ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                return View();
            }

            //Converting into Bytes
            try
            {
                using (UOSEntities db = new UOSEntities())
                {
                    //domicile image
                    Stream fs = model.DomicileImage.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    //model.domicile = br.ReadBytes((Int32)fs.Length);
                    //model.domicileName = Path.GetFileName(model.DomicileImage.FileName);
                    if (model.DomicileImage != null)
                    {
                        fs = model.DomicileImage.InputStream;
                        br = new BinaryReader(fs);
                        model.domicile = br.ReadBytes((Int32)fs.Length);
                        model.domicileName = Path.GetFileName(model.DomicileImage.FileName);
                    }

                    //cninc image
                    fs = model.CnicImage.InputStream;
                    br = new BinaryReader(fs);
                    model.attsted_cnic = br.ReadBytes((Int32)fs.Length);
                    model.attsted_cnicName = Path.GetFileName(model.CnicImage.FileName);

                    //lastdegree image
                    //fs = model.LastDegreeImage.InputStream;
                    //br = new BinaryReader(fs);
                    //model.l_dgree = br.ReadBytes((Int32)fs.Length);
                    //model.l_dgreeName = Path.GetFileName(model.LastDegreeImage.FileName);

                    //serving Noc Image
                    if (model.ServingNocImage != null)
                    {
                        fs = model.ServingNocImage.InputStream;
                        br = new BinaryReader(fs);
                        model.noc_serving = br.ReadBytes((Int32)fs.Length);
                        model.noc_servingName = Path.GetFileName(model.ServingNocImage.FileName);
                    }

                    //Migration Noc Image
                    if (model.MigrationNocImage != null)
                    {
                        fs = model.MigrationNocImage.InputStream;
                        br = new BinaryReader(fs);
                        model.migration_noc = br.ReadBytes((Int32)fs.Length);
                        model.migration_nocName = Path.GetFileName(model.MigrationNocImage.FileName);
                    }


                    if (model.Id == 0)
                    {
                        obj = GetData();
                        regsession = GetRegSessions();
                        var result = db.BI_REG_INSERT_STU_DOC(regsession.StudentId, model.domicile, model.domicileName, model.attsted_cnic, model.attsted_cnicName, model.noc_serving, model.noc_servingName, model.migration_noc, model.migration_nocName).ToList();
                        foreach (var item in result)
                        {
                            if (item.Value == 0)
                            {
                                TempData["Exception"] = "Basic Info Or Other Required Data Not Found , Enter The Basic Info First....!";
                                return RedirectToAction("StudentBasicInfo", "Registration");
                            }
                            else if (item.Value == 1)
                            {
                                TempData["Success"] = "Bachelors Details Added Successfully..!";
                                return RedirectToAction("StuAffiliatedClg", "Registration");
                            }

                        }
                        TempData["Success"] = "Documents uploaded Successfully";
                    }
                    else if (model.Id != 0)
                    {
                        //store procedure of update
                    }

                    return RedirectToAction("StuAffiliatedClg", "Registration");
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion Please Try Again";
                return RedirectToAction("StuAttachDocument", "Registration");
            }
        }

        public ActionResult StuAffiliatedClg()
        {
            
            regsession = GetRegSessions();
            if (Convert.ToInt32(regsession.StudentId) == 0)
            {
                return RedirectToAction("Granted_cources", "Registration");
            }
            else
            {
                using (UOSEntities db = new UOSEntities())
                {
                    string s = CheckApplicationStatus(regsession.StudentId);
                    if (s == "OK")
                        return RedirectToAction("stuApplicationFormComplete", "Registration");
                    else if (s == "StuAffiliatedClg")
                    {
                        AffiliatedCollegeDetails objmdl = new AffiliatedCollegeDetails();
                        var InsInfo = db.BI_pop_institute_info_wrt_app(regsession.AppId).ToList();
                        foreach (var item in InsInfo)
                        {
                            objmdl.clg_name = item.Ins_Name;
                            objmdl.clg_ID = item.instituteId.ToString();
                            objmdl.clg_code = item.Inst_code.ToString();
                        }
                        var programinfo = db.BI_POP_program_with_section(regsession.AppId, regsession.StudentId).ToList();
                        foreach (var item in programinfo)
                        {
                            objmdl.education = item.Eaducation_system;
                            objmdl.program = item.Program_Desc;
                            objmdl.session = item.Session;
                        }
                        return View(objmdl);
                    }
                    else
                        return RedirectToAction(s, "Registration");
                }
            }
        }

        [HttpPost]
        public ActionResult StuAffiliatedClg(AffiliatedCollegeDetails model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (!ModelState.IsValid)
                    return View(model);

                obj = GetData();
                regsession = GetRegSessions();
                var result = db.BI_REG_SUBMIT_APPLICATION_AC(Convert.ToInt32(GetData().Institute_Id), regsession.AppId, regsession.StudentId, Convert.ToDateTime(model.admission_date), model.clg_roll_no, model.session).ToList();
                int inst = Convert.ToInt32(GetData().Institute_Id);
                int AppID = Convert.ToInt32(regsession.AppId);
                int stdID = Convert.ToInt32(regsession.StudentId);
                var vlu = db.stu_applied_reg_f_affi_clg.Where(x => x.inst_id == inst).Where(x => x.app_id == AppID).Where(x => x.st_id == stdID).Where(x => x.is_active == 1);
                foreach (var itm in vlu)
                {
                    Session["studentApplicationID"] = itm.id;
                }
                foreach (var item in result)
                {
                    if (item.Value == 0)
                    {
                        TempData["Exception"] = "Basic Info Or Other Required Data Not Found , Enter The Basic Info First....!";
                        return RedirectToAction("StudentBasicInfo", "Registration");
                    }
                    else if (item.Value == 1)
                    {
                        TempData["Success"] = "Bachelors Details Added Successfully..!";
                        // Load Student Application

                        //   DownloadStudentApplicationForm();


                        //Redrect to Registion Start Page
                        return RedirectToAction("StuApplicationFormComplete", "Registration");
                        //  return RedirectToAction("DownloadStudentApplicationForm", "Registration");
                    }

                }
                TempData["Exception"] = "Basic Info Or Other Required Data Not Found , Enter The Basic Info First....!";
                return RedirectToAction("StudentBasicInfo", "Registration");
            }
        }

        public JsonResult GetProvinces(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                var provinces = db.BI_POP_ALL_PROVIENCES(id).ToList();
                return Json(provinces, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDistricts(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                var districts = db.BI_POP_ALL_DISTT_WRT_PROVIENCE(id).ToList();

                return Json(districts, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTehsils(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                var tehsils = db.BI_POP_ALL_TEHSIL_WRT_DIST(id).ToList();
                return Json(tehsils, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: /Registration/
        public ActionResult Applicationform()
        {
            

            //string[] arraycountry = new string[3] { "Pakistan", "India", "Russia" };
            //List<Country> listcountry = new List<Country>();
            //for (var i = 0; i < 3; i++)
            //{
            //    Country country = new Country() { Name = arraycountry[i] };
            //    listcountry.Add(country);
            //}

            //string[] arrayprovince = new string[4] { "Punjab", "Sindh", "KPK", "Balochistan" };
            //List<UOS.Models.Common.Province> listprovince = new List<UOS.Models.Common.Province>();
            //for (var i = 0; i < 4; i++)
            //{
            //    UOS.Models.Common.Province province = new UOS.Models.Common.Province() { Name = arrayprovince[i] };
            //    listprovince.Add(province);
            //}

            //string[] arraydistrict = new string[3] { "Chakwal", "Sargodha", "Rawalpindi", };
            //List<District> listdistrict = new List<District>();
            //for (var i = 0; i < 3; i++)
            //{
            //    District district = new District() { Name = arraydistrict[i] };
            //    listdistrict.Add(district);
            //}

            //string[] arraytehsil= new string[3] { "KalarKahar", "Chowa", "Talagang", };
            //List<Tehsil> listtehsil = new List<Tehsil>();
            //for (var i = 0; i < 3; i++)
            //{
            //    Tehsil tehsil = new Tehsil() { Name = arraytehsil[i] };
            //    listtehsil.Add(tehsil);
            //}

            //StudentBasicDetails StudentDetails = new StudentBasicDetails()
            //{
            //    GenderDD=listgender,
            //    m_statusDD=listmeritalstatus,
            //    CountryDD=listcountry,
            //    ProvinceDD=listprovince,
            //    DistrictDD=listdistrict,
            //    TehsilDD=listtehsil,
            //    Per_countryDD=listcountry,
            //    Per_provinceDD=listprovince,
            //    Per_districtDD=listdistrict,
            //    Per_tehsilDD=listtehsil
            //};
            //ApplicationForm mainclass = new ApplicationForm()
            //{
            //    StudentBasicDetails=StudentDetails
            //};

            return View();
        }

        //affiliation notification attachment
        public ActionResult NotificationAttachment()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();

                Session["year_id"] = "1";
                AffiliationNotification objm = new Models.UserClassesReg.AffiliationNotification();


                if (Is_Notification_uploaded() && Convert.ToInt32(Session["year_id"]) > 0)
                {
                    return RedirectToAction("Granted_cources", "Registration");
                }
                else
                {

                    var year = db.Affi_ins_Year.ToList();

                    List<Affi_ins_Year> objY = new List<Affi_ins_Year>();
                    Affi_ins_Year yr = new Affi_ins_Year();
                    yr.ID = 0;
                    yr.Aff_Year = 0000;
                    objY.Add(yr);
                    foreach (var item in year)
                    {
                        Affi_ins_Year yrO = new Affi_ins_Year();
                        yrO.ID = item.ID;
                        yrO.Aff_Year = item.Aff_Year;
                        objY.Add(yrO);
                        objm.yeardd = objY;
                    }
                }
                return View(objm);
            }
        }

        [HttpGet]
        public JsonResult Notifection_Verify(string parameterValue)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Session["year_id"] = parameterValue.ToString();
                //string Mothed = null;
                if (Is_Notification_uploaded() && Convert.ToInt32(Session["year_id"]) > 0)
                {
                    //   var result1 = new { _result = "Redirect", _abc = "asdasd" };


                    var result1 = new { Success = true, url = "Url.Action('Granted_Cources_I', 'Registration')" };

                    //  var result = new { Success : "True", Message = "Error Message" };
                    return Json(result1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result1 = new { Success = true, url = "Url.Action('NotificationAttachment', 'Registration')" };
                    return Json(result1, JsonRequestBehavior.AllowGet);
                }



                //string output =  "Fuck" ;  

                //return Json(output, JsonRequestBehavior.AllowGet);


                //var result = new { Success : "True", Message = "Error Message" };
                //return Json(result, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //   // return RedirectToAction("NotificationAttachment", "Registration");
                //    var result1 = new { result = "Redirect", url = "Url.Action('NotificationAttachment', 'Registration')" };
                //    return Json(result1, JsonRequestBehavior.AllowGet);
                // }
            }
       }

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //populate image of Affiliation Notification
        public JsonResult pop_notification_image()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                var notification_img = db.BI_pop_notification_reg(Convert.ToInt32(obj.Institute_Id)).ToList();
                List<AffiliationNotification> list = new List<AffiliationNotification>();
                foreach (var ddobject in notification_img)
                {
                    AffiliationNotification ddc = new AffiliationNotification();
                    //Converting Bytes to File and string it into the directory of Server
                    string filePath = "~/Images/" + ddobject.imageName;
                    System.IO.File.WriteAllBytes(Server.MapPath(filePath), ddobject.notification_scan);
                    string servername = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                    ddc.FileName = servername + "/Images/" + ddobject.imageName;
                    list.Add(ddc);
                }
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult NotificationAttachment(AffiliationNotification model)
        {
            obj = GetData();
            if (!ModelState.IsValid)
                return View(model);
            if (model.ImageFile == null || model.ImageFile.ContentLength <= 0)
            {
                ViewBag.DDImageNull = "Please Choose the Image Of Demand Draft";
                return View();
            }
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                return View();
            }
            if (model.ImageFile.ContentLength > (1024 * 300))
            {
                ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                return View();
            }
            string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);

            //Converting into Bytes
            try
            {
                using (UOSEntities db = new UOSEntities())
                {
                    Stream fs = model.ImageFile.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    model.notification_Scan = br.ReadBytes((Int32)fs.Length);
                    model.FileName = Path.GetFileName(model.ImageFile.FileName);

                    if (model.Id == 0)
                    {
                        db.BI_reg_insert_notification_scan(Convert.ToInt32(obj.Institute_Id), model.notification_Scan, Convert.ToString(model.FileName)).ToList();
                        db.SaveChanges();
                        TempData["Success"] = "File Uploaded Successfully";
                    }
                    else if (model.Id != 0)
                    {

                    }
                }
            }
            catch (Exception e)
            {
                ViewBag.Exception = "Error In Uploading Please Try Again";
                return View();
            }


            return RedirectToAction("NotificationAttachment", "Registration");
        }

        //granted_cources
        public ActionResult Granted_cources()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (Is_Notification_uploaded())
                {
                    obj = GetData();

                    Granted_courses_cls objGC = new Granted_courses_cls();
                    //string[] instituteName = new string[2] { "Annual", "Term" };
                    //List<DDL_Category> List_category = new List<DDL_Category>();
                    //for (var i = 0; i < instituteName.Length; i++)
                    //{
                    //    DDL_Category objddlcat = new DDL_Category();
                    //    objddlcat.catId = instituteName[i];
                    //    objddlcat.category = instituteName[i];
                    //    List_category.Add(objddlcat);
                    //}

                    var Program_Type = db.Uos_All_Program_Apply_Category.Where(x => x.ID == 1 || x.ID == 3).ToList();
                    objGC.objCat = Program_Type.Select(x => new Uos_All_Program_Apply_Category { ID = x.ID, App_Program_Catg = x.App_Program_Catg }).ToList();

                    int YearID = Convert.ToInt32(Session["year_id"]);
                    var programs = db.BI_pop_granted_programs_wrt_inst(Convert.ToInt32(obj.Institute_Id), YearID,1).ToList();
                    List<DDL_prgram_desc> List_program = new List<DDL_prgram_desc>();
                    foreach (var item in programs)
                    {
                        objGC.AppId = Convert.ToInt32(item.App_ID);

                        DDL_prgram_desc obj1 = new DDL_prgram_desc();
                        obj1.program_id = Convert.ToInt32(item.Program_ID);
                        obj1.program_Desc = item.Program_Desc;
                        obj1.GrantedSeats = Convert.ToInt32(item.Granted_Seats);
                        List_program.Add(obj1);
                    }
                    objGC.objprogram = List_program;
                    //objGC.objCat = List_category;
                    return View(objGC);
                }
                return RedirectToAction("NotificationAttachment", "Registration");
            }
        }

        public JsonResult GetAdmissionDates(string DlCatogery)
        {
            
                using (UOSEntities obj_db = new UOSEntities())
                {
                    int YearID = Convert.ToInt32(Session["year_id"]);
                    int pApplyCat = Convert.ToInt32(DlCatogery);
                    var RetuenQuery = (from data in obj_db.Reg_Affi_Student_Schedule
                                       where (data.Program_Catg_ID == pApplyCat && data.Aff_year == YearID)
                                       select new { data.Start_Admin, data.Close_Admin, data.Program_catg, data.Admin_Catg }).ToList();

                    string objCurrentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).ToShortDateString();
                    foreach (var item in RetuenQuery)
                    {
                        if (Convert.ToDateTime(Convert.ToDateTime(item.Start_Admin).ToString("yyyy-MM-dd")) <= Convert.ToDateTime(objCurrentDate) && Convert.ToDateTime(Convert.ToDateTime(item.Close_Admin).ToString("yyyy-MM-dd")) >= Convert.ToDateTime(objCurrentDate))
                        {
                            return Json("1", JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            return Json("0", JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                return Json("2", JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public ActionResult Granted_cources(Granted_courses_cls model)
        {
            if (Convert.ToInt32(model.discipline) > 0 )
            {
                var userId = User.Identity.GetUserId();
             var claims = UserManager.GetClaims(userId);
            foreach (var claim in claims)
            {
                if (claim.Type == "ApplicationId")
                    UserManager.RemoveClaim(userId, claim);
                if (claim.Type == "GrantedSeats")
                    UserManager.RemoveClaim(userId, claim);
                if (claim.Type == "Discipline")
                    UserManager.RemoveClaim(userId, claim);
                if (claim.Type == "Category")
                    UserManager.RemoveClaim(userId, claim);
            }
            SessionRegistration obj = new SessionRegistration()
            {
                AppId=model.AppId,
                GrantedSeats=model.GrantedSeats,
                discipline=model.discipline,
                category=model.category
            };
            UserManager.AddClaim(userId, new Claim("ApplicationId", obj.AppId.ToString()));
            UserManager.AddClaim(userId, new Claim("GrantedSeats", obj.GrantedSeats.ToString()));
            UserManager.AddClaim(userId, new Claim("Discipline", obj.discipline.ToString()));
            UserManager.AddClaim(userId, new Claim("Category", obj.category.ToString()));

            return RedirectToAction("StudentBasicInfo", "Registration");
            }
            else
            {
                TempData["Exception"] = "Please Select Any Program";
                return RedirectToAction("Granted_Cources", "Registration");
            }
        }
        // submited students
        public ActionResult SubmittedStudents()
        {
            return View();
        }
        // CHECK NOTIFICATION UPLOADED OR NOT        
        public bool Is_Notification_uploaded()
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
            using (UOSEntities db = new UOSEntities())
            {
                var status = db.BI_reg_is_notification_uploaded(Convert.ToInt32(obj.Institute_Id)).ToList();
                foreach (var item in status)
                {
                    if (Convert.ToInt32(item.Value) > 0)
                        return true;
                }
                return false;
            }
        }

        // pop granted programs
        public ActionResult Pop_program(int val)
        {
            using (UOSEntities db = new UOSEntities())
            {
            obj =GetData();
            int YearID = Convert.ToInt32(Session["year_id"]);
            Affiliation objAffiliation = new Affiliation();
            var programs = db.BI_pop_granted_programs_wrt_inst(Convert.ToInt32(obj.Institute_Id), YearID, val).ToList();
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(programs);
            return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult PopGrantedAndConsumedSeats(int ProgramId , string ProgramCat)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                int YearID = Convert.ToInt32(Session["year_id"]);
                int instituteId = Convert.ToInt32(obj.Institute_Id);
                var QueryReturn = (from T1 in db.stu_applied_reg_f_affi_clg
                                   join t2 in db.Affi_Ins_Apply_Programs on new { a = T1.program_id, b = T1.app_id } equals new { a = t2.Program_ID, b = t2.App_ID }
                                   where (T1.inst_id == instituteId && T1.year_id == YearID && T1.Prog_Category == ProgramCat && T1.program_id == ProgramId && T1.is_in_reurn > 0 && T1.St_Enroll_status>0  && (T1. IS_Drop_Return == null || T1.IS_Drop_Return == 0 ))
                                   select new { T1.id, t2.Grant_seats }).ToList();
                int ConsumedSeats = QueryReturn.Count;
                string GrantedSeats;
                try { GrantedSeats = QueryReturn[0].Grant_seats.ToString(); }
                catch (Exception) { GrantedSeats = "0"; }
                return Json(new { ConsumedSeats = ConsumedSeats, bGrantedSeatsaz = GrantedSeats });
                // return View();
            }
        }

        public JsonResult pop_seats_wrt_program(string data)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                string[] ids = data.Split(',');
                int YearID = Convert.ToInt32(Session["year_id"]);
                var program = db.TA_pop_granted_programs_wrt_Program(Convert.ToInt32(obj.Institute_Id), Convert.ToInt32(ids[0]), YearID, Convert.ToInt32(ids[1])).ToList();
                return Json(program, JsonRequestBehavior.AllowGet);
            }
        }

        //student basic information view
        public ActionResult _Stu_basic_info()
        {
            return View();
        }
        //stu gardian info
        public ActionResult _Gardian_detail()
        {
            return View();
        }
        //stu matric info
        public ActionResult _Stu_matric_info()
        {
            return View();
        }
        //stu intermediate info
        public ActionResult _Stu_Inter_info()
        {
            return View();
        }
        //stu Bachelors info
        public ActionResult _Stu_Bachelors_info()
        {
            return View();
        }

        // student documents attachment
        public ActionResult _Stu_App_doc_Attach()
        {
            return View();
        }

        // Application tab for affiliated colleges only
        public ActionResult _affiliated_Clg_Info()
        {
            return View();
        }

        //granted cources view fro incomplete students
        public ActionResult Granted_cources_I()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                Granted_courses_cls objGC = new Granted_courses_cls();
                var Program_Type = db.Uos_All_Program_Apply_Category.Where(x => x.ID == 1 || x.ID == 3).ToList();
                objGC.objCat = Program_Type.Select(x => new Uos_All_Program_Apply_Category { ID = x.ID, App_Program_Catg = x.App_Program_Catg }).ToList();
                //string[] instituteName = new string[2] { "Annual", "Term" };
                //List<DDL_Category> List_category = new List<DDL_Category>();
                //for (var i = 0; i < instituteName.Length; i++)
                //{
                //    DDL_Category objddlcat = new DDL_Category();
                //    objddlcat.catId = instituteName[i];
                //    objddlcat.category = instituteName[i];
                //    List_category.Add(objddlcat);
                //}

                int YearID = Convert.ToInt32(Session["year_id"]);
                var programs = db.BI_pop_granted_programs_wrt_inst(Convert.ToInt32(obj.Institute_Id), YearID, 1).ToList();
                List<DDL_prgram_desc> List_program = new List<DDL_prgram_desc>();
                foreach (var item in programs)
                {
                    objGC.AppId = Convert.ToInt32(item.App_ID);
                    DDL_prgram_desc obj1 = new DDL_prgram_desc();
                    obj1.program_id = Convert.ToInt32(item.Program_ID);
                    obj1.program_Desc = item.Program_Desc;
                    obj1.GrantedSeats = Convert.ToInt32(item.Granted_Seats);
                    List_program.Add(obj1);
                }
                objGC.objprogram = List_program;
                // objGC.objCat = List_category;
                return View(objGC);
            }
        }

        public ActionResult Incomplete_Applications()
        {
            return View();
        }

        public JsonResult IncompleteStuDtblViewer(string applicationid , string programId, int Program_type)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (Convert.ToInt32(applicationid) > 0)
                {
                    var userId = User.Identity.GetUserId();
                    var claims = UserManager.GetClaims(userId);
                    foreach (var claim in claims)
                    {
                        if (claim.Type == "ApplicationId")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "GrantedSeats")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "Discipline")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "Category")
                            UserManager.RemoveClaim(userId, claim);
                    }
                    SessionRegistration obj = new SessionRegistration()
                    {
                        AppId = Convert.ToInt32(applicationid),
                        discipline = Convert.ToInt32(programId),
                    };

                    UserManager.AddClaim(userId, new Claim("ApplicationId", obj.AppId.ToString()));
                    UserManager.AddClaim(userId, new Claim("Discipline", obj.discipline.ToString()));
                    var dbpendingApps = db.bi_reg_pop_incomplete_stu_wrt_AC(Convert.ToInt32(GetData().Institute_Id), Convert.ToInt32(applicationid), Convert.ToInt32(programId), Program_type, false).ToList();
                    return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Exception"] = "Please Select Any Program";
                    var dbpendingApps = db.bi_reg_pop_incomplete_stu_wrt_AC(Convert.ToInt32(GetData().Institute_Id), 0, 0, Program_type, false).ToList();
                    return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]//remove incomplete student
        public JsonResult RemoveIncompleteStudent(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                regsession = GetRegSessions();
                db.bi_reg_remove_incomplete_stu_wrt_AC(regsession.AppId, regsession.discipline, id);
                return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        // student application form 
        public ActionResult StuApplicationFormComplete()
        {
            using (UOSEntities db = new UOSEntities())
            {
                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                var stu_ID = Convert.ToInt32(Session["studentApplicationId"]);
                var DBvlu = db.Stud_Admin_Basic_info(stu_ID).ToList();
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Student_info_fr_Admin.rdlc");
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Stud_Admin_Basic_info", DBvlu));

                ViewBag.ReportViewer = reportViewer;

                //Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();

                //rpt.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Student_info_fr_Admin.rdlc");
                //rpt.DataSources.Add(new ReportDataSource("Stud_Admin_Basic_info", DBvlu));

                //  DownloadStudentApplicationForm();
                return View();
            }
        }


     //   public ActionResult DownloadStudentApplicationForm()
        void DownloadStudentApplicationForm()  //Granted_cources_R
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                var stu_ID = Convert.ToInt32(Session["studentApplicationId"]);
                var DBvlu = db.Stud_Admin_Basic_info(stu_ID).ToList();
                rpt.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Student_info_fr_Admin.rdlc");
                rpt.DataSources.Add(new ReportDataSource("Stud_Admin_Basic_info", DBvlu));
                byte[] bytes = rpt.Render("PDF");
                Response.Buffer = true;
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename= Student_Application_Form" + "." + "PDF");
                Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
                Response.Flush(); // send it to the client to download  
                Response.End();
                //  return RedirectToAction("StuApplicationFormComplete", "Registration");
                //  return RedirectToAction("Granted_cources_R", "Registration");
            }
        }
        /// check status of student information
        public string CheckApplicationStatus(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                var status = db.stu_get_info_status(id).ToList();

                foreach (var row in status)
                {
                    if (row.is_gardian_info == false)
                    {
                        return "StuGuardianDetails";
                    }
                    else if (row.is_matric_info == false)
                    {
                        return "StuMetricDetails";
                    }
                    else if (row.is_inter_info == false)
                    {
                        return "StuInterDetails";
                    }
                    else if (row.is_bechelor_info == false)
                    {
                        regsession = GetRegSessions();
                        obj = GetData();
                        var result = db.stu_get_info_Program_Cat_status(id, Convert.ToInt32(obj.Institute_Id), regsession.discipline, Convert.ToInt32(obj.Id_year_for_affilation), regsession.AppId).ToList();
                        foreach (var item in result)
                        {
                            //if (item.Value == 2 && row.is_bechelor_info == false)
                            //{
                            //    return "StuBachelorsDetails";        
                            //}
                            if (item.Value == 1 && row.is_document_attached == false)
                            {
                                return "StuAttachDocument";
                            }
                            else if (item.Value == 1 && row.is_document_attached == true)
                            {
                                return "StuAffiliatedClg";
                            }
                        }
                        return "StuBachelorsDetails";
                    }
                    else if (row.is_document_attached == false)
                    {
                        return "StuAttachDocument";
                    }
                    else if (row.is_complete == false)
                    {
                        return "StuAffiliatedClg";
                    }
                }

                return "OK";
            }
        }

        // granted couces for send return
        public ActionResult Granted_cources_R()
        {
            obj = GetData();
            Granted_courses_cls objGC = new Granted_courses_cls();

            string[] instituteName = new string[2] { "Annual", "Term" };
           // List<DDL_Category> List_category = new List<DDL_Category>();

            using (UOSEntities db = new UOSEntities())
            {
                var Program_Type = db.Uos_All_Program_Apply_Category.Where(x => x.ID == 1 || x.ID == 3).ToList();
                objGC.objCat = Program_Type.Select(x => new Uos_All_Program_Apply_Category { ID = x.ID, App_Program_Catg = x.App_Program_Catg }).ToList();

                for (var i = 0; i < instituteName.Length; i++)
                {
                    DDL_Category objddlcat = new DDL_Category();
                    objddlcat.catId = instituteName[i];
                    objddlcat.category = instituteName[i];
                    //List_category.Add(objddlcat);
                }

                int YearID = Convert.ToInt32(Session["year_id"]);
                var programs = db.BI_pop_granted_programs_wrt_inst(Convert.ToInt32(obj.Institute_Id), YearID, 1).ToList();
                List<DDL_prgram_desc> List_program = new List<DDL_prgram_desc>();
                foreach (var item in programs)
                {
                    objGC.AppId = Convert.ToInt32(item.App_ID);
                    DDL_prgram_desc obj1 = new DDL_prgram_desc();
                    obj1.program_id = Convert.ToInt32(item.Program_ID);
                    obj1.program_Desc = item.Program_Desc;
                    obj1.GrantedSeats = Convert.ToInt32(item.Granted_Seats);
                    List_program.Add(obj1);
                }
                objGC.objprogram = List_program;
                // objGC.objCat = List_category;
                return View(objGC);
            }
        }

        // complete student datatabel vuewer
        public JsonResult completeStuDtblViewer(string applicationid, string programId, string Program_cat)
        {
            using (UOSEntities db = new UOSEntities())
            {
               
                if (Convert.ToInt32(applicationid) > 0)
                {
                    var userId = User.Identity.GetUserId();
                    var claims = UserManager.GetClaims(userId);
                    foreach (var claim in claims)
                    {
                        if (claim.Type == "ApplicationId")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "GrantedSeats")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "Discipline")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "Category")
                            UserManager.RemoveClaim(userId, claim);
                    }
                   
                    //var QueryReturn = (from T1 in db.stu_applied_reg_f_affi_clg
                    //                   join t2 in db.Affi_Ins_Apply_Programs on new { a = T1.program_id, b = T1.app_id } equals new { a = t2.Program_ID, b = t2.App_ID }
                    //                   where (T1.inst_id == instituteId && T1.year_id == YearID && T1.Prog_Category == ProgramCat && T1.program_id == ProgramId && T1.is_in_reurn > 0 && (T1.IS_Drop_Return == null || T1.IS_Drop_Return == 0))
                    //                   select new { T1.id, t2.Grant_seats }).ToList();

                    SessionRegistration obj = new SessionRegistration()
                    {
                        AppId = Convert.ToInt32(applicationid),
                        discipline = Convert.ToInt32(programId),
                    };
                    UserManager.AddClaim(userId, new Claim("ApplicationId", obj.AppId.ToString()));
                    UserManager.AddClaim(userId, new Claim("Discipline", obj.discipline.ToString()));
                    int inst_ID = Convert.ToInt32(GetData().Institute_Id);
                    var dbpendingApps = db.bi_reg_pop_incomplete_stu_wrt_AC(inst_ID, Convert.ToInt32(applicationid), Convert.ToInt32(programId), Convert.ToInt32(Program_cat), true).ToList();
                    return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Exception"] = "Please Select Any Program";
                    var dbpendingApps = db.bi_reg_pop_incomplete_stu_wrt_AC(Convert.ToInt32(GetData().Institute_Id), 0, 0, Convert.ToInt32(Program_cat), true).ToList();
                    return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpGet]//Drop Stucent
        public JsonResult DropStuFromReturn(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                regsession = GetRegSessions();
                db.bi_reg_drop_or_active_stu_wrt_AC(Convert.ToInt32(GetData().Institute_Id), regsession.AppId, regsession.discipline, id, true);
                return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]//Active Stucent
        public JsonResult ActiveStuFromReturn(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                regsession = GetRegSessions();
                db.bi_reg_drop_or_active_stu_wrt_AC(Convert.ToInt32(GetData().Institute_Id), regsession.AppId, regsession.discipline, id, false);
                return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //granted cources view fro Droped Students
        public ActionResult Granted_cources_D()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                Granted_courses_cls objGC = new Granted_courses_cls();

                var Program_Type = db.Uos_All_Program_Apply_Category.Where(x => x.ID == 1 || x.ID == 3).ToList();
                objGC.objCat = Program_Type.Select(x => new Uos_All_Program_Apply_Category { ID = x.ID, App_Program_Catg = x.App_Program_Catg }).ToList();
                //string[] instituteName = new string[2] { "Annual", "Term" };
                //List<DDL_Category> List_category = new List<DDL_Category>();
                //for (var i = 0; i < instituteName.Length; i++)
                //{
                //    DDL_Category objddlcat = new DDL_Category();
                //    objddlcat.catId = instituteName[i];
                //    objddlcat.category = instituteName[i];
                //    List_category.Add(objddlcat);
                //}

                int YearID = Convert.ToInt32(Session["year_id"]);
                var programs = db.BI_pop_granted_programs_wrt_inst(Convert.ToInt32(obj.Institute_Id), YearID, 1).ToList();
                List<DDL_prgram_desc> List_program = new List<DDL_prgram_desc>();
                foreach (var item in programs)
                {
                    objGC.AppId = Convert.ToInt32(item.App_ID);
                    DDL_prgram_desc obj1 = new DDL_prgram_desc();
                    obj1.program_id = Convert.ToInt32(item.Program_ID);
                    obj1.program_Desc = item.Program_Desc;
                    obj1.GrantedSeats = Convert.ToInt32(item.Granted_Seats);
                    List_program.Add(obj1);
                }
                objGC.objprogram = List_program;
                //  objGC.objCat = List_category;
                return View(objGC);
            }
        }

         //Droped student datatabel viewer
        public JsonResult DropedStuDtblViewer(string applicationid, string programId)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (Convert.ToInt32(applicationid) > 0)
                {
                    var userId = User.Identity.GetUserId();
                    var claims = UserManager.GetClaims(userId);
                    foreach (var claim in claims)
                    {
                        if (claim.Type == "ApplicationId")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "GrantedSeats")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "Discipline")
                            UserManager.RemoveClaim(userId, claim);
                        if (claim.Type == "Category")
                            UserManager.RemoveClaim(userId, claim);
                    }
                    SessionRegistration obj = new SessionRegistration()
                    {
                        AppId = Convert.ToInt32(applicationid),
                        discipline = Convert.ToInt32(programId),
                    };
                    UserManager.AddClaim(userId, new Claim("ApplicationId", obj.AppId.ToString()));
                    UserManager.AddClaim(userId, new Claim("Discipline", obj.discipline.ToString()));
                    var dbpendingApps = db.bi_reg_pop_droped_stu_wrt_AC(Convert.ToInt32(GetData().Institute_Id), Convert.ToInt32(applicationid), Convert.ToInt32(programId)).ToList();
                    return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["Exception"] = "Please Select Any Program";
                    var dbpendingApps = db.bi_reg_pop_droped_stu_wrt_AC(Convert.ToInt32(GetData().Institute_Id), 0, 0);
                    return Json(dbpendingApps, JsonRequestBehavior.AllowGet);
                }
            }
        }

        // send retun mathod Annual
        public ActionResult Send_Retuen_Annual(string applicationid, string program_id)
        {



            using (UOSEntities db = new UOSEntities())
            {
                int isAdmin = 0;
                int Prog_cat = 1;
                int YearID = Convert.ToInt32(Session["year_id"]);

                var RetuenQuery = (from data in db.Reg_Affi_Student_Schedule
                                   where (data.Program_Catg_ID == Prog_cat && data.Aff_year == YearID)
                                   select new { data.Start_Admin, data.Close_Admin, data.Program_catg, data.Admin_Catg }).ToList();

                string objCurrentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).ToShortDateString();
                foreach (var item in RetuenQuery)
                {
                    if (Convert.ToDateTime(Convert.ToDateTime(item.Start_Admin).ToString("yyyy-MM-dd")) <= Convert.ToDateTime(objCurrentDate) && Convert.ToDateTime(Convert.ToDateTime(item.Close_Admin).ToString("yyyy-MM-dd")) >= Convert.ToDateTime(objCurrentDate))
                    {
                        isAdmin = 1;

                    }
                    else
                    {
                        isAdmin = 0;
                    }

                }
                if (isAdmin > 0)
                {
                    int ToSeat = 0, OldAdmin = 0, NewAdmin = 0;
                    int APP_ID = Convert.ToInt32(applicationid);
                    int Prog_ID = Convert.ToInt32(program_id);
                    var queryResult = db.AJ_Reg_Stud_Chk_Seat_For_Retn(APP_ID, Prog_ID, Prog_cat).ToList();
                    if (queryResult.Count > 0)
                    {
                        ToSeat =Convert.ToInt32( queryResult[0].Grant_seats);
                        OldAdmin = Convert.ToInt32( queryResult[0].ConsumedSeats);
                        NewAdmin = Convert.ToInt32( queryResult[0].CurrentStudent);
                    }
                    if (ToSeat >= (OldAdmin + NewAdmin) && ToSeat > 0)
                    {
                         db.BI_REG_SEND_RETURN(Convert.ToInt32(GetData().Institute_Id), Convert.ToInt32(applicationid), Convert.ToInt32(program_id), 1);
                        return RedirectToAction("ReturnStatus", "Registration");
                    }
                    else
                    {
                        TempData["Exception"] = "Insufficient Seats";
                        return RedirectToAction("Granted_cources_R", "Registration");
                    }

                }
                else
                {
                    TempData["Exception"] = "Admission Is Closed";
                    return RedirectToAction("Granted_cources_R", "Registration");
                }
            }
        }
        // send retun mathod Term
        public ActionResult Send_Retuen_Term(string applicationid, string program_id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                int isAdmin = 0;
                int Prog_cat = 3;
                int YearID = Convert.ToInt32(Session["year_id"]);

                var RetuenQuery = (from data in db.Reg_Affi_Student_Schedule
                                   where (data.Program_Catg_ID == Prog_cat && data.Aff_year == YearID)
                                   select new { data.Start_Admin, data.Close_Admin, data.Program_catg, data.Admin_Catg }).ToList();

                string objCurrentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")).ToShortDateString();
                foreach (var item in RetuenQuery)
                {
                    if (Convert.ToDateTime(Convert.ToDateTime(item.Start_Admin).ToString("yyyy-MM-dd")) <= Convert.ToDateTime(objCurrentDate) && Convert.ToDateTime(Convert.ToDateTime(item.Close_Admin).ToString("yyyy-MM-dd")) >= Convert.ToDateTime(objCurrentDate))
                    {
                        isAdmin = 1;

                    }
                    else
                    {
                        isAdmin = 0;
                    }

                }
                if (isAdmin > 0)
                {
                    int ToSeat = 0, OldAdmin = 0, NewAdmin = 0;
                    int APP_ID = Convert.ToInt32(applicationid);
                    int Prog_ID = Convert.ToInt32(program_id);
                    var queryResult = db.AJ_Reg_Stud_Chk_Seat_For_Retn(APP_ID, Prog_ID, Prog_cat).ToList();
                    if (queryResult.Count > 0)
                    {
                        ToSeat =Convert.ToInt32(  queryResult[0].Grant_seats);
                        OldAdmin =Convert.ToInt32(  queryResult[0].ConsumedSeats);
                        NewAdmin =Convert.ToInt32(  queryResult[0].CurrentStudent);
                    }
                    if (ToSeat >= (OldAdmin + NewAdmin) && ToSeat > 0)
                    {

                        db.BI_REG_SEND_RETURN(Convert.ToInt32(GetData().Institute_Id), Convert.ToInt32(applicationid), Convert.ToInt32(program_id), 3);
                        return RedirectToAction("ReturnStatus", "Registration");
                    }
                    else
                    {
                        TempData["Exception"] = "Insufficient Seats";
                        return RedirectToAction("Granted_cources_R", "Registration");
                    }

                }
                else
                {
                    TempData["Exception"] = "Admission Is Closed";
                    return RedirectToAction("Granted_cources_R", "Registration");
                }
            }


        }

        // return status view 
        public ActionResult ReturnStatus()
        {
            return View();
        }

        // return (Return Staus)
        public JsonResult dtblReturnStatus()
        {
            using (UOSEntities db = new UOSEntities())
            {
                int inst_id = Convert.ToInt32(GetData().Institute_Id);
                var result = db.fatch_app_id_wrt_int_id_year_id(inst_id, Convert.ToInt32(GetData().Id_year_for_affilation)).ToList();
                int ap_id = 0;
                foreach (var item in result)
                {
                    ap_id = item.Value;
                }
                var returnstatus = db.bi_reg_pop_return(inst_id, ap_id, 1);
                return Json(returnstatus, JsonRequestBehavior.AllowGet);
            }
        }

        // return view of student with respect to return
        public ActionResult StuWRTReturn(string returnId)
        {
            if (returnId == null)
                returnId = "0";
            TempData["ReturnID"] = returnId.ToString();
            return View();
        }

        // return data table
        public JsonResult dtblReturnStudents(string returnid)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (returnid == null)
                    returnid = "0";
                var returnstatus = db.bi_reg_pop_stu_wrt_return(Convert.ToInt32(returnid)).ToList();
                return Json(returnstatus, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult uploadChallanForm(DemandDraft model)
        {


            obj = GetData();
            if (!ModelState.IsValid)
            {
                ViewBag.Exception = "Please Choose the Image Of Draft";
                return RedirectToAction("ReturnStatus", "Registration");
            }
            if (model.ImageFile == null || model.ImageFile.ContentLength <= 0)
            {
                ViewBag.DDImageNull = "Please Choose the Image Of Draft";
                return RedirectToAction("ReturnStatus", "Registration");
            }
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                return View();
            }
            if (model.ImageFile.ContentLength > (1024 * 300))
            {
                ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                return View();
            }
            string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);

            //Converting into Bytes
            try
            {
                using (UOSEntities db = new UOSEntities())
                {
                    Stream fs = model.ImageFile.InputStream;
                    BinaryReader br = new BinaryReader(fs);
                    model.Draft_Scan = br.ReadBytes((Int32)fs.Length);
                    model.FileName = Path.GetFileName(model.ImageFile.FileName);

                    if (model.Id > 0)
                    {
                        db.bi_insert_return_challan_form(Convert.ToInt32(model.Id), Convert.ToInt32(model.branch_code), model.deposite_date.ToString(), model.bank_name, model.Draft_Scan);
                        db.SaveChanges();
                    }
                    TempData["Success"] = "Challan upload Successfully";
                }
            }
            catch (Exception e)
            {
                ViewBag.Exception = "Error In Insertion Please Try Again";
                return RedirectToAction("ReturnStatus", "Registration");
            }
                      
            return RedirectToAction("ReturnStatus","Registration");
        }

        public ActionResult returnPendingChallanVerify()
        {
            return View();
        }
        // Annex-B Data Get
        byte[] Annex_Cget(int st_id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                var DBvlu = db.AJ_Stud_Admin_Return_Reg_B(st_id).ToList();
                rpt.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Aff_Stud_admin_return_Nur_B.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Stud_Admin_Return_Reg_A", DBvlu));
                return rpt.Render("PDF");
            }
        }

        //Download Fee Slip
        public ActionResult DownloadFeeSlip(string returnId)
        {
            using (UOSEntities db = new UOSEntities())
            {
                byte[] bytes = null;
                if (returnId == null)
                    returnId = "0";
                int catg = Convert.ToInt32(returnId.Substring(0, 1));
                int RntID = Convert.ToInt32(returnId.Substring(1, returnId.Length - 1));
                string DocumetNAme = "";
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                if (catg == 1)
                {
                    var DBvlu = db.AJ_Stud_Admin_Return_Reg_A(RntID).ToList();
                    rpt.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/rdlc_Stud_Admin_Return_Reg_A.rdlc");
                    rpt.DataSources.Add(new ReportDataSource("AJ_Stud_Admin_Return_Reg_A", DBvlu));
                    DocumetNAme = "Annex-A" + "." + "PDF";
                    rpt.GetDefaultPageSettings();
                    bytes = rpt.Render("PDF");
                }
                else if (catg == 2)
                {
                    List<byte[]> obj1 = new List<byte[]>();
                    var queryLondonCustomers = (from cust in db.stu_applied_reg_f_affi_clg
                                               where (cust.is_in_reurn == RntID)
                                               select cust).ToList();
                    foreach (var item in queryLondonCustomers)
                    {
                        obj1.Add(Annex_Cget(item.id));
                    }
                    DocumetNAme = "Annex-B" + "." + "PDF";
                    bytes = concatAndAddContent(obj1);
                }
                else if (catg == 3)
                {
                    var DBvlu = db.AJ_Stud_Admin_Return_Reg_C(RntID).ToList();
                    rpt.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Affi_reg_return_Anax_C.rdlc");
                    rpt.DataSources.Add(new ReportDataSource("Rdlc_Affi_reg_return_Anax_C", DBvlu));
                    DocumetNAme = "Annex-C" + "." + "PDF";
                    rpt.GetDefaultPageSettings();
                    bytes = rpt.Render("PDF");
                }
                else if (catg == 4)
                {
                    var DBvlu = db.Aj_Aff_Ins_Admin_Fee_slip(RntID).ToList();
                    rpt.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Reg_Admin_Fee_Slip.rdlc");
                    rpt.DataSources.Add(new ReportDataSource("Aj_Aff_Ins_Admin_Fee_slip", DBvlu));
                    DocumetNAme = "Fee_Challan" + "." + "PDF";
                    bytes = rpt.Render("PDF");
                }
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
        }
        public static byte[] concatAndAddContent(List<byte[]> pdfByteContent)
        {

            using (var ms = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    using (var copy = new PdfSmartCopy(doc, ms))
                    {
                        doc.Open();

                        //Loop through each byte array
                        foreach (var p in pdfByteContent)
                        {

                            //Create a PdfReader bound to that byte array
                            using (var reader = new PdfReader(p))
                            {

                                //Add the entire document instead of page-by-page
                                copy.AddDocument(reader);
                            }
                        }

                        doc.Close();
                    }
                }
                //  ms.Flush();
                //Return just before disposing
                return ms.ToArray();
            }
        }

        public JsonResult dtblReturnpendignForChallan(int RetStu)
        {
            using (UOSEntities db = new UOSEntities())
            {
                int inst_id = Convert.ToInt32(GetData().Institute_Id);
                var result = db.fatch_app_id_wrt_int_id_year_id(inst_id, Convert.ToInt32(GetData().Id_year_for_affilation)).ToList();
                int ap_id = 0;
                foreach (var item in result)
                {
                    ap_id = item.Value;
                }
                var returnstatus = db.bi_reg_pop_return(inst_id, ap_id, RetStu);
                return Json(returnstatus, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult VerifiedChallan()
        {
            return View();
        }
        public ActionResult VerifiedReturn()
        {
            return View();
        }
        public ActionResult DownloadAnex(int RntID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                byte[] bytes = null;
                //if (returnId == null)
                //    returnId = "0";
                //int catg = Convert.ToInt32(returnId.Substring(0, 1));
                //int RntID = Convert.ToInt32(returnId.Substring(1, returnId.Length - 1));
                string DocumetNAme = "";
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();

                var DBvlu = db.AJ_Stud_Admin_Return_Reg_C(RntID).ToList();
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
        }

        public ActionResult CNICCheck(String CNIC)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                regsession = GetRegSessions();
                var ReturnQuery = db.Updated_bi_get_Student_Status_from_BothSides(CNIC).ToList();
                
                foreach (var item in ReturnQuery)
                {
                    if (item.StStatus == "Exists")
                    {
                        if (item.StCurrentStatus == 1)
                        {
                            var result = new { Message = "AlreadyRegisterdInSOmeProgram" };
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else 
                        {
                            if(item.RegNumber != null)
                            {
                                int YearIddd = Convert.ToInt32(Session["year_id"]);
                                var QuerRes = db.Aj_Eligibility_Check_Exite_student(Convert.ToInt32(regsession.discipline), YearIddd, item.StId, Convert.ToInt32(regsession.category)).ToList();
                                int temp = Convert.ToInt32(QuerRes[0].Value);
                                if (temp == 0)
                                {
                                    // NOt Eligiable 
                                    var result = new { Message = "NotEligiable" };
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                }
                                else if (temp == 1)
                                {
                                    // Eligiable 
                                    var result = new { Message = "ContinueWithTrap", StudentId = item.StId };
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                }
                                else if (temp == 2)
                                {
                                    //Data Not Found REturn To Form to Get Data
                                    var result = new { Message = "ContinueWithTrap", StudentId = item.StId };
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                    
                                }
                                else if (temp == 3)
                                {
                                    // System Not Have Eligibility Critiria For this Aplication
                                    var result = new { Message = "CritariaNotEligiable" };
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                if (item.LastInstituteId == Convert.ToInt32(obj.Institute_Id))
                                {
                                    // continue with check in array 
                                    var result = new { Message = "ContinueWithTrap" , StudentId = item.StId };
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    // no permittion to edit data show pop and generate some notification in registration side 
                                    var result = new { Message = "AccessDenied" };
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
                    }
                    else if (item.StStatus == "Old")
                    {
                        if (item.StCurrentStatus == 1)
                        {
                            var result = new { Message = "AlreadyRegisterdInSOmeProgram" };
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (item.RegNumber != null)
                            {
                                var result = new { Message = "Old", RegNumber = item.RegNumber };
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                var result = new { Message = "ErrorIsRegistration" };
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else if (item.StStatus == "New")
                    {
                        var result = new { Message = "NewStudent"};
                        return Json(result, JsonRequestBehavior.AllowGet);  
                    }
                    else
                    {
                        //return Json("ErrorRedirection", JsonRequestBehavior.AllowGet);  
                        var result = new { Message = "ErrorIsRegistration" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                //return Json("ErrorRedirection", JsonRequestBehavior.AllowGet);
                //return RedirectToAction("ErrorIsRegistration","Registration");
                var result1 = new { Message = "ErrorIsRegistration" };
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult ErrorIsRegistration()
        {
            return View();
        }
        public ActionResult AlreadyRegisterdInRunningProgram()
        {
            return View();
        }
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult CheckAndMoveApplicaion(string StudentID)
        {
            try
            {
                regsession = GetRegSessions();
                regsession.StudentId = Convert.ToInt32(StudentID);
                UserManager.AddClaim(User.Identity.GetUserId(), new Claim("StudentId", regsession.StudentId.ToString()));
                string s = CheckApplicationStatus(Convert.ToInt32(StudentID));
                if (s == "OK")
                    return RedirectToAction("stuApplicationFormComplete", "Registration");
                else
                    return RedirectToAction(s, "Registration");
            }
            catch (Exception exp)
            {
                string temp = exp.Message;
                return RedirectToAction("ErrorIsRegistration", "Registration");
            }
            
        }



//Controller Ending......
	}
}