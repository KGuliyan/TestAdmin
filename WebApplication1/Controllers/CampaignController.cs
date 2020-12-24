//using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Classes;
using static WebApplication1.AppLogic.CampaignProcessor;

namespace WebApplication1.Controllers
{
    public class CampaignController : Controller
    {
  
        public ActionResult Campaign(int id, string edForm, string stName, string cmgType, int cmpnID = 0, int selectDate = 0)
        {
            Campaign comp = new Campaign();

            comp.InstitutionID = id;

            comp.CampaignID = cmpnID;

            //В модель Campaign записываются данные из DropDownList (если выбраны) для отображения изменений во View

            if (!String.IsNullOrEmpty(edForm))
            {
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

          
            var data = SearchCampaign(id, selectDate);
            

            if (selectDate == 0)
            {
                ViewBag.SelectedYear = "Выберите год";
            }
            else
            {
                ViewBag.SelectedYear = selectDate.ToString();
            }

            SelectList dates = new SelectList(Dates, selectDate);
            ViewBag.dates = dates;

            SelectList cmgTypeName = new SelectList(CmgnTypes, "CampaignTypeID", "Name");
            ViewBag.CmgTypeName = cmgTypeName;


            SelectList eduForm = new SelectList(EduForm, "Id", "Name");
            ViewBag.EduForm = eduForm;

            SelectList stsName = new SelectList(StsName, "StatusID", "Name");
            ViewBag.StsName = stsName;

            ViewBag.campaigns = data;

            return View("Campaign", comp);
        }


        //Изменение формы, типа, статуса ПК
        [HttpPost]
        public ActionResult Campaign(int id, Campaign comp, FormCollection form, string save, string EduForm, string StsName, string CmgnTypes, string deleteCmp, string CampaignID, int selectDate = 0)
        {

            int cmpID = int.Parse(CampaignID);

            string date = form["dates"];
            if (form.AllKeys.Contains("dates") && !String.IsNullOrEmpty(date))
            {
                selectDate = int.Parse(date);
            }

            var rowsAffected = UpdateCampaign(id, save, EduForm, StsName, CmgnTypes, deleteCmp, cmpID, selectDate);

            if (rowsAffected > 0 || selectDate > 0)
            {
                return RedirectToAction("Campaign", "Campaign",
                    new { id = id, selectDate = selectDate, edForm = EduForm, stName = StsName, cmgType = CmgnTypes, cmpnID = comp.CampaignID });
            }

            return View();

        }
    }
}