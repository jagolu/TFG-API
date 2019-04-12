using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Limitations
    {
        [Key]
        public Guid userId { get; set; }
        public User User { get; set; }

        [Required]
        public int socialGroup { get; set; } = 1;

        [Required]
        public int virtualGroup { get; set; } = 1;
    }
}
