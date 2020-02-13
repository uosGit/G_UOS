using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UOS.Models;

namespace UOS.Controllers
{

    public class V1Controller : ApiController
    {
        [Route("Api/v1/Colleges")]
        [HttpGet]
        public IEnumerable<AllColleges> Colleges()
        {
            using (UOSEntities db = new UOSEntities())
            {
                var data = (from t1 in db.Affil_Ins_Info select new {t1.ID, t1.Ins_Name , t1.Ins_IsGov, t1.Inst_code }).ToList();
                List<AllColleges> listobj = new List<AllColleges>();
                foreach (var item in data)
                {
                    AllColleges obj = new AllColleges();
                    obj.id = item.ID.ToString();
                    obj.collegeName = item.Ins_Name;
                    obj.isGovCollege = item.Ins_IsGov == true ? true : false;
                    obj.collegeCode = item.Inst_code.ToString();
                    listobj.Add(obj);
                }
                return listobj;
            }
        }


        [HttpGet]
        public IEnumerable<AllColleges> Colleges(int id)
        {
            int val =0;
            if (int.TryParse(id.ToString(), out val ))
            {
                using (UOSEntities db = new UOSEntities())
                {
                    var data = db.bi_get_affi_clgs_with_pagination(val);
                    List<AllColleges> listobj = new List<AllColleges>();
                    foreach (var item in data)
                    {
                        AllColleges obj = new AllColleges();
                        obj.id = item.ID.ToString();
                        obj.collegeName = item.Ins_Name;
                        obj.isGovCollege = item.Ins_IsGov == true ? true : false;
                        obj.collegeCode = item.Inst_code.ToString();
                        listobj.Add(obj);
                    }
                    return listobj;
                }
            }
            else
            {
                List<AllColleges> listobj = new List<AllColleges>();
                return listobj;
            }
        }

