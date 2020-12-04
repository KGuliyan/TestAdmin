using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class SearchController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Search()
        {

            string sqlExpression = "SELECT COUNT(InstitutionID) FROM Institution;";

            InfoFromDB<int> cntInst = new InfoFromDB<int>();

            cntInst.Request(sqlExpression);

            ViewBag.InstitutionCount = "Количество образовательных организаций: " + cntInst.Result[0];

            ViewBag.InstitutionInfo = new List<InstitutionFullInfo<DateTime>>();
            InstitutionViewModel vmm = new InstitutionViewModel();
            return View(vmm);
        }
        public ActionResult Results(InstitutionViewModel model)

        {

            var institutionId = model.InstitutionID;
            var institutionName = model.FullName;
            bool isEmptyTextBoxes = true;

            string sqlExpression = "SELECT * FROM Institution  ";


            if (!String.IsNullOrEmpty(institutionName) && institutionId != 0)
            {
                sqlExpression += " WHERE InstitutionID = @InstitutionID AND (FullName LIKE '%' + @Name + '%' OR BriefName LIKE '%' + @Name + '%') ";
                isEmptyTextBoxes = false;
            }


            if (!String.IsNullOrEmpty(institutionName) && isEmptyTextBoxes)
            {
                sqlExpression += " WHERE FullName LIKE '%' + @Name + '%' OR BriefName LIKE '%' + @Name + '%' ";
            }

            if (institutionId != 0 && isEmptyTextBoxes)
            {
                sqlExpression += " WHERE InstitutionID = @InstitutionID";
            }

            List<InstitutionFullInfo<DateTime>> institutions = new List<InstitutionFullInfo<DateTime>>();

            InfoFromDB<InstitutionFullInfo<DateTime>> insts = new InfoFromDB<InstitutionFullInfo<DateTime>>();

            insts.Request(sqlExpression, institutionId, institutionName);

            ViewBag.InstitutionInfo = insts.Result;

            return View("Search");
        }

        public ActionResult EditInstitutionInfo(int id, int entrTestID = 0, int dateInt = 0)
        {
            //ViewBag.institutionEditInfo = new List<Institution>();
            ViewBag.FormEtID = entrTestID;

            int InstitutionId = id;

            List<int> CompetitiveGroupDates = new List<int>();


            List<InstitutionFullInfo<int>> compGroupsInfo = new List<InstitutionFullInfo<int>>();




            string sqlFirst = @"SELECT 
                                DISTINCT DATEPART(YEAR, CompetitiveGroup.CreatedDate) AS CreatedDate,
                                CompetitiveGroup.CompetitiveGroupID AS CompetitiveGroup ,
                                Institution.FullName AS FullName,
                                Institution.InstitutionID AS InstitutionID,
                                EntranceTestItemC.MinScore AS MinScore ,
                                EntranceTestItemC.EntranceTestItemID AS EntranceTestItemID,
                                CompetitiveGroup.Name AS Name,
                                CompetitiveGroup.CompetitiveGroupID AS CompetitiveGroupID

                                FROM Institution 
                            
                                JOIN CompetitiveGroup  on CompetitiveGroup.InstitutionID = Institution.InstitutionID
                                JOIN EntranceTestItemC on EntranceTestItemC.CompetitiveGroupID = CompetitiveGroup.CompetitiveGroupID
                                WHERE CompetitiveGroup.InstitutionID = @InstitutionId";

            //string sqlCompGroup = @"SELECT 
            //                CompetitiveGroup.CompetitiveGroupID,
            //                CompetitiveGroup.Name

            //                FROM CompetitiveGroup 
            //                WHERE Institution.InstitutionID = @InstitutionId";



            if (dateInt != 0)
            {
                sqlFirst += " AND DATEPART(YEAR, CompetitiveGroup.CreatedDate) = " + dateInt;
            }

            string sqlSearchDate = @"SELECT DISTINCT DATEPART(YEAR, CreatedDate) AS CompetitiveYear FROM CompetitiveGroup WHERE CompetitiveGroup.InstitutionID = @InstitutionId";

            InfoFromDB<InstitutionFullInfo<int>> cmpGrpInfo = new InfoFromDB<InstitutionFullInfo<int>>();
            cmpGrpInfo.Request(sqlFirst, InstitutionId);
            compGroupsInfo = cmpGrpInfo.Result;

            InfoFromDB<int> cmpGrpDates = new InfoFromDB<int>();
            cmpGrpDates.Request(sqlSearchDate, InstitutionId);
            CompetitiveGroupDates = cmpGrpDates.Result;

            SelectList dates = new SelectList(CompetitiveGroupDates, dateInt);

            ViewBag.Dates = dates;
            if (dateInt == 0)
            {
                ViewBag.SelectedYear = "Выберите год";
            }
            else
            {
                ViewBag.SelectedYear = dateInt.ToString();
            }
            ViewBag.compGroupsInfo = compGroupsInfo;

            return View("EditInstitutionInfo");
        }

        [HttpPost]
        public ActionResult EditInstitutionInfo(int id, InstitutionFullInfo<int> EntranceTestItemCInfo, FormCollection form)
        {
            ViewBag.institutionEditInfo = new List<InstitutionFullInfo<int>>();
            int rowsAffected = 2;
            //int id = EntranceTestItemCInfo.InstitutionID;

            //int instid = int.Parse(form["InstitutionID"]);
            int selectDate = 0;

            if (form.AllKeys.Contains("Dates"))
            {
                int dateInt = int.Parse(form["Dates"]);
                selectDate = dateInt;
            }

            string sqlQuery = "UPDATE EntranceTestItemC set MinScore='" + EntranceTestItemCInfo.MinScore +
                       "' WHERE EntranceTestItemC.EntranceTestItemID=" + EntranceTestItemCInfo.EntranceTestItemID;

            InfoFromDB<int> editMinScore = new InfoFromDB<int>();
            editMinScore.ChangeInfo(sqlQuery);
            rowsAffected = editMinScore.rowsAffected;

            //ViewBag.BackgroundColor = "#FFFFFF";
            if (selectDate != 0 && rowsAffected == 1 || rowsAffected == 1)
            {
                ViewBag.BackgroundColor = "#66CC99";
                return RedirectToAction("EditInstitutionInfo", "Search",
                    new { id = id, dateInt = EntranceTestItemCInfo.CreatedDate, entrTestID = EntranceTestItemCInfo.EntranceTestItemID });

            } else if (selectDate != 0)

            {
                return RedirectToAction("EditInstitutionInfo", "Search", new { id = id, dateInt = selectDate });
            }



            //return rowsAffected == 1 ? View("SuccessfullEdit") : View("Error");
            //Redirect("/Search/EditInstitutionInfo/" + id);


            return RedirectToAction("EditInstitutionInfo");
        }


        public ActionResult Campaign(int id, string edForm, string stName, string cmgType, int cmpnID = 0, int selectDate = 0)
        {
            Campaign comp = new Campaign();

            comp.CampaignID = cmpnID;

            if (!String.IsNullOrEmpty(edForm)) {
                comp.EducationFormFlag = int.Parse(edForm);
            }

            if (!String.IsNullOrEmpty(stName))
            {
                comp.StatusID = int.Parse(stName);
            }

            if (!String.IsNullOrEmpty(cmgType))
            {
                comp.CampaignTypeID = int.Parse(cmgType);
            }




            ViewBag.campaigns = ViewBag.CmgTypeName = ViewBag.StsName = new List<Campaign>();
            //ViewBag.CmgTypeName = new List<Campaign>();
            //ViewBag.StsName = new List<Campaign>();

            var Campaigns = new List<Campaign>();
            var CmgnTypes = new List<CampaignTypes>();
            var EduForm = new List<EducationForm>();
            var StsName = new List<CampaignStatus>();
            var Dates = new List<int>();
            //List<InstitutionFullInfo> Institution = new List<InstitutionFullInfo>();
            //var Dates = new List<Campaign>(); ;
            string sqlDates = " SELECT DISTINCT DATEPART(YEAR, CreatedDate) FROM Campaign ";

            string sqlCmgn = @"  SELECT * FROM Campaign   
                              WHERE Campaign.InstitutionID = @InstitutionID";

            string sqlEduForms = @"SELECT * FROM EducationForms";

            string sqlStsName = @"SELECT c.StatusID, c.Name FROM CampaignStatus c";

            if (selectDate != 0)
            {
                sqlCmgn += " AND YearStart =" + selectDate;
            }

            string sqlCmgnTypes = @"  SELECT * FROM CampaignTypes";


            InfoFromDB<Campaign> allCampagns = new InfoFromDB<Campaign>();
            allCampagns.Request(sqlCmgn, id);
            Campaigns = allCampagns.Result;

            InfoFromDB<CampaignTypes> allCmgnTypes = new InfoFromDB<CampaignTypes>();
            allCmgnTypes.Request(sqlCmgnTypes);
            CmgnTypes = allCmgnTypes.Result;

            InfoFromDB<EducationForm> allEduForm = new InfoFromDB<EducationForm>();
            allEduForm.Request(sqlEduForms);
            EduForm = allEduForm.Result;

            InfoFromDB<CampaignStatus> allStsName = new InfoFromDB<CampaignStatus>();
            allStsName.Request(sqlStsName);
            StsName = allStsName.Result;

            InfoFromDB<int> allDates = new InfoFromDB<int>();
            allDates.Request(sqlDates);
            Dates = allDates.Result;


            if (selectDate == 0)
            {
                ViewBag.SelectedYear = "Выберите год";
            }
            else
            {
                ViewBag.SelectedYear = selectDate.ToString();
            }
            SelectList dates = new SelectList(Dates, selectDate);
            //SelectList dates = new SelectList(Dates, selectDate, selectDate == 0 ? "Выберите год" : selectDate.ToString());
            ViewBag.dates = dates;


            var cmgnJoins = Campaigns.Join(CmgnTypes, c => c.CampaignTypeID, s => s.CampaignTypeID, (c, s) =>
            {
                c.CampaignTypeName = s.Name;
                return c;
            }).Join(EduForm, c => c.EducationFormFlag, e => e.Id, (c, e) =>
            {
                c.EducationFormName = e.Name;
                return c;
            }).Join(StsName, c => c.StatusID, e => e.StatusID, (c, e) =>
            {
                c.CampaignStatusName = e.Name;
                return c;
            }).ToList();



            SelectList cmgTypeName = new SelectList(CmgnTypes, "CampaignTypeID", "Name");
            ViewBag.CmgTypeName = cmgTypeName;


            SelectList eduForm = new SelectList(EduForm, "Id", "Name");
            ViewBag.EduForm = eduForm;

            SelectList stsName = new SelectList(StsName, "StatusID", "Name");
            ViewBag.StsName = stsName;



            ViewBag.campaigns = cmgnJoins;

            //Campaign comp = new Campaign();



            return View(comp);
        }

        [HttpPost]
        public ActionResult Campaign(int id, Campaign comp, FormCollection form, string save, string EduForm, string StsName, string CmgnTypes, int selectDate = 0)
        {
            int rowsAffected = 0;

            string date = form["dates"];
            if (form.AllKeys.Contains("dates") && !String.IsNullOrEmpty(date))
            {
                selectDate = int.Parse(date);
            }

            string sqlQuery = @"UPDATE Campaign SET ";



            if (!String.IsNullOrEmpty(EduForm))
                sqlQuery += " EducationFormFlag = " + EduForm;


            if (!String.IsNullOrEmpty(StsName) && String.IsNullOrEmpty(EduForm))
            {
                sqlQuery += " StatusID = " + StsName;
            }
            else if (!String.IsNullOrEmpty(StsName) && !String.IsNullOrEmpty(EduForm))
            {
                sqlQuery += ", StatusID = " + StsName;
            }


            if (!String.IsNullOrEmpty(CmgnTypes) && String.IsNullOrEmpty(EduForm) && String.IsNullOrEmpty(StsName))
            {
                sqlQuery += " CampaignTypeID = " + CmgnTypes;
            }
            else if (!String.IsNullOrEmpty(CmgnTypes) && (!String.IsNullOrEmpty(EduForm) || !String.IsNullOrEmpty(StsName)))
            {
                sqlQuery += ", CampaignTypeID = " + CmgnTypes;
            }

            sqlQuery += " WHERE Campaign.CampaignID = " + comp.CampaignID;


            InfoFromDB<int> editCampaign = new InfoFromDB<int>();

            if (save != null)
            {
                editCampaign.ChangeInfo(sqlQuery);
                rowsAffected = editCampaign.rowsAffected;
            }



            if (rowsAffected > 0 || selectDate > 0)
            {
                ViewBag.BackgroundColor = "#66CC99";
                return RedirectToAction("Campaign", "Search",  
                    new { id = id, selectDate = selectDate, edForm = EduForm, stName = StsName, cmgType = CmgnTypes, cmpnID = comp.CampaignID });
            }

            return View();

        }

        public class InfoFromDB<T>
        {

            public List<T> Result { get; set; }
            //public T Model { get; set; }
            public int rowsAffected { get; set; }



            public void Request ( string sql, int id = 0, string institutionName = "")
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                {
                    Result = db.Query<T>(sql, new { InstitutionID = id, Name = institutionName }).ToList();             
                }

            }

            public void ChangeInfo(string sql)
            {
               
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                {
                    rowsAffected = db.Execute(sql);

                }
            }
        }

    }
}