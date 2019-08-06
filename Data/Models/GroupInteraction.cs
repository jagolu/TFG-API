using System;
using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class GroupInteraction
    {
        public Guid userid { get; set; }
        public User User { get; set; }

        public Guid groupid { get; set; }
        public Group Group { get; set; }

        public DateTime dateLeave { get; set; } = DateTime.Now;

        [Required]
        public bool kicked { get; set; } = false;

        [Required]
        public bool leaved { get; set; } = false;
    }
}
