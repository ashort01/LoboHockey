using Newtonsoft.Json;
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
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only alphanumeric characters are allowed for this field.")]
        public string FromName { get; set; }

        [Display(Name = "Your email"), EmailAddress]
        [Required(ErrorMessage = "* Required")]
        public string FromEmail { get; set; }


        [Display(Name = "Your phone number")]
        [Required(ErrorMessage = "* Required")]
        //regular expression to ensure only numbers are allowed
        [Phone]
        public string PhoneNumber { get; set; }

        [Display(Name = "Message (optional)")]
        [RegularExpression("^[a-zA-Z0-9.!?,]*$", ErrorMessage = "Only alphanumeric characters are allowed for the message field.")]
        public string Message { get; set; }

    }
}