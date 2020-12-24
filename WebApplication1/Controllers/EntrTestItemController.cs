using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Classes;
using static WebApplication1.AppLogic.EntrTestItemProcessor;

namespace WebApplication1.Controllers
{
    public class EntrTestItemController : Controller
    {
        public ActionResult EntrTestItem(int id, int entrTestID = 0, int dateInt = 0, int cgID = 0)
        {
            ViewBag.FormEtID = entrTestID;

            InstitutionFullInfo<int> inst = new InstitutionFullInfo<int>();
            inst.InstitutionID = id;

            var data = SearchEntrTestItem(id, cgID);

            List<InstitutionFullInfo<int>> compGroupsInfo = new List<InstitutionFullInfo<int>>();

            ViewBag.compGroupsInfo = data;

            return View("EntrTestItem", inst);
        }

        [HttpPost]
        public ActionResult EntrTestItem(int id, InstitutionFullInfo<int> EntranceTestItemCInfo, FormCollection form, string submitButton, string deleteEntrTestItem)
        {
            ViewBag.institutionEditInfo = new List<InstitutionFullInfo<int>>();

            var rowsAffected = EditMinScore(EntranceTestItemCInfo.MinScore, EntranceTestItemCInfo.EntranceTestItemID, submitButton);

            if (rowsAffected > 0)
            { 
                return RedirectToAction("EntrTestItem", "EntrTestItem",
                    new
                    {
                        id = id,
                        dateInt = EntranceTestItemCInfo.CreatedDate,
                        entrTestID = EntranceTestItemCInfo.EntranceTestItemID,
                        cgID = EntranceTestItemCInfo.CompetitiveGroupID
                    });
            }
            
            //return rowsAffected == 1 ? View("SuccessfullEdit") : View("Error");
            //Redirect("/Search/EditInstitutionInfo/" + id);

            return RedirectToAction("EntrTestItem");
        }
    }
}