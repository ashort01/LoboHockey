using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class GameStatsViewModel
    {
        public string Opponent { get; set; }

        public bool IsHome { get; set; }

        public int goals { get; set; }

        public int assists { get; set; }

        public int points { get; set; }

        public int gameId { get; set; }

        public DateTime Date { get; set; }
    }
}