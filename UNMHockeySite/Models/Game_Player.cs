
namespace UNMHockeySite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("Game_Player")]
    public class Game_Player
    {
        public int Id { get; set; }

        public virtual Player Player { get; set; }

        public virtual Game Game { get; set; }
    }
}