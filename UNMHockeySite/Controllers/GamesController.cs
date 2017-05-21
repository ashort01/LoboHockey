using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Models;
using UNMHockeySite.Services;

namespace UNMHockeySite.Controllers
{
    public class GamesController : Controller
    {
        private TeamEntities db = new TeamEntities();

        // GET: Games
        public ActionResult Index()
        {
            Season s = DataService.GetCurrentSeason();
            List<Game> games = DataService.GetActiveGames(s.Id);
            List<GameViewModel> gameVms = DataService.GetGameViewModels(games);

            return View(gameVms);
        }

        // GET: Manage
        public ActionResult Manage(int SeasonId)
        {
            List<Game> games = DataService.GetActiveGames(SeasonId);
            ManageGamesIndexViewModel vm = new ManageGamesIndexViewModel();
            vm.games = DataService.GetGameViewModels(games);
            vm.seasonId = SeasonId;
            return View(vm);
        }


        // GET: Games/Details/5
        public ActionResult Details(int? id)
        {
            System.Diagnostics.Debug.WriteLine("in method");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // GET: Games/Create
        public ActionResult Create(int seasonId)
        {
            Game g = new Game();
            Season s = new Season();
            s.Id = seasonId;
            g.Season = s;
            return View(g);
        }

        public ActionResult GetEvents()
        {
            var eventList = GetGames();
            foreach(Events newevent in eventList)
            {
                if(newevent.isHome == true)
                {
                    newevent.color = "#D41045";
                }
                else if(newevent.isHome == false)
                {
                    newevent.color = "#C4C4C4";
                }
            }

            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        private List<Events> GetGames()
        {
            List<Events> eventList = new List<Events>();
            var games = db.Games;
            List<Game> schedule = new List<Game>();
            schedule = games.ToList();
            foreach (Game game in schedule)
            {
                Events newEvent = new Events();
                newEvent.id = game.Id.ToString();
                if (!game.IsHome.Value)
                {
                    newEvent.title = " @ " + DataService.GetOpponentAbbreviation(game.Opponent);
                }
                else newEvent.title = " " + DataService.GetOpponentAbbreviation(game.Opponent);
                newEvent.start = game.Date.ToString();
                newEvent.end = game.Date.Value.AddHours(3).ToString();
                newEvent.allDay = false;
                newEvent.isHome = game.IsHome;
                eventList.Add(newEvent);

            }
            return eventList;
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Opponent,IsHome,Date,TeamScore,OpponentScore")] Game game)
        {
            if (ModelState.IsValid)
            {
                db.Games.Add(game);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(game);
        }



        // GET: Games/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        public ActionResult EditGamePhotos(int gameId)
        {
            if (gameId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(gameId);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }


        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Game game)
        {
            if (ModelState.IsValid)
            {
                db.Entry(game).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EditDetails", new { gameId = game.Id});
            }
            return View(game);
        }

        //GET: Edit details
        [HttpGet]
        public ActionResult EditDetails(int? gameId)
        {
            Game game = db.Games.Find(gameId);
            GameManagerViewModel g = DataService.GetGameManager(game);
            return View(g);
        }

        //POST: Edit details
        [HttpPost]
        public ActionResult EditDetails(GameManagerViewModel game)
        {
            DataService.SaveStats(game);
            return RedirectToAction("Manage");
        }


        // GET: Games/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Game game = db.Games.Find(id);
            int seasonIdNumber = game.Season.Id;
            foreach(Goal g in game.Goals.ToList())
            {
                db.Goals.Remove(g);
            }
            db.Games.Remove(game);
            db.SaveChanges();
            return RedirectToAction("Manage",new { seasonId= seasonIdNumber });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        //[HttpPost]
        //public async Task<ActionResult> ManagerLogin(PlayerLogInViewModel loginInfo)
        //{
        //    if (ModelState.IsValid == false)
        //    {
        //        return View(loginInfo);
        //    }

        //    else if (loginInfo.Password != "ilovehockey")
        //    {
        //        return View(new PlayerLogInViewModel());
        //    }
        //    else
        //    {
        //        return RedirectToAction("Manage","Games");
        //    }
        //}

        [HttpGet]
        public ActionResult ManagerLogin()
        {
            PlayerLogInViewModel PLIVM = new PlayerLogInViewModel();
            return View(PLIVM);
        }
    }
}
