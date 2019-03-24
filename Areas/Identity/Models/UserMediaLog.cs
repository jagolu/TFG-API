using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    public class UserMediaLog
    {
        [Required]
        public string authToken { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string firstName { get; set; }

        [Required]
        public string id { get; set; }

        [Required]
        public string socialProvider { get; set; }

        [Required]
        public Boolean provider { get; set; } = false;
    }
}
