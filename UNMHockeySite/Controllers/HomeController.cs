using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using UNMHockeySite.Services;

namespace UNMHockeySite.Controllers
{
    public class HomeController : Controller
    {
        private TeamEntities db = new TeamEntities();
        // GET: Next Game
        public ActionResult Index()
        {

            DataService.IncrementHitCount();
            HomePageViewModel vm = DataService.GetHomePage();
            //vm.scoringLeaders = DataService.GetScoringLeaders(DataService.GetCurrentSeason().Id);
            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Advertisement()
        {
            return View();
        }

        public ActionResult Video()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}