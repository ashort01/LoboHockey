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
            HomePageViewModel vm = new HomePageViewModel();
            vm.nextFiveGames = DataService.GetNextFiveGames();
            vm.nextGame = DataService.GetNextGame();
            vm.featuredPlayer = DataService.GetFeaturedPlayer();
            vm.wins = DataService.GetWins();
            vm.losses = DataService.GetLosses();
            vm.otls = DataService.GetOtls();
            vm.scoringLeaders = DataService.GetScoringLeaders();
            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public ActionResult SeasonTickets()
        {
            EmailFormModel model = new EmailFormModel();
            bool state = ModelState.IsValid;
            return View(model);
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SeasonTickets")]
        public async Task<ActionResult> SendEmailOrder(EmailFormModel model)
        {
            if (ModelState.IsValid)
            {

                EmailService.SendOrderEmail(model);
                return RedirectToAction("OrderSuccess");
            }
            return View(model);
        }



    }
}