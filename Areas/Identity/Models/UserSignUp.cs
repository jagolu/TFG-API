using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    public class UserSignUp
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[A-Za-z0-9_-]*$")]
        public string username { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        public string password { get; set; }

        public Boolean provider { get; set; } = false;
    }
}
