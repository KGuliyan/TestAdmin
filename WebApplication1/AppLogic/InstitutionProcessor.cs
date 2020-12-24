using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Models.DBModels;
using WebApplication1.DataAccess;


namespace WebApplication1.AppLogic
{
    public static class InstitutionProcessor
    {
        public static List<InstitutionDB> SearchInstitution( int institutionId, string institutionName)
        {

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

            var p = new
            {
                InstitutionID = institutionId,
                Name = institutionName
            };

            SqlDataAccess<InstitutionDB> institutions = new SqlDataAccess<InstitutionDB>();

            institutions.Request(sqlExpression, p);

            return institutions.Result;

        }
    }
}