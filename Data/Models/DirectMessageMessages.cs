using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class DirectMessageMessages
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        public bool isAdmin { get; set; }

        [Required]
        [MaxLength(512)]
        public string message { get; set; }

        [Required]
        public DateTime time { get; set; } = DateTime.Now;

        [Required]
        public DirectMessageTitle DirectMessageTitle { get; set; }
        public Guid directMessageTitleid { get; set; }
    }
}
