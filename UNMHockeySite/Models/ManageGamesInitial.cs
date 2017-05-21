using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UNMHockeySite.Models
{
    public class ManageGamesInitial
    {
        [Required]
        public string SelectedSeasonId { get; set; }

        public List<SelectListItem> SeasonIdList { get; set; }
    }
}