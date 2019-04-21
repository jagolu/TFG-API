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
        public int createOfficialGroup { get; set; } = 1;

        [Required]
        public int createVirtualGroup { get; set; } = 1;
    }
}
