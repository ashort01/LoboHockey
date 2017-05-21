using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }

        [Column(TypeName = "image")]
        public byte[] ImageData { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

    }
}