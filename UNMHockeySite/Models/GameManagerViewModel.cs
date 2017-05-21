using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Services;

namespace UNMHockeySite.Models
{
    public class GameManagerViewModel
    {
        public int Id { get; set; }
        public string Opponent { get; set; }
        public Nullable<bool> IsHome { get; set; }
        [DisplayFormat(DataFormatString = "{0: M/d/yyyy  h:mm tt}")]
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> TeamScore { get; set; }
        public Nullable<int> OpponentScore { get; set; }
        public string OpponentAbbr { get; set; }
        public List<GoalViewModel> goals {get; set;}
        public int? teamShots { get; set; }
        public int? opponentShots { get; set; }

    }
}