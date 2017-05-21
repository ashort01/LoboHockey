using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UNMHockeySite.Models
{
    public class ManageRosterViewModel
    {
        public int SeasonId { get; set; }

        public List<Season_Player> players {get;set;}

        public List<PlayerRequest> playerRequests { get; set; }
    }
}