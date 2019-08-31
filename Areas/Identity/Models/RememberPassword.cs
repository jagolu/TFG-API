using System.ComponentModel.DataAnnotations;
namespace API.Areas.Identity.Models
{
    /// <summary>
    /// The info to remember a password
    /// </summary>
    public class RememberPassword
    {
        [Required]
        [EmailAddress]
        /// <value>The email of the user who wants to remember his password</value>
        public string email { get; set; }
    }

    /// <summary>
    /// THe info to reset a password
    /// </summary>
    public class ResetPassword
    {
        [Required]
        /// <value>The token password</value>
        public string tokenPassword { get; set; }

        [Required]
        /// <value>The new password of the user</value>
        public string password { get; set; }
    }
}
