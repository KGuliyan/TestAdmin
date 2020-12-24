using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
public class InstitutionFullInfo <T>
{
        public int InstitutionID { get; set; }
        public string FullName { get; set; }
        public string BriefName { get; set; }



        public int CompetitiveGroupID { get; set; }
        public string Name { get; set; }
        public T CreatedDate { get; set; }


        public int EntranceTestItemID { get; set; }
        public int MinScore { get; set; }


        public virtual List<Campaign> Campaigns { get; set; }

    }



    public class Campaign
    {
        public int InstitutionID { get; set; }
        public int CampaignID { get; set; }
        public string Name { get; set; }
        public int YearStart { get; set; }
        public int YearEnd { get; set; }

        public int EducationFormFlag { get; set; }
        public string EducationFormName { get; set; }

        public int StatusID { get; set; }
        public string CampaignStatusName { get; set; }

        public int CampaignTypeID { get; set; }
        public string CampaignTypeName { get; set; }

    }

    public class CampaignTypes
    {
        public int CampaignTypeID { get; set; }
        public string Name { get; set; }

    }

    public class EducationForm
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class CampaignStatus
    { 
        public int StatusID { get; set; }
        public string Name { get; set; }

    }

    public class CompetitiveGroup
    {
        public string Name { get; set; }
        public int CompetitiveGroupID { get; set; }
        public int  InstitutionID { get; set; }
        public int CampaignID { get; set; }
        public DateTime CreatedDate { get; set; }

        public int EducationLevelID { get; set; }
        public string EducationLevelName { get; set; }

        public int EducationFormId { get; set; }
        public string EducationFormName { get; set; }

        public int DirectionID { get; set; }
        public string DirectionName { get; set; }



    }

    public class AdmissionItemType
    {
        public string Name { get; set; }
        public int ItemTypeID { get; set; }
        public int ItemLevel { get; set; }

    }

    public class Direction
    {
        public string Name { get; set; }
        public int DirectionID { get; set; }

    }






    //public class Institution
    //{

    //    public int InstitutionID { get; set; }
    //    public string FullName { get; set; }
    //    public string BriefName { get; set; }
    //    public virtual List<CompetitiveGroup> CompetitiveGroups { get; set; }
    //    public virtual List<EntranceTestItemC> EntranceTestItemCs { get; set; }

    //}

    //public class CompetitiveGroup
    //{

    //    public int CompetitiveGroupID { get; set; }
    //    public string Name { get; set; }
    //    public int CreatedDate { get; set; }
    //    public virtual List<EntranceTestItemC> EntranceTestItemCs { get; set; }
    //}

    //public class EntranceTestItemC
    //{
    //   // public int CompetitiveGroupID { get; set; }
    //    public int EntranceTestItemID { get; set; }
    //    public int MinScore { get; set; }
    //    //public List<Institution> Institutions { get; set; }
    //    //public virtual List<CompetitiveGroup> CompetitiveGroups { get; set; }

    //}




    public class InstitutionViewModel
    {
        [Required]
        [Display(Name = "Идентификатор ОО")]
        public int InstitutionID { get; set; }

        [Required]
        [Display(Name = "Название ОО")]
        public string FullName { get; set; }

        public string BriefName { get; set; }

    }

    
}