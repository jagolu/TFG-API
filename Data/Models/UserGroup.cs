using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class UserGroup
    {
        public Guid userid { get; set; }
        public User User { get; set; }

        public Guid groupid { get; set; }
        public Group Group { get; set; }

        [Required]
        public Role role { get; set; }

        [Required]
        public Boolean blocked { get; set; } = false;

        public Role blockedBy { get; set; }

        [Required]
        public int coins { get; set; }

        [Required]
        public DateTime dateJoin { get; set; } = DateTime.Today;

        [Required]
        public DateTime dateRole { get; set; }
    }
}
