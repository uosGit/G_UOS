using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UOS.Models;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.crypto;
using System.Web.Security;


namespace UOS.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {

        }
        public ActionResult Index()
        {
            // MembershipUser u;
            //u= Membership.GetUser(User.Identity.Name);
            // Membership.GetPassword(username, "");

            return View();
        }
        public ActionResult ReportTest()
        {
            using (UOSEntities db = new UOSEntities())
            {
                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(900);
                reportViewer.Height = Unit.Percentage(900);

                var DBvlu = db.AJ_Stud_Admin_Return_Reg_C(2);
                // var DBvlu = db.Aj_Aff_Ins_Admin_Fee_slip(2);
                // var DBvlu = db.Stud_Admin_Basic_info(1);
                // reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/rdlc_Stud_Admin_Return_Reg_A.rdlc");
                //   reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Affi_reg_return_Anax_C.rdlc"); ;
                reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_aff_reg_chak_list.rdlc"); ;
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("AJ_Stud_Admin_Return_Reg_A", DBvlu));

                //reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Student_info_fr_Admin.rdlc");
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Stud_Admin_Basic_info", DBvlu));
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Rdlc_Affi_reg_return_Anax_C", DBvlu));//Rdlc_aff_reg_chak_list
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Rdlc_aff_reg_chak_list", DBvlu));//Rdlc_aff_reg_chak_list

                //reportViewer.LocalReport.ReportPath = Server.MapPath(@"~/Reports/Regirstion_Reports/Rdlc_Reg_Admin_Fee_Slip.rdlc");
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Aj_Aff_Ins_Admin_Fee_slip", DBvlu));
                ViewBag.ReportViewer = reportViewer;
                return View();
            }
        }
        public ActionResult PDF()
        {
            using (UOSEntities db = new UOSEntities())
            {
                int instID = 1;
                //List<byte[]> obj = new List<byte[]>();
                //obj.Add(Download_Rpt_Insitut_info(instID));
                //obj.Add(Download_Rpt_Room_info(instID));
                //obj.Add(Download_Rpt_Non_Requir_info(instID));
                //obj.Add(Download_Rpt_Requir_info(instID));
                //obj.Add(Download_Rpt_Sport_info(instID));
                //obj.Add(Download_Rpt_Staff_info(instID, 1));
                //obj.Add(Download_Rpt_Staff_info(instID, 0));
                //obj.Add(Download_Rpt_Libaray_info(instID));
                //obj.Add(Laboratory(instID));

                // byte[] bytes = concatAndAddContent(obj);
                //Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                //rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Rep_Affi_Ins_Visting_Member.rdlc");
                //rpt.DataSources.Add(new ReportDataSource("AJ_Aff_Ins_Vist_Rep_Commety", db.AJ_Aff_Ins_Vist_Rep_Commety(Convert.ToInt32(1))));
                //rpt.DataSources.Add(new ReportDataSource("AJ_Aff_Ins_Vist_Rep_MEmBEr_Commety", db.AJ_Aff_Ins_Vist_Rep_MEmBEr_Commety(Convert.ToInt32(1))));

                byte[] bytes = Download_Rpt_Room_info(instID); //rpt.Render("PDF");  
                Response.Buffer = true;
                Response.Clear();
                Response.AddHeader("content-disposition", "attachment; filename= Application" + "." + "PDF");
                Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
                Response.Flush(); // send it to the client to download  
                Response.End();
                return View();
            }
        }
        byte[] Download_Rpt_Insitut_info(int Inst_ID)
        {
            using (UOSEntities db = new UOSEntities())
            {
                Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
                rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Rpt_Affi_Instu_Info.rdlc");
                rpt.DataSources.Add(new ReportDataSource("AJ_Affi_Ins_Pop_Instu_Info", db.AJ_Affi_Ins_Pop_Instu_Info(Convert.ToInt32(Inst_ID))));

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
            using (UOSEntities db = new UOSEntities())
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
        }

    }
}