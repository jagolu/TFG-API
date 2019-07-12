using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class New
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        public Guid? groupId { get; set; } = null;
        public Group Group { get; set; } = null;

        public Guid? userId { get; set; } = null;
        public User User { get; set; } = null;

        [Required]
        public DateTime date { get; set; } = DateTime.Now;

        [Required]
        public string title { get; set; }

        [Required]
        [MaxLength(257)]
        public string message { get; set; }
    }
}
