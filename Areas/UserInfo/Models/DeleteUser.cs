using System.ComponentModel.DataAnnotations;

namespace API.Areas.UserInfo.Models
{
    public class DeleteUser
    {
        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
