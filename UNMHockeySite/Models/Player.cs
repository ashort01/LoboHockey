namespace UNMHockeySite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Player")]
    public partial class Player
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Player()
        {
            Goals = new HashSet<Goal>();
            Goals1 = new HashSet<Goal>();
            Goals2 = new HashSet<Goal>();
            StatsYears = new HashSet<StatsYear>();
        }

        [Required]
        public int Id { get; set; }

        public AspNetUser User { get; set; }

        public Photo PlayerImage { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public int? Number { get; set; }

        [StringLength(100)]
        public string Birthplace { get; set; }

        public int? RosterId { get; set; }

        public bool IsActive { get; set; }

        [StringLength(50)]
        public string Height { get; set; }

        public int? Weight { get; set; }

        [StringLength(50)]
        public string Position { get; set; }

        [StringLength(5000)]
        public string Bio { get; set; }



        [Column("Team Role")]
        [StringLength(50)]
        public string Team_Role { get; set; }

        [StringLength(50)]
        public string Major { get; set; }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Goal> Goals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Goal> Goals1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Goal> Goals2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StatsYear> StatsYears { get; set; }

        public virtual Roster Roster { get; set; }
    }
}
