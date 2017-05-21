using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class StatsViewModel
    {
        public int CurrentSeasonGoals { get; set; }

        public int CurrentSeasonAssists { get; set; }

        public int CurrentSeasonPoints { get; set; }

        public int TotalGamesPlayed { get; set; }

        public List<GameStatsViewModel> LastFiveGames { get; set; }

        public List<SeasonStatsViewModel> SeasonList { get; set; }

        public int TotalGoals { get; set; }

        public int TotalAssists { get; set; }

        public int TotalPoints { get; set; }

    }
}