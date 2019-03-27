using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class UserGroup
    {
        public Guid userId { get; set; }
        public User User { get; set; }

        public Guid groupId { get; set; }
        public Group Group { get; set; }

        [Required]
        public virtual Role role { get; set; }

        [Required]
        public Boolean open { get; set; } = true;

        [Required]
        public DateTime dateJoin { get; set; } = DateTime.Today;

        [Required]
        public DateTime dateRole { get; set; }
    }
}
