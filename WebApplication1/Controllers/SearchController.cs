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
using WebApplication1.Classes;
using static WebApplication1.AppLogic.InstitutionProcessor;

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

            //string sqlExpression = "SELECT COUNT(InstitutionID) FROM Institution;";

            //InfoFromDB<int> cntInst = new InfoFromDB<int>();

            //cntInst.Request(sqlExpression);

            //ViewBag.InstitutionCount = "Количество образовательных организаций: " + cntInst.Result[0];

            ViewBag.InstitutionInfo = new List<InstitutionFullInfo<DateTime>>();
            InstitutionViewModel vmm = new InstitutionViewModel();
            return View(vmm);
        }

        public ActionResult Results(InstitutionViewModel model, int id = 0)

        {
            var institutionId = model.InstitutionID;

            if (id != 0)
                institutionId = id;

            var institutionName = model.FullName;

            var data = SearchInstitution(institutionId, institutionName);

            List<InstitutionViewModel> insts = new List<InstitutionViewModel>();

            foreach (var row in data)
            {
                insts.Add(new InstitutionViewModel
                {
                    InstitutionID = row.InstitutionID,
                    FullName = row.FullName,
                    BriefName = row.BriefName
                });
            }

            ViewBag.InstitutionInfo = insts;

            return View("Search");
        }
       
    }
}