namespace UNMHockeySite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Game")]
    public partial class Game
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Game()
        {
            Goals = new HashSet<Goal>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string Opponent { get; set; }

        public bool? IsHome { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? Date { get; set; }

        public int? TeamScore { get; set; }

        public int? OpponentScore { get; set; }

        public int? TeamShots { get; set; }

        public int? OpponentShots { get; set; }

        public bool? OverTime { get; set; }

        public virtual Season Season { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Goal> Goals { get; set; }
    }
}
