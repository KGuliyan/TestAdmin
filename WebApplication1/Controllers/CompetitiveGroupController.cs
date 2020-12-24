
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Classes;
using static WebApplication1.AppLogic.CompetitiveGroupProcessor;


namespace WebApplication1.Controllers
{
    public class CompetitiveGroupController : Controller
    {
        

        public ActionResult CompetitiveGroup(int id, string Name, string cgName, string EdLvlId, string EdFrmId, string DrctnId, int cgDate = 0)
        {
            ViewBag.compGroups = new List<CompetitiveGroup>();

            if (String.IsNullOrEmpty(cgName) && String.IsNullOrEmpty(EdLvlId) && String.IsNullOrEmpty(EdFrmId) && String.IsNullOrEmpty(DrctnId))
            {
                ViewBag.emptyResult = "hide";
            }

            var cg = new CompetitiveGroup();
            cg.InstitutionID = id;
            cg.Name = cgName;

            if (cgName != null)
            {
               
                var data = SearchCompetitiveGroup(id, cgName);

                SelectList eduFormName = new SelectList(EduForm, "ItemTypeID", "Name");
                ViewBag.eduFormName = eduFormName;

                SelectList eduLevelName = new SelectList(EduLevel, "ItemTypeID", "Name");
                ViewBag.eduLevelName = eduLevelName;

                SelectList drctnName = new SelectList(direction, "DirectionID", "Name");
                ViewBag.drctnName = drctnName;

                SelectList Dates = new SelectList(cmpGrDates, "CreatedDate");
                ViewBag.Dates = Dates;

                ViewBag.compGroups = data;
                   
             }

            if (cgDate == 0)
            {
                ViewBag.cgDate = "Выберите год";
            }
            else
            {
                ViewBag.cgDate = cgDate.ToString();
            }

            return View(cg);
        }

        [HttpPost]
        public ActionResult CompetitiveGroup(int id, CompetitiveGroup cg)
        {
            return RedirectToAction("CompetitiveGroup", "CompetitiveGroup",
                    new { id = id, cgName = cg.Name });
        }


        [HttpPost]
        public ActionResult UpdateCompetitiveGroup(int id, CompetitiveGroup cg, FormCollection form, string EdLvlId, string EdFrmId, string DrctnId, string save)
        {
            //int selectDate = 0;
            //string date = form["CreatedDate"];
            //if (form.AllKeys.Contains("CreatedDate") && !String.IsNullOrEmpty(date))
            //{
            //    selectDate = int.Parse(date);
            //}

            ViewBag.compGroups = new List<CompetitiveGroup>();
            var compGroups = new List<CompetitiveGroup>();

            if (cg.EducationLevelID != 0)
                EdLvlId = cg.EducationLevelID.ToString();

            if (cg.EducationFormId != 0)
                EdFrmId = cg.EducationFormId.ToString();

            if (cg.DirectionID != 0)
                DrctnId = cg.DirectionID.ToString();

            var rowsAffected = EditCompetitiveGroup(id, cg.CompetitiveGroupID, EdLvlId, EdFrmId, DrctnId, save);


            if (rowsAffected > 0)
            {
                return RedirectToAction("CompetitiveGroup", "CompetitiveGroup",
                    new { id = id, cgName = cg.Name, EdLvlId = EdLvlId, EdFrmId = EdFrmId, DrctnId = DrctnId });
            }


            return RedirectToAction("CompetitiveGroup", "CompetitiveGroup", new { id = id, cgName = cg.Name});
        }

    }
}
