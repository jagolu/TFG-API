using System.ComponentModel.DataAnnotations;
namespace API.Areas.Identity.Models
{
    public class RememberPassword
    {
       [Required]
       public string email { get; set; }
    }
    public class ResetPassword
    {
        [Required]
        public string tokenPassword { get; set; }

        [Required]
        public string password { get; set; }
    }
}
