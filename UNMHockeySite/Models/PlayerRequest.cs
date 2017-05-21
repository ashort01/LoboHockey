using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("PlayerRequest")]
    public class PlayerRequest
    {
        public int Id { get; set; }

        public virtual Player Player { get; set; }

        public virtual Season Season { get; set; }

        public bool? approved { get; set; }
    }
}