﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class GameViewModel
    {
        public int Id { get; set; }
        public string Opponent { get; set; }
        public Nullable<bool> IsHome { get; set; }
        [DisplayFormat(DataFormatString = "{0: M/d/yyyy  h:mm tt}")]
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> TeamScore { get; set; }
        public Nullable<int> OpponentScore { get; set; }
        public string OpponentAbbr { get; set; }
        public bool overtime { get; set; }
        public int SeasonId { get; set; }
    }
}