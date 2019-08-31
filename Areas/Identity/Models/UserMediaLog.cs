using System;
using System.ComponentModel.DataAnnotations;

namespace API.Areas.Identity.Models
{
    /// <summary>
    /// The info to do a social log/sign
    /// </summary>
    public class UserMediaLog
    {
        [Required]
        /// <value>The social token</value>
        public string authToken { get; set; }

        [Required]
        /// <value>The email of the user</value>
        public string email { get; set; }

        [Required]
        /// <value>The first name of the user</value>
        public string firstName { get; set; }

        [Required]
        /// <value>The id of the user in the social platform</value>
        public string id { get; set; }

        [Required]
        /// <value>The name of the social provider</value>
        public string socialProvider { get; set; }

        [Required]
        /// <value>The provider of the calller</value>
        public Boolean provider { get; set; } = false;

        [Required]
        /// <value>The url of the profile image of the user</value>
        public string urlImage { get; set; }

        /// <value>The password of the user</value>
        public string password { get; set; }
    }
}
