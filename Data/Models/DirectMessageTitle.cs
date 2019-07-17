using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    public class DirectMessageTitle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }

        [Required]
        public User Sender { get; set; }
        public Guid senderId { get; set; }

        [Required]
        public User Receiver { get; set; }

        [Required]
        [MaxLength(64)]
        public string title { get; set; }

        [Required]
        public DateTime lastUpdate { get; set; } = DateTime.Now;

        [Required]
        public bool closed { get; set; } = false;

        [Required]
        public int unreadMessagesForUser { get; set; } = 0;

        [Required]
        public int unreadMessagesForAdmin { get; set; } = 0;

        public ICollection<DirectMessageMessages> messages { get; set; } = new HashSet<DirectMessageMessages>();
    }
}
