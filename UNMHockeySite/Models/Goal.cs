namespace UNMHockeySite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Goal")]
    public partial class Goal
    {
        public int Id { get; set; }

        public int Goal_PlayerID { get; set; }

        public int? Assist1_PlayerID { get; set; }

        public int? Assist2_PlayerID { get; set; }

        public int? GameID { get; set; }

        public virtual Game Game { get; set; }

        public virtual Player Player { get; set; }

        public virtual Player Player1 { get; set; }

        public virtual Player Player2 { get; set; }
    }
}
