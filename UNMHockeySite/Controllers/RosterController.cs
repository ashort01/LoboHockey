using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using UNMHockeySite.Models;
using UNMHockeySite.Services;

namespace UNMHockeySite.Controllers
{
    public class RosterController : Controller
    {
        private TeamEntities db = new TeamEntities();
        private UserManager<ApplicationUser> manager;
        [HttpGet]
        public ActionResult Index()
        {
            return View(DataService.GetActivePlayers());
        }

        //[HttpGet]
        //public ActionResult PlayerLogIn()
        //{
        //    PlayerLogInViewModel PLIVM = new PlayerLogInViewModel();
        //    return View(PLIVM);
        //}

        //[HttpPost]
        //public async Task<ActionResult> PlayerLogin(PlayerLogInViewModel loginInfo)
        //{
        //    if (ModelState.IsValid == false)
        //    {
        //        return View(loginInfo);
        //    }

        //    else if(loginInfo.Password != "ilovehockey")
        //    {
        //        ModelState.AddModelError("Password", "Invalid username or password");
        //        ApplicationUser user = new ApplicationUser();
        //        FormsAuthentication.SetAuthCookie(loginInfo.Email, true);
        //        user.Email = loginInfo.Email;
        //        await user.GenerateUserIdentityAsync(manager);
        //        return View(new PlayerLogInViewModel());
        //    }
        //    else
        //    {
        //        return RedirectToAction("CreatePlayer", "Roster");
        //    }
        //}
        [HttpGet]
        public ActionResult CreatePlayer()
        {
            Player p = DataService.GetPlayer(User.Identity.GetUserId());
            return View(p);
        }

        [HttpPost]
        public ActionResult CreatePlayer(Player player, FormCollection form, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                //int month = 0;
                //Int32.TryParse(form["month"], out month);
                //int day = 0;
                //Int32.TryParse(form["day"], out day);
                //int year = 0;
                //Int32.TryParse(form["birthyear"], out year);
                //if (month != 0 && day != 0 && year != 0)
                //{
                //    player.Birthdate = month + "/" + day + "/" + year;
                //}
                player.IsActive = true;
                if (player.BirthDay != null && player.BirthMonth != null && player.BirthMonth != null)
                {
                    DateTime birthdate = new DateTime(player.BirthYear.Value, player.BirthMonth.Value, player.BirthDay.Value);
                    DateTime now = DateTime.Now;
                    int age = now.Year - birthdate.Year;
                    if (now < birthdate.AddYears(age)) age--;
                    player.Age = age;
                }



                if (file != null)
                {
                    DataService.SavePlayerAndImage(file, player);
                }
                else
                {
                    DataService.SavePlayer(player);
                }
                
                return RedirectToAction("Index", "Manage");
            }
            else
            {
                return View(player);
            }
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = db.Players.Include("PlayerImage").FirstOrDefault(i => i.Id == id);
            if (player == null)
            {
                return HttpNotFound();
            }
            PlayerStatsBioViewModel vm = new PlayerStatsBioViewModel();
            vm.player = player;
            vm.stats = DataService.GetStatsForPlayer(player.Id);
            
            return View(vm);
        }

        public ActionResult LoadPlayerImage(int id)
        {

            byte[] img = DataService.GetImageForPlayer(id);
            if (img != null)
            {
                return File(img, "image/jpg");
            }
            else
            {
                return null;
            }
        }





    }
}