using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    public class UserLogIn
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        public string password { get; set; }

        public Boolean provider { get; set; } = false;
    }
}
