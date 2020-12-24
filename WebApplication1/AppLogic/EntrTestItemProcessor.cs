using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models;
using WebApplication1.DataAccess;

namespace WebApplication1.AppLogic
{
    public class EntrTestItemProcessor
    {
        public static List<InstitutionFullInfo<int>> SearchEntrTestItem(int id, int cgID)
        {

            List<InstitutionFullInfo<int>> compGroupsInfo = new List<InstitutionFullInfo<int>>();

            string sqlFirst = @"SELECT 
                                DISTINCT DATEPART(YEAR, CompetitiveGroup.CreatedDate) AS CreatedDate,
                                CompetitiveGroup.CompetitiveGroupID AS CompetitiveGroup ,
                                Institution.FullName AS FullName,
                                Institution.InstitutionID AS InstitutionID,
                                EntranceTestItemC.MinScore AS MinScore ,
                                EntranceTestItemC.EntranceTestItemID AS EntranceTestItemID,
                                EntranceTestItemC.SubjectName AS Name,
                                CompetitiveGroup.CompetitiveGroupID AS CompetitiveGroupID

                                FROM Institution 
                            
                                JOIN CompetitiveGroup  on CompetitiveGroup.InstitutionID = Institution.InstitutionID
                                JOIN EntranceTestItemC on EntranceTestItemC.CompetitiveGroupID = CompetitiveGroup.CompetitiveGroupID
                                WHERE CompetitiveGroup.InstitutionID = @InstitutionId AND CompetitiveGroup.CompetitiveGroupID = @CompetitiveGroupID ";

            var entrTestItemParams = new
            {
                InstitutionId = id,
                CompetitiveGroupID = cgID
            };

            SqlDataAccess<InstitutionFullInfo<int>> cmpGrpInfo = new SqlDataAccess<InstitutionFullInfo<int>>();
            cmpGrpInfo.Request(sqlFirst, entrTestItemParams);
            compGroupsInfo = cmpGrpInfo.Result;

            return compGroupsInfo;

        }

        public static int EditMinScore(int minScore, int testItemID, string submitButton)
        {
            int rowsAffected = 0;

            SqlDataAccess<int> editEntrTestItem = new SqlDataAccess<int>();

            string editMinscore = "";

            var minScoreParams = new
            {
                MinScore = minScore,
                TestitemID = testItemID
            };

            if (submitButton != null)
            {

                editMinscore = @"UPDATE EntranceTestItemC set MinScore = @MinScore  
                          WHERE EntranceTestItemC.EntranceTestItemID = @TestitemID ";


                editEntrTestItem.ChangeInfo(editMinscore, minScoreParams);
                rowsAffected = editEntrTestItem.rowsAffected;
            }

            return rowsAffected; 
        }
    }
}