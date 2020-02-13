using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UOS.Models.UserClasses;
using UOS.Models;
using UOS.App_Start;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;


namespace UOS.Controllers.UserController
{
    public class UserStatusController : Controller
    {
        SessionObject obj;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserStatusController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            
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

        public UserStatusController()
        {
           
        }

        public SessionObject GetData()
        {
            var claims = UserManager.GetClaims(User.Identity.GetUserId());
            SessionObject obj = new SessionObject()
            {
                Institute_Id = Convert.ToString(claims[0].Value),
                Affiliation_Year = Convert.ToInt32(claims[1].Value),
                Id_year_for_affilation = Convert.ToInt32(claims[2].Value)
            };
            return obj;
        }

        public ActionResult status()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                int instid = Convert.ToInt32(obj.Institute_Id);
                var list = db.AJ_Affi_Ins_Pop_Instu_Info(instid);
                UserStatus status = new UserStatus();
                foreach (var item in list)
                {
                    status.Email = item.Ins_Email;
                    status.InstituteName = item.Ins_Name;
                    status.MobileNumber = item.Ins_OfficMobile;
                    status.Nature = item.College_Catg;
                    status.PhoneNumber = item.Ins_ContactPTCL;
                    status.PostalAddress = item.Ins_PostAddres;
                    status.PrincipalName = item.Ins_PrinciPal_Name;
                    obj.Application_id = Convert.ToInt32(item.application_id);


                    if (item.Securty_status == 1)
                    {
                        status.Status = "successfully Granted";
                        status.ReturnStatusIdentity = 1;
                    }
                    else if (item.is_grant_Affiliation == true && (item.Securty_status == 0 || item.Securty_status == null))
                    {
                        var dv = db.Aj_affi_Get_application_Prog_scurity_Fee(item.application_id);
                        foreach (var ite in dv)
                        {

                            status.Status = "Affiliation conditionally granted Submit your Secuirty Fee as soon as possiable  with demand daraf of RS.";
                            status.StatusValueFOrLink = ite.Value.ToString();
                            status.Status1 = "And Upload Draft.";
                            status.ReturnStatusIdentity = 2;
                        }
                    }
                    else if (item.Is_visit == true && (item.is_grant_Affiliation == false || item.is_grant_Affiliation == null) && (item.Securty_status == 0 || item.Securty_status == null))
                    {
                        status.Status = "Application is in visiting schedule. Affiliation Inspection committee will visit you soon.";
                    }
                    else if (item.Is_visit_assign == true && (item.Is_visit == false || item.Is_visit == null))
                    {
                        status.Status = "Application is in process wait for visit of affiliation insecption committee.";
                    }
                    else if (item.IS_submit == true && (item.Is_visit_assign == false || item.Is_visit_assign == null))
                    {
                        status.Status = "Your Application Is submit from your side please wait for further action from Affiliation Branch of UOS.";
                    }
                    else if ((item.IS_submit == false || item.IS_submit == null) && (item.Is_visit_assign == false || item.Is_visit_assign == null))
                    {
                        status.Status = "You Application Is not complet Or Reject from Admin due to some deficieny.";
                    }
                }
                return View(status);
            }
        }

        public ActionResult UploadSecurityDeposite()
        {

            return View();
        }
        [HttpPost]

        public ActionResult UploadSecurityDeposite(DemandDraft model)
        {
            using (UOSEntities db = new UOSEntities())
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
                Stream fs = model.ImageFile.InputStream;
                BinaryReader br = new BinaryReader(fs);
                model.Draft_Scan = br.ReadBytes((Int32)fs.Length);
                model.FileName = Path.GetFileName(model.ImageFile.FileName);
                int instid=Convert.ToInt32(obj.Institute_Id);
                db.BI_Affi_insert_Draft_detail_program_security(instid, obj.Id_year_for_affilation, Convert.ToString(model.bank_name), Convert.ToString(model.bank_address), Convert.ToString(model.branch_code), Convert.ToString(model.amount), Convert.ToDateTime(Convert.ToDateTime(model.deposite_date).ToString("yyyy-MM-dd")), model.Draft_Scan, model.FileName);
                TempData["Success"] = "Demand Draft Inserted Successfully";
            }
            catch (Exception e)
            {
                ViewBag.Exception = "Error In Insertion Please Try Again";
                return View();
            }
            return RedirectToAction("DemandDraft", "UserAccount");
            }
        }

        public ActionResult PDF()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            int instID = Convert.ToInt32(obj.Institute_Id);
            var AppID = db.Aff_ins_Application.Where(a => a.Aff_Year == obj.Id_year_for_affilation && a.Inst_ID == instID).Select(a => a.ID).Single();
            List<byte[]> obj1 = new List<byte[]>();
            obj1.Add(Download_Rpt_Insitut_info(Convert.ToInt32(AppID)));
            obj1.Add(Download_Rpt_Appl_Declaration_info(instID));
            obj1.Add(Download_Rpt_Room_info(instID));
            obj1.Add(Download_Rpt_Non_Requir_info(instID));
            obj1.Add(Download_Rpt_Requir_info(instID));
            obj1.Add(Download_Rpt_Sport_info(instID));
            obj1.Add(Download_Rpt_Staff_info(instID, 1));
            obj1.Add(Download_Rpt_Staff_info(instID, 0));
            obj1.Add(Download_Rpt_Libaray_info(instID));
            obj1.Add(Laboratory(instID));

            byte[] bytes = concatAndAddContent(obj1); 
            Response.Buffer = true;
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename= Application" + "." + "PDF");
            Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
            Response.Flush(); // send it to the client to download  
            Response.End();
            return RedirectToAction("/Status");
            }
        }

        byte[] Download_Rpt_Insitut_info(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Rpt_Affi_Instu_Info.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Affi_Ins_Pop_Instu_Info", db.AJ_Affi_Ins_App_Instu_Info(Convert.ToInt32(Inst_ID))));

            return rpt.Render("PDF");
            }
        }

        byte[] Download_Rpt_Appl_Declaration_info(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Ins_Apply_Rep_Declaration.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Aff_Ins_Apply_Rep_Declaration", db.AJ_Aff_Ins_Apply_Rep_Declaration(Convert.ToInt32(Inst_ID))));

                return rpt.Render("PDF");
            }
        }

        byte[] Download_Rpt_Room_info(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_BuldingDetails.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_POP_Bulding_INFO", db.AJ_POP_Bulding_INFO(Convert.ToInt32(Inst_ID))));

                return rpt.Render("PDF");
            }
        }

        byte[] Download_Rpt_Sport_info(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_SportFculty.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Rep_SportFacilities_INFO", db.AJ_Rep_SportFacilities_INFO(Convert.ToInt32(Inst_ID))));

                return rpt.Render("PDF");
            }
        }

        byte[] Download_Rpt_Staff_info(int Inst_ID, int cag)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_Staff_Info.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Employee_INFO", db.AJ_Rep_Employee_INFO(Convert.ToInt32(Inst_ID), cag)));

                return rpt.Render("PDF");
            }
        }

        byte[] Download_Rpt_Non_Requir_info(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_ins_Non_Recuring_INFO.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Non_Recuring_INFO", db.AJ_Rep_Non_Recuring_INFO(Convert.ToInt32(Inst_ID))));

                return rpt.Render("PDF");
            }
        }

        byte[] Download_Rpt_Requir_info(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_ins_Recuring_INFO.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Recuring_INFO", db.AJ_Rep_Recuring_INFO(Convert.ToInt32(Inst_ID))));

                return rpt.Render("PDF");
            }
        }

        byte[] Download_Rpt_Libaray_info(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_Library_INFO.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Library_INFO", db.AJ_Rep_Library_INFO(Convert.ToInt32(Inst_ID))));
                rpt.DataSources.Add(new ReportDataSource("Affi_ins_Rep_OtherFacilities_INFO", db.AJ_Affi_ins_Rep_OtherFacilities_INFO(Convert.ToInt32(Inst_ID))));

                return rpt.Render("PDF");
            }
        }

        byte[] Laboratory(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_inst_AJ_Rep_Laboratory_INFO.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Laboratory_INFO", db.AJ_Rep_Laboratory_INFO(Convert.ToInt32(Inst_ID))));

                return rpt.Render("PDF");
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


        public ActionResult DownloadFiles()
        {
            return View();
        }

        public JsonResult StatusViewer()
        {
            obj = GetData();
            int instId = Convert.ToInt32(obj.Institute_Id);
            var list="";
            //store procedure for status
            return Json(list,JsonRequestBehavior.AllowGet);
        }

	}
}