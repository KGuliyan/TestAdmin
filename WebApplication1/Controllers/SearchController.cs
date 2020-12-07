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

        //Метод считает количество ОО, передает модель в View 
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

        // метод принимает модель с заполненными полями InstitutionID и FullName, по которым осуществляет поиск ОО
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


        // Метод для отображения ВИ 
        public ActionResult EditEntrTestItem(int id, int entrTestID = 0, int dateInt = 0)
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
                sqlFirst += " AND DATEPART(YEAR, CompetitiveGroup.CreatedDate) = @date ";
            }

            string sqlSearchDate = @"SELECT DISTINCT DATEPART(YEAR, CreatedDate) AS CompetitiveYear FROM CompetitiveGroup WHERE CompetitiveGroup.InstitutionID = @InstitutionId";

            InfoFromDB<InstitutionFullInfo<int>> cmpGrpInfo = new InfoFromDB<InstitutionFullInfo<int>>();
            cmpGrpInfo.Request(sqlFirst, InstitutionId, "", dateInt);
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

            return View("EditEntrTestItem");
        }

        // Изменение мин балла ВИ 
        [HttpPost]
        public ActionResult EditEntrTestItem(int id, InstitutionFullInfo<int> EntranceTestItemCInfo, FormCollection form, string submitButton, string deleteEntrTestItem)
        {
            ViewBag.institutionEditInfo = new List<InstitutionFullInfo<int>>();
            InfoFromDB<int> editEntrTestItem = new InfoFromDB<int>();
            string editMinscore = "";


            int rowsAffected = 2;
         
            int selectDate = 0;

            if (form.AllKeys.Contains("Dates"))
            {
                int dateInt = int.Parse(form["Dates"]);
                selectDate = dateInt;
            }
            ////// Удаление пока не реализовано
            string removeEntrTestItem = @"";

            if (deleteEntrTestItem != null)
            {
                editEntrTestItem.ChangeInfo(removeEntrTestItem);
                return RedirectToAction("EditInstitutionInfo", "Search");
            }


            // По нажатию на кнопку "submitButton" изменяется мин балл ВИ
            if (submitButton != null)
            {

               editMinscore = "UPDATE EntranceTestItemC set MinScore='" + EntranceTestItemCInfo.MinScore +
                         "' WHERE EntranceTestItemC.EntranceTestItemID=" + EntranceTestItemCInfo.EntranceTestItemID;


                editEntrTestItem.ChangeInfo(editMinscore);
                rowsAffected = editEntrTestItem.rowsAffected;
            }

            

             //редирект с измененными полями ВИ
            if (selectDate != 0 && rowsAffected == 1 || rowsAffected == 1)
            {
                //ViewBag.BackgroundColor = "#66CC99";
                return RedirectToAction("EditEntrTestItem", "Search",
                    new { id = id, dateInt = EntranceTestItemCInfo.CreatedDate, entrTestID = EntranceTestItemCInfo.EntranceTestItemID });

            } else if (selectDate != 0)

            {
                return RedirectToAction("EditEntrTestItem", "Search", new { id = id, dateInt = selectDate });
            }



            //return rowsAffected == 1 ? View("SuccessfullEdit") : View("Error");
            //Redirect("/Search/EditInstitutionInfo/" + id);


            return RedirectToAction("EditEntrTestItem");
        }

        // Метод для отображения информации по ПК
        public ActionResult Campaign(int id, string edForm, string stName, string cmgType, int cmpnID = 0, int selectDate = 0)
        {
            Campaign comp = new Campaign();

            comp.CampaignID = cmpnID;

            //В модель Campaign записываются данные из DropDownList (если выбраны)

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
            

            var Campaigns = new List<Campaign>();
            var CmgnTypes = new List<CampaignTypes>();
            var EduForm = new List<EducationForm>();
            var StsName = new List<CampaignStatus>();
            var Dates = new List<int>();


            string sqlDates = " SELECT DISTINCT DATEPART(YEAR, CreatedDate) FROM Campaign ";

            string sqlCmgn = @"  SELECT * FROM Campaign   
                              WHERE Campaign.InstitutionID = @InstitutionID";

            string sqlEduForms = @"SELECT * FROM EducationForms";

            string sqlStsName = @"SELECT c.StatusID, c.Name FROM CampaignStatus c";


            if (selectDate != 0)
            {
                sqlCmgn += " AND YearStart =" + selectDate;
                //cmd.AddWithValue("@InstitutionID", ID);
            }

            string sqlCmgnTypes = @"  SELECT * FROM CampaignTypes";

            // Отдельные запросы на данные из Campaign и справочников
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



            //Формирование одного листа по данным из Campaign и справочников по Id 
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


        //Изменение формы, типа, статуса ПК
        [HttpPost]
        public ActionResult Campaign(int id, Campaign comp, FormCollection form, string save, string EduForm, string StsName, string CmgnTypes, string deleteCmp, int selectDate = 0)
        {
            int rowsAffected = 0;

            InfoFromDB<int> editCampaign = new InfoFromDB<int>();


            string date = form["dates"];
            if (form.AllKeys.Contains("dates") && !String.IsNullOrEmpty(date))
            {
                selectDate = int.Parse(date);
            }

            //удаление не реализовано
            string sqlDelete = @"DELETE Campaign WHERE CampaignID = @CmpgnID";

            if (!String.IsNullOrEmpty(deleteCmp))
            {
                editCampaign.ChangeInfo(sqlDelete, comp.CampaignID);
          
                return RedirectToAction("Campaign", "Search", new { id = id, selectDate = selectDate });
            }



            if (save != null)
            {
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

                editCampaign.ChangeInfo(sqlQuery);
                rowsAffected = editCampaign.rowsAffected;
            }




            if (rowsAffected > 0 || selectDate > 0)
            {
                //ViewBag.BackgroundColor = "#66CC99";
                return RedirectToAction("Campaign", "Search",  
                    new { id = id, selectDate = selectDate, edForm = EduForm, stName = StsName, cmgType = CmgnTypes, cmpnID = comp.CampaignID });
            }

            return View();

        }

        public ActionResult CompetitiveGroup(int id)
        {
            var cg = new CompetitiveGroup();
            

            ViewBag.compGroups = new List<CompetitiveGroup>();
            ViewBag.foundCg = "false";



            return View(cg);
        }

        //Изменение КГ
        [HttpPost]
        public ActionResult CompetitiveGroup(int id, CompetitiveGroup cg)
        {
            
            ViewBag.compGroups = new List<CompetitiveGroup>();
            var compGroups = new List<CompetitiveGroup>();

            string findCmpGrps = @"SELECT * FROM CompetitiveGroup 
                                    WHERE InstitutionID = @InstitutionID 
                                    AND Name LIKE '%' + @Name +'%'";

            //string findCmpGrps = @"SELECT COUNT(CompetitiveGroupID) FROM CompetitiveGroup 
            //                        WHERE InstitutionID = @InstitutionID 
            //                        AND Name LIKE '%' + @Name +'%'";


            string sqlAdmsn = @"SELECT * FROM AdmissionItemType";
            string sqlDirection = @"SELECT * FROM Direction";

            var compGroupsInfo = new InfoFromDB<CompetitiveGroup>();
            compGroupsInfo.Request(findCmpGrps, id, cg.Name);
            compGroups = compGroupsInfo.Result;
           
            


            InfoFromDB<AmissionItemType> allAdmItems = new InfoFromDB<AmissionItemType>();
            allAdmItems.Request(sqlAdmsn);
            var edLevel = allAdmItems.Result;
            var edForm = allAdmItems.Result;

      

            InfoFromDB<Direction> allDirection = new InfoFromDB<Direction>();
            allDirection.Request(sqlDirection);
            var direction = allDirection.Result;

            var CompetitiveGroups = compGroups.Join(edLevel, c => c.EducationLevelID, e => e.ItemTypeID, (c, e) =>
            {
                c.EducationLevelName = e.Name;
                return c;
            }).Join(edForm, c => c.EducationFormId, ef => ef.ItemTypeID, (c, ef) =>
            {
                c.EducationFormName = ef.Name;
                return c;
            }).Join(direction, c => c.DirectionID, d => d.DirectionID, (c, d) =>
            {
                c.DirectionName = d.Name;
                return c;
            }).ToList();

            var eduForm = edForm.Select(i => i.ItemLevel == 2);

            SelectList eduFormName = new SelectList(edForm, "ItemTypeID", "Name");
            ViewBag.eduFormName = eduFormName;



            ViewBag.compGroups = CompetitiveGroups;

            if (Enumerable.Any(ViewBag.compGroups))
            {
                ViewBag.foundCg = "";
            }


            // ViewBag.compGroups = compGroupsInfo.Result;

            return View();
        }


        //Класс для формирования запросов к БД
        public class InfoFromDB<T>
        {

            public List<T> Result { get; set; }
            //public T Model { get; set; }
            public int rowsAffected { get; set; }



            public void Request ( string sql, int id = 0, string Name = "", int date = 0)
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                {
                    Result = db.Query<T>(sql, new { InstitutionID = id, Name = Name, date = date }).ToList();             
                }

            }

            public void ChangeInfo(string sql, int cmpID = 0)
            {
                using (IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                {
                    if (cmpID > 0)
                    {
                        rowsAffected = db.Execute(sql, new { CmpgnID = cmpID });
                        
                    }
                    else
                    {
                        rowsAffected = db.Execute(sql);
                    }

                }
                return;
            }

        }

    }
}