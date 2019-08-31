using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    /// <summary>
    /// The info to do a log in action
    /// </summary>
    public class UserLogIn
    {
        [Required]
        [EmailAddress]
        /// <value>The email of the user to log</value>
        public string email { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        /// <value>The password of the user</value>
        public string password { get; set; }

        /// <value>The provider of the caller</value>
        public Boolean provider { get; set; } = false;
    }
}
