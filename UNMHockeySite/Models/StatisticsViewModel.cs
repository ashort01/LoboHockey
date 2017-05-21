using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UNMHockeySite.Services;

namespace UNMHockeySite.Models
{
    public class StatisticsViewModel
    {
        public StatisticsViewModel()
        {
            PlayerList = DataService.CreateStatistics();
            PlayerList = PlayerList.OrderByDescending(i => i.points).ToList();
            MostGoals = PlayerList.OrderByDescending(r => r.goals).FirstOrDefault();
            MostAssists = PlayerList.OrderByDescending(r => r.assists).FirstOrDefault();
            MostPoints = PlayerList.OrderByDescending(r => r.points).FirstOrDefault();
        }
        public List<PlayerStats> PlayerList { get; set; }
        public PlayerStats MostGoals { get; set; }
        public PlayerStats MostAssists { get; set; }
        public PlayerStats MostPoints { get; set; }
    }
}