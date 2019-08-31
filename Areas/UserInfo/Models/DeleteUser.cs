using System.ComponentModel.DataAnnotations;

namespace API.Areas.UserInfo.Models
{
    //The info to delete an account
    public class DeleteUser
    {
        [Required]
        [EmailAddress]
        /// <value>The email of the user</value>
        public string email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        /// <value>The password of the user</value>
        public string password { get; set; }
    }
}
