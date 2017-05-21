using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class ManageGamesIndexViewModel
    {
        public List<GameViewModel> games { get; set; }
        public int seasonId { get; set; }
    }
}