using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.DirectMessages.Models
{
    public class CreateDMTitle
    {
        [Required]
        [MinLength(3)]
        [MaxLength(60)]
        public string title { get; set; }

        [EmailAddress]
        public String emailReceiver { get; set; }
    }
}
