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
using System.Drawing;
using RestSharp;
using RestSharp.Deserializers;

namespace UNMHockeySite.Services
{
    public class DataService
    {
        private TeamEntities db = new TeamEntities();

        public static void IncrementHitCount()
        {
            using (TeamEntities db = new TeamEntities())
            {
                HomePageHit rec = new HomePageHit { Date = DateTime.Now };
                db.HomePageHits.Add(rec);
                db.SaveChanges();
            }
        }

        public static HomePageViewModel GetHomePage()
        {
            HomePageViewModel vm = new HomePageViewModel();
            using (TeamEntities db = new TeamEntities())
            {
                var currentSeason = GetCurrentSeason();
                DateTime today = DateTime.Now.AddHours(-8);
                vm.nextFiveGames = db.Games.Where(a => (a.Date >= today)).OrderBy(a => a.Date).Take(5).ToList();
                vm.nextGame = vm.nextFiveGames.First();
                vm.wins = 0;
                vm.losses = 0;
                vm.otls = 0;
                if (db.Games.Any(g => g.TeamScore != null && g.OpponentScore != null && currentSeason.Id == g.Season.Id))
                {
                    List<Game> games = db.Games.Include("Season").Where(g => g.TeamScore != null && g.OpponentScore != null && currentSeason.Id == g.Season.Id).ToList();
                    foreach (Game g in games)
                    {
                        if (g.TeamScore > g.OpponentScore)
                        {
                            vm.wins++;
                        }
                        else if (g.TeamScore < g.OpponentScore && g.OverTime == false)
                        {
                            vm.losses++;
                        }
                        else
                        {
                            vm.otls++;
                        }
                    }
                }
                List<int> seasongames = db.Games.Where(i => i.Season.Id == currentSeason.Id).Select(i => i.Id).ToList();
                var goals = db.Goals.Where(i => i.GameID != null).Where(i => seasongames.Contains(i.GameID.Value)).ToList();
                goals = goals.Where(i => seasongames.Contains(i.GameID.Value)).ToList();
                var goalsDictionary = db.SeasonPlayer.Include("Player").Where(i => i.Season.Id == currentSeason.Id).ToDictionary(c => c.Player.Id, c => 0);
                var assistsDictionary = db.SeasonPlayer.Include("Player").Where(i => i.Season.Id == currentSeason.Id).ToDictionary(c => c.Player.Id, c => 0);
                var pointsDictionary = db.SeasonPlayer.Include("Player").Where(i => i.Season.Id == currentSeason.Id).ToDictionary(c => c.Player.Id, c => 0);
                foreach(Goal g in goals)
                {
                    goalsDictionary[g.Goal_PlayerID]++;
                    pointsDictionary[g.Goal_PlayerID]++;
                    if (g.Assist1_PlayerID != null)
                    {
                        assistsDictionary[g.Assist1_PlayerID.Value]++;
                        pointsDictionary[g.Assist1_PlayerID.Value]++;
                    }
                    if (g.Assist2_PlayerID != null)
                    {
                        assistsDictionary[g.Assist2_PlayerID.Value]++;
                        pointsDictionary[g.Assist2_PlayerID.Value]++;
                    }
                }
                var goalsId = goalsDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                var assistsId = assistsDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                var pointsId = pointsDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                var mostGoalsPlayer = db.Players.Where(i => i.Id == goalsId).FirstOrDefault();
                vm.mostGoals =
                    new PlayerStats
                    {
                        id = mostGoalsPlayer.Id,
                        name = mostGoalsPlayer.Name,
                        picture = GetImageForPlayer(mostGoalsPlayer.Id)
                    };
                var mostAssistsPlayer = db.Players.Where(i => i.Id == assistsId).FirstOrDefault();
                vm.mostAssists =
                    new PlayerStats
                    {
                        id = mostAssistsPlayer.Id,
                        name = mostAssistsPlayer.Name,
                        picture = GetImageForPlayer(mostAssistsPlayer.Id)
                    };
                var mostPointsPlayer = db.Players.Where(i => i.Id == pointsId).FirstOrDefault();
                vm.mostPoints =
                    new PlayerStats
                    {
                        id = mostPointsPlayer.Id,
                        name = mostPointsPlayer.Name,
                        picture = GetImageForPlayer(mostPointsPlayer.Id)
                    };
                vm.mostGoalsNum = goalsDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
                vm.mostAssistsNum = assistsDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
                vm.mostPointsNum = pointsDictionary.Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
            }
            return vm;
        }

        public static HomePageHitsViewModel GetSiteHitStats()
        {
            HomePageHitsViewModel vm = new HomePageHitsViewModel();
            using (TeamEntities db = new TeamEntities())
            {
                vm.thisMonthHits = db.HomePageHits.Where(i => i.Date.Month == DateTime.Now.Month).Count();
                vm.thisYearHits = db.HomePageHits.Where(i => i.Date.Year == DateTime.Now.Year).Count();
                vm.totalHits = db.HomePageHits.Count();
            }
            return vm;
        }


