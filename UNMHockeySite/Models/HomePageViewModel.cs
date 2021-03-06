﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class HomePageViewModel
    {
        public Game nextGame { get; set; }
        public List<Game> nextFiveGames { get; set; }
        public Player featuredPlayer { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int otls { get; set; }
        public PlayerStats mostPoints { get; set; }
        public PlayerStats mostGoals { get; set; }
        public PlayerStats mostAssists { get; set; }
        public int mostPointsNum { get; set; }
        public int mostGoalsNum { get; set; }
        public int mostAssistsNum { get; set; }

    }
}