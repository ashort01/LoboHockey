using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace UNMHockeySite.Services
{
    public class DataService
    {
        private TeamEntities db = new TeamEntities();

        public static List<PlayerStats> CreateStatistics()
        {
            List<PlayerStats> stats = new List<PlayerStats>();
            using (TeamEntities db = new TeamEntities())
            {
                stats = db.Players.Where(g => g.IsActive && g.Position.ToLower() != "goalie").Select(f => new PlayerStats
                {
                     id = f.Id,
                    name = f.Name,
                    //picture = f.Picture
                }).ToList();
                foreach (PlayerStats p in stats.ToList())
                {
                    p.goals = GetPlayerGoals(p.id);
                    p.assists = GetPlayerAssists(p.id);
                    p.position = GetPlayerPosition(p.id);
                    p.points = p.goals + p.assists;
                }
            }
            return stats;
        }

        //public static void SavePlayerPhoto(byte[] photo, int id)
        //{
        //    // Retrieve storage account from connection string.
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
        //        CloudConfigurationManager.GetSetting("StorageConnectionString"));

        //    // Create the blob client.
        //    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        //    // Retrieve a reference to a container.
        //    CloudBlobContainer container = blobClient.GetContainerReference("playercontainer");

        //    // Create the container if it doesn't already exist.
        //    container.CreateIfNotExists();

        //    // Retrieve reference to a blob named "myblob".
        //    CloudBlockBlob blockBlob = container.GetBlockBlobReference(id.ToString());

        //    blockBlob.UploadFromByteArray(photo, 0, 0, null, null, null);


        //    // Loop over items within the container and output the length and URI.
        //    foreach (IListBlobItem item in container.ListBlobs(null, false))
        //    {
        //        if (item.GetType() == typeof(CloudBlockBlob))
        //        {
        //            CloudBlockBlob blob = (CloudBlockBlob)item;

        //            Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

        //        }
        //        else if (item.GetType() == typeof(CloudPageBlob))
        //        {
        //            CloudPageBlob pageBlob = (CloudPageBlob)item;

        //            Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

        //        }
        //        else if (item.GetType() == typeof(CloudBlobDirectory))
        //        {
        //            CloudBlobDirectory directory = (CloudBlobDirectory)item;

        //            Console.WriteLine("Directory: {0}", directory.Uri);
        //        }
        //    }
        //}



        public static List<PlayerStats> GetScoringLeaders()
        {
            List<PlayerStats> allStats = CreateStatistics();
            return allStats.OrderByDescending(i => i.points).Take(5).ToList();
        }

        public static string GetPlayerPosition(int id)
        {
            string position = "";
            using (TeamEntities db = new TeamEntities())
            {
                Player p = db.Players.Where(g => g.Id == id).FirstOrDefault();
                position = p.Position;
            }
            return position;
        }

        public static Player GetPlayer(string v)
        {
            Player p = new Player();
            using (TeamEntities db = new TeamEntities())
            {
                if (db.Players.Where(g => g.User.Id == v).Any())
                {
                    p = db.Players.Include("User").Include("PlayerImage").Where(g => g.User.Id == v).FirstOrDefault();
                }
            }
            
            return p;
        }

        public static Season GetCurrentSeason()
        {
            Season s = new Season();
            using (TeamEntities db = new TeamEntities())
            {
                s = db.Seasons.OrderByDescending(e=> e.Id).FirstOrDefault<Season>();
            }
            return s;
        }

        internal static bool IsPlayerCurrentlyRequesting(int id)
        {
            using (TeamEntities db = new TeamEntities())
            {
                int CurrentSeasonId = GetCurrentSeason().Id;
                return db.PlayerRequest.Where(e => e.Player.Id == id && e.Season.Id == CurrentSeasonId && e.approved != false).Any();
            }
        }

        public static bool IsPlayerOnCurrentTeam(int playerId)
        {
            using (TeamEntities db = new TeamEntities())
            {
                int CurrentSeasonId = GetCurrentSeason().Id;
                return db.SeasonPlayer.Where(e=> e.Player.Id == playerId && e.Season.Id == CurrentSeasonId).Any();
            }
        }

        public static int GetPlayerGoals(int id)
        {
            List<Goal> goals = new List<Goal>();
            using (TeamEntities db = new TeamEntities())
            {
                goals = db.Goals.Where(g => g.Goal_PlayerID == id).ToList();
            }
            return goals.Count();
        }

        public static int GetPlayerAssists(int id)
        {
            List<Goal> goals = new List<Goal>();
            using (TeamEntities db = new TeamEntities())
            {
                goals = db.Goals.Where(g => g.Assist1_PlayerID == id || g.Assist2_PlayerID == id).ToList();
            }
            return goals.Count();
        }

        public static List<Game> GetActiveGames(int seasonId)
        {
            TeamEntities db = new TeamEntities();
            var games = db.Games;
            
            var filteredGames = games.Where(i => i.Season.Id == seasonId).OrderBy(a => a.Date);
            return filteredGames.ToList();
        }



        public static StatsViewModel GetStatsForPlayer(int id)
        {
            DateTime today = DateTime.Now;
            StatsViewModel result = new StatsViewModel();
            List<SeasonStatsViewModel> seasonStats = new List<SeasonStatsViewModel>();
            List<GameStatsViewModel> gameStats = new List<GameStatsViewModel>();
            using (TeamEntities db = new TeamEntities())
            {
                var goals = db.Goals.Include("Game").ToList();
                result.TotalGoals = goals.Where(e => e.Goal_PlayerID == id).Count();
                result.TotalAssists = goals.Where(e => e.Assist1_PlayerID == id || e.Assist2_PlayerID == id).Count();
                result.TotalPoints = result.TotalGoals + result.TotalAssists;
                var lastfiveGames = db.GamePlayer.Include("Game").Include("Goals").Where(e=> e.Player.Id == id).OrderByDescending(e => e.Game.Date).Take(5).Select(e => e.Game).ToList().OrderBy(e=> e.Date);
                foreach (var item in lastfiveGames)
                {
                    GameStatsViewModel gs = new GameStatsViewModel();
                    gs.goals = item.Goals.Where(e => e.Goal_PlayerID == id).Count();
                    gs.assists = item.Goals.Where(e => e.Assist1_PlayerID == id || e.Assist2_PlayerID == id).Count();
                    gs.points = gs.goals + gs.assists;
                    gs.Date = item.Date.Value;
                    gs.gameId = item.Id;
                    gs.IsHome = item.IsHome.Value;
                    gs.Opponent = item.Opponent;
                    gameStats.Add(gs);
                }
                var seasons = db.SeasonPlayer.Include("Season").Where(e => e.Player.Id == id).Select(e=> e.Season).ToList();
                var career = db.GamePlayer.Include("Game").Include("Goals").Where(e => e.Player.Id == id).Select(e => e.Game).ToList();
                result.TotalGamesPlayed = 0;
                foreach (var season in seasons)
                {
                    SeasonStatsViewModel gs = new SeasonStatsViewModel();
                    var seasonGames = career.Where(e => e.Season.Id == season.Id).ToList();
                    foreach (var game in seasonGames)
                    {
                        result.TotalGamesPlayed++;
                        gs.gamesPlayed++;
                        gs.goals += game.Goals.Where(e => e.Goal_PlayerID == id).Count();
                        gs.assists += game.Goals.Where(e => e.Assist1_PlayerID == id || e.Assist2_PlayerID == id).Count();
                    }
                    gs.title = season.Season_Duration;
                    gs.points = gs.goals + gs.assists;
                    seasonStats.Add(gs);

                    if (season.Id == GetCurrentSeason().Id)
                    {
                        foreach (var game in seasonGames)
                        {
                            result.CurrentSeasonGoals += game.Goals.Where(e => e.Goal_PlayerID == id).Count();
                            result.CurrentSeasonAssists += game.Goals.Where(e => e.Assist1_PlayerID == id || e.Assist2_PlayerID == id).Count();
                        }
                        result.CurrentSeasonPoints = result.CurrentSeasonAssists + result.CurrentSeasonGoals;
                    }
                }
                result.LastFiveGames = gameStats;
                result.SeasonList = seasonStats;
            }
            return result;
        }

        public static int GetOtls()
        {
            int losses = 0;
            Season currentSeason = GetCurrentSeason();
            using (TeamEntities db = new TeamEntities())
            {
                if (db.Games.Include("Season").Any(g => g.TeamScore != null && g.OpponentScore != null && currentSeason.Id == g.Season.Id))
                {
                    List<Game> games = db.Games.Include("Season").Where(g => g.TeamScore != null && g.OpponentScore != null && currentSeason.Id == g.Season.Id).ToList();
                    foreach (Game g in games)
                    {
                        if (g.TeamScore < g.OpponentScore && g.OverTime == true)
                        {
                            losses++;
                        }
                    }
                }
            }
            return losses;
        }
        public static int GetWins()
        {
            int wins = 0;
            Season currentSeason = GetCurrentSeason();
            using (TeamEntities db = new TeamEntities())
            {
                if (db.Games.Any(g => g.TeamScore != null && g.OpponentScore != null && currentSeason.Id == g.Season.Id))
                {
                    List<Game> games = db.Games.Include("Season").Where(g => g.TeamScore != null && g.OpponentScore != null && currentSeason.Id == g.Season.Id).ToList();
                    foreach (Game g in games)
                    {
                        if (g.TeamScore > g.OpponentScore)
                        {
                            wins++;
                        }
                    }
                }
            }
            return wins;
        }
        public static int GetLosses()
        {
            int losses = 0;
            Season currentSeason = GetCurrentSeason();
            using (TeamEntities db = new TeamEntities())
            {
                if (db.Games.Any(g => g.TeamScore != null && g.OpponentScore != null && currentSeason.Id == g.Season.Id))
                {
                    List<Game> games = db.Games.Include("Season").Where(g => g.TeamScore != null && g.OpponentScore != null && currentSeason.Id == g.Season.Id).ToList();
                    foreach (Game g in games)
                    {
                        if (g.TeamScore < g.OpponentScore && g.OverTime == false)
                        {
                            losses++;
                        }
                    }
                }
            }
            return losses;
        }

        public static void AddPlayerToRoster(int id)
        {
            using (TeamEntities db = new TeamEntities())
            {
                var PlayerRequest = db.PlayerRequest.Where(e => e.Id == id).FirstOrDefault();
                PlayerRequest.approved = true;
                if (!db.SeasonPlayer.Where(e => e.Season.Id == PlayerRequest.Season.Id && e.Player.Id == PlayerRequest.Player.Id).Any())
                {
                    Season_Player Seasonplayer = new Season_Player();
                    Seasonplayer.Player = PlayerRequest.Player;
                    Seasonplayer.Season = PlayerRequest.Season;
                    db.SeasonPlayer.Add(Seasonplayer);
                    db.SaveChanges();
                }
            }
        }

        public static void PlayerRequestThisSeason(string userId)
        {
            using (TeamEntities db = new TeamEntities())
            {
                PlayerRequest pr = new PlayerRequest();
                pr.Player = db.Players.Where(g => g.User.Id == userId).FirstOrDefault();
                pr.Season = db.Seasons.OrderByDescending(e=> e.Id).FirstOrDefault<Season>();
                pr.approved = null;
                db.PlayerRequest.Add(pr);
                db.SaveChanges();
            }
        }

        public static void CreateNewPlayerOnRegistrations(string userId)
        {
            using (TeamEntities db = new TeamEntities())
            {
                Player p = new Player();
                p.User = db.AspNetUsers.Where(e => e.Id == userId).FirstOrDefault<AspNetUser>();
                //p.User.UserName = db.AspNetUsers.Where(e => e.Id == userId).Select(e => e.UserName).FirstOrDefault();
                db.Players.Add(p);
                db.SaveChanges();
                //catch (DbEntityValidationException e)
                //{
                //    foreach (var eve in e.EntityValidationErrors)
                //    {
                //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                //        foreach (var ve in eve.ValidationErrors)
                //        {
                //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                //                ve.PropertyName, ve.ErrorMessage);
                //        }
                //    }
                //    throw;
                //}
            }
        }



        public static GameManagerViewModel GetGameManager(Game g)
        {
            GameManagerViewModel result = new GameManagerViewModel();
            result.Opponent = g.Opponent;
            result.Id = g.Id;
            result.IsHome = g.IsHome;
            result.OpponentScore = g.OpponentScore;
            result.TeamScore = g.TeamScore;
            result.Date = g.Date;
            result.OpponentAbbr = GetOpponentAbbreviation(g.Opponent);
            result.goals = new List<GoalViewModel>();
            List<Goal> goals = GetGoalsForGame(g);
            foreach (Goal go in goals)
            {
                GoalViewModel vm = new GoalViewModel();
                vm.GoalPlayerId = go.Goal_PlayerID;
                if (go.Assist1_PlayerID == null)
                {
                    vm.Assist1PlayerId = 0;
                }
                else
                {
                    vm.Assist1PlayerId = go.Assist1_PlayerID;
                }
                if (go.Assist2_PlayerID == null)
                {
                    vm.Assist2PlayerId = 0;
                }
                else
                {
                    vm.Assist2PlayerId = go.Assist2_PlayerID;
                }
                result.goals.Add(vm);
            }
            if (goals.Count < result.TeamScore.Value)
            {
                for(int i = goals.Count; i< result.TeamScore; i++)
                {
                    GoalViewModel vm = new GoalViewModel();
                    result.goals.Add(vm);
                }
            }
            result.teamShots = g.TeamShots;
            result.opponentShots = g.OpponentShots;
            return result;
        }



        public static List<Goal> GetGoalsForGame(Game g)
        {
            List<Goal> goals = new List<Goal>();
            using (TeamEntities db = new TeamEntities())
            {
                goals = db.Goals.Where(go => go.GameID == g.Id).ToList();
            }
            return goals;
        }

        public static List<SelectListItem> GetPlayersSelectList()
        {
            List<SelectListItem> players = new List<SelectListItem>();
            using (TeamEntities db = new TeamEntities())
            {
                players = db.Players.Where(g => g.IsActive).Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name + " " + f.Number
                }).ToList();
            }
            players.Insert(0,new SelectListItem { Value = null, Text = "-- Select a Player --" });
            return players;
        }

        public static List<GameViewModel> GetGameViewModels(List<Game> games)
        {
            List<GameViewModel> results = new List<GameViewModel>();
            foreach (Game g in games)
            {
                GameViewModel result = new GameViewModel();
                result.Opponent = g.Opponent;
                result.Id = g.Id;
                result.IsHome = g.IsHome;
                result.OpponentScore = g.OpponentScore;
                result.TeamScore = g.TeamScore;
                result.Date = g.Date;
                result.OpponentAbbr = GetOpponentAbbreviation(g.Opponent);
                result.SeasonId = g.Season.Id;
                if (g.OverTime != null)
                {
                    result.overtime = g.OverTime.Value;
                }
                else
                {
                    result.overtime = false;
                }
                results.Add(result);
            }
            return results;
        }

        public static string GetOpponentAbbreviation(string str)
        {
            if (str != null && str.Contains(' '))
            {
                return String.Concat((str.Split(' ').Where(s => Char.IsUpper(s[0])).Select(s => s[0])));
            }
            else return str;
            
        }
        public static Game GetNextGame()
        {
            Game game = new Game();
            using (TeamEntities db = new TeamEntities())
            {
                DateTime today = DateTime.Now.AddHours(-8);
                var gamelist = db.Games.Where(a => (a.Date >= today));
                if (gamelist.Any())
                {
                    game = gamelist.OrderBy(a => a.Date).First();
                }
                else
                {
                    game = null;
                }
            }
            return game;
        }

        public static void SaveStats(GameManagerViewModel game)
        {
            using (TeamEntities db = new TeamEntities())
            {
                if (db.Games.Any(g => g.Id == game.Id))
                {
                    Game g = db.Games.Where(ga => ga.Id == game.Id).First();
                    g.OpponentScore = game.OpponentScore;
                    g.TeamScore = game.TeamScore;
                    g.TeamShots = game.teamShots;
                    g.OpponentShots = game.opponentShots;
                    db.SaveChanges();
                    RemoveAllGoalsForGame(game);
                    foreach (GoalViewModel goal in game.goals)
                    {
                        if (goal.GoalPlayerId != 0)
                        {
                            Goal dbGoal = new Goal();
                            if (db.Players.Any(p => p.Id == goal.Assist1PlayerId))
                                dbGoal.Assist1_PlayerID = goal.Assist1PlayerId;
                            if (db.Players.Any(p => p.Id == goal.Assist2PlayerId))
                                dbGoal.Assist2_PlayerID = goal.Assist2PlayerId;
                            if (db.Players.Any(p => p.Id == goal.GoalPlayerId))
                                dbGoal.Goal_PlayerID = goal.GoalPlayerId;
                            dbGoal.GameID = game.Id;
                            db.Goals.Add(dbGoal);
                        }
                    db.SaveChanges();
                    }
                }
            }
        }

        public static void RemoveAllGoalsForGame(GameManagerViewModel game)
        {
            List<Goal> goalsInGame = new List<Goal>();
            using (TeamEntities db = new TeamEntities())
            {
                goalsInGame = db.Goals.Where(g => g.GameID == game.Id).ToList();
                foreach (Goal goal in goalsInGame)
                {
                    db.Goals.Remove(goal);
                }
                db.SaveChanges();
            }
        }

        public static Player GetFeaturedPlayer()
        {
            Player p = new Player();
            List<Player> ps = new List<Player>();
            Random rand = new Random();
            using (TeamEntities db = new TeamEntities())
            {
                int index = rand.Next(0, (int)db.Players.Where(a => a.IsActive == true).Count());
                ps = db.Players.Where(a => a.IsActive == true).ToList();
                if (ps.Count >= 1)
                {
                    p = ps.ElementAt(index);
                }
            }
            return p;
        }

        //public static Player CheckForPlayersWithSameEmail(string email)
        //{
        //    //Player player = new Player();
        //    //player.Email = email;
        //    //TeamEntities db = new TeamEntities();
        //    //if (db.Players.Any(a => a.Email == email && a.IsActive == true))
        //    //{
        //    //    player = db.Players.Where(a => a.Email == email && a.IsActive == true).FirstOrDefault();
        //    //    player.isEdit = true;
        //    //}
        //    //player.Picture = null;
        //    //return player;
        //}

        public static ManageRosterViewModel GetManageRosterVM(int seasonId)
        {
            ManageRosterViewModel MRVM = new ManageRosterViewModel();
            using (TeamEntities db = new TeamEntities())
            {
                MRVM.playerRequests = db.PlayerRequest.Include("Season").Include("Player").Where(e => e.Season.Id == seasonId && e.approved == null).ToList();
                MRVM.players = db.SeasonPlayer.Include("Season").Include("Player").Where(e => e.Season.Id == seasonId).ToList();
            }
            return MRVM;
        }

        internal static void RemovePlayer(int id)
        {
            using (TeamEntities db = new TeamEntities())
            {
                var seasonPlayer = db.SeasonPlayer.Where(e => e.Id == id).FirstOrDefault();
                db.SeasonPlayer.Remove(seasonPlayer);
                db.SaveChanges();
            }
        }

        public static void DeclineRequest(int id)
        {
            using (TeamEntities db = new TeamEntities())
            {
                var PlayerRequest = db.PlayerRequest.Where(e => e.Id == id).FirstOrDefault();
                PlayerRequest.approved = false;
                db.SaveChanges();
            }
        }

        public static CreateSeasonViewModel GetCreateSeasonViewModel()
        {
            using (TeamEntities db = new TeamEntities())
            {
                int thisYear = DateTime.Now.Year;
                CreateSeasonViewModel vm = new CreateSeasonViewModel();
                vm.YearList = new List<SelectListItem>();
                for (int i = 0; i<5; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = (thisYear + i).ToString() + " - " + (thisYear + i + 1).ToString();
                    item.Value = (thisYear + i).ToString() + " - " + (thisYear + i + 1).ToString();
                    vm.YearList.Add(item);
                }
                vm.YearList.Insert(0, new SelectListItem() { Text = "Select a Year", Value = "" });
                return vm;
            }
        }

        internal static void CreateNewSeason(CreateSeasonViewModel vm)
        {
            using (TeamEntities db = new TeamEntities())
            {
                Season s = new Season();
                s.Season_Duration = vm.SelectedYear;
                db.Seasons.Add(s);
                db.SaveChanges();
            }
        }

        public static ManageRosterViewModelInitial GetInitialManageRosterVM()
        {
            using (TeamEntities db = new TeamEntities())
            {
                ManageRosterViewModelInitial MRVM = new ManageRosterViewModelInitial();
                MRVM.SeasonIdList = db.Seasons.Select(e => new SelectListItem()
                            {
                                Text = e.Season_Duration,
                                Value = e.Id.ToString()
                            }).ToList();
                MRVM.SeasonIdList.Insert(0,new SelectListItem(){ Text="Select a season", Value=""});
                return MRVM;            
            }
        }

        public static ManageGamesInitial GetInitialManageGames()
        {
            using (TeamEntities db = new TeamEntities())
            {
                ManageGamesInitial MRVM = new ManageGamesInitial();
                MRVM.SeasonIdList = db.Seasons.Select(e => new SelectListItem()
                {
                    Text = e.Season_Duration,
                    Value = e.Id.ToString()
                }).ToList();
                MRVM.SeasonIdList.Insert(0, new SelectListItem() { Text = "Select a season", Value = "" });
                return MRVM;
            }
        }

        public static void SavePlayer(Player player)
        {
            Player tempPlayer = new Player();
            using (TeamEntities db = new TeamEntities())
            {
                if (db.Players.Any(a => a.Id == player.Id))
                {
                    db.Players.Attach(player);
                    db.Entry(player).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.Players.Add(player);
                    db.SaveChanges();
                }
            }
        }

        //public static void createStatsYearIfThereIsntOne(int playerId)
        //{
        //    using (TeamEntities db = new TeamEntities())
        //    {
        //        if (!db.StatsYears.Any(a => a.PlayerId == playerId))
        //        {
        //            StatsYear stat = new StatsYear();
        //            stat.PlayerId = playerId;
        //            db.StatsYears.Add(stat);
        //            db.SaveChanges();
        //        }
        //    }
        //}

        //public static Player ConvertVMToDB(PlayerViewModel player)
        //{
        //    DataService service = new DataService();
        //    Player tempPlayer = new Player();
        //    tempPlayer.Id = player.Id;
        //    tempPlayer.Number = player.Number;
        //    tempPlayer.Birthplace = player.Birthplace;
        //    tempPlayer.IsActive = player.IsActive;
        //    tempPlayer.Height = player.Height;
        //    tempPlayer.Weight = player.Weight;
        //    tempPlayer.Position = player.Position;
        //    tempPlayer.Name = player.Name;
        //    tempPlayer.Bio = player.Bio;
        //    tempPlayer.Email = player.Email;
        //    tempPlayer.Team_Role = player.Team_Role;
        //    tempPlayer.Major = player.Major;
        //    tempPlayer.Picture = service.ConvertToBytes(player.Picture);
        //    tempPlayer.Year = player.Year;
        //    tempPlayer.SnapChatURL = player.SnapChatURL;
        //    tempPlayer.TwitterURL = player.TwitterURL;
        //    tempPlayer.FacebookURL = player.FacebookURL;
        //    tempPlayer.InstagramURL = player.InstagramURL;
        //    tempPlayer.BirthYear = player.BirthYear;
        //    tempPlayer.Birthplace = player.Birthplace;
        //    tempPlayer.BirthYear = player.BirthYear;
        //    tempPlayer.isEdit = player.isEdit;
        //    tempPlayer.Age = player.Age;
        //    return tempPlayer;
        //}
        //public static PlayerViewModel ConvertDBtoVM(Player tempPlayer)
        //{
        //    DataService service = new DataService();
        //    PlayerViewModel player = new PlayerViewModel();
        //    player.Id = tempPlayer.Id;
        //    player.Number = tempPlayer.Number;
        //    player.Birthplace = tempPlayer.Birthplace;
        //    player.IsActive = tempPlayer.IsActive;
        //    player.Height = tempPlayer.Height;
        //    player.Weight = tempPlayer.Weight;
        //    player.Position = tempPlayer.Position;
        //    player.Name = tempPlayer.Name;
        //    player.Bio = tempPlayer.Bio;
        //    player.Email = tempPlayer.Email;
        //    player.Team_Role = tempPlayer.Team_Role;
        //    player.Major = tempPlayer.Major;
        //    player.Picture = tempPlayer.Picture;
        //    player.Year = tempPlayer.Year;
        //    player.SnapChatURL = tempPlayer.SnapChatURL;
        //    player.TwitterURL = tempPlayer.TwitterURL;
        //    player.FacebookURL = tempPlayer.FacebookURL;
        //    player.InstagramURL = tempPlayer.InstagramURL;
        //    player.BirthYear = tempPlayer.BirthYear;
        //    player.Birthplace = tempPlayer.Birthplace;
        //    player.BirthYear = tempPlayer.BirthYear;
        //    player.isEdit = tempPlayer.isEdit;
        //    player.Age = tempPlayer.Age;
        //}


        public static void SavePlayerAndImage(HttpPostedFileBase file, Player player)
        {
            DataService service = new DataService();
            using (TeamEntities db = new TeamEntities())
            {
                Photo photo = new Photo();
                photo.ImageData = service.ConvertToBytes(file);
                photo.IsActive = true;
                db.Photos.Add(photo);
                player.PlayerImage = photo;
                db.Players.Attach(player);
                db.SaveChanges();
            }
        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
        public static List<Game> GetNextFiveGames()
        {
            List <Game> nextGames = new List<Game>();
            using (TeamEntities db = new TeamEntities())
            {
                DateTime today = DateTime.Now.AddHours(-8);
                nextGames = db.Games.Where(a => (a.Date >= today)).OrderBy(a => a.Date).Take(5).ToList();
            }
            return nextGames;
        }

        public static List<Player> GetActivePlayers()
        {
            List<Player> players = new List<Player>();
            using (TeamEntities db = new TeamEntities())
            {
                players = db.Players.Where(a => a.IsActive == true).ToList();
            }
            return players;
        }

        public static byte[] GetImageForPlayer(int playerId)
        {
            using (TeamEntities db = new TeamEntities())
            {
                if (db.Players.Any(a => a.IsActive == true && a.Id == playerId))
                {
                    var record = db.Players.Where(a => a.IsActive == true && a.Id == playerId).Select(a => a.PlayerImage).First();
                    if (record!= null)
                    {
                        return record.ImageData;
                    }
                }
            }
            return null;
        }

    }
}