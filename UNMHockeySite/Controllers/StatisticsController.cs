using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Models;
using UNMHockeySite.Services;

namespace UNMHockeySite.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index(int? seasonId)
        {
            if (seasonId == null)
            {
                seasonId = DataService.GetCurrentSeason().Id;
            }
            StatisticsViewModel vm = new StatisticsViewModel(seasonId.Value);
            return View(vm);
        }

    }
}