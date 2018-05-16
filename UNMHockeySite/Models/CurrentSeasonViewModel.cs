using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UNMHockeySite.Models
{
    public class CurrentSeasonViewModel
    {
        public List<SelectListItem> Seasons {get;set;}

        public int SelectedSeason { get; set; }
    }
}