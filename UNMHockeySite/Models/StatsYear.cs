namespace UNMHockeySite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StatsYear")]
    public partial class StatsYear
    {
        public int Id { get; set; }

        public int? Year { get; set; }

        public int? Goals { get; set; }

        public int? Assists { get; set; }

        [Column("Goals Against Average")]
        public double? Goals_Against_Average { get; set; }

        public int? Saves { get; set; }

        [Column("Shots Against")]
        public int? Shots_Against { get; set; }

        public int? PlayerId { get; set; }

        public virtual Player Player { get; set; }
    }
}