        [Route("Api/v1/personal/{cnic}")]
        [HttpGet]
        public IEnumerable<studentPersonalInfo> personal(string cnic)
        {
            if (cnic.Length == 13)
            {
                string part1 = cnic.Substring(0, 5);
                string part2 = cnic.Substring(5, 7);
                string part3 = cnic.Substring(12, 1);

                string FinalCnic = part1 + "-" + part2 + "-" + part3;


                using (UOSEntities db_obj = new UOSEntities())
                {
                    var data = (from st in db_obj.stu_basic_info
                                join C in db_obj.countries on st.st_counter_id equals C.ID
                                join PRC in db_obj.countries on st.st_pr_country_id equals PRC.ID

                                join P in db_obj.Provinces on st.st_provience_id equals P.ID
                                join PRP in db_obj.Provinces on st.st_pr_provience_id equals PRP.ID

                                join D in db_obj.districts on st.st_dist_id equals D.ID
                                join PRD in db_obj.districts on st.st_pr_dist_id equals PRD.ID

                                join T in db_obj.District_tehsils on st.st_tehsil_id equals T.ID
                                join PRT in db_obj.District_tehsils on st.st_pr_tehsil_id equals PRT.ID

                                where (st.st_cinc == FinalCnic.ToString())
                                select new
                                {
                                    st.st_id,
                                    st.reg_number,
                                    st.st_name,
                                    st.Father_name,
                                    st.st_dob,
                                    st.st_birth_place,
                                    st.st_contact,
                                    st.st_bloodgroup,
                                    st.st_email,
                                    st.st_gender,
                                    st.st_martial_status,
                                    st.st_religion,
                                    st.st_cinc,
                                    st.st_nationality,
                                    st.st_pr_country_id,
                                    PRCountry_Name = PRC.Country_Name,
                                    st.st_pr_provience_id,
                                    PRProvince_name = PRP.Province_name,
                                    st.st_pr_dist_id,
                                    PRDist_Name = PRD.Dist_Name,
                                    st.st_pr_tehsil_id,
                                    PRTehsils = PRT.Tehsils,
                                    st.st_pr_address,
                                    st.st_counter_id,
                                    C.Country_Name,
                                    st.st_provience_id,
                                    P.Province_name,
                                    st.st_dist_id,
                                    D.Dist_Name,
                                    st.st_tehsil_id,
                                    T.Tehsils,
                                    st.st_address,
                                    st.st_gr_name,
                                    st.st_gr_cnic,
                                    st.st_gr_cell,
                                    st.st_relation_id,
                                    st.st_gr_eaducation,
                                    st.st_gr_occupation,
                                    st.st_gr_job_sector,
                                    st.st_gr_anual_income,
                                    st.st_image
                                }
                                ).ToList();
                    List<studentPersonalInfo> objList = new List<studentPersonalInfo>();
                    foreach (var item in data)
                    {
                        studentPersonalInfo obj = new studentPersonalInfo();

                        obj.stid = item.st_id.ToString();
                        obj.regNumber = item.reg_number.ToString();
                        obj.cnic = item.st_cinc.ToString();
                        obj.name = item.st_name.ToString();
                        obj.fName = item.st_gr_name.ToString();
                        obj.gender = item.st_gender.ToString();
                        obj.bloodGroup = item.st_bloodgroup.ToString();
                        obj.dob = item.st_dob.ToString();
                        obj.religion = item.st_religion.ToString();
                        obj.martialStatus = item.st_martial_status.ToString();
                        obj.cellNo = item.st_contact.ToString();
                        obj.email = item.st_email.ToString();
                        obj.nationality = item.st_nationality.ToString();
                        obj.pob = item.st_birth_place.ToString();

                        obj.countryId = item.st_counter_id.ToString();
                        obj.countryName = item.Country_Name.ToString();

                        obj.provienceId = item.st_provience_id.ToString();
                        obj.provienceName = item.Province_name.ToString();

                        obj.disttId = item.st_dist_id.ToString();
                        obj.disttName = item.Province_name.ToString();

                        obj.tehsilId = item.st_tehsil_id.ToString();
                        obj.tehsilName = item.Tehsils.ToString();

                        obj.Address = item.st_address.ToString();

                        obj.PRcountryId = item.st_pr_country_id.ToString();
                        obj.PRcountryName = item.PRCountry_Name.ToString();

                        obj.PRprovienceId = item.st_pr_provience_id.ToString();
                        obj.PRprovienceName = item.PRProvince_name.ToString();

                        obj.PRdisttId = item.st_pr_dist_id.ToString();
                        obj.PRdisttName = item.PRDist_Name.ToString();

                        obj.PRtehsilId = item.st_pr_tehsil_id.ToString();
                        obj.PRtehsilName = item.PRTehsils.ToString();

                        obj.PRparpermanentAddress = item.st_pr_address.ToString();

                        obj.grName = item.st_gr_name.ToString();
                        obj.grCnic = item.st_gr_cnic.ToString();
                        obj.grCell = item.st_gr_cell.ToString();
                        obj.grRelation = item.st_relation_id.ToString();
                        obj.grEaducation = item.st_gr_eaducation.ToString();
                        obj.grOccupation = item.st_gr_occupation.ToString();
                        obj.grJobSector = item.st_gr_job_sector.ToString();
                        obj.grAnnualIncome = item.st_gr_anual_income.ToString();
                        Byte[] bytes = item.st_image;
                        string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);

                        obj.studentImage = base64String;
                        objList.Add(obj);
                    }
                    return objList;
                }
            }
            else
            {
                List<studentPersonalInfo> objList = new List<studentPersonalInfo>();
                return objList;
            }
        }

