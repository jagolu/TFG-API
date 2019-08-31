using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    /// <summary>
    /// Session token of a user
    /// </summary>
    public class UserToken
    {
        /// <value>The id of the user who owns the token</value>
        public Guid userId { get; set; }
        
        /// <value>The user who owns the token</value>
        public User User { get; set; }

        [Required]
        /// <value>True if the user calls from the web platform, false if the user calls from a mobile app</value>
        public Boolean loginProvider { get; set; } // 0 web, 1 phone

        [Required]
        [MaxLength]
        /// <value>The refresh session token</value>
        public string refreshToken { get; set; } = Guid.NewGuid().ToString();

        [Required]
        /// <value>The time when the session token expires</value>
        public DateTime expirationTime { get; set; } = DateTime.Now.AddHours(1);
    }
}
