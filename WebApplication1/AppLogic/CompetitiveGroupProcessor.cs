using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.DataAccess;
using static WebApplication1.Classes.Values;

namespace WebApplication1.AppLogic
{
    public class CompetitiveGroupProcessor
    {
        public static List<int> cmpGrDates { get; set; }
        public static List<AdmissionItemType> EduLevel { get; set; }
        public static List<Direction> direction { get; set; }
        public static List<AdmissionItemType> EduForm { get; set; }


        public static List<CompetitiveGroup> SearchCompetitiveGroup(int id, string cgName, int cgDate = 0)
        {

            string findCmpGrps = @"SELECT * FROM CompetitiveGroup 
                                    WHERE InstitutionID = @InstitutionID
                                    AND Name LIKE '%' + @Name +'%' ";
            var cgParams = new
            {
                InstitutionID =id,
                Name = cgName
            };

            var dateParams = new
            {
                InstitutionID = id
            };

            if (cgDate != 0)
                findCmpGrps += " AND DATEPART(YEAR, CreatedDate) = @date";

            string sqlAdmsn = @"SELECT * FROM AdmissionItemType";

            string sqlDirection = @"SELECT * FROM Direction";

            string dates = @"SELECT DISTINCT DATEPART(YEAR, CreatedDate) FROM CompetitiveGroup WHERE InstitutionID = @InstitutionID  ";


            var compGroups = new List<CompetitiveGroup>();
            var edLevel = new List<AdmissionItemType>();
            var edForm = new List<AdmissionItemType>();
 

            SqlDataAccess<int> compGrpDates = new SqlDataAccess<int>();
            compGrpDates.Request(dates, dateParams);
            cmpGrDates = compGrpDates.Result;

            SqlDataAccess<CompetitiveGroup> compGroupsInfo = new SqlDataAccess<CompetitiveGroup>();
            compGroupsInfo.Request(findCmpGrps, cgParams);
            compGroups = compGroupsInfo.Result;

            SqlDataAccess<AdmissionItemType> allAdmItems = new SqlDataAccess<AdmissionItemType>();
            allAdmItems.Request(sqlAdmsn);
            edLevel = allAdmItems.Result;
            edForm = allAdmItems.Result;

            SqlDataAccess<Direction> allDirection = new SqlDataAccess<Direction>();
             allDirection.Request(sqlDirection);
            direction = allDirection.Result;

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

            EduForm = edForm.Where(e => e.ItemLevel == 7).ToList();
            EduLevel = edLevel.Where(e => e.ItemLevel == 2).ToList();

            return CompetitiveGroups;

            }

        public static int EditCompetitiveGroup(int id, int compGrpId, string EdLvlId, string EdFrmId, string DrctnId, string save)
        {
            int rowsAffected = 0;

            if (save != null)
            {
                var compGroups = new List<CompetitiveGroup>();

                SqlDataAccess<int> editCompGroup = new SqlDataAccess<int>();
                string[] sqlArr = { @"UPDATE CompetitiveGroup SET ", " EducationLevelID = ",
                             " EducationFormId = "," DirectionID = ", " WHERE CompetitiveGroupID = @CompetitiveGroupID"};

                string sql = CheckFields(sqlArr, EdLvlId, EdFrmId, DrctnId);

                var cgParams = new
                {
                    CompetitiveGroupID = compGrpId
                };

                editCompGroup.ChangeInfo(sql, cgParams);

                return rowsAffected = editCompGroup.rowsAffected;
            }
            return rowsAffected;
        }
    }
}