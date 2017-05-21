﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Services;

namespace UNMHockeySite.Models
{
    public class GoalViewModel
    {
        public GoalViewModel()
        {
            Players = DataService.GetPlayersSelectList();
        }
        public int GoalPlayerId { get; set; }
        public int? Assist1PlayerId { get; set; }

        public int? Assist2PlayerId { get; set; }
        public List<SelectListItem> Players { get; set; }

    }
}