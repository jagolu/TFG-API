using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    public class UserSignUp
    {
        [Required]
        [EmailAddress(ErrorMessage = "This is not a valid email")]
        public string email { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Username must have at least 3 characters")]
        [MaxLength(20, ErrorMessage = "Username must have less than 20 characters")]
        public string username { get; set; }

        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        [MaxLength(20, ErrorMessage = "Password must have less than 20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$",
            ErrorMessage = "Password must have at least a lowercase, a uppercase and a number")]
        public string password { get; set; }

        public Boolean provider { get; set; } = false;
    }
}
