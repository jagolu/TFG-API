using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class GroupInteraction
    {
        public Guid userId { get; set; }
        public User User { get; set; }

        public Guid groupId { get; set; }
        public Group Group { get; set; }

        public DateTime dateLeave { get; set; } = DateTime.Now;

        [Required]
        public bool kicked { get; set; } = false;

        [Required]
        public bool leaved { get; set; } = false;
    }
}
