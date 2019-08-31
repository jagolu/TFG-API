using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Models
{
    /// <summary>
    /// A direct messages conversation
    /// </summary>
    public class DirectMessageTitle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <value>The id of the dm</value>
        public Guid id { get; set; }

        [Required]
        /// <value>The sender of the dm</value>
        public User Sender { get; set; }
        
        /// <value>The id of the sender of the dm</value>
        public Guid senderid { get; set; }

        [Required]
        /// <value>The receiver of the dm</value>
        public User receiver { get; set; }

        [Required]
        [MaxLength(64)]
        /// <value>The title of the dm</value>
        public string title { get; set; }

        [Required]
        /// <value>The time of the last update in the dm</value>
        public DateTime lastUpdate { get; set; } = DateTime.Now;

        [Required]
        /// <value>True if the dm is closed, false otherwise</value>
        public bool closed { get; set; } = false;

        [Required]
        /// <value>Unread messages in the conversation for the normal user</value>
        public int unreadMessagesForUser { get; set; } = 0;

        [Required]
        /// <value>Unread messages in the conversation for the admin</value>
        public int unreadMessagesForAdmin { get; set; } = 0;

        /// <value>The messages in the dm</value>
        public ICollection<DirectMessageMessages> messages { get; set; } = new HashSet<DirectMessageMessages>();
    }
}
