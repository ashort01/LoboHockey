using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class PlayerViewModel
    {
        public int Id { get; set; }
        public int? Number { get; set; }
        public string Birthplace { get; set; }
        public bool IsActive { get; set; }
        public string Height { get; set; }
        public int? Weight { get; set; }
        public string Position { get; set; }
        [Required]
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
        public string Team_Role { get; set; }
        public string Major { get; set; }
        public HttpPostedFileBase Picture { get; set; }
        public string Year { get; set; }
        public string SnapChatURL { get; set; }
        public string FacebookURL { get; set; }
        public string TwitterURL { get; set; }
        public string InstagramURL { get; set; }
        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }
        public bool? isEdit { get; set; }
        public int? Age { get; set; }
    }
}