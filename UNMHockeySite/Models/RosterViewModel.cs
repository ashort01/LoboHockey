using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Services;

namespace UNMHockeySite.Models
{
    public class RosterViewModel
    {
        public RosterViewModel(int seasonId)
        {
            players = DataService.GetPlayersForSeason(seasonId);
            Seasons = DataService.GetSeasonSelectList();
            SelectedSeason = seasonId.ToString();
        }
        public List<Player> players { get; set; }
        public IEnumerable<SelectListItem> Seasons { get; set; }
        public string SelectedSeason { get; set; }
    }
}