        public static List<PlayerStats> CreateStatistics(int seasonId)
        {
            List<PlayerStats> stats = new List<PlayerStats>();
            using (TeamEntities db = new TeamEntities())
            {
                var seasonPlayers = db.SeasonPlayer.Include(i => i.Player).Include(i => i.Season).Where(i => i.Season.Id == seasonId).Select(i => i.Player.Id);
                stats = db.Players.Where(g => g.IsActive && g.Position.ToLower() != "goalie" && seasonPlayers.Contains(g.Id)).Select(f => new PlayerStats
                {
                     id = f.Id,
                    name = f.Name,
                }).ToList();
                foreach (PlayerStats p in stats.ToList())
                {
                    p.picture = GetImageForPlayer(p.id);
                    p.goals = GetPlayerGoals(p.id, seasonId);
                    p.assists = GetPlayerAssists(p.id, seasonId);
                    p.position = GetPlayerPosition(p.id);
                    p.games_played = GetGamesPlayed(p.id, seasonId);
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



        public static List<PlayerStats> GetScoringLeaders(int seasonId)
        {
            List<PlayerStats> allStats = CreateStatistics(seasonId);
            return allStats.OrderByDescending(i => i.points).Take(5).ToList();
        }

        public static int GetGamesPlayed(int playerId, int SeasonId)
        {
            using (TeamEntities db = new TeamEntities())
            {
                var games = db.Games.Include(g=>g.Season).Where(g => g.Season.Id == SeasonId).Select(g=> g.Id).ToList();
                var gameList = db.GamePlayer.Include(g => g.Game).Include(g => g.Player).Where(g => g.Player.Id == playerId && games.Contains(g.Game.Id));
                return gameList.Count();
            }
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

        internal static void CreateGame(GameViewModel vm)
        {
            using (TeamEntities db = new TeamEntities())
            {
                Season season = db.Seasons.Where(e => e.Id == vm.SeasonId).FirstOrDefault();
                if (season != null)
                {
                    Game game = new Game();
                    game.Opponent = vm.Opponent;
                    game.IsHome = vm.IsHome;
                    game.Season = season;
                    game.Date = DateTime.Parse(vm.DateString + " " + vm.TimeString);
                    db.Games.Add(game);
                    db.SaveChanges();
                }
            }
        }

        public static Season GetCurrentSeason()
        {
            Season s = new Season();
            using (TeamEntities db = new TeamEntities())
            {
                s = db.Seasons.FirstOrDefault(e => e.IsCurrent == true);
            }
            return s;
        }

        public static bool IsPlayerCurrentlyRequesting(int id)
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

        public static int GetPlayerGoals(int id, int seasonId)
        {
            List<Goal> goals = new List<Goal>();
            using (TeamEntities db = new TeamEntities())
            {
                var games = db.Games.Include(g => g.Season).Where(g => g.Season.Id == seasonId).Select(g => g.Id).ToList();
                goals = db.Goals.Where(g => g.Goal_PlayerID == id && games.Contains(g.Game.Id)).ToList();
            }
            return goals.Count();
        }

        public static int GetPlayerAssists(int id, int seasonId)
        {
            List<Goal> goals = new List<Goal>();
            using (TeamEntities db = new TeamEntities())
            {
                var games = db.Games.Include(g => g.Season).Where(g => g.Season.Id == seasonId).Select(g => g.Id).ToList();
                goals = db.Goals.Where(g => (g.Assist1_PlayerID == id || g.Assist2_PlayerID == id) && games.Contains(g.Game.Id)).ToList();
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
                    gs.Opponent = GetOpponentAbbreviation(item.Opponent);
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
                result.SeasonList = seasonStats.OrderBy(e=> e.title).ToList();
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

        public static List<Player> GetCurrentPlayers(Season s)
        {
            using (TeamEntities db = new TeamEntities())
            {
                var seasonId = s.Id;
                var seasonPlayers = db.SeasonPlayer.Include(i => i.Player).Where(e => e.Season.Id == seasonId).Select(i => i.Player.Id);
                var players = db.Players.Where(i => seasonPlayers.Contains(i.Id));
                return players.ToList(); 
            }
        }



        public static GameManagerViewModel GetGameManager(int gameId)
        {
            using (TeamEntities db = new TeamEntities())
            {
                GameManagerViewModel result = new GameManagerViewModel();
                var g = db.Games.Include(e => e.Season).SingleOrDefault(e => e.Id == gameId);
                if (g != null)
                {
                    result.Opponent = g.Opponent;
                    result.Id = g.Id;
                    result.SeasonId = g.Season.Id;
                    result.IsHome = g.IsHome;
                    result.OpponentScore = g.OpponentScore;
                    result.TeamScore = g.TeamScore;
                    result.Date = g.Date;
                    result.OpponentAbbr = GetOpponentAbbreviation(g.Opponent);
                    result.goals = new List<GoalViewModel>();
                    List<Goal> goals = GetGoalsForGame(g);
                    int seasonId = g.Season.Id;
                    foreach (Goal go in goals)
                    {
                        GoalViewModel vm = new GoalViewModel(seasonId);
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
                        for (int i = goals.Count; i < result.TeamScore; i++)
                        {
                            GoalViewModel vm = new GoalViewModel(seasonId);
                            result.goals.Add(vm);
                        }
                    }
                    result.teamShots = g.TeamShots;
                    result.opponentShots = g.OpponentShots;
                    result.currentPlayers = GetCurrentPlayers(g.Season);
                    result.selectedPlayers = GetCurrentPlayers(g.Season);
                }
                return result;
            }
        }

        public static CurrentSeasonViewModel GetCurrentSeasonViewModel()
        {
            using (TeamEntities db = new TeamEntities())
            {
                CurrentSeasonViewModel vm = new CurrentSeasonViewModel();
                vm.Seasons = new List<SelectListItem>();
                var seasonList = db.Seasons;
                foreach (var i in seasonList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Season_Duration;
                    item.Value = i.Id.ToString();
                    vm.Seasons.Add(item);
                }
                vm.Seasons.Insert(0, new SelectListItem() { Text = "Select a Year", Value = "" });
                return vm;
            }
        }

        public static List<SelectListItem> GetSeasonSelectList()
        {
            using (TeamEntities db = new TeamEntities())
            {
                var seasonList = db.Seasons;
                var seasons = new List<SelectListItem>();
                foreach (var i in seasonList)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = i.Season_Duration;
                    item.Value = i.Id.ToString();
                    if (db.Games.Include(g => g.Season).Any(g => g.Season.Id == i.Id))
                    {
                        seasons.Add(item);
                    }
                }
                return seasons;
            }
        }


        public static void SetCurrentSeason(CurrentSeasonViewModel vm)
        {
            using (TeamEntities db = new TeamEntities())
            {
                if(vm.SelectedSeason != null)
                {
                    var season = db.Seasons.FirstOrDefault(e => e.Id == vm.SelectedSeason);
                    season.IsCurrent = true;
                    var otherSeason = db.Seasons.Where(e => e.Id != vm.SelectedSeason);
                    foreach(var item in otherSeason)
                    {
                        item.IsCurrent = false;
                    }
                    db.SaveChanges();
                }
            }
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

        public static List<SelectListItem> GetPlayersSelectList(int seasonId)
        {
            List<SelectListItem> players = new List<SelectListItem>();
            using (TeamEntities db = new TeamEntities())
            {
                var seasonPlayers = db.SeasonPlayer.Include(i => i.Season).Include(i=> i.Player).Where(i => i.Season.Id == seasonId).Select(i=> i.Player.Id);
                players = db.Players.Where(g => seasonPlayers.Contains(g.Id)).Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name + " " + f.Number
                }).ToList();
            }
            players.Insert(0,new SelectListItem { Value = null, Text = "-- Select a Player --" });
            return players;
        }

        public static List<Player> GetPlayersForSeason(int seasonId)
        {
            using (TeamEntities db = new TeamEntities())
            {
                var seasonPlayers = db.SeasonPlayer.Include(i => i.Season).Include(i => i.Player).Where(i => i.Season.Id == seasonId).Select(i => i.Player.Id).ToList();
                List<Player> players = db.Players.Where(i => seasonPlayers.Contains(i.Id)).ToList();
                return players;
            }
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
            str = str.Trim(' ');
            if (!String.IsNullOrEmpty(str) && str.Contains(' '))
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

        public static void SaveGamePlayers(GameManagerViewModel game)
        {
            using (TeamEntities db = new TeamEntities())
            {
                foreach (string playerId in game.selectedPlayerIds)
                {
                    int p = Int32.Parse(playerId);
                    if (!db.GamePlayer.Include(i => i.Player).Include(g => g.Game).Any(i => i.Game.Id == game.Id && i.Player.Id == p))
                    {
                        var Gamem = db.Games.Where(s => s.Id == game.Id).FirstOrDefault();
                        var Playerm = db.Players.Where(s => s.Id == p).FirstOrDefault();
                        db.GamePlayer.Add(new Game_Player() { Game = Gamem, Player = Playerm });
                    }
                }
                db.SaveChanges();
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
            var season = GetCurrentSeason();
            Random rand = new Random();
            using (TeamEntities db = new TeamEntities())
            {
                var seasonPlayers = db.SeasonPlayer.Where(s => s.Season.Id == season.Id).Select(s => s.Player.Id);
                ps = db.Players.Include("PlayerImage").Where(a => a.IsActive == true && seasonPlayers.Contains(a.Id)).ToList();
                p = ps.OrderBy(c => Guid.NewGuid()).FirstOrDefault();
                //int index = rand.Next(0, ps.Count());
                //if (ps.Count >= 1)
                //{
                //    p = ps.ElementAt(index);
                //}
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

        public static void SavePlayer(PlayerViewModel p)
        {
            Player tempPlayer = new Player();
            using (TeamEntities db = new TeamEntities())
            {
                var player = db.Players.FirstOrDefault(i => i.Id == p.Id);
                player = SetPlayerFields(player, p);
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

        public static PlayerViewModel ConvertPlayerDBtoVM(Player tempPlayer)
        {
            DataService service = new DataService();
            PlayerViewModel player = new PlayerViewModel();
            player.Id = tempPlayer.Id;
            player.Number = tempPlayer.Number;
            player.Birthplace = tempPlayer.Birthplace;
            player.IsActive = tempPlayer.IsActive;
            player.Height = tempPlayer.Height;
            player.Weight = tempPlayer.Weight;
            player.Position = tempPlayer.Position;
            player.Name = tempPlayer.Name;
            player.Bio = tempPlayer.Bio;
            player.Team_Role = tempPlayer.Team_Role;
            player.Major = tempPlayer.Major;
            player.Picture = tempPlayer.PlayerImage;
            player.Year = tempPlayer.Year;
            player.SnapChatURL = tempPlayer.SnapChatURL;
            player.TwitterURL = tempPlayer.TwitterURL;
            player.FacebookURL = tempPlayer.FacebookURL;
            player.InstagramURL = tempPlayer.InstagramURL;
            player.Birthplace = tempPlayer.Birthplace;
            player.BirthYear = tempPlayer.BirthYear;
            player.BirthMonth = tempPlayer.BirthMonth;
            player.BirthDay = tempPlayer.BirthDay;
            player.isEdit = tempPlayer.isEdit;
            player.Age = tempPlayer.Age;
            return player;
        }

        public static Player SetPlayerFields(Player player, PlayerViewModel vm)
        {
            player.Number = vm.Number;
            player.Birthplace = vm.Birthplace;
            player.Height = vm.Height;
            player.Weight = vm.Weight;
            player.Position = vm.Position;
            player.Name = vm.Name;
            player.Bio = vm.Bio;
            player.Team_Role = vm.Team_Role;
            player.Major = vm.Major;
            player.Year = vm.Year;
            player.SnapChatURL = vm.SnapChatURL;
            player.TwitterURL = vm.TwitterURL;
            player.FacebookURL = vm.FacebookURL;
            player.InstagramURL = vm.InstagramURL;
            player.Birthplace = vm.Birthplace;
            player.BirthYear = vm.BirthYear;
            player.BirthMonth = vm.BirthMonth;
            player.BirthDay = vm.BirthDay;
            player.Age = vm.Age;
            return player;
        }


        public static void SavePlayerAndImage(HttpPostedFileBase file, PlayerViewModel p)
        {
            DataService service = new DataService();
            using (TeamEntities db = new TeamEntities())
            {
                var player = db.Players.FirstOrDefault(i => i.Id == p.Id);
                player = SetPlayerFields(player, p);
                Photo photo = new Photo();
                if (p.pic_x1 != null && p.pic_width != null && p.pic_y1 != null && p.pic_height != null)
                {
                    photo.ImageData = CropImage(service.ConvertToBytes(file),(int) Math.Round(p.pic_x1.Value), (int)Math.Round(p.pic_y1.Value), (int)Math.Round(p.pic_width.Value), (int)Math.Round(p.pic_height.Value));
                }
                else
                {
                    photo.ImageData = service.ConvertToBytes(file);
                }
                photo.IsActive = true;
                db.Photos.Add(photo);
                player.PlayerImage = photo;
                db.Players.Attach(player);
                db.SaveChanges();
            }
        }

        public static byte[] CropImage(byte[] picData, int x, int y, int width, int height)
        {
            MemoryStream ms = new MemoryStream(picData);
            Image source = Image.FromStream(ms);

            Rectangle crop = new Rectangle(x, y, width, height);

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(bmp, typeof(byte[]));
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
            var season = GetCurrentSeason();
            using (TeamEntities db = new TeamEntities())
            {
                var seasonPlayers = db.SeasonPlayer.Include(e => e.Season).Include(e => e.Player).Where(s => s.Season.Id == season.Id).Select(e => e.Player.Id).ToList();
                players = db.Players.Where(a => a.IsActive == true && seasonPlayers.Contains(a.Id)).ToList();
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