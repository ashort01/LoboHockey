using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Services;

namespace UNMHockeySite.Models
{
    public class StatisticsViewModel
    {
        public StatisticsViewModel(int seasonId)
        {
            PlayerList = DataService.CreateStatistics(seasonId);
            PlayerList = PlayerList.OrderByDescending(i => i.points).ToList();
            MostGoals = PlayerList.OrderByDescending(r => r.goals).FirstOrDefault();
            MostAssists = PlayerList.OrderByDescending(r => r.assists).FirstOrDefault();
            MostPoints = PlayerList.OrderByDescending(r => r.points).FirstOrDefault();
            Seasons = DataService.GetSeasonSelectList();
            SelectedSeason = seasonId.ToString();
        }
        public List<PlayerStats> PlayerList { get; set; }
        public PlayerStats MostGoals { get; set; }
        public PlayerStats MostAssists { get; set; }
        public PlayerStats MostPoints { get; set; }
        public IEnumerable<SelectListItem> Seasons { get; set; }
        public string SelectedSeason { get; set; }

    }
}