        [Route("Api/v1/matric/{cnic}")]
        [HttpGet]
        public IEnumerable<studentMatricInfo> matric(string cnic)
        {
            if (cnic.Length == 13)
            {
                string part1 = cnic.Substring(0, 5);
                string part2 = cnic.Substring(5, 7);
                string part3 = cnic.Substring(12, 1);

                string FinalCnic = part1 + "-" + part2 + "-" + part3;


                using (UOSEntities db_obj = new UOSEntities())
                {
                    var data = (from BInfo in db_obj.stu_basic_info
                                join Minfo in db_obj.stu_ssc_info
                                on BInfo.st_id equals Minfo.st_id
                                join Brds in db_obj.boards
                                on Minfo.m_board_id equals Brds.ID
                                where (BInfo.st_cinc == FinalCnic)
                                select new
                                {
                                    BInfo.st_id,
                                    BInfo.st_cinc,
                                    Minfo.m_degree_name,
                                    Minfo.m_subject,
                                    Minfo.m_board_id,
                                    Brds.Board_Name,
                                    Minfo.m_passing_year,
                                    Minfo.m_examination_type,
                                    Minfo.m_rollnumber,
                                    Minfo.m_total_marks,
                                    Minfo.m_obtained_marks,
                                    Minfo.m_division,
                                    Minfo.m_percentage,
                                    Minfo.m_result_scan
                                }).ToList();

                    List<studentMatricInfo> objList = new List<studentMatricInfo>();
                    foreach (var item in data)
                    {
                        studentMatricInfo obj = new studentMatricInfo();

                        obj.stid = item.st_id.ToString();
                        obj.cnic = item.st_cinc.ToString();
                        obj.degree = item.m_degree_name.ToString();
                        obj.subject = item.m_subject.ToString();
                        obj.boardId = item.m_board_id.ToString();
                        obj.boardName = item.Board_Name.ToString();
                        obj.passingYear = item.m_passing_year.ToString();
                        obj.examinationType = item.m_examination_type.ToString();
                        obj.boardRollNumber = item.m_rollnumber.ToString();
                        obj.totalMarks = item.m_total_marks.ToString();
                        obj.obtainedMarks = item.m_obtained_marks.ToString();
                        obj.division = item.m_division.ToString();
                        obj.percentage = item.m_percentage.ToString();
                        Byte[] bytes = item.m_result_scan;
                        string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                        obj.matricImage = base64String;
                        objList.Add(obj);
                    }
                    return objList;
                }
            }
            else
            {
                List<studentMatricInfo> objList = new List<studentMatricInfo>();
                return objList;
            }
        }

        [Route("Api/v1/inter/{cnic}")]
        [HttpGet]
        public IEnumerable<studentInterInfo> inter(string cnic)
        {
            if (cnic.Length == 13)
            {
                string part1 = cnic.Substring(0, 5);
                string part2 = cnic.Substring(5, 7);
                string part3 = cnic.Substring(12, 1);

                string FinalCnic = part1 + "-" + part2 + "-" + part3;


                using (UOSEntities db_obj = new UOSEntities())
                {
                    var data = (from BInfo in db_obj.stu_basic_info
                                join Iinfo in db_obj.stu_inter_info
                                on BInfo.st_id equals Iinfo.st_id
                                join IDN in db_obj.Intermediate_Programs
                                on Iinfo.i_degree_name equals IDN.ID.ToString()
                                join IB in db_obj.boards
                                on Iinfo.i_board_id equals IB.ID
                                where (BInfo.st_cinc == FinalCnic)
                                select new
                                {
                                    BInfo.st_id,
                                    BInfo.st_cinc,
                                    Iinfo.i_degree_name,
                                    IDN.Inter_Program,
                                    Iinfo.i_board_id,
                                    IB.Board_Name,
                                    Iinfo.i_passing_year,
                                    Iinfo.i_examination_type,
                                    Iinfo.i_rollnumber,
                                    Iinfo.i_total_marks,
                                    Iinfo.i_obtained_marks,
                                    Iinfo.i_division,
                                    Iinfo.i_percentage,
                                    Iinfo.i_result_scan
                                }).ToList();
                    List<studentInterInfo> objList = new List<studentInterInfo>();
                    foreach (var item in data)
                    {
                        studentInterInfo obj = new studentInterInfo();

                        obj.stid =item.st_id.ToString();
                        obj.cnic =item.st_cinc.ToString();
                        
                        obj.degreeId =item.i_degree_name.ToString();
                        obj.degreeName =item.Inter_Program.ToString();
                        
                        obj.boardId =item.i_board_id.ToString();
                        obj.boardName =item.Board_Name.ToString();
                        
                        obj.passingYear =item.i_passing_year.ToString();
                        obj.examinationType = item.i_examination_type.ToString();
                        obj.boardRollNumber = item.i_rollnumber.ToString();
                        obj.totalMarks =item.i_total_marks.ToString();
                        obj.obtainedMarks =item.i_obtained_marks.ToString();
                        obj.division = item.i_division.ToString();
                        obj.percentage = item.i_percentage.ToString();

                        Byte[] bytes = item.i_result_scan;
                        string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                        obj.interImage = base64String;
                        objList.Add(obj);
                    }
                    return objList;
                }
            }
            else
            {
                List<studentInterInfo> objList = new List<studentInterInfo>();
                return objList;
            }
        }


