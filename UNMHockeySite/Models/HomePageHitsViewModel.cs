using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class HomePageHitsViewModel
    {
        public int totalHits { get; set; }

        public int thisYearHits { get; set; }

        public int thisMonthHits { get; set; }
    }
}