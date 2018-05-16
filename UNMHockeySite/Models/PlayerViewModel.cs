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
        [StringLength(100)]
        public string Birthplace { get; set; }
        public bool IsActive { get; set; }
        [StringLength(50)]
        public string Height { get; set; }
        public int? Weight { get; set; }
        [StringLength(50)]
        public string Position { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(5000)]
        public string Bio { get; set; }
        public string Email { get; set; }
        [StringLength(50)]
        public string Team_Role { get; set; }
        [StringLength(50)]
        public string Major { get; set; }
        public Photo Picture { get; set; }
        [StringLength(50)]
        public string Year { get; set; }
        [StringLength(1000)]
        public string SnapChatURL { get; set; }
        [StringLength(1000)]
        public string FacebookURL { get; set; }
        [StringLength(1000)]
        public string TwitterURL { get; set; }
        [StringLength(1000)]
        public string InstagramURL { get; set; }
        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }
        public bool? isEdit { get; set; }
        public int? Age { get; set; }
        public float? pic_x1 { get; set; }
        public float? pic_width { get; set; }
        public float? pic_y1 { get; set; }
        public float? pic_height { get; set; }
    }
}