using System.ComponentModel.DataAnnotations;

namespace API.Areas.UserInfo.Models
{
    public class DeleteUser
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        public string password { get; set; }
    }
}
