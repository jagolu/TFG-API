using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserToken
    {
        public Guid userId { get; set; }
        public User User { get; set; }

        [Required]
        public Boolean loginProvider { get; set; } // 0 web, 1 phone

        [Required]
        [MaxLength]
        public string refreshToken { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public DateTime expirationTime { get; set; } = DateTime.Now.AddHours(1);
    }
}
