using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UNMHockeySite.Models
{
    public class CreateSeasonViewModel
    {
        [Required]
        public string SelectedYear { get; set; }
        public List<SelectListItem> YearList { get; set; }
    }
}