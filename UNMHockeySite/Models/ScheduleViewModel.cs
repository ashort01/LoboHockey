using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Services;

namespace UNMHockeySite.Models
{
    public class ScheduleViewModel
    {
        public ScheduleViewModel(int seasonId)
        {
            List<Game> games = DataService.GetActiveGames(seasonId);
            GameList = DataService.GetGameViewModels(games);
            Seasons = DataService.GetSeasonSelectList();
            SelectedSeason = seasonId.ToString();
        }
        public IEnumerable<GameViewModel> GameList { get; set; }
        public IEnumerable<SelectListItem> Seasons { get; set; }
        public string SelectedSeason { get; set; }

    }
}