using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class Notifications
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        public Guid Userid { get; set; }
        public User User { get; set; }

        [Required]
        [MaxLength(64)]
        public string message { get; set; }
    }
}
