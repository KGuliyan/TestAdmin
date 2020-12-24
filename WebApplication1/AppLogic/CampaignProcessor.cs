using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.DataAccess;
using static WebApplication1.Classes.Values;

namespace WebApplication1.AppLogic
{
    public static class CampaignProcessor
    {
        public static List<Campaign> Campaigns { get; set; }
        public static List<CampaignTypes> CmgnTypes { get; set; }
        public static List<EducationForm> EduForm { get; set; }
        public static List<CampaignStatus> StsName { get; set; }
        public static List<int> Dates { get; set; }

        public static List<Campaign> SearchCampaign(int id,  int selectDate = 0)
        {
            string sqlDates = " SELECT DISTINCT DATEPART(YEAR, CreatedDate) FROM Campaign ";

            string sqlCmgn = @"  SELECT * FROM Campaign   
                              WHERE Campaign.InstitutionID = @InstitutionID";

            var p = new { InstitutionID = id };

            if (selectDate != 0)
            {
                sqlCmgn += " AND YearStart = " + selectDate; 
            }

            string sqlEduForms = @"SELECT * FROM EducationForms";

            string sqlStsName = @"SELECT c.StatusID, c.Name FROM CampaignStatus c";

            string sqlCmgnTypes = @"  SELECT * FROM CampaignTypes";

            // Отдельные запросы на данные из Campaign и справочников
            SqlDataAccess<Campaign> allCampagns = new SqlDataAccess<Campaign>();
            allCampagns.Request(sqlCmgn, p);
            Campaigns = allCampagns.Result;

            SqlDataAccess<CampaignTypes> allCmgnTypes = new SqlDataAccess<CampaignTypes>();
            allCmgnTypes.Request(sqlCmgnTypes);
            CmgnTypes = allCmgnTypes.Result;

            SqlDataAccess<EducationForm> allEduForm = new SqlDataAccess<EducationForm>();
            allEduForm.Request(sqlEduForms);
            EduForm = allEduForm.Result;

            SqlDataAccess<CampaignStatus> allStsName = new SqlDataAccess<CampaignStatus>();
            allStsName.Request(sqlStsName);
            StsName = allStsName.Result;

            SqlDataAccess<int> allDates = new SqlDataAccess<int>();
            allDates.Request(sqlDates);
            Dates = allDates.Result;

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

            return cmgnJoins;
        }

        public static int UpdateCampaign(int id, string save, string EduForm, string StsName, string CmgnTypes, string deleteCmp, int compID, int selectDate = 0)
        {
            int rowsAffected = 0;

            if (save != null)
            {

                SqlDataAccess<int> editCampaign = new SqlDataAccess<int>();

                string[] sqlArr = { @"UPDATE Campaign SET ", " EducationFormFlag = ",
                             " StatusID = "," CampaignTypeID = ", " WHERE Campaign.CampaignID = @CampaignID" };


                string sql = CheckFields(sqlArr, EduForm, StsName, CmgnTypes);


                var p = new
                {
                    CampaignID = compID,
                };

                editCampaign.ChangeInfo(sql, p);

                return rowsAffected = editCampaign.rowsAffected;
            }
            return rowsAffected;
        }
    }
}