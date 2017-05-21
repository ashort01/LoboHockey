using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class SeasonStatsViewModel
    {
        public string title { get; set; }

        public int gamesPlayed { get; set; }

        public int goals { get; set; }

        public int assists { get; set; }

        public int points { get; set; }
    }
}