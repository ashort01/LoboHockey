using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UNMHockeySite.Models
{
    public class EmailFormModel
    {
        [Display(Name = "Your name")]
        [Required (ErrorMessage = "* Required")]
        public string FromName { get; set; }
        [Display(Name = "Your email"), EmailAddress]
        [Required(ErrorMessage = "* Required")]
        public string FromEmail { get; set; }
        [Display(Name = "Your phone number"), Phone]
        [Required(ErrorMessage = "* Required")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Message (optional)")]
        public string Message { get; set; }
    }
}