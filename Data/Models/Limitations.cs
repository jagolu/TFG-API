using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Limitations
    {
        [Key]
        public Guid userId { get; set; }
        public User User { get; set; }

        [Required]
        public int officialGroup { get; set; } = 1;

        [Required]
        public int virtualGroup { get; set; } = 1;
    }
}
