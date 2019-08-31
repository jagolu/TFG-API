using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    /// <summary>
    /// The info to do a sign up
    /// </summary>
    public class UserSignUp
    {
        [Required]
        [EmailAddress]
        /// <value>The email of the user to sign up</value>
        public string email { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[A-Za-z0-9_-]*$")]
        /// <value>The username of the new user</value>
        public string username { get; set; }

        [MinLength(8)]
        [MaxLength(20)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{1,}$")]
        /// <value>The password of the new user</value>
        public string password { get; set; }

        /// <value>The provider of the caller</value>
        public Boolean provider { get; set; } = false;
    }
}
