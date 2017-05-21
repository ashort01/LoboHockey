using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class PlayerStats
    {
        public int id { get; set; }
        public int goals { get; set; }
        public int assists { get; set; }
        public int points { get; set; }
        public string name { get; set; }
        public byte[] picture { get; set; }
        public int games_played { get; set; }
        public string position { get; set; }
    }
}