        public class AllColleges
        {
            public string id { get; set; }
            public string collegeName { get; set; }
            public bool isGovCollege { get; set; }
            public string collegeCode { get; set; }
        }

        public class studentPersonalInfo
        {
            public string stid { get; set; }
            public string regNumber { get; set; }
            public string cnic { get; set; }
            public string name { get; set; }
            public string fName { get; set; }
            public string gender { get; set; }
            public string bloodGroup { get; set; }
            public string dob { get; set; }
            public string religion { get; set; }
            public string martialStatus { get; set; }
            public string cellNo { get; set; }
            public string email { get; set; }
            public string nationality { get; set; }
            public string pob { get; set; }

            public string countryId { get; set; }
            public string countryName { get; set; }

            public string provienceId { get; set; }
            public string provienceName { get; set; }

            public string disttId { get; set; }
            public string disttName { get; set; }

            public string tehsilId { get; set; }
            public string tehsilName { get; set; }

            public string Address { get; set; }


            public string PRcountryId { get; set; }
            public string PRcountryName { get; set; }

            public string PRprovienceId { get; set; }
            public string PRprovienceName { get; set; }

            public string PRdisttId { get; set; }
            public string PRdisttName { get; set; }

            public string PRtehsilId { get; set; }
            public string PRtehsilName { get; set; }

            public string PRparpermanentAddress { get; set; }

            public string grName { get; set; }
            public string grCnic { get; set; }
            public string grCell { get; set; }
            public string grRelation { get; set; }
            public string grEaducation { get; set; }
            public string grOccupation { get; set; }
            public string grJobSector { get; set; }
            public string grAnnualIncome { get; set; }

            public string studentImage { get; set; }
        }

        public class studentMatricInfo
        {
            public string stid { get; set; }
            public string cnic { get; set; }
            public string degree { get; set; }
            public string subject { get; set; }

            public string boardId { get; set; }
            public string boardName { get; set; }

            public string passingYear { get; set; }
            public string examinationType { get; set; }
            public string boardRollNumber { get; set; }
            public string totalMarks { get; set; }
            public string obtainedMarks { get; set; }
            public string division { get; set; }
            public string percentage { get; set; }

            public string matricImage { get; set; }
        }

        public class studentInterInfo
        {
            public string stid { get; set; }
            public string cnic { get; set; }

            public string degreeId { get; set; }
            public string degreeName { get; set; }

            public string boardId { get; set; }
            public string boardName { get; set; }

            public string passingYear { get; set; }
            public string examinationType { get; set; }
            public string boardRollNumber { get; set; }
            public string totalMarks { get; set; }
            public string obtainedMarks { get; set; }
            public string division { get; set; }
            public string percentage { get; set; }

            public string interImage { get; set; }
        }
    }
}
