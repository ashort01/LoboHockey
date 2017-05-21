using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class PlayerStatsBioViewModel
    {
        public Player player { get; set; }

        public StatsViewModel stats {get; set;}
    }
}