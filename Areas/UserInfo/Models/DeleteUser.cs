using System.ComponentModel.DataAnnotations;

namespace API.Areas.UserInfo.Models
{
    public class DeleteUser
    {
        [EmailAddress(ErrorMessage = "This is not a valid email")]
        public string email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        [MaxLength(20, ErrorMessage = "Password must have less than 20 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$",
                ErrorMessage = "Password must have at least a lowercase, a uppercase and a number")]
        public string password { get; set; }
    }
}
