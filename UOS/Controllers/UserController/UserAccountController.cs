using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using UOS.Models;
using UOS.Models.UserClasses;
using System.Security.Claims;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Text;
using UOS.App_Start;
using Microsoft.Reporting.WebForms;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace UOS.Controllers.UserController
{
    [Authorize(Roles = "User")]
    public class UserAccountController : Controller
    {
        SessionObject obj;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public UserAccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        public UserAccountController()
        {
        }

        public bool IsRoll()
        {
            using (UOSEntities db = new UOSEntities())
            {
                var claims = UserManager.GetClaims(User.Identity.GetUserId());
                SessionObject obj = new SessionObject()
                {
                    Institute_Id = Convert.ToString(claims[0].Value),
                    Affiliation_Year = Convert.ToInt32(claims[1].Value),
                    Id_year_for_affilation = Convert.ToInt32(claims[2].Value)
                };
                var status = db.check_affi_app_status(Convert.ToInt32(obj.Institute_Id)).ToList();
                foreach (var item in status)
                {
                    if (item.IS_Submit == true)
                        return false;
                }
                return true;
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

        //User Home Page
        public ActionResult Index()
        {
            return View();
        }


        //Demand Draft
        [HttpGet]
        public ActionResult DemandDraft()
        {
            if (IsRoll())
            {
                return View();
            }
            return RedirectToAction("Status", "UserStatus");
        }

        //Post & Update
        [HttpPost]
        public ActionResult DemandDraft(DemandDraft demanddraft)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
                return View(demanddraft);
            if (demanddraft.ImageFile == null || demanddraft.ImageFile.ContentLength <= 0)
            {
                ViewBag.DDImageNull = "Please Choose the Image Of Demand Draft";
                return View();
            }
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(demanddraft.ImageFile.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                return View();
            }
            if (demanddraft.ImageFile.ContentLength > (1024 * 300))
            {
                ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                return View();
            }
            string FileName = Path.GetFileNameWithoutExtension(demanddraft.ImageFile.FileName);

            //Converting into Bytes
            try
            {
                Stream fs = demanddraft.ImageFile.InputStream;
                BinaryReader br = new BinaryReader(fs);
                demanddraft.Draft_Scan = br.ReadBytes((Int32)fs.Length);
                demanddraft.FileName = Path.GetFileName(demanddraft.ImageFile.FileName);

                if (demanddraft.Id == 0)
                {
                    db.BI_Affi_insert_Draft_detail(Convert.ToInt32(obj.Institute_Id), Convert.ToInt32(obj.Id_year_for_affilation), Convert.ToString(demanddraft.bank_name), Convert.ToString(demanddraft.bank_address), Convert.ToString(demanddraft.branch_code), Convert.ToString(demanddraft.amount), Convert.ToDateTime(demanddraft.deposite_date), demanddraft.Draft_Scan, Convert.ToString(demanddraft.FileName));
                    db.SaveChanges();
                }
                else if (demanddraft.Id != 0)
                {

                }
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

        //Update View
        [HttpGet]
        public ActionResult DemandDraftUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                DemandDraft dd = new DemandDraft();
                var dbdemand = db.Affi_ins_Draft_Detail.Single(d => d.ID == id && d.inst_id == Convert.ToInt32(obj.Institute_Id));
                if (dbdemand != null)
                {
                    dd.amount = Convert.ToInt32(dbdemand.Draft_amount);
                    dd.bank_address = dbdemand.Bank_Address;
                    dd.bank_name = dbdemand.Bank_Name;
                    dd.branch_code = dbdemand.Bank_branch_code;
                    dd.deposite_date = Convert.ToDateTime(dbdemand.Deposit_Date);
                    dd.Id = dbdemand.ID;
                }
                return View(dd);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        //DataTable D Draft
        public JsonResult DDraftDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                try
            {
                obj = GetData();
                var demandDraft = db.TA_Pop_Draft_Detail(Convert.ToInt32(obj.Institute_Id));
                List<DemandDraft> list = new List<DemandDraft>();
                foreach (var ddobject in demandDraft)
                {
                    DemandDraft ddc = new DemandDraft();
                    ddc.bank_name = ddobject.Bank_Name;
                    ddc.branch_code = ddobject.Bank_branch_code;
                    ddc.amount = Convert.ToInt32(ddobject.Draft_amount);
                    ddc.bank_address = ddobject.Bank_Address;
                    ddc.deposite_date = Convert.ToDateTime(ddobject.Deposit_Date);

                    //Converting Bytes to File and string it into the directory of Server
                    string filePath = "~/Images/" + ddobject.ScanName;

                    System.IO.File.WriteAllBytes(Server.MapPath(filePath), ddobject.Draft_Scan);
                    //string deployedFolder = new DirectoryInfo(Environment.CurrentDirectory).Name;
                    string servername = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/");
                    ddc.FileName = servername + "/Images/" + ddobject.ScanName;
                    list.Add(ddc);
                }

                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, responseText = "Error In Finding scan Try Again Or Update it" }, JsonRequestBehavior.AllowGet);
            }
            }
        }

        //College Owner
        [HttpGet]
        public ActionResult CollegeOwner()
        {
            if (IsRoll())
            {
                return View();
            }
            return RedirectToAction("Status", "UserStatus");
        }

        [HttpGet]
        public ActionResult CollegeOwnerUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                var obj = GetData();
                var instId = Convert.ToInt32(obj.Institute_Id);
                var cowner = db.Affi_Int_Owner_Info.Single(co => co.ID == id && co.Int_ID == instId);
                CollegeOwner owner = new CollegeOwner();
                if (cowner != null)
                {
                    owner.Id = cowner.ID;
                    owner.cnic = cowner.Owner_Cnic;
                    owner.contact = cowner.Owner_Contact;
                    owner.owner_name = cowner.Int_Owner_Name;
                }
                return View(owner);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        [HttpPost]
        public ActionResult CollegeOwner(CollegeOwner model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data Please Try Again";
                return RedirectToAction("CollegeOwner", "UserAccount");
            }
            try
            {
                if (model.Id == 0)
                {
                    var ownerInfo = db.BI_Affi_pop_Owner_Info(Convert.ToInt32(obj.Institute_Id));
                    foreach (var owner in ownerInfo)
                    {
                        if (owner.Owner_Cnic == model.cnic)
                        {
                            TempData["CNICError"] = "User With This CNIC Already Exists";
                            return View(model);
                        }
                    }
                    db.BI_Affi_Insert_Owner_Info(Convert.ToInt32(obj.Institute_Id), model.owner_name, model.cnic, model.contact);
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }
                else if (model.Id != 0)
                {
                    var ownerInfo = db.BI_Affi_pop_Owner_Info(Convert.ToInt32(obj.Institute_Id));
                    foreach (var owner in ownerInfo)
                    {
                        if (owner.Owner_Cnic == model.cnic && owner.ID != model.Id)
                        {
                            TempData["CNICError"] = "User With This CNIC Already Exists";
                            return View();
                        }
                    }
                    db.BI_Affi_Update_Owner_Info(model.Id, Convert.ToInt32(obj.Institute_Id), model.owner_name, model.cnic, model.contact);
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
                return RedirectToAction("CollegeOwner", "UserAccount");
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Deletion Please Try Again";
                return View("CollegeOwner", "UserAccount");
            }
            }
        }

        public JsonResult COwnerDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                var obj = GetData();
            var ownerInfo = db.BI_Affi_pop_Owner_Info(Convert.ToInt32(obj.Institute_Id));
            List<CollegeOwner> ownerlist = new List<CollegeOwner>();
            foreach (var owner in ownerInfo)
            {
                CollegeOwner co = new CollegeOwner();
                co.owner_name = owner.Owner_Name;
                co.contact = owner.Owner_Contact;
                co.cnic = owner.Owner_Cnic;
                co.Id = owner.ID;
                ownerlist.Add(co);
            }
            return Json(ownerlist, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteOwnerInfo(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_Owner_Info(id, Convert.ToInt32(obj.Institute_Id));
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //Building Detail
        [HttpGet]
        public ActionResult BuildingDetail()
        {
            if (IsRoll())
            {
                return View();
            }
            return RedirectToAction("Status", "UserStatus");
        }

        [HttpPost]
        public ActionResult BuildingDetail(BuildingDetail model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data Please Try Again";
                return RedirectToAction("BuildingDetail", "UserAccount");
            }
            if (model.ImageFile == null || model.ImageFile.ContentLength <= 0)
            {
                ViewBag.DDImageNull = "Please Choose the Image Of Building Maps";
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

            try
            {
                //Converting into Bytes
                Stream fs = model.ImageFile.InputStream;
                BinaryReader br = new BinaryReader(fs);
                model.map = br.ReadBytes((Int32)fs.Length);
                model.FileName = Path.GetFileName(model.ImageFile.FileName);

                if (model.Id == 0)
                {
                    db.BI_Affi_Insert_Building_Info(Convert.ToInt32(obj.Institute_Id), model.building_name, Convert.ToDouble(model.c_area), Convert.ToDouble(model.uc_area), model.map, model.FileName);
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }
                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_Building_Info(model.Id, model.building_name, Convert.ToDouble(model.c_area), Convert.ToDouble(model.uc_area));
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
                return RedirectToAction("BuildingDetail", "UserAccount");
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
                return RedirectToAction("BuildingDetail", "UserAccount");
            }
            }
        }

        public JsonResult ShowBuildingMap(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                try
            {
                var img = db.BI_Affi__ins_pop_building_map(Convert.ToInt32(id));

                string filePath = "";
                var filename = "";
                foreach (var i in img)
                {
                    filePath = "~/Images/" + i.imageName;
                    filename = i.imageName;
                    System.IO.File.WriteAllBytes(Server.MapPath(filePath), i.Inst_buld_Doc);
                }
                string servername = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + Url.Content("~/");
                var FileName = servername + "/Images/" + filename;
                return Json(FileName, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, responseText = "Error In Finding Your Building Map Try Again Or Update it" }, JsonRequestBehavior.AllowGet);
            }
            }
        }

        public JsonResult BDetailDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var dbInfo = db.BI_Pop_Building_Info(Convert.ToInt32(obj.Institute_Id));
            List<BuildingDetail> list = new List<BuildingDetail>();
            foreach (var info in dbInfo)
            {
                BuildingDetail bd = new BuildingDetail();
                bd.building_name = info.Building_Name;
                bd.c_area = Convert.ToString(info.Coverd_Area);
                bd.uc_area = Convert.ToString(info.UnCoverd_Area);
                bd.Id = Convert.ToInt32(info.ID);
                list.Add(bd);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveBuildingDetail(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Delete_Building_Info(Convert.ToInt32(id));
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult BuildingDetailUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                BuildingDetail bd = new BuildingDetail();
                var instid = Convert.ToInt32(obj.Institute_Id);
                var dbbuilding = db.Affil_Ins_Bulid_Info.Single(b => b.ID == id && b.Int_ID == instid);
                if (dbbuilding != null)
                {
                    bd.c_area = Convert.ToString(dbbuilding.Int_Buld_Cov_Area);
                    bd.uc_area = Convert.ToString(dbbuilding.Int_Buld_Uncov_Area);
                    bd.Id = Convert.ToInt32(dbbuilding.ID);
                    bd.building_name = dbbuilding.Int_Buld_Name;
                }
                return View(bd);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        //Room Detail
        [HttpGet]
        public ActionResult RoomDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var result = db.BI_Pop_Building_Info(Convert.ToInt32(obj.Institute_Id));
                RoomDetail rd = new RoomDetail();
                List<BuildingDetail> listbd = new List<BuildingDetail>();
                foreach (var rs in result)
                {
                    BuildingDetail bd = new BuildingDetail();
                    bd.Id = Convert.ToInt32(rs.ID);
                    bd.building_name = rs.Building_Name;
                    listbd.Add(bd);
                }
                rd.BuildingDetail = listbd;
                return View(rd);
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }

        [HttpPost]
        public ActionResult RoomDetail(RoomDetail model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Invalid Data Please Try Again";
                return RedirectToAction("RoomDetail", "UserAccount");
            }
            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_Insert_Rooms_Detail(Convert.ToInt32(obj.Institute_Id), model.building_id, model.room_type, Convert.ToDouble(model.length), Convert.ToDouble(model.width), Convert.ToInt32(model.quantity), model.remarks);
                    TempData["Success"] = "Inserted Successfully";
                }

                else if (model.Id != 0)
                {
                    db.BI_UPDATE_ROOMS_DETAIL(Convert.ToInt32(obj.Institute_Id), model.Id, model.building_id, model.room_type, Convert.ToDouble(model.length), Convert.ToDouble(model.width), model.quantity, model.remarks);
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Update / Insert Please Try Again";
            }

            return RedirectToAction("RoomDetail", "UserAccount");
            }
        }

        public JsonResult RDetailDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var rooms = db.BI_POP_ROOMS_INFO(Convert.ToInt32(obj.Institute_Id));
            List<RoomDetail> list = new List<RoomDetail>();
            foreach (var room in rooms)
            {
                RoomDetail rd = new RoomDetail();
                rd.room_type = room.Type;
                rd.length = Convert.ToInt32(room.Length);
                rd.width = Convert.ToInt32(room.Width);
                rd.quantity = Convert.ToInt32(room.Quantity);
                rd.remarks = room.Remarks;
                rd.building_id = Convert.ToInt32(room.Building_ID);
                rd.BuidingName = room.Building_Name;
                rd.Id = Convert.ToInt32(room.Room_ID);
                list.Add(rd);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveRoomDetail(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Remove_ROOMS_DETAIL(Convert.ToInt32(obj.Institute_Id), id);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadBuldingDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_BuldingDetails.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_POP_Bulding_INFO", db.AJ_POP_Bulding_INFO(Convert.ToInt32(obj.Institute_Id))));
            DownloadPDF_File(rpt.Render("PDF"));
            return View();
            }
        }

        void DownloadPDF_File(byte[] bytes)
        {
            Response.Buffer = true;
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment; filename= Document" + "." + "PDF");
            Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
            Response.Flush(); // send it to the client to download  
            Response.End();
        }

        [HttpGet]
        public ActionResult RoomDetailUpdate(int id)
        {

            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                //here is to get the details by creating store procedure for id
                var dbroom = db.BI_POP_ROW_F_ROOMS_INFO(Convert.ToInt32(obj.Institute_Id), id);
                var result = db.BI_Pop_Building_Info(Convert.ToInt32(obj.Institute_Id));
                RoomDetail rd = new RoomDetail();
                foreach (var room in dbroom)
                {
                    rd.Id = Convert.ToInt32(room.Room_ID);
                    rd.length = Convert.ToInt32(room.Length);
                    rd.width = Convert.ToInt32(room.Width);
                    rd.room_type = room.Type;
                    rd.quantity = Convert.ToInt32(room.Quantity);
                    rd.remarks = room.Remarks;
                }
                List<BuildingDetail> listbd = new List<BuildingDetail>();
                foreach (var rs in result)
                {
                    BuildingDetail bd = new BuildingDetail();
                    bd.Id = Convert.ToInt32(rs.ID);
                    bd.building_name = rs.Building_Name;
                    listbd.Add(bd);
                }
                rd.BuildingDetail = listbd;
                return View(rd);
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }

        //Sports Detail
        [HttpGet]
        public ActionResult SportsDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var sports = db.BI_POP_ALL_SPORTS();
                List<BI_POP_ALL_SPORTS_Result> list = new List<BI_POP_ALL_SPORTS_Result>();
                foreach (var sport in sports)
                {
                    BI_POP_ALL_SPORTS_Result sr = new BI_POP_ALL_SPORTS_Result();
                    sr.Sport_ID = sport.Sport_ID;
                    sr.Sport_Name = sport.Sport_Name;
                    list.Add(sr);
                }
                SportsDetail sd = new SportsDetail();
                sd.Sports = list;
                return View(sd);
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }
        [HttpPost]
        public ActionResult SportsDetail(SportsDetail model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data Please Try Again";
                return RedirectToAction("SportsDetail", "UserAccount");
            }
            try
            {
                if (model.SportsId == 0)
                {
                    db.BI_Affi_Insert_Sport_Detail(Convert.ToInt32(obj.Institute_Id), model.sports_name, model.item_name, model.quantity, model.remarks);
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }

                else if (model.SportsId != 0)
                {
                    db.BI_Affi_Update_Sport_Detail(Convert.ToInt32(obj.Institute_Id), model.SportsId, model.sports_name, model.item_name, model.quantity, model.remarks);
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
                return RedirectToAction("SportsDetail", "UserAccount");
            }

            return RedirectToAction("SportsDetail", "UserAccount");
            }
        }

        public JsonResult SDetailDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
                var sportsDetail = (db.BI_POP_SPORT_DETAIL(Convert.ToInt32(obj.Institute_Id))).ToList();
            return Json(sportsDetail, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadSportDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            int Inst_ID = Convert.ToInt32(obj.Institute_Id);
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_SportFculty.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Rep_SportFacilities_INFO", db.AJ_Rep_SportFacilities_INFO(Convert.ToInt32(Inst_ID))));

            DownloadPDF_File(rpt.Render("PDF"));
            return View();
            }
        }

        [HttpGet]
        public ActionResult SportsDetailUpdate(int id)
        {
            using (UOSEntities db  = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                SportsDetail sd = new SportsDetail();
                var dbsport = db.BI_POP_ROW_F_SPORT_DETAIL(Convert.ToInt32(obj.Institute_Id), id);
                foreach (var sport in dbsport)
                {
                    sd.item_name = sport.Item_Name;
                    sd.quantity = Convert.ToInt32(sport.Item_Quantity);
                    sd.remarks = sport.Remarks;
                    sd.SportsId = Convert.ToInt32(sport.Sport_Id);

                }

                var sports = db.BI_POP_ALL_SPORTS();
                List<BI_POP_ALL_SPORTS_Result> list = new List<BI_POP_ALL_SPORTS_Result>();
                foreach (var sport in sports)
                {
                    BI_POP_ALL_SPORTS_Result sr = new BI_POP_ALL_SPORTS_Result();
                    sr.Sport_ID = sport.Sport_ID;
                    sr.Sport_Name = sport.Sport_Name;
                    list.Add(sr);
                }

                sd.Sports = list;
                return View(sd);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        public ActionResult RemoveSportsDetail(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Delete_Sport_Detail(Convert.ToInt32(obj.Institute_Id), id);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult TeachingStaff()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                List<YearPopulate> listyear = new List<YearPopulate>();
                for (var i = 0; i < 60; i++)
                {
                    int currentyear = DateTime.Now.Year;
                    YearPopulate year = new YearPopulate();
                    if (i == 0)
                    {
                        year.Id = i + 1;
                        year.Year = Convert.ToString(currentyear);
                    }
                    else
                    {
                        year.Id = i + 1;
                        year.Year = Convert.ToString(currentyear - i);
                    }

                    listyear.Add(year);
                }

                var degree = db.BI_POP_All_Degree();
                List<BI_POP_All_Degree_Result> listdegree = new List<BI_POP_All_Degree_Result>();
                foreach (var dg in degree)
                {
                    BI_POP_All_Degree_Result dr = new BI_POP_All_Degree_Result();
                    dr.ID = dg.ID;
                    dr.Degree_Description = dg.Degree_Description;
                    listdegree.Add(dr);
                }
                var designation = (db.BI_POP_Designation_Teaching_Staff()).ToList();
                List<BI_POP_Designation_Teaching_Staff_Result> listdesignation = new List<BI_POP_Designation_Teaching_Staff_Result>();
                foreach (var dg in designation)
                {
                    BI_POP_Designation_Teaching_Staff_Result dts = new BI_POP_Designation_Teaching_Staff_Result();
                    dts.ID = dg.ID;
                    dts.Designation = dg.Designation;
                    listdesignation.Add(dts);
                }

                var nature = db.BI_pop_staff_appoint_nature();
                List<BI_pop_staff_appoint_nature_Result> listnature = new List<BI_pop_staff_appoint_nature_Result>();
                foreach (var dg in nature)
                {
                    BI_pop_staff_appoint_nature_Result nat = new BI_pop_staff_appoint_nature_Result();
                    nat.ID = dg.ID;
                    nat.Nature_Desc = dg.Nature_Desc;
                    listnature.Add(nat);
                }

                var experience = db.BI_POP_Duration_Experience_Teaching_Staff();
                List<BI_POP_Duration_Experience_Teaching_Staff_Result> listexperience = new List<BI_POP_Duration_Experience_Teaching_Staff_Result>();
                foreach (var dg in experience)
                {
                    BI_POP_Duration_Experience_Teaching_Staff_Result de = new BI_POP_Duration_Experience_Teaching_Staff_Result();
                    de.ID = dg.ID;
                    de.Experience_Duration = dg.Experience_Duration;
                    listexperience.Add(de);
                }

                TeachingStaff ts = new TeachingStaff()
                {
                    Degree = listdegree,
                    Nature = listnature,
                    Designation = listdesignation,
                    Experience = listexperience,
                    Year = listyear
                };
                return View(ts);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        public ActionResult DownloadTeachingDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            int Inst_ID = Convert.ToInt32(obj.Institute_Id);
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_Staff_Info.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Employee_INFO", db.AJ_Rep_Employee_INFO(Convert.ToInt32(Inst_ID), 1)));

            DownloadPDF_File(rpt.Render("PDF"));
            return View();
            }
        }

        public ActionResult DownloadNon_TeachingDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            int Inst_ID = Convert.ToInt32(obj.Institute_Id);
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_Staff_Info.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Employee_INFO", db.AJ_Rep_Employee_INFO(Convert.ToInt32(Inst_ID), 0)));

            DownloadPDF_File(rpt.Render("PDF"));
            return View();
            }
        }

        [HttpPost]
        public ActionResult TeachingStaff(TeachingStaff model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                   obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data Please Try Again";
                return RedirectToAction("TeachingStaff", "UserAccount");
            }

            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_Insert_Staff_Detail(Convert.ToInt32(obj.Institute_Id), model.name, Convert.ToString(1), Convert.ToString(model.degreeName), Convert.ToString(model.subject), Convert.ToString(model.pass_yearname), Convert.ToString(model.experiencename), Convert.ToString(model.uni), model.designationname, Convert.ToString(model.naturename), Convert.ToString(model.salary), Convert.ToDateTime(model.appoint_date), Convert.ToString(model.remarks));
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }
                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_Staff_Detail(Convert.ToInt32(obj.Institute_Id), model.Id, model.name, Convert.ToString(1), Convert.ToString(model.degreeName), Convert.ToString(model.subject), Convert.ToString(model.pass_yearname), Convert.ToString(model.experiencename), Convert.ToString(model.degreeName), model.designationname, Convert.ToString(model.naturename), Convert.ToString(model.salary), Convert.ToDateTime(model.appoint_date), Convert.ToString(model.remarks));
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
            }
            return RedirectToAction("TeachingStaff", "UserAccount");
            }
        }

        public JsonResult TStaffDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var staff = db.BI_POP_Teaching_staff(Convert.ToInt32(obj.Institute_Id), Convert.ToString(1)).ToList();
            return Json(staff, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveTeachingStaff(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_Staff_Detail(Convert.ToInt32(obj.Institute_Id), id);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult TeachingStaffUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                List<YearPopulate> listyear = new List<YearPopulate>();
                for (var i = 0; i < 60; i++)
                {
                    int currentyear = DateTime.Now.Year;
                    YearPopulate year = new YearPopulate();
                    if (i == 0)
                    {
                        year.Id = i + 1;
                        year.Year = Convert.ToString(currentyear);
                    }
                    else
                    {
                        year.Id = i + 1;
                        year.Year = Convert.ToString(currentyear - i);
                    }

                    listyear.Add(year);
                }

                var degree = db.BI_POP_All_Degree();
                List<BI_POP_All_Degree_Result> listdegree = new List<BI_POP_All_Degree_Result>();
                foreach (var dg in degree)
                {
                    BI_POP_All_Degree_Result dr = new BI_POP_All_Degree_Result();
                    dr.ID = dg.ID;
                    dr.Degree_Description = dg.Degree_Description;
                    listdegree.Add(dr);
                }
                var designation = db.BI_POP_Designation_Teaching_Staff();
                List<BI_POP_Designation_Teaching_Staff_Result> listdesignation = new List<BI_POP_Designation_Teaching_Staff_Result>();
                foreach (var dg in designation)
                {
                    BI_POP_Designation_Teaching_Staff_Result dts = new BI_POP_Designation_Teaching_Staff_Result();
                    dts.ID = dg.ID;
                    dts.Designation = dg.Designation;
                    listdesignation.Add(dts);
                }

                var nature = db.BI_pop_staff_appoint_nature();
                List<BI_pop_staff_appoint_nature_Result> listnature = new List<BI_pop_staff_appoint_nature_Result>();
                foreach (var dg in nature)
                {
                    BI_pop_staff_appoint_nature_Result nat = new BI_pop_staff_appoint_nature_Result();
                    nat.ID = dg.ID;
                    nat.Nature_Desc = dg.Nature_Desc;
                    listnature.Add(nat);
                }

                var experience = db.BI_POP_Duration_Experience_Teaching_Staff();
                List<BI_POP_Duration_Experience_Teaching_Staff_Result> listexperience = new List<BI_POP_Duration_Experience_Teaching_Staff_Result>();
                foreach (var dg in experience)
                {
                    BI_POP_Duration_Experience_Teaching_Staff_Result de = new BI_POP_Duration_Experience_Teaching_Staff_Result();
                    de.ID = dg.ID;
                    de.Experience_Duration = dg.Experience_Duration;
                    listexperience.Add(de);
                }

                TeachingStaff ts = new TeachingStaff()
                {
                    Degree = listdegree,
                    Nature = listnature,
                    Designation = listdesignation,
                    Experience = listexperience,
                    Year = listyear
                };

                var dbTeacher = db.BI_POP_ROW_F_Teaching_staff(Convert.ToInt32(obj.Institute_Id), "1", id);
                foreach (var teacher in dbTeacher)
                {
                    // ts.appoint_date = Convert.ToDateTime(teacher.Date_Of_Appointment);
                    ts.Id = Convert.ToInt32(teacher.Employee_ID);
                    ts.name = teacher.Name;
                    ts.remarks = teacher.Remarks;
                    ts.salary = Convert.ToInt32(teacher.Salary);
                    ts.subject = teacher.Subject;
                    ts.uni = teacher.Institute;
                }

                return View(ts);
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }

        //Non Teaching
        [HttpGet]
        public ActionResult NonTeachingStaff()
        {
            using (UOSEntities db = new UOSEntities())
            {
                 if (IsRoll())
            {
                obj = GetData();
                List<YearPopulate> listyear = new List<YearPopulate>();
                for (var i = 0; i < 60; i++)
                {
                    int currentyear = DateTime.Now.Year;
                    YearPopulate year = new YearPopulate();
                    if (i == 0)
                    {
                        year.Id = i + 1;
                        year.Year = Convert.ToString(currentyear);
                    }
                    else
                    {
                        year.Id = i + 1;
                        year.Year = Convert.ToString(currentyear - i);
                    }

                    listyear.Add(year);
                }

                var degree = db.BI_POP_All_Degree();
                List<BI_POP_All_Degree_Result> listdegree = new List<BI_POP_All_Degree_Result>();
                foreach (var dg in degree)
                {
                    BI_POP_All_Degree_Result dr = new BI_POP_All_Degree_Result();
                    dr.ID = dg.ID;
                    dr.Degree_Description = dg.Degree_Description;
                    listdegree.Add(dr);
                }
                var designation = (db.BI_POP_Designation_Teaching_Staff()).ToList();
                List<BI_POP_Designation_Teaching_Staff_Result> listdesignation = new List<BI_POP_Designation_Teaching_Staff_Result>();
                foreach (var dg in designation)
                {
                    BI_POP_Designation_Teaching_Staff_Result dts = new BI_POP_Designation_Teaching_Staff_Result();
                    dts.ID = dg.ID;
                    dts.Designation = dg.Designation;
                    listdesignation.Add(dts);
                }

                var nature = db.BI_pop_staff_appoint_nature();
                List<BI_pop_staff_appoint_nature_Result> listnature = new List<BI_pop_staff_appoint_nature_Result>();
                foreach (var dg in nature)
                {
                    BI_pop_staff_appoint_nature_Result nat = new BI_pop_staff_appoint_nature_Result();
                    nat.ID = dg.ID;
                    nat.Nature_Desc = dg.Nature_Desc;
                    listnature.Add(nat);
                }

                var experience = db.BI_POP_Duration_Experience_Teaching_Staff();
                List<BI_POP_Duration_Experience_Teaching_Staff_Result> listexperience = new List<BI_POP_Duration_Experience_Teaching_Staff_Result>();
                foreach (var dg in experience)
                {
                    BI_POP_Duration_Experience_Teaching_Staff_Result de = new BI_POP_Duration_Experience_Teaching_Staff_Result();
                    de.ID = dg.ID;
                    de.Experience_Duration = dg.Experience_Duration;
                    listexperience.Add(de);
                }

                NonTeachingStaff nts = new NonTeachingStaff()
                {
                    Degree = listdegree,
                    Nature = listnature,
                    Designation = listdesignation,
                    Experience = listexperience,
                    Year = listyear
                };
                return View(nts);
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }

        [HttpGet]
        public ActionResult NonTeachingStaffUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                 if (IsRoll())
            {
                obj = GetData();
                List<YearPopulate> listyear = new List<YearPopulate>();
                for (var i = 0; i < 60; i++)
                {
                    int currentyear = DateTime.Now.Year;
                    YearPopulate year = new YearPopulate();
                    if (i == 0)
                    {
                        year.Id = i + 1;
                        year.Year = Convert.ToString(currentyear);
                    }
                    else
                    {
                        year.Id = i + 1;
                        year.Year = Convert.ToString(currentyear - i);
                    }

                    listyear.Add(year);
                }

                var degree = db.BI_POP_All_Degree();
                List<BI_POP_All_Degree_Result> listdegree = new List<BI_POP_All_Degree_Result>();
                foreach (var dg in degree)
                {
                    BI_POP_All_Degree_Result dr = new BI_POP_All_Degree_Result();
                    dr.ID = dg.ID;
                    dr.Degree_Description = dg.Degree_Description;
                    listdegree.Add(dr);
                }
                var designation = db.BI_POP_Designation_Teaching_Staff();
                List<BI_POP_Designation_Teaching_Staff_Result> listdesignation = new List<BI_POP_Designation_Teaching_Staff_Result>();
                foreach (var dg in designation)
                {
                    BI_POP_Designation_Teaching_Staff_Result dts = new BI_POP_Designation_Teaching_Staff_Result();
                    dts.ID = dg.ID;
                    dts.Designation = dg.Designation;
                    listdesignation.Add(dts);
                }

                var nature = db.BI_pop_staff_appoint_nature();
                List<BI_pop_staff_appoint_nature_Result> listnature = new List<BI_pop_staff_appoint_nature_Result>();
                foreach (var dg in nature)
                {
                    BI_pop_staff_appoint_nature_Result nat = new BI_pop_staff_appoint_nature_Result();
                    nat.ID = dg.ID;
                    nat.Nature_Desc = dg.Nature_Desc;
                    listnature.Add(nat);
                }

                var experience = db.BI_POP_Duration_Experience_Teaching_Staff();
                List<BI_POP_Duration_Experience_Teaching_Staff_Result> listexperience = new List<BI_POP_Duration_Experience_Teaching_Staff_Result>();
                foreach (var dg in experience)
                {
                    BI_POP_Duration_Experience_Teaching_Staff_Result de = new BI_POP_Duration_Experience_Teaching_Staff_Result();
                    de.ID = dg.ID;
                    de.Experience_Duration = dg.Experience_Duration;
                    listexperience.Add(de);
                }

                NonTeachingStaff nts = new NonTeachingStaff()
                {
                    Degree = listdegree,
                    Nature = listnature,
                    Designation = listdesignation,
                    Experience = listexperience,
                    Year = listyear
                };

                var dbTeacher = db.BI_POP_ROW_F_Teaching_staff(Convert.ToInt32(obj.Institute_Id), "0", id);
                foreach (var teacher in dbTeacher)
                {
                    // nts.appoint_date = Convert.ToDateTime(teacher.Date_Of_Appointment);
                    nts.Id = Convert.ToInt32(teacher.Employee_ID);
                    nts.name = teacher.Name;
                    nts.remarks = teacher.Remarks;
                    nts.salary = Convert.ToInt32(teacher.Salary);
                    nts.subject = teacher.Subject;
                    nts.uni = teacher.Institute;
                }
                return View(nts);
            }
            return RedirectToAction("NonTeachingStaff", "UserAccount");
            }

        }

        [HttpPost]
        public ActionResult NonTeachingStaff(NonTeachingStaff model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("NonTeachingStaff", "UserAccount");
            }
            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_Insert_Staff_Detail(Convert.ToInt32(obj.Institute_Id), model.name, Convert.ToString(0), Convert.ToString(model.degreename), Convert.ToString(model.subject), Convert.ToString(model.pass_yearname), Convert.ToString(model.experiencename), Convert.ToString(model.uni), model.designationname, Convert.ToString(model.naturename), Convert.ToString(model.salary), Convert.ToDateTime(model.appoint_date), Convert.ToString(model.remarks));
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }
                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_Staff_Detail(Convert.ToInt32(obj.Institute_Id), model.Id, model.name, Convert.ToString(0), Convert.ToString(model.degreename), Convert.ToString(model.subject), Convert.ToString(model.pass_yearname), Convert.ToString(model.experiencename), Convert.ToString(model.uni), model.designationname, Convert.ToString(model.naturename), Convert.ToString(model.salary), Convert.ToDateTime(model.appoint_date), Convert.ToString(model.remarks));
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
            }
            return RedirectToAction("NonTeachingStaff", "UserAccount");
            }
        }

        public ActionResult RemoveNonTeachingStaff(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_Staff_Detail(Convert.ToInt32(obj.Institute_Id), id);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult NTStaffDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var staff = db.BI_POP_Teaching_staff(Convert.ToInt32(obj.Institute_Id), Convert.ToString(0)).ToList();
            return Json(staff, JsonRequestBehavior.AllowGet);
            }
        }

        //Labortary
        [HttpGet]
        public ActionResult labortary()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var dblabortary = db.affi_ins_pop_all_subject_lab().ToList();
                List<affi_ins_pop_all_subject_lab_Result> lablist = new List<affi_ins_pop_all_subject_lab_Result>();
                foreach (var lab in dblabortary)
                {
                    affi_ins_pop_all_subject_lab_Result res = new affi_ins_pop_all_subject_lab_Result();
                    res.Lab_Description = lab.Lab_Description;
                    res.ID = lab.ID;
                    lablist.Add(res);
                }
                labortary labcls = new labortary()
                {
                    LabSubject = lablist
                };
                return View(labcls);
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }

        public JsonResult LabDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var dblab = db.BI_Affi_Pop_LaboratryDetail(Convert.ToInt32(obj.Institute_Id)).ToList();
            return Json(dblab, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveLabortaryDetail(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_LaboratryDetail(id, Convert.ToInt32(obj.Institute_Id));
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadLabortaryDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            int Inst_ID = Convert.ToInt32(obj.Institute_Id);
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_inst_AJ_Rep_Laboratory_INFO.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Laboratory_INFO", db.AJ_Rep_Laboratory_INFO(Convert.ToInt32(Inst_ID))));

            DownloadPDF_File(rpt.Render("PDF"));
            return View();
            }
        }

        [HttpPost]
        public ActionResult labortary(labortary model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("labortary", "UserAccount");
            }

            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_insert_LaboratryDetail(Convert.ToInt32(obj.Institute_Id), model.sbjct_labname, model.item_name, model.quantity, model.remarks);
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }
                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_LaboratryDetail(model.Id, Convert.ToInt32(obj.Institute_Id), model.sbjct_labname, model.item_name, model.quantity, model.remarks);
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
            }
            return RedirectToAction("labortary", "UserAccount");
            }
        }

        [HttpGet]
        public ActionResult labortaryUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var dblabortary = db.affi_ins_pop_all_subject_lab();
                List<affi_ins_pop_all_subject_lab_Result> lablist = new List<affi_ins_pop_all_subject_lab_Result>();
                foreach (var lab in dblabortary)
                {
                    affi_ins_pop_all_subject_lab_Result res = new affi_ins_pop_all_subject_lab_Result();
                    res.Lab_Description = lab.Lab_Description;
                    res.ID = lab.ID;
                    lablist.Add(res);
                }
                labortary labcls = new labortary()
                {
                    LabSubject = lablist
                };
                var dblab = db.BI_Affi_Pop_ROW_F_LaboratryDetail(Convert.ToInt32(obj.Institute_Id), id);
                foreach (var lab in dblab)
                {
                    labcls.Id = Convert.ToInt32(lab.ID);
                    labcls.item_name = lab.Item_Name;
                    labcls.quantity = Convert.ToInt32(lab.Quantity);
                    labcls.remarks = lab.Remarks;
                }
                return View(labcls);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        //Non Recurring
        [HttpGet]
        public ActionResult NonRecurring()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var nrr = db.Affi_ins_Non_Recuring_All_Recipts.ToList();
                var nre = db.Affi_ins_Non_Recuring_all_expences.ToList();
                NonRecurrClass nrc = new NonRecurrClass()
                {
                    ReceiptDD = nrr,
                    ExpenditureDD = nre
                };
                return View(nrc);
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }

        [HttpPost]
        public ActionResult NonRecurrRecipts(NonRecurrClass model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("NonRecurring", "UserAccount");
            }
            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_insert_Recipts(Convert.ToInt32(obj.Institute_Id), model.s_receiptname, Convert.ToString(model.amount), false);
                    TempData["Success"] = "Inserted Successfully";
                }

                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_Recipts(model.Id, Convert.ToInt32(obj.Institute_Id), model.s_receiptname, Convert.ToString(model.amount), false);
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
            }
            return RedirectToAction("NonRecurring", "UserAccount");
            }
        }

        public ActionResult RemoveNRRecipt(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_Recipts(id, Convert.ToInt32(obj.Institute_Id), false);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult NRRecpDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var recp = (db.BI_Affi_pop_recipt(Convert.ToInt32(obj.Institute_Id), false)).ToList();
            return Json(recp, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult NRExpDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var exp = (db.BI_Affi_pop_Expence(Convert.ToInt32(obj.Institute_Id), false)).ToList();
            return Json(exp, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveNRExpenditure(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_expence(id, Convert.ToInt32(obj.Institute_Id), false);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult NonRecurrExpenditure(NonRecurrClass model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                 obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("NonRecurring", "UserAccount");
            }
            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_insert_Expence(Convert.ToInt32(obj.Institute_Id), model.s_expenditurename, Convert.ToString(model.amount), false);
                    TempData["Success"] = "Inserted Successfully";
                }
                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_Expence(model.Id, Convert.ToInt32(obj.Institute_Id), model.s_expenditurename, Convert.ToString(model.amount), false);
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
            }
            return RedirectToAction("NonRecurring", "UserAccount");
            }
        }

        public ActionResult DownloadNon_requirDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            int Inst_ID = Convert.ToInt32(obj.Institute_Id);
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_ins_Non_Recuring_INFO.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Non_Recuring_INFO", db.AJ_Rep_Non_Recuring_INFO(Convert.ToInt32(Inst_ID))));

            DownloadPDF_File(rpt.Render("PDF"));
            return View();
            }
        }

        [HttpGet]
        public ActionResult NonRecurrExpenditureUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var nre = db.Affi_ins_Non_Recuring_all_expences.ToList();
                NonRecurrClass nrc = new NonRecurrClass()
                {
                    ExpenditureDD = nre
                };
                var dbexpense = db.BI_Affi_ROW_F_pop_Expence(Convert.ToInt32(obj.Institute_Id), false, id);
                foreach (var expense in dbexpense)
                {
                    nrc.Id = expense.Expence_ID;
                    nrc.amount = Convert.ToInt32(expense.Amount);
                }
                return View(nrc);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        [HttpGet]
        public ActionResult NonRecurrReceiptsUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var nrr = db.Affi_ins_Non_Recuring_All_Recipts.ToList();
                NonRecurrClass nrc = new NonRecurrClass()
                {
                    ReceiptDD = nrr
                };
                var dbrecipt = db.BI_Affi_pop_ROW_F_recipt(Convert.ToInt32(obj.Institute_Id), false, id);
                foreach (var recipt in dbrecipt)
                {
                    nrc.Id = recipt.Recipt_ID;
                    nrc.amount = Convert.ToInt32(recipt.Amount);
                }
                return View(nrc);
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }

        //Recurring
        public ActionResult DownloadRequirDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            int Inst_ID = Convert.ToInt32(obj.Institute_Id);
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_ins_Recuring_INFO.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Recuring_INFO", db.AJ_Rep_Recuring_INFO(Convert.ToInt32(Inst_ID))));

            DownloadPDF_File(rpt.Render("PDF"));
            return View();
            }
        }


        [HttpGet]
        public ActionResult Recurring()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var nrr = db.Affi_ins_Recuring_All_recipts.ToList();
                var nre = db.Affi_ins_Recuring_All_Expence.ToList();
                RecurrClass nrc = new RecurrClass()
                {
                    ReceiptDD = nrr,
                    ExpenditureDD = nre
                };
                return View(nrc);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        [HttpPost]
        public ActionResult RecurrRecipts(RecurrClass model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                if (!ModelState.IsValid)
                {
                    TempData["Exception"] = "Error In Data please Try Again";
                    return RedirectToAction("Status", "UserStatus");
                }
                try
                {
                    if (model.Id == 0)
                    {
                        db.BI_Affi_insert_Recipts(Convert.ToInt32(obj.Institute_Id), model.s_receiptname, Convert.ToString(model.amount), true);
                        TempData["Success"] = "Inserted Successfully";
                    }

                    else if (model.Id != 0)
                    {
                        db.BI_Affi_Update_Recipts(model.Id, Convert.ToInt32(obj.Institute_Id), model.s_receiptname, Convert.ToString(model.amount), true);
                        TempData["Success"] = "Updated Successfully";
                    }
                }
                catch (Exception e)
                {
                    TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
                }
                return RedirectToAction("Recurring", "UserAccount");
            }
            return RedirectToAction("Status", "UserStatus");
            }

        }

        public ActionResult RemoveRRecipt(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_Recipts(id, Convert.ToInt32(obj.Institute_Id), true);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RRecpDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var recp = (db.BI_Affi_pop_recipt(Convert.ToInt32(obj.Institute_Id), true)).ToList();
            return Json(recp, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RExpDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var exp = (db.BI_Affi_pop_Expence(Convert.ToInt32(obj.Institute_Id), true)).ToList();
            return Json(exp, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RemoveRExpenditure(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_expence(id, Convert.ToInt32(obj.Institute_Id), true);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult RecurrExpenditure(RecurrClass model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("Recurring", "UserAccount");
            }

            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_insert_Expence(Convert.ToInt32(obj.Institute_Id), model.s_expenditurename, Convert.ToString(model.amount), true);
                    TempData["Success"] = "Inserted Successfully";
                }
                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_Expence(model.Id, Convert.ToInt32(obj.Institute_Id), model.s_expenditurename, Convert.ToString(model.amount), true);
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion/Updating Please Reload Page & Try Again";
            }
            return RedirectToAction("Recurring", "UserAccount");
            }
        }

        [HttpGet]
        public ActionResult RecurrExpendituresUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                   if (IsRoll())
            {
                obj = GetData();
                var nre = db.Affi_ins_Recuring_All_Expence.ToList();
                RecurrClass nrc = new RecurrClass()
                {
                    ExpenditureDD = nre
                };
                var dbexpense = db.BI_Affi_ROW_F_pop_Expence(Convert.ToInt32(obj.Institute_Id), true, id);
                foreach (var expense in dbexpense)
                {
                    nrc.Id = expense.Expence_ID;
                    nrc.amount = Convert.ToInt32(expense.Amount);
                }
                return View(nrc);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        [HttpGet]
        public ActionResult RecurrReceiptsUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var nrr = db.Affi_ins_Recuring_All_recipts.ToList();
                RecurrClass nrc = new RecurrClass()
                {
                    ReceiptDD = nrr,
                };
                var dbrecipt = db.BI_Affi_pop_ROW_F_recipt(Convert.ToInt32(obj.Institute_Id), true, id);
                foreach (var recipt in dbrecipt)
                {
                    nrc.Id = recipt.Recipt_ID;
                    nrc.amount = Convert.ToInt32(recipt.Amount);
                }
                return View(nrc);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }


        //Other Document
        [HttpGet]
        public ActionResult OtherDoc()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var dbdoc = db.BI_affi_pop_ins_related_doc_cat();
                List<BI_affi_pop_ins_related_doc_cat_Result> doclist = new List<BI_affi_pop_ins_related_doc_cat_Result>();
                foreach (var doc in dbdoc)
                {
                    BI_affi_pop_ins_related_doc_cat_Result res = new BI_affi_pop_ins_related_doc_cat_Result();
                    res.ID = doc.ID;
                    res.Cat_Desc = doc.Cat_Desc;
                    doclist.Add(res);
                }
                OtherDoc document = new OtherDoc()
                {
                    DocumentDD = doclist
                };
                return View(document);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        [HttpPost]
        public ActionResult OtherDoc(OtherDoc model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("OtherDoc", "UserAccount");
            }
            if (model.ImageFile == null || model.ImageFile.ContentLength <= 0)
            {
                //ViewBag.DDImageNull = "Please Choose the Image Of Document";
                TempData["Exception"] = "Please Choose the Image Of Document";
                return RedirectToAction("OtherDoc", "UserAccount");
            }
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                //ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                TempData["Exception"] = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                return RedirectToAction("OtherDoc", "UserAccount");
            }
            if (model.ImageFile.ContentLength > (1024 * 300))
            {
                //ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                TempData["Exception"] = "Image Size Must BE Less Than 300KB";
                return RedirectToAction("OtherDoc", "UserAccount");
            }
            string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);

            try
            {
                //Converting into Bytes
                Stream fs = model.ImageFile.InputStream;
                BinaryReader br = new BinaryReader(fs);
                model.Image = br.ReadBytes((Int32)fs.Length);
                model.FileName = Path.GetFileName(model.ImageFile.FileName);

                if (model.Id == 0)
                {
                    db.BI_Affi_insert_building_related_document(Convert.ToInt32(obj.Institute_Id), model.doc_cat_Id, model.Image, model.pg_no, obj.Affiliation_Year);
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion Please Reload the Page & Try Again";
            }
            return RedirectToAction("OtherDoc", "UserAccount");
            }
        }

        //Library Details
        [HttpGet]
        public ActionResult LibraryDetails()
        {
            if (IsRoll())
            {
                return View();
            }
            return RedirectToAction("Status", "UserStatus");
        }

        public ActionResult DownloadLibearyDetail()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            int Inst_ID = Convert.ToInt32(obj.Institute_Id);
            Microsoft.Reporting.WebForms.LocalReport rpt = new Microsoft.Reporting.WebForms.LocalReport();
            rpt.ReportPath = Server.MapPath(@"~/Aff_Reports/Aff_Inst_Library_INFO.rdlc");
            rpt.DataSources.Add(new ReportDataSource("AJ_Rep_Library_INFO", db.AJ_Rep_Library_INFO(Convert.ToInt32(Inst_ID))));
            rpt.DataSources.Add(new ReportDataSource("Affi_ins_Rep_OtherFacilities_INFO", db.AJ_Affi_ins_Rep_OtherFacilities_INFO(Convert.ToInt32(Inst_ID))));

            DownloadPDF_File(rpt.Render("PDF"));
            return View();
            }
        }

        [HttpPost]
        public ActionResult LibraryDetails(LibraryDetails model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("LibraryDetails", "UserAccount");
            }
            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_Insert_Library_info(Convert.ToInt32(obj.Institute_Id), Convert.ToString(model.subject), Convert.ToString(model.no_titles), Convert.ToString(model.no_books), Convert.ToString(model.no_relevent), Convert.ToString(model.no_reff), Convert.ToString(model.other_rel));
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }
                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_Library_info(model.Id, Convert.ToInt32(obj.Institute_Id), Convert.ToString(model.subject), Convert.ToString(model.no_titles), Convert.ToString(model.no_books), Convert.ToString(model.no_relevent), Convert.ToString(model.no_reff), Convert.ToString(model.other_rel));
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion Please Reload the Page & Try Again";
            }

            return RedirectToAction("LibraryDetails", "UserAccount");
            }
        }

        [HttpGet]
        public ActionResult LibraryDetailsUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var dblib = db.BI_POP_ROW_F_AFFI_LIBRARY_Info(Convert.ToInt32(obj.Institute_Id), id);
                LibraryDetails ld = new LibraryDetails();

                foreach (var lib in dblib)
                {
                    ld.Id = Convert.ToInt32(lib.ID);
                    ld.subject = Convert.ToString(lib.Subject);
                    ld.no_books = Convert.ToInt32(lib.No_Of_Books);
                    ld.no_reff = Convert.ToInt32(lib.Refference_Books);
                    ld.no_relevent = Convert.ToInt32(lib.Refference_Books);
                    ld.no_titles = Convert.ToInt32(lib.Title);
                    ld.other_rel = Convert.ToInt32(lib.Related_Books);

                };
                return View(ld);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        public ActionResult RemoveLibraryDetails(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_Library_info(id, Convert.ToInt32(obj.Institute_Id));
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LDetailDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var detail = db.BI_POP_AFFI_LIBRARY_Info(Convert.ToInt32(obj.Institute_Id)).ToList();
            return Json(detail, JsonRequestBehavior.AllowGet);
            }
        }

        //Library Other Details
        public ActionResult LibraryOtherDetails()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var dbdetails = db.BI_pop_All_library_Other_item();
                List<BI_pop_All_library_Other_item_Result> liblist = new List<BI_pop_All_library_Other_item_Result>();
                foreach (var detail in dbdetails)
                {
                    BI_pop_All_library_Other_item_Result res = new BI_pop_All_library_Other_item_Result();
                    res.ID = detail.ID;
                    res.Item_Name = detail.Item_Name;
                    liblist.Add(res);
                }
                LibraryOtherDetails ld = new LibraryOtherDetails()
                {
                    ItemDD = liblist
                };
                return View(ld);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }
        [HttpPost]
        public ActionResult LibraryOtherDetails(LibraryOtherDetails model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("LibraryOtherDetails", "UserAccount");
            }
            try
            {
                if (model.Id == 0)
                {
                    db.BI_Affi_Insert_Other_OtherFacility(Convert.ToInt32(obj.Institute_Id), model.item, model.quantity, model.remarks);
                    db.SaveChanges();
                    TempData["Success"] = "Inserted Successfully";
                }

                else if (model.Id != 0)
                {
                    db.BI_Affi_Update_Other_OtherFacility(model.Id, Convert.ToInt32(obj.Institute_Id), model.item, model.quantity, model.remarks);
                    db.SaveChanges();
                    TempData["Success"] = "Updated Successfully";
                }
            }
            catch (Exception e)
            {
                TempData["Exception"] = "Error In Insertion Please Reload the Page & Try Again";
            }
            return RedirectToAction("LibraryOtherDetails", "UserAccount");
            }
        }

        public ActionResult RemoveOtherDetails(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_Remove_Other_OtherFacility(id, Convert.ToInt32(obj.Institute_Id));
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LOtherDetailDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            var others = db.BI_Affi_POP_Other_OtherFacility(Convert.ToInt32(obj.Institute_Id)).ToList();
            return Json(others, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LibraryOtherDetailsUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var dbdetails = db.BI_pop_All_library_Other_item();
                List<BI_pop_All_library_Other_item_Result> liblist = new List<BI_pop_All_library_Other_item_Result>();
                foreach (var detail in dbdetails)
                {
                    BI_pop_All_library_Other_item_Result res = new BI_pop_All_library_Other_item_Result();
                    res.ID = detail.ID;
                    res.Item_Name = detail.Item_Name;
                    liblist.Add(res);
                }
                LibraryOtherDetails ld = new LibraryOtherDetails()
                {
                    ItemDD = liblist
                };
                var dbothers = db.BI_Affi_POP_ROW_F_Other_OtherFacility(Convert.ToInt32(obj.Institute_Id), id);
                foreach (var other in dbothers)
                {
                    ld.item = other.Name;
                    ld.quantity = Convert.ToInt32(other.Quantity);
                    ld.remarks = other.Remarks;
                }
                return View(ld);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        //Affiliation
        [HttpGet]
        public ActionResult Affiliation()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                Affiliation objAffiliation = new Affiliation();
                objAffiliation.year = obj.Affiliation_Year.ToString();
                string[] instituteName = new string[2] { "Annual", "Term" };
                List<DDL_Category> List_category = new List<DDL_Category>();
                for (var i = 0; i < instituteName.Length; i++)
                {
                    DDL_Category objddlcat = new DDL_Category();
                    objddlcat.catId = instituteName[i];
                    objddlcat.category = instituteName[i];
                    List_category.Add(objddlcat);
                }
                var programs = db.BI_pop_Term_and_Anual_Programs(true);
                List<DDL_prgram_desc> List_program = new List<DDL_prgram_desc>();
                foreach (var item in programs)
                {
                    DDL_prgram_desc obj1 = new DDL_prgram_desc();
                    obj1.program_id = item.ID;
                    obj1.program_Desc = item.Program_Name;
                    List_program.Add(obj1);
                }
                objAffiliation.objprogram = List_program;
                objAffiliation.objCat = List_category;
                return View(objAffiliation);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        public ActionResult Affiliation(Affiliation model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("Affiliation", "UserAccount");
            }
            if (model.Id == 0)
            {
                db.AJ_Affi_Insert_snd_updaet_Program_Apply(Convert.ToInt32(obj.Institute_Id), model.discipline, obj.Id_year_for_affilation, model.no_seats, model.category, 0);
                db.SaveChanges();
                TempData["Success"] = "Inserted Successfully";
            }
            else if (model.Id != 0)
            {
                db.AJ_Affi_Insert_snd_updaet_Program_Apply(Convert.ToInt32(obj.Institute_Id), model.discipline, obj.Id_year_for_affilation, model.no_seats, model.category, model.Id);
                db.SaveChanges();
                TempData["Success"] = "Updated Successfully";
            }
            return RedirectToAction("Affiliation", "UserAccount");
            }
        }

        public JsonResult AffDataTableViewer()
        {
            using (UOSEntities db = new UOSEntities())
            {
                var obj = GetData();
            var dblist = db.BI_Affi_pop_Apply_Programs(Convert.ToInt32(obj.Institute_Id), obj.Id_year_for_affilation);
            List<BI_Affi_pop_Apply_Programs_Result> json = new List<BI_Affi_pop_Apply_Programs_Result>();
            foreach (var list in dblist)
            {
                BI_Affi_pop_Apply_Programs_Result res = new BI_Affi_pop_Apply_Programs_Result();
                res.Program_Name = list.Program_Name;
                res.Category = list.Category;
                res.No_Of_Seats = list.No_Of_Seats;
                res.Year = list.Year;
                res.ID = Convert.ToInt32(list.ID);
                json.Add(res);
            }
            return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        // population of program in frop down list affiliation view
        public ActionResult Pop_program(bool val)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            Affiliation objAffiliation = new Affiliation();
            var programs = db.BI_pop_Term_and_Anual_Programs(val).ToList();
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(programs);
            return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult AffiliationUpdate(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                obj = GetData();
                var instid = Convert.ToInt32(obj.Institute_Id);
                var dbuser = db.Affi_Ins_Apply_Programs.Single(m => m.id == id && m.Inst_ID == instid);
                Affiliation objAffiliation = new Affiliation();
                string[] instituteName = new string[2] { "Anual", "Term" };
                List<DDL_Category> List_category = new List<DDL_Category>();
                for (var i = 0; i < instituteName.Length; i++)
                {
                    DDL_Category objddlcat = new DDL_Category();
                    objddlcat.catId = instituteName[i];
                    objddlcat.category = instituteName[i];
                    List_category.Add(objddlcat);
                }
                var programs = db.BI_pop_Term_and_Anual_Programs(true);
                List<DDL_prgram_desc> List_program = new List<DDL_prgram_desc>();
                foreach (var item in programs)
                {
                    DDL_prgram_desc obj1 = new DDL_prgram_desc();
                    obj1.program_id = item.ID;
                    obj1.program_Desc = item.Program_Name;
                    List_program.Add(obj1);
                }
                objAffiliation.objprogram = List_program;
                objAffiliation.objCat = List_category;
                objAffiliation.Id = Convert.ToInt32(dbuser.id);
                objAffiliation.no_seats = Convert.ToInt32(dbuser.No_Of_Seat);
                return View(objAffiliation);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }

        public ActionResult DeleteAffiliation(int id)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            db.BI_Affi_remove_Program_Apply(Convert.ToInt32(obj.Institute_Id), id);
            return Json(new { success = true, responseText = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        //BankDraft
        [HttpGet]
        public ActionResult BankDraft()
        {
            return View();
        }

        //New Registration
        [HttpGet]
        public ActionResult NewRegistration()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult NewRegistration()
        //{
        //    return View();
        //}

        //Last
        [HttpGet]
        public ActionResult last()
        {
            using (UOSEntities db = new UOSEntities())
            {
                if (IsRoll())
            {
                var boards = db.pop_inter_boards().ToList();
                List<InterBoards> l_obj_b = new List<InterBoards>();
                last obj_last = new last();
                foreach (var item in boards)
                {
                    InterBoards obj1 = new InterBoards();
                    obj1.B_ID = Convert.ToInt32(item.ID);
                    obj1.Board_Name = item.Board_Name;
                    l_obj_b.Add(obj1);
                }
                obj_last.obj_intr_Board = l_obj_b;

                var cources = db.pop_inter_cources().ToList();
                List<InterCourse> l_obj_c = new List<InterCourse>();

                foreach (var item in cources)
                {
                    InterCourse obj2 = new InterCourse();
                    obj2.C_ID = Convert.ToInt32(item.ID);
                    obj2.Course_Name = item.Inter_Program;
                    l_obj_c.Add(obj2);
                }
                obj_last.obj_intr_Board = l_obj_b;
                obj_last.obj_intr_Course = l_obj_c;

                return View(obj_last);
            }
            return RedirectToAction("Status", "UserStatus");
            }
        }
        // post last method
        [HttpPost]
        public ActionResult last(last model)
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            if (!ModelState.IsValid)
            {
                TempData["Exception"] = "Error In Data please Try Again";
                return RedirectToAction("last", "UserAccount");
            }
            if (model.ImageFile == null || model.ImageFile.ContentLength <= 0)
            {
                //ViewBag.DDImageNull = "Please Choose the Image Of Document";
                TempData["Exception"] = "Please Choose the Image Of Document";
                return RedirectToAction("last", "UserAccount");
            }
            var allowedExtensions = new[] { ".pdf", ".jpg", ".png", ".jpeg" };
            var extension = Path.GetExtension(model.ImageFile.FileName);
            if (!allowedExtensions.Contains(extension.ToLower()))
            {
                //ViewBag.DDImageNull = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                TempData["Exception"] = "Image Extension Must Be JPG , JPEG , PNG , PDF";
                return RedirectToAction("last", "UserAccount");
            }
            if (model.ImageFile.ContentLength > (1024 * 300))
            {
                //ViewBag.DDImageNull = "Image Size Must BE Less Than 300KB";
                TempData["Exception"] = "Image Size Must BE Less Than 300KB";
                return RedirectToAction("OtherDoc", "UserAccount");
            }
            string FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
            db.AJ_Affi_Ins_insert_other_decplines(Convert.ToInt32(obj.Institute_Id), model.discipline, model.board, model.no_students, obj.Affiliation_Year, model.Image);
            db.SaveChanges();
            TempData["Success"] = "Inserted Successfully";

            return RedirectToAction("last", "UserAccount");
            }
        }
        // Application Finish Mathod
        public ActionResult FinishApplication()
        {
            using (UOSEntities db = new UOSEntities())
            {
                obj = GetData();
            //AJ_Affi_Ins_App_Instu_Info
            // TempData["Exception"] = "Image Size Must BE Less Than 300KB";
            int tempvariable = Convert.ToInt32(obj.Institute_Id);
            var AppID = db.Aff_ins_Application.Where(a => a.Aff_Year == obj.Id_year_for_affilation && a.Inst_ID == tempvariable).Select(a => a.ID).Single();
            var vlue = db.AJ_Affi_Ins_App_Instu_Info(AppID);
            bool Chakvalue = false;
            foreach (var item in vlue)
            {
                if (item.Ann_A < 1)
                {
                    TempData["Exception"] = "Add Application Program ";
                }
                else if (item.form1 < 1)
                {
                    TempData["Exception"] = "Add Bulding Details";
                }
                else if (item.Finance < 1)
                {
                    TempData["Exception"] = "Add Account Details";
                }
                else if (item.Sports < 1)
                {
                    TempData["Exception"] = "Add Sports Item Details";
                }
                else if (item.Teaching_Staff < 1)
                {
                    TempData["Exception"] = "Add Teaching Staff Detail";
                }
                else if (item.Non_Teaching_Staff < 1)
                {
                    TempData["Exception"] = "Add Non-Teaching Staff Detail";
                }
                else if (item.Laboratories < 1)
                {
                    TempData["Exception"] = "Add Laboratory Detail";
                }
                else if (item.Library < 1)
                {
                    TempData["Exception"] = "Add Library Detail";
                }
                else if (item.NOC_GoV < 1)
                {
                    TempData["Exception"] = "Add NOC From Government";
                }
                else
                { Chakvalue = true; }

            }
            if (Chakvalue)
            {
                int instid = Convert.ToInt32(obj.Institute_Id);
                db.AJ_Affi_Ins_Application_Submit(instid);
                if (IsRoll())
                    return RedirectToAction("last");
                else
                    return RedirectToAction("Status", "UserStatus");
            }
            else
                return RedirectToAction("last");
            }
        }

    }
}