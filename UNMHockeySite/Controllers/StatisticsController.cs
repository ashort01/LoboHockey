using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Models;

namespace UNMHockeySite.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index()
        {
            StatisticsViewModel vm = new StatisticsViewModel();
            return View(vm);
        }
    